from time import sleep
from copy import copy

from boto.exception import JSONResponseError
from boto.dynamodb2.types import Dynamizer
from boto.dynamodb2.fields import HashKey
from boto.dynamodb2.types import STRING
from boto.dynamodb2.items import Item
from boto.dynamodb2.table import Table
from boto.dynamodb2.exceptions import ConditionalCheckFailedException, ItemNotFound

from .policy import DynamoDBLockPolicy
from .schema import DynamoDBLockSchema
from .worker import DynamoDBLockWorker

#--------------------------------------------------------------------------------
# logging
#--------------------------------------------------------------------------------

import logging
_logger = logging.getLogger(__name__)

#--------------------------------------------------------------------------------
# classes
#--------------------------------------------------------------------------------

class DynamoDBLockClient(object):

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the DynamoDBLockClient class

        :param locks: The collection of locks to watch, default {}
        :param policy: The timing policy for taking and timing out locks
        :param schema: The schema of the database table to work with
        :param owner: The owner of the locks created by this client
        :param table: The current handle to the dynamodb table client
        :param worker: The underlying heartbeat worker to work with
        '''
        self.locks  = kwargs.get('locks', {})
        self.policy = kwargs.get('policy', DynamoDBLockPolicy())
        self.schema = kwargs.get('schema', DynamoDBLockSchema())
        self.owner  = kwargs.get('owner', self.policy.get_new_owner())
        self.table  = kwargs.get('table', None) or self._create_table()
        self.worker = kwargs.get('worker', DynamoDBLockWorker(client=self))

    # ------------------------------------------------------------
    # worker methods
    # ------------------------------------------------------------

    def startup(self):
        ''' Start the heartbeat thread and perform any lock
        initialization.
        '''
        self.worker.start()

    def shutdown(self):
        ''' Stop the heartbeat thread and close all of the existing
        lock handles that we have outstanding leases to.
        '''
        self.worker.stop()
        self.release_all_locks()

    # ------------------------------------------------------------
    # locking methods
    # ------------------------------------------------------------

    def is_lock_alive(self, lock):
        ''' Given a lock, test if it is still active
        for the current owner.

        :param lock: The lock to check for liveness
        :returns: True if the lock is alive, False otherwise
        '''
        return ((lock != None)
            and (lock.is_locked)
            and (lock.owner == self.owner)
            and (not lock.is_expired))

    def touch_lock(self, lock):
        ''' Touch the lock and update its version to renew the
        lease we are currently holding on the lock (if we can).

        :param lock: The lock to attempt to touch
        :returns: True if the lock was updated, False otherwise
        '''
        if not self.is_lock_alive(lock):
            return False

        new_lock = self._update_entry(lock)
        if new_lock:
            self.locks[lock.name] = new_lock
        return new_lock

    def release_lock(self, lock, delete=None, **kwargs):
        ''' Release the supplied lock and apply any supplied udpates
        to the underlying name.

        :param lock: The lock to attempt to release
        :param delete: True to also delete locks, False to mark them unlocked
        :returns: True if the lock was released, False otherwise
        '''
        if not self.is_name_valid(name):
            return False

        if not isinstance(lock, DynamoDBLock):
            lock = self._retrieve_lock(lock)
        delete = delete if delete != None else self.policy.delete_lock

        # ------------------------------------------------------------
        # Case 1:
        # ------------------------------------------------------------
        # If we do not currently own the lock, we are not allowed to
        # delete it. The only person who can delete the lock is the
        # owner. As such, we must take ownership to delete the lock.
        # ------------------------------------------------------------
        if lock.owner != self.owner:
            return False

        # ------------------------------------------------------------
        # Case 2:
        # ------------------------------------------------------------
        # If we do not delete the lock, we simply update the is_locked
        # flag as long as we are still the owner of the lock at the
        # lock version is still what we expect it to be.
        # ------------------------------------------------------------
        if not delete:
            kwargs['is_locked'] = False
            new_lock = self._update_entry(lock, update=kwargs)
            is_released = bool(new_lock)

        # ------------------------------------------------------------
        # Case 3:
        # ------------------------------------------------------------
        # If we do delete the lock, we simply remove it from the
        # database as long as the version number has not changed.
        # Otherwise we simply fail.
        # ------------------------------------------------------------
        else: is_released = self._delete_entry(lock)

        # ------------------------------------------------------------
        # Cleanup:
        # ------------------------------------------------------------
        # After releasing the lock, we remove it from our cache only
        # if we did in fact release the lock.
        # ------------------------------------------------------------
        if is_released and (lock.name in self.locks):
            del self.locks[lock.name]
        return is_released

    def release_all_locks(self, delete=None, **kwargs):
        ''' Release all the currently held locks by this instance of
        the lock client (cached).

        :param delete: True to also delete locks, False to mark them unlocked
        :returns: True if all locks were released, False otherwise
        '''
        results = [self.release_lock(lock, delete, **kwargs)
            for lock in self.locks.values()]
        return all(results) # so we don't short circuit any evaluation

    def acquire_lock(self, name, no_wait=False, **kwargs):
        ''' Attempt to acquire the lock with the paramaters specified
        in the initial lock policy.

        :param name: The name of the lock to acquire
        :param no_wait: Try to acquire the lock without waiting
        :returns: The acquired lock on success, or None
        '''
        if not self.is_name_valid(name):
            return None

        initial_time   = self.policy.get_new_timestamp() # the time we started trying to acquire
        lock_timeout   = self.policy.acquire_timeout     # how long to wait until we fail
        refresh_time   = self.policy.retry_period        # how long to wait between database reads
        waited_time    = 0                               # the total amount of time we have waited
        watching_lock  = None                            # the watch we are currently trying to get
        created_lock   = None                            # the watch that we created and is valid
        tried_one_time = False                           # indicates if we have made one attempt at the lock

        while (get_new_timestamp() < (initial_time + lock_timeout)
           or (no_wait and tried_one_time)):             # if the user wants to try_acquire
            current_lock = self._retrieve_entry(name)

            # ------------------------------------------------------------
            # Case 1:
            # ------------------------------------------------------------
            # There is no existing lock in the database, so we can simply
            # grab the lock if we are able to, otherwise we loop and try
            # again.
            # ------------------------------------------------------------
            if not current_lock:
                created_lock = self._create_entry(name, **kwargs)

            # ------------------------------------------------------------
            # Case 2:
            # ------------------------------------------------------------
            # There is an existing lock in the database, however, it has
            # already been unlocked and exists because a previous user
            # chose not to delete it or failed to do so. Regardless, we
            # can simply overwrite the lock and make use of the existing
            # data if we so choose.
            # ------------------------------------------------------------
            elif not current_lock.is_locked:
                kwargs['owner'] = self.owner
                expect = ['is_locked', 'version', 'name']
                created_lock = self._update_entry(current_lock, expect=expect, update=kwargs)

            # ------------------------------------------------------------
            # Case 3:
            # ------------------------------------------------------------
            # If we are currently watching a lock and it has locally
            # become expired (we have waited the specified lease of the
            # lock) and the version has not changed in the interum, we
            # are allowed to take control of the lock if we can.
            # ------------------------------------------------------------
            elif (watching_lock
             and (watching_lock.is_expired)
             and (watching_lock.version == current_lock.version)):
                kwargs['owner'] = self.owner
                expect = ['version', 'name']
                created_lock = self._update_entry(current_lock, expect=expect, update=kwargs)

            # ------------------------------------------------------------
            # Case 4:
            # ------------------------------------------------------------
            # If we are currently not watching a lock, but someone has
            # the lock that we want, we start watching it and update our
            # timeout to match the lease of the lock.
            # ------------------------------------------------------------
            elif not watching_lock:
                timeout += current_lock.duration
                watching_lock = current_lock

            # ------------------------------------------------------------
            # Case 5:
            # ------------------------------------------------------------
            # If we are currently watching a lock and waiting for it to
            # expire and someone has gotten a new lease on that lock in
            # the interum between our delay, then we are forced to watch
            # the new lock. However, we do not update our delay time as
            # we might otherwise wait forever.
            # ------------------------------------------------------------
            elif (watching_lock
             and (watching_lock.version != current_lock.version)):
                watching_lock = current_lock

            # ------------------------------------------------------------
            # Cleanup:
            # ------------------------------------------------------------
            # If we were able to create a lock, then we add it to our
            # cache, update its timestamp locally, and return a copy to
            # the system so they cannot modify our state. Next, if we
            # plan on waiting for the lock, we sleep until the next retry
            # period. Otherwise, if we refused to wait for the lock, we
            # simply exit if we failed.
            # ------------------------------------------------------------
            if created_lock:
                self.locks[name] = created_lock
                return copy(created_lock)
            elif not no_wait:
                _logger.debug("waiting %d to acquire lock %s, total wait %d", refresh_time, name, waited_time)
                sleep(refresh_time)
                waited_time += refresh_time
            else: tried_one_time = True

        # ------------------------------------------------------------
        # Failure:
        # ------------------------------------------------------------
        # If after waiting the supplied buffer time plus the original
        # lock duration we still were not able to get a lock handle,
        # we simply fail and let the user know.
        # ------------------------------------------------------------
        return None

    def try_acquire_lock(self, name, **kwargs):
        ''' Attempt to acquire the lock without waiting, instead
        simply fail fast.

        :param name: The name of the lock to acquire
        :returns: The lock on success, None on failure
        '''
        return self.acquire_lock(name, no_wait=True, **kwargs)

    def retrieve_lock(self, name):
        ''' Retrieve the lock by the supplied name strictly
        to view its data, but not to perform any updates.

        :param name: The lock name to retrieve
        :returns: The lock at the supplied name or None
        '''
        if not self.is_name_valid(name):
            return None

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
    # style of using locks (to avoid the try-finally pattern).
    # ------------------------------------------------------------

    # TODO add context manager object
    def __enter__(self):pass
    def __exit__(self):pass

    # ------------------------------------------------------------
    # dynamo operations
    # ------------------------------------------------------------

    def _create_table(self):
        ''' Create the underlying dynamodb table for writing
        locks to if it does not exist, otherwise uses the existing
        table.

        :returns: A handle to the underlying dynamodb table
        '''
        try:
            table = Table(self.schema.table_name)
        except JSONResponseError, ex:
            _logger.exception("table %s does not exist, creating it", self.schema.table_name)
            table = Table.create(self.schema.table_name,
                schema = [ HashKey(self.schema.name, data_type=STRING) ],
                throughput = {
                    'read':  self.schema.read_capacity,
                    'write': self.schema.write_capacity,
                })
        _logger.debug("current table description: %s", table.describe())
        return table

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

        try:
            record = self.table.get_item(**query)
            params = self.schema.to_dict(record)
            return DynamoDBLock(**params)
        except ItemNotFound, ex:
            _logger.exception("failed to retrieve item: %s", name)
        return None

    def _delete_entry(self, lock):
        ''' Attempt to delete the lock from dynamodb with
        the supplied name.
        
        We only allow a lock to be deleted if we know the
        current version number and we are the current owner
        of the lock. In order to delete another users lock, we
        must update it such that we are the owner.

        :param lock: The lock to delete
        :returns: True if successful, False otherwise
        '''
        expected = { 'name': lock.name, 'version': lock.version }
        expected = self.schema.to_schema(kwargs)
        expected = { '%s__eq' % key : val for key, val in expected }

        try:
            return self.table.delete_item(name=name, expected=expected)
        except ConditionalCheckFailedException, ex:
            _logger.exception("failed to delete item: %s", name)
        return False

    def _create_entry(self, name, **kwargs):
        ''' Attempt to update the underlying lock on dynamodb
        with the supplied values.

        :param name: The name of the lock to update
        :returns: True if successful, False otherwise
        '''
        kwargs.update({
            'name':      name,
            'owner':     self.owner,
            'version':   self.policy.get_new_version(),
            'is_locked': True,
            'duration':  kwargs.get('duation', self.policy.lock_duration),
        })

        # We have to make sure that no one beat us in creating an
        # entry at this specified key, otherwise we should fail.
        expects = { self.schema.name: { 'Exists' : "false" } }
        record  = self.schema.to_schema(kwargs)
        record  = self.table._encode_keys(record)

        try:
            self.table._put_item(record, expects=expects)
            return DynamoDBLock(**kwargs)
        except (JSONResponseError, ConditionalCheckFailedException):
            _logger.exception("failed to create item: %s", name)
        return None

    def _update_entry(self, lock, expect=None, update=None):
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
        version = self.policy.get_new_version()
        updates = { self.schema.version : version }
        if update: updates.update(self.schema.to_schema(update))
        expects = expect or ['version', 'owner', 'name']
        expects = { key : lock.__dict__[key] for key in expects }
        expects = self.schema.to_schema(expects)

        try:
            self.table._update_item(lock.name, updates, expects=expects)
            new_lock = copy(lock)      # defensive copy
            new_lock.version = version # TODO apply all updates, copy lock
            return new_lock
        except ConditionalCheckFailedException, ex:
            _logger.exception("failed to create item: %s", name)
        return None
