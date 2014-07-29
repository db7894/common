'''
- logging
- should dynamo methods create a lock instance (orm)
- atomic time operations / cache operations
  * check if we need to synchronize operations
- get updates timestamp
- heartbeat thread / heartbeat method
  * if update failed, remove from cache
  * shutdown method to close all locks and stop thread
- check if table exists works correctly
- add range key if needed
- add additional attributes if needed
- unit tests, failure tests
- add methods to lock instance
- make sure to/from dynamodb types are okay (serialize/deserialize)
- try to get lock for N time at rate of M (retryable class)
- thunk update data for when the lock is released
- acquire is update w/ expect { is_locked: False }
  * doesn't matter which version
  * doesn't matter which owner
- otherwise we wait until lock.is_expired
  * version number is same
  * name is same
- otherwise, if someone updated the version
  * we get the new lock and retry
'''
import time
import uuid
import socket
import json
from copy import copy
from datetime import timedelta
from boto.dynamodb2.fields import HashKey
from boto.dynamodb2.types import STRING
from boto.dynamodb2.items import Item
from boto.dynamodb2.table import Table

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

    __slots__ = [
        'name', 'duration', 'is_locked',
        'owner', 'version', 'payload', 'table_name',
        'read_capacity', 'write_capacity'
    ]

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
        self.timestamp = self.client._get_new_timestamp()

    def is_expired(self):
        ''' Check if the current lock is expired and needs to be
        re-aquired.

        :returns: True if expired, False otherwise
        '''
        current = self.client._get_new_timestamp()
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
        self.owner   = kwargs.get('owner', self._get_new_owner())
        self.schema  = kwargs.get('schema', DynamoDBLockSchema())
        self.table   = kwargs.get('table', self._create_table())
        self.timeout = kwargs.get('timeout', long(timedelta(minutes=5) * 1000))

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
    # new data operations
    # ------------------------------------------------------------

    def _get_new_owner(self):
        ''' Helper method to retrieve a new owner name that is
        not only unique to the server, but unique to the application
        on this server.

        :returns: A new owner name to operate with
        '''
        return socket.gethostname() + str(uuid.uuid4())

    def _get_new_version(self):
        ''' Helper method to retrieve a new version number
        for a lock. This can be overloaded to provide a custom
        protocol.

        :returns: A new version number
        '''
        return str(uuid.uuid4())

    def _get_new_timestamp(self):
        ''' Helper method to retrieve the current time since
        the epoch in milliseconds.

        :returns: The current time in milliseconds
        '''
        return long(time.time() * 1000)

    # ------------------------------------------------------------
    # schema operations
    # ------------------------------------------------------------

    def _dict_to_schema(self, params):
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

    def _schema_to_dict(self, record):
        ''' Given a lock record, convert it to a dict of
        the query parameter names.

        :param record: The record to convert to a dict
        :returns: The converted dict with query parameter names
        '''
        return {
            'duration':  record[self.schema.duration],
            'is_locked': record[self.schema.is_locked],
            'owner':     record[self.schema.owner],
            'version':   record[self.schema.version],
            'payload':   record[self.schema.payload],
        }

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
        query  = { self.schema.name: name, 'consistent': True }
        record = self.table.get_item(**query)
        if not record:
            return None

        params = self._schema_to_dict(record)
        return DynamoDBLock(**params)

    def _delete_entry(self, name, version):
        ''' Attempt to delete the lock from dynamodb with
        the supplied name.

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
        params = self._dict_to_schema(kwargs)
        params.update({
            self.schema.name: name,
            self.schema.duration: duration,
            self.schema.owner: self.owner,
            self.schema.version: self._get_new_version(),
            self.schema.is_locked: True,
        })
        is_created = self.table._put_item(params)
        return (is_created, params[self.schema.version])

    def _update_entry(self, name, current, **kwargs):
        ''' Attempt to update the underlying lock on dynamodb
        with the supplied values.

        :param name: The name of the lock to update
        :param version: The current version of the lock
        :returns: True if successful, False otherwise
        '''
        params  = self._dict_to_schema(kwargs)
        params[self.schema.version] = self._get_new_version()
        expect  = {
            self.schema.owner:   self.owner,
            self.schema.version: version,
        })
        is_updated = self.table._update_item(name, params, expects=expect)
        return (is_updated, params[self.schema.version])

    __enter__ = acquire_lock
    __exit__  = release_lock
