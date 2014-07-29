'''
- release all, retry, try all not first fail (compute then all())
- split lock pojo and context manager
  * no client in the DynamoDBLock
  * how to overload time method
- logging
- atomic time operations / cache operations
  * check if we need to synchronize operations
- get updates timestamp
- fix tangled worker thread (internal or seperate)
- shutdown method to close all locks and stop thread
- check if table exists works correctly
- unit tests, failure tests
- make sure to/from dynamodb types are okay (serialize/deserialize)
- try to get lock for N time at rate of M (retryable class)
- thunk update data for when the lock is released
  * callback on release
- acquire is update w/ expect { is_locked: False }
  * doesn't matter which version
  * doesn't matter which owner
- otherwise we wait until lock.is_expired
  * version number is same
  * name is same
- otherwise, if someone updated the version
  * we get the new lock and retry
- extra schema
  * add range key if needed
  * add additional attributes if needed
'''
import time
import uuid
import socket
import json
from threading import Thread
from copy import copy
from datetime import timedelta
from boto.dynamodb2.fields import HashKey
from boto.dynamodb2.types import STRING
from boto.dynamodb2.items import Item
from boto.dynamodb2.table import Table

#--------------------------------------------------------------------------------
# Logging
#--------------------------------------------------------------------------------

import logging
_logger = logging.getLogger(__name__)

#--------------------------------------------------------------------------------
# Worker Thread
#--------------------------------------------------------------------------------

class DynamoDBLockWorker(object):
    ''' The worker that runs to periodically update lock leases as long
    as the system is alive. This prevents long running processes from
    losing their locks by possibly fast clients.
    '''

    def __init__(self, **kwargs):
        ''' Initializes a new instance of the DynamoDBLock class

        :param client: The client to perform management with
        :param cycle: The length of each cycle in seconds (default 1 minutes)
        :param locks: The dictionary of locks to manage (default the client locks)
        '''
        self.client = kwargs.get('client')
        self.locks  = kwargs.get('locks', self.client.locks)
        self.cycle  = kwargs.get('cycle', timedelta(minutes=1).total_seconds())
        self.thread = Thread(target=self.worker, args=(self,))

    def stop(self):
        ''' Stops the worker thread '''
        self.is_running = False
        self.thread.join()

    def start(self):
        ''' Starts the worker thread '''
        self.is_running = True
        self.thread.start()

    def worker(self): 
        ''' The worker thread used to update the lock leases
        for the currently handled locks.
        '''
        while self.is_running:
            start = self.client.get_new_timestamp()
            for lock in self.locks.values():
                try:
                    self.client.touch_lock(lock)
                except LockNotGrantedException, ex: 
                    del self.locks[lock.name]
            elapsed = self.client.get_new_timestamp() - start
            time.sleep(max(self.cycle - elapsed, 0))

#--------------------------------------------------------------------------------
# Exceptions
#--------------------------------------------------------------------------------

class DynamoDBLockException(Exception): pass
class LockNotGrantedException(DynamoDBLockException): pass

#--------------------------------------------------------------------------------
# Database Schema Class
#--------------------------------------------------------------------------------

class DynamoDBLockSchema(object):
    ''' A collection of the schema names for the underlying
    locks table. This can be overridden by simply supplying
    new names in the constructor.
    '''

    def __init__(self, **kwargs):
        ''' Initializes a new instance of the DynamoDBLock class

        :param duration: The relative duration of the held lock
        '''
        self.name           = kwargs.get('name',       'N')
        self.duration       = kwargs.get('duration',   'D')
        self.is_locked      = kwargs.get('is_locked',  'L')
        self.owner          = kwargs.get('owner',      'O')
        self.version        = kwargs.get('version',    'V')
        self.payload        = kwargs.get('payload',    'P')
        self.table_name     = kwargs.get('table_name', 'Locks')
        self.read_capacity  = kwargs.get('read_capacity', 1)
        self.write_capacity = kwargs.get('write_capacity', 1)

    # ------------------------------------------------------------
    # schema operations
    # ------------------------------------------------------------
    # These methods convert to and from the underlying table
    # schema
    # ------------------------------------------------------------

    def to_schema(self, params):
        ''' Given a dict of query params, convert them to the
        underlying schema, make sure they are valid, and remove
        paramaters that are not used.

        :param params: The paramaters to query with
        :returns: The converted query parameters
        '''
        query = {}
        if 'name'      in params: query[self.schema.name]      = params['name']
        if 'duration'  in params: query[self.schema.duration]  = params['duration']
        if 'is_locked' in params: query[self.schema.is_locked] = params['is_locked']
        if 'owner'     in params: query[self.schema.owner]     = params['owner']
        if 'version'   in params: query[self.schema.version]   = params['version']
        if 'payload'   in params: query[self.schema.payload]   = params['payload']
        return query

    def to_dict(self, schema):
        ''' Given a lock record, convert it to a dict of
        the query parameter names.

        :param schema: The record to convert to a dict
        :returns: The converted dict with query parameter names
        '''
        return {
            'name':      schema[self.schema.name],
            'duration':  schema[self.schema.duration],
            'is_locked': schema[self.schema.is_locked],
            'owner':     schema[self.schema.owner],
            'version':   schema[self.schema.version],
            'payload':   schema[self.schema.payload],
        }

    def __str__(self):
        return json.dumps(self.__dict__)

    __repr__ = __str__

#--------------------------------------------------------------------------------
# Lock Instance Class
#--------------------------------------------------------------------------------

class DynamoDBLock(object):
    ''' Represents a single instance of a lock along with the
    relevant information needed track its state.
    '''

    def __init__(self, **kwargs):
        ''' Initializes a new instance of the DynamoDBLock class

        :param duration: The relative duration of the held lock
        '''
        self.client    = kwargs.get('client')
        self.name      = kwargs.get('name')
        self.duration  = kwargs.get('duration')
        self.timestamp = kwargs.get('timestamp', None)
        self.is_locked = kwargs.get('is_locked', False)
        self.owner     = kwargs.get('owner', None)
        self.version   = kwargs.get('version', 1)
        self.payload   = kwargs.get('payload', None)

        if not self.timestamp:
            self.update_timestamp()

    def update_timestamp(self):
        ''' If the lock has changed in dynamodb, update the timestamp
        as we need to wait for the new client to finish its work. This
        causes us to wait at least another duration of time.
        '''
        self.timestamp = self.client.get_new_timestamp()

    def is_expired(self):
        ''' Check if the current lock is expired and needs to be
        re-aquired.

        :returns: True if expired, False otherwise
        '''
        current = self.client.get_new_timestamp()
        return (self.timestamp + self.delta) > current

    def __hash__(self):
        return hash(self.name + self.owner)

    def __str__(self):
        return json.dumps(self.__dict__)

    __repr__ = __str__

#--------------------------------------------------------------------------------
# Client Class
#--------------------------------------------------------------------------------

class DynamoDBLockClient(object):

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the DynamoDBLockClient class

        :param owner: The owner of the locks created by this client
        '''
        self.locks   = kwargs.get('locks', {})
        self.owner   = kwargs.get('owner', self.get_new_owner())
        self.schema  = kwargs.get('schema', DynamoDBLockSchema())
        self.table   = kwargs.get('table', self._create_table())
        self.timeout = kwargs.get('timeout', long(timedelta(minutes=5) * 1000))
        self.worker  = kwargs.get('worker', DynamoDBLockWorker(client=self))

    def is_lock_alive(self, lock):
        ''' Given a lock, test if it is still active
        for the current owner.

        :param lock: The lock to check for liveness
        :returns: True if the lock is alive, False otherwise
        '''
        is_alive = (bool(lock)
                && (lock.is_locked)
                && (lock.owner == self.owner)
                && (not lock.is_expired()))
        return is_alive

    def touch_lock(self, lock):
        ''' Touch the lock and update its version to renew the
        lease we are currently holding on the lock (if we can).

        :param lock: The lock to attempt to touch
        :returns: True if the lock was updated, False otherwise
        '''
        if not self.is_lock_alive(lock):
            return False

        (is_updated, version) = self._update_entry(name=lock.name, version=lock.version)
        if is_updated:
            lock.version = version
            lock.update_timestamp()
        return is_updated

    def release_lock(self, lock, delete=True, **kwargs):
        ''' Release the supplied lock and apply any supplied udpates
        to the underlying name.

        :param lock: The lock to attempt to release
        :param delete: True to also delete the lock, False otherwise
        :returns: True if the lock was released, False otherwise
        '''
        if not lock: return False
        if not isinstance(lock, DynamoDBLock):
            lock = self._retrieve_lock(lock)

        # we cannot release a lock that we do not own
        if lock.owner != self.owner:
            return False

        # if we do not delete, we simply update the is_locked flag
        # and verify that we are still the owner of the lock at the
        # correct version
        if not delete:
            (is_released, _) = self._update_entry(name=lock.name,
                version=lock.version, is_locked=False, **kwargs)
        else: is_released = self._delete_entry(name=lock.name, version=lock.version)

        if is_released and (lock.name in self.locks):
            del self.locks[lock.name]
        return is_released

    def release_all_locks(self, delete=True, **kwargs):
        ''' Release all the currently held locks by this instance of
        the lock client (cached).

        :param delete: True to also delete locks, False otherwise
        :returns: True if _all_ locks were released, False otherwise
        '''
        return all(self.release_lock(lock, delete, **kwargs)
            for lock in self.locks.values())

    def acquire_lock(self, name, timeout=None, **kwargs):
        ''' Attempt to acquire the lock with the specified
        timeout.

        :param name: The name of the lock to acquire
        :param timeout: The amount of time to wait, default on None
        :returns: The lock on success, Exception on failure
        '''
        if not name: return False

        timeout = timeout or self.timeout
        lock = self._retrieve_entry(name)
        pass

    def try_acquire_lock(self, name, **kwargs):
        ''' Attempt to acquire the lock without waiting.

        :param name: The name of the lock to acquire
        :returns: The lock on success, None on failure
        '''
        try:
            return self.acquire_lock(name, timeout=0, **kwargs)
        except LockNotGrantedException, ex:
            return None

    def retrieve_lock(self, name):
        ''' Retrieve the lock by the supplied name strictly
        to view its data, but not to perform any updates.

        :param name: The lock name to retrieve
        :returns: The lock at the supplied name or None
        '''
        if not name: return None

        if name in self.locks:             # try the cache first
            lock = copy(self.locks[name])  # defensive copy
        else: lock = self._retrieve_entry(name)

        if lock: lock.version = None       # do not allow user to update lock
        if not lock.is_locked: lock = None # treat stale locks as None
        return lock

    # ------------------------------------------------------------
    # magic methods
    # ------------------------------------------------------------
    # These are the context managers that allow for the `with`
    # style of using locks (to avoid the try-finally pattern)
    # ------------------------------------------------------------

    __enter__ = acquire_lock
    __exit__  = release_lock

    # ------------------------------------------------------------
    # new data operations
    # ------------------------------------------------------------
    # These can be overridden by users to supply custom methods of
    # managing time, version, and ownership.
    # ------------------------------------------------------------

    def get_new_owner(self):
        ''' Helper method to retrieve a new owner name that is
        not only unique to the server, but unique to the application
        on this server.

        :returns: A new owner name to operate with
        '''
        return socket.gethostname() + str(uuid.uuid4())

    def get_new_version(self):
        ''' Helper method to retrieve a new version number
        for a lock. This can be overloaded to provide a custom
        protocol.

        :returns: A new version number
        '''
        return str(uuid.uuid4())

    def get_new_timestamp(self):
        ''' Helper method to retrieve the current time since
        the epoch in milliseconds.

        :returns: The current time in milliseconds
        '''
        return long(time.time() * 1000)

    # ------------------------------------------------------------
    # dynamo operations
    # ------------------------------------------------------------

    def _create_table(self):
        ''' Create the underlying dynamodb table for writing
        locks to if it does not exist, otherwise uses the existing
        table.

        :returns: A handle to the underlying dynamodb table
        '''
        table = Table(self.schema.table_name)
        if table: return table

        return Table.create(self.schema.table_name,
            schema = [
                HashKey(self.schema.name, data_type=STRING)
            ],
            throughput = {
                'read':  self.schema.read_capacity,
                'write': self.schema.write_capacity,
            })

    def _retrieve_entry(self, name):
        ''' Given the name of a lock, attempt to retrieve the
        lock and update its value in the cache.

        :param name: The name of the lock to retrieve
        :returns: The lock if it exists, None otherwise
        '''
        query  = {
            self.schema.name: name,
            'consistent': True,
        }
        record = self.table.get_item(**query)
        if not record:
            return None

        params = self.schema.to_dict(record)
        return DynamoDBLock(**params)

    def _delete_entry(self, name, version):
        ''' Attempt to delete the lock from dynamodb with
        the supplied name.
        
        We only allow a lock to be deleted if we know the
        current version number and we are the current owner
        of the lock. In order to delete another users lock, we
        must update it such that we are the owner.

        :param name: The name of the lock to delete
        :returns: True if successful, False otherwise
        '''
        qversion = '%s__eq' % self.schema.version
        qowner   = '%s__eq' % self.schema.owner
        return self.table.delete_item(name=name,
            expected={ qversion: version, qowner: self.owner })

    def _create_entry(self, name, duration, **kwargs):
        ''' Attempt to update the underlying lock on dynamodb
        with the supplied values.

        :param name: The name of the lock to update
        :param duration: The amount of time to hold the lock
        :returns: True if successful, False otherwise
        '''
        record = self.schema.to_schema(kwargs)
        record.update({
            self.schema.name:      name,
            self.schema.duration:  duration,
            self.schema.owner:     self.owner,
            self.schema.version:   self.get_new_version(),
            self.schema.is_locked: True,
        })

        if self.table._put_item(record):
            params = self.schema.to_dict(record)
            return DynamoDBLock(**params)
        return None

    def _update_entry(self, name, current, force=False, **kwargs):
        ''' Attempt to update the underlying lock on dynamodb
        with the supplied values.

        In order to update an entry, at least the version number
        must be the same. If we are overwriting a timed out lock
        we change ourself to the new owner, otherwise, we must be
        the current owner to update.

        Every update will automatically bump the version number
        of the underlying lock so clients can refresh their lease.

        :param name: The name of the lock to update
        :param version: The current version of the lock
        :returns: True if successful, False otherwise
        '''
        params = self.schema.to_schema(kwargs)
        params[self.schema.version] = self.get_new_version()

        expect = {
            self.schema.version: version,
            self.schema.name:    name,
        }
        if not force:
            expect[self.schema.owner] = self.owner

        is_updated = self.table._update_item(name, params, expects=expect)
        return (is_updated, params[self.schema.version])
