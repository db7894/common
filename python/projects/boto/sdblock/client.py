#!/usr/bin/env python
import sys
import argparse
import time
import socket
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
    ''' A simple utility that uses SDB CAS to ensure that
    only one system can operate at once.
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the lock client

        :param domain: The SDB domain to operate with
        :param lock_name: The name of the lock to work with
        :param timeout: The max lease time for a lock (default 15 minutes)
        '''
        self.domain_name = kwargs.get('domain', 'system_locks')
        self.lock_name   = kwargs.get('lock_name')
        self.timeout     = kwargs.get('timeout', timedelta(minutes=15).seconds)
        self.hostname    = kwargs.get('hostname', socket.gethostname())

        try:
            self.socket  = boto.connect_sdb()
            self.domain  = self.socket.get_domain(self.domain_name, validate=True)
        except SDBResponseError:
            logger.exception("creating the initial lock domain")
            self.domain  = self.socket.create_domain(self.domain_name)

    def force_lock(self):
        ''' Force take the lock regardless if it is currently being held
        by another system.

        :returns: True if the lock was locked, False otherwise
        '''
        updates  = { 'locked_by': self.hostname, 'timestamp' : int(time.time()) }
        self.is_locked = self.domain.put_attributes(self.lock_name, updates)
        return self.is_locked

    def lock(self):
        ''' Attempt to acquire the lock and check if it is currently
        locked.

        :returns: True if the lock was locked, False otherwise
        '''
        updates  = { 'locked_by': self.hostname, 'timestamp' : int(time.time()) }
        expected = [ 'locked_by', False ]

        try:
            logger.debug("attempting to take the lock: %s", self.lock_name)
            self.is_locked = self.domain.put_attributes(self.lock_name, updates,
                expected_value=expected)
        except SDBResponseError as ex:
            import pdb;pdb.set_trace()
            if ex.error_code == 'ConditionalCheckFailed':
                self.is_locked = False # The lock is already locked

        #------------------------------------------------------------
        # so we didn't get the lock, but the other host may be dead,
        # so we assume that if 15 minutes have passed (hopefully the
        # datacenter time drift isn't that much) then it is now our
        # turn to take over and be the lock manager. If this doesn't
        # work, then something woke up and changed the value while
        # we were working.
        #------------------------------------------------------------

        if not self.is_locked:
            instance = self.domain.get_item(self.lock_name, consistent_read=True)
            expected = [ 'locked_by', instance['locked_by'], 'timestamp', instance['timestamp'] ]

            if (updates['timestamp'] - self.timeout) > int(instance['timestamp']):
                logger.debug("attempting to take the timed out lock: %s", self.lock_name)
                self.is_locked = self.domain.put_attributes(self.lock_name, updates,
                    expected_value=expected)

        return self.is_locked

    def force_unlock(self):
        ''' Force unlock the lock regardless if it is currently being held
        by another system.

        :returns: True if the lock was unlocked, False otherwise
        '''
        updates = [ 'locked_by' ]
        self.is_locked = not self.domain.delete_attributes(self.lock_name, updates)
        return not self.is_locked

    def unlock(self):
        ''' If the lock is currently locked, then unlock it.

        :returns: True if the lock was unlocked, False otherwise
        '''
        updates  = [ 'locked_by' ]
        expected = [ 'locked_by' , self.hostname ]

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
        
#--------------------------------------------------------------------------------
# main
#--------------------------------------------------------------------------------

def get_options():
    ''' Retrieve the command line options for the script

    :returns: The parsed command line options
    '''
    parser = argparse.ArgumentParser(description="System Lock Utility")
    parser.add_argument('-v', '--verbose', dest='debug', action='store_true',
        default=False, help='Enable debug logging if set')
    parser.add_argument('-n', '--name', dest='name',
        default='name', help='The name of the lock to operate on')
    parser.add_argument('-l', '--lock', dest='lock', action="store_true",
        default=True, help='Attempt to lock the requested lock')
    parser.add_argument('-u', '--unlock', dest='lock', action="store_false",
        default=True, help='Attempt to unlock the requested lock')
    parser.add_argument('-f', '--force', dest='force', action="store_true",
        default=False, help='Force the lock operation regardless if it is currently held')

    return parser.parse_args()

def main():
    ''' An example main for the locker script
    '''
    options = get_options()

    if options.debug:
        logging.getLogger().setLevel(logging.DEBUG)
        logging.basicConfig()

    try:
        locker = LockClient(lock_name=options.name)
        action = {
            (True,   True) : lambda: locker.force_lock(),
            (True,  False) : lambda: locker.lock(),
            (False,  True) : lambda: locker.force_unlock(),
            (False, False) : lambda: locker.unlock(),
        }[(options.lock, options.force)]

        return 0 if action() else 1

    except Exception:
        logger.exception("encountered an error working with the lock client")
        return -1

#--------------------------------------------------------------------------------
# main launcher
#--------------------------------------------------------------------------------

if __name__ == '__main__':
    sys.exit(main())
