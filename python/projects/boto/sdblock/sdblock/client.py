import time
import socket
import uuid
import boto
from boto.exception import SDBResponseError
from datetime import timedelta

#------------------------------------------------------------
# logging
#------------------------------------------------------------

import logging
logger = logging.getLogger(__name__)

#------------------------------------------------------------
# SDB Lock Client
#------------------------------------------------------------

class LockClient(object):
    ''' A simple utility that uses simpleDB (SDB) conditional puts
    to ensure that only one system gets a lock at once. Systems can
    use this primitive to coordinate activities. A general usage is
    as follows::

        with LockClient(name='operation_name'):
            ... perform coordinated activity here

    The lock is maintained in SDB with the following schema per lock
    (this can easily be extended to add more data or be more complex)::

        (locked)   str(name) -> { timestamp : int(epoch_local) }
        (unlocked) str(name) -> { locked_by : str(system), timestamp : int(epoch_local) }
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the lock client

        :param domain: The SDB domain to operate with
        :param lock_name: The name of the lock to work with
        :param host_name: The name of the host taking the lock
        :param lock_time: The max lease time for a lock (default 15 minutes)
        :param wait_time: The max amount of time to wait for a lock (default 5 minutes)
        '''
        self.domain_name = kwargs.get('domain', 'system_locks')
        self.lock_name   = kwargs.get('lock_name', None) or str(uuid.uuid4())
        self.host_name   = kwargs.get('host_name', None) or socket.gethostname()
        self.lock_time   = kwargs.get('lock_time', timedelta(minutes=15)).seconds
        self.wait_time   = kwargs.get('wait_time', timedelta(minutes=5)).seconds 
        self.sleep_time  = kwargs.get('sleep_time', timedelta(seconds=1)).seconds

        try:
            self.socket  = boto.connect_sdb()
            self.domain  = self.socket.get_domain(self.domain_name, validate=True)
        except SDBResponseError:
            logger.exception("creating the initial lock domain")
            self.domain  = self.socket.create_domain(self.domain_name)

    def now(self):
        ''' Method to return the next time value. This can be
        overridden to supply another method of time.
        '''
        return int(time.time())

    def force_lock(self):
        ''' Force take the lock regardless if it is currently being held
        by another system.

        :returns: True if the lock was locked, False otherwise
        '''
        updates  = { 'locked_by': self.host_name, 'timestamp' : self.now() }
        self.is_locked = self.domain.put_attributes(self.lock_name, updates)
        return self.is_locked

    def try_lock(self):
        ''' Attempt to acquire the lock and check if it is currently
        locked one time and then fail (i.e. we do not wait).

        :returns: True if the lock was acquired, False otherwise
        '''
        return self.lock(no_wait=True)

    def lock(self, no_wait=False):
        ''' Attempt to acquire the lock and check if it is currently
        locked. If we do not aquire the lock, we will sleep and then
        try again.

        :param no_wait: True to try the lock once and fail, False to wait
        :returns: True if the lock was acquired, False otherwise
        '''
        timeout = self.now() + self.wait_time # how long we plan to wait
        while (not self._lock_internal() and not no_wait and (timeout > self.now())):
            time.sleep(self.sleep_time)

        return self.is_locked

    def _lock_internal(self):
        ''' Attempt to acquire the lock and check if it is currently
        locked.

        :returns: True if the lock was locked, False otherwise
        '''
        updates  = { 'locked_by': self.host_name, 'timestamp' : self.now() }
        expected = [ 'locked_by', False ] # the lock shouldn't be held

        try:
            logger.debug("attempting to take the lock: %s", self.lock_name)
            self.is_locked = self.domain.put_attributes(self.lock_name, updates,
                expected_value=expected)
        except SDBResponseError as ex:
            if ex.error_code == 'ConditionalCheckFailed':
                self.is_locked = False # The lock is already locked

        #------------------------------------------------------------
        # We didn't get the lock, but the other host may be dead,
        # so now we try to see if the lock is expired. If it is, then
        # we are free to take it. It should be noted that another
        # host could notice this and perform the same action and thus
        # get the timed out lock before we get it.
        #------------------------------------------------------------

        if not self.is_locked:
            instance = self.domain.get_item(self.lock_name, consistent_read=True)
            expected = [ 'locked_by', instance['locked_by'], 'timestamp', instance['timestamp'] ]

            if (updates['timestamp'] - self.lock_time) > int(instance['timestamp']):
                logger.debug("attempting to take the timed out lock: %s", self.lock_name)
                self.is_locked = self.domain.put_attributes(self.lock_name, updates,
                    expected_value=expected)

        return self.is_locked

    def force_unlock(self):
        ''' Force unlock the lock regardless if it is currently being held
        by another system.

        :returns: True if the lock was unlocked, False otherwise
        '''
        updates = [ 'locked_by' ] # we force unlock whowever has the lock
        self.is_locked = not self.domain.delete_attributes(self.lock_name, updates)
        return not self.is_locked

    def unlock(self):
        ''' If the lock is currently locked, then unlock it.

        :returns: True if the lock was unlocked, False otherwise
        '''
        updates  = [ 'locked_by' ]
        expected = [ 'locked_by' , self.host_name ] # cannot unlock another host

        try:
            self.is_locked = not self.domain.delete_attributes(self.lock_name,
                updates, expected_values=expected)
        except SDBResponseError as ex:
            if ex.error_code == 'AttributeDoesNotExist':
                self.is_locked = False # The lock is already unlocked

        if self.is_locked:
            logger.error("another process has taken the lock: %s", self.lock_name)

        return not self.is_locked

    def __enter__(self): self.lock(); return self
    def __exit__(self, type, value, traceback): self.unlock()
