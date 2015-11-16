'''
work remaining:

* custom info_log
* custom comparator
* custom filter_policy
'''
from leveldb_native import LDB, RangeCollection
from leveldb_native import get_pointer_to, new_native_string
from leveldb_native import get_reference_to, as_python_string
from leveldb_native import new_native_size

class DatabaseException(Exception):
    ''' Represents an error working with the underlying
    leveldb database.
    '''
    pass

class WriteBatch(object):
    ''' Represents a single atomic write batch operation.
    Simply add all of your puts and deletes and then simply
    call `commit` to write them all to the database.

    This class allows a fluent style for easy composition of
    operations:

        batch.put("new-key", "new-value").delete("old-key").commit()
    '''

    def __init__(self, database, synchronous=False):
        '''
        :param database: The database client to issue the write with.
        :param synchronous: True to make the put to disk synchronous, False to buffer
        '''
        self.write_batch = LDB.leveldb_writebatch_create()
        self.database = database
        self.synchronous = synchronous

    def put(self, key, value):
        ''' Put the supplied value at the given key.

        :param key: The key to store the value at
        :param value: The value to store at the key
        '''
        LDB.leveldb_writebatch_put(self.write_batch, key, len(key), value, len(value))
        return self

    def put_many(self, items):
        ''' Put the supplied values at the given keys.
        The value can be a dictionary or a list of tuples.

        :param items: The key value pairs to store.
        '''
        if isinstance(items, dict):
            items = items.items()

        for key, value in items:
            self.put(key, value)
        return self

    def delete(self, key):
        ''' Delete the supplied value at the given key.

        :param key: The key to delete the value at
        '''
        LDB.leveldb_writebatch_delete(self.write_batch, key, len(key))
        return self

    def delete_many(self, keys):
        ''' Delete the supplied values at the given keys.
        The value can be an iterable of keys.

        :param keys: The keys to delete the values at
        '''
        for key in keys:
            self.delete(key)
        return self

    def clear(self):
        ''' Clear the current write batch values written
        so far. This current write batch can still be
        used, however all operations written up to this
        point will be gone.
        '''
        LDB.leveldb_writebatch_clear(self.write_batch)
        return self

    def commit(self):
        ''' Commit the total write batch operation. After this
        call, this write batch instance may no longer be used.

        :throws DatabaseException: If an error has occurred
        '''
        self.database.write(self.write_batch, self.synchronous)
        LDB.leveldb_writebatch_destroy(self.write_batch)
        self.write_batch = None

    def __enter__(self): return self
    def __exit__(self, type, value, traceback): self.commit()

class DatabaseReadOperations(object):
    ''' The read operations on the database
    ''' 

    def get_read_options(self, verify_checksum, fill_cache):
        ''' Get the options for a read operation on the database.

        :param verify_checksum: True to verify the checksum on read, False otherwise
        :param fill_cache: True to populate the cache on read, False otherwise
        :returns: The read options
        '''
        options = LDB.leveldb_readoptions_create()
        LDB.leveldb_readoptions_set_verify_checksums(options, verify_checksum)
        LDB.leveldb_readoptions_set_fill_cache(options, fill_cache)
        return options

    def new_iterator(self, verify_checksum=False, fill_cache=True):
        ''' Create a new interator around the database.

        :returns: The DatabaseIterator context object.
        '''
        options = self.get_read_options(verify_checksum, fill_cache)
        return DatabaseIterator(self.database, options)

    def get(self, key, verify_checksum=False, fill_cache=True):
        ''' Delete the supplied value at the given key.

        :param key: The key to delete the value at
        :throws DatabaseException: If an error has occurred
        '''
        result  = None
        options = self.get_read_options(verify_checksum, fill_cache)

        try:
            error = new_native_string()
            size = new_native_size()
            value = LDB.leveldb_get(self.database, options, key, len(key),
                get_reference_to(size), get_reference_to(error))
            self.handle_error(error)
            return as_python_string(value, size)
        finally:
            if options: LDB.leveldb_readoptions_destroy(options)

class DatabaseSnapshot(DatabaseReadOperations):
    ''' A snapshot of the database at a given point in time.
    '''

    def __init__(self, database):
        ''' Initialize a new instance of the DatabaseSnapshot
        '''
        self.database = database
        self.snapshot = LDB.leveldb_create_snapshot(self.database)

    def get_read_options(self, verify_checksum, fill_cache):
        ''' Get the options for a read operation on the database.

        :param verify_checksum: True to verify the checksum on read, False otherwise
        :param fill_cache: True to populate the cache on read, False otherwise
        :returns: The read options
        '''
        options = LDB.leveldb_readoptions_create()
        LDB.leveldb_readoptions_set_verify_checksums(options, verify_checksum)
        LDB.leveldb_readoptions_set_fill_cache(options, fill_cache)
        LDB.leveldb_readoptions_set_snapshot(options, self.snapshot)
        return options

    def close(self):
        ''' Close the underlying snapshot and free all resources.
        '''
        if self.snapshot:
            LDB.leveldb_release_snapshot(self.snapshot)
            self.snapshot = None

    def __enter__(self): return self
    def __exit__(self, type, value, traceback): self.close()

class DatabaseIterator(object):
    ''' An iterator around a portion of the database along
    with operations to easily move around the entire database.
    '''

    def __init__(self, database, options):
        ''' Initialize a new instance of the DatabaseIterator.

        :param options: The read options to operate with.
        :param database: The databaes to build an iterator on.
        '''
        self.iterator = LDB.leveldb_create_iterator(database, options)
        LDB.leveldb_readoptions_destroy(options)

    def get_key(self):
        ''' Get the current key at the iterator

        :returns: The current key at the iterator
        '''
        size = new_native_size()
        pointer = LDB.leveldb_iter_key(self.iterator, get_reference_to(size))
        return as_python_string(pointer, size)

    def get_value(self):
        ''' Get the current value at the iterator

        :returns: The current value at the iterator
        '''
        size = new_native_size()
        pointer = LDB.leveldb_iter_value(self.iterator, ctypes.byref(length))
        return as_python_string(pointer, size)

    def get_pair(self):
        ''' Get the current pair at the iterator.

        :returns: The current pair at the iterator.
        '''
        return (self.get_key(), self.get_value())

    def seek_next(self):
        ''' Move to the next value in the database.
        '''
        self.assert_is_valid()
        LDB.leveldb_iter_next(self.iterator)
        return self

    def seek_prev(self):
        ''' Move to the previous value in the database.
        '''
        self.assert_is_valid()
        LDB.leveldb_iter_prev(self.iterator)
        return self

    def seek(self, key):
        ''' Move to the specified key in the database.

        :param key: The key in the database to move to.
        '''
        LDB.leveldb_iter_seek(self.iterator, key, len(key))
        self.assert_no_error()
        return self

    def seek_to_last(self):
        ''' Move to the last entry in the database.
        '''
        LDB.leveldb_iter_seek_to_last(self.iterator)
        self.assert_no_error()
        return self

    def seek_to_first(self):
        ''' Move to the first element in the database.
        '''
        LDB.leveldb_iter_seek_to_first(self.iterator)
        self.assert_no_error()
        return self

    def is_valid(self):
        ''' Check if the current iterator is valid.

        :returns: True if the iterator is valid, False otherwise
        '''
        return LDB.leveldb_iter_valid(self.iterator)

    def assert_is_valid(self):
        ''' Check if the current iterator is valid and if not
        throw an exception.

        :throws StopIteration: If the iterator is not valid
        '''
        if not self.is_valid():
            raise StopIteration()

    def assert_no_error(self):
        ''' Check if there was an error after the last error.
        If there was, then throw an exception.

        :throws DatabaseException: If there was a problem with the iterator
        '''
        error = new_native_string()
        LDB.leveldb_iter_get_error(error, get_reference_to(error))
        message = as_python_string(error)
        if message:
            raise DatabaseException(message)

    def close(self):
        ''' Close the iterator and free any held resources.
        '''
        if self.iterator:
            LDB.leveldb_iter_destroy(self.iterator)
            self.iterator = None

    def next(self):
        ''' Get the next value in the iterator and advance
        to the next entry.

        :returns: The next value in the iterator.
        '''
        self.assert_is_valid()
        pair = self.get_pair()
        self.seek_next()
        return pair

    def __bool__(self): return self.is_valid()
    def __iter__(self): return self
    def __enter__(self): return self
    def __exit__(self, type, value, traceback): self.close()

class Database(DatabaseReadOperations):
    ''' High level pythonic wrapper around the underlying
    leveldb database.
    '''

    def __init__(self, **kwargs):
        ''' Create a new instance of the Database

        :param filename: The filename to store the database at.
        :param create_if_missing: True to create the database if it does not exist
        :param error_if_exists: True to throw an error if the database exists
        :param paranoid_checks: True to run all the paranoid checks on the database
        :param write_buffer_size: The write buffer size on the database
        :param max_open_files: The max number of memory mapped files open
        :param block_size: The max block size on the database
        :param block_restart_interval: 
        :param compression: True to enable compression on the database
        '''
        self.filename = kwargs.pop('filename', 'db')
        self.options  = dict(kwargs)
        self.database = None # allow for lazy opening of database

    def close(self):
        ''' Close the database and underlying structures. After
        this the database can no longer be used until open is called
        again.
        '''
        if self.database:
            LDB.leveldb_close(self.database)
            self.database = None # prevent segfaults

        if self.cache:
            LDB.leveldb_cache_destroy(self.cache)
            self.cache = None # prevent segfaults

        if self.filter_policy:
            LDB.leveldb_filterpolicy_destroy(self.filter_policy)
            self.filter_policy = None # prevent segfaults

        if self.environment:
            LDB.leveldb_env_destroy(self.environment)
            self.environment = None # prevent segfaults

    def get_open_options(self):
        ''' Get the options for opening a database as well
        as creating all the relevant data-structures needed
        for operation.

        :returns: The database open options.
        '''
        options = LDB.leveldb_options_create()
        self.cache = LDB.leveldb_cache_create_lru(self.options.get('cache_block_size', 8 * 1024 * 1024))
        self.filter_policy = LDB.leveldb_filterpolicy_create_bloom(self.options.get('bloom_filter_size', 10))
        self.environment = LDB.leveldb_create_default_env()

        LDB.leveldb_options_set_create_if_missing(options, self.options.get('create_if_missing', True))
        LDB.leveldb_options_set_error_if_exists(options, self.options.get('error_if_exists', False))
        LDB.leveldb_options_set_paranoid_checks(options, self.options.get('paranoid_checks', False))
        LDB.leveldb_options_set_write_buffer_size(options, self.options.get('write_buffer_size', 32 * 1024 * 1024))
        LDB.leveldb_options_set_max_open_files(options, self.options.get('max_open_files', 256))
        LDB.leveldb_options_set_block_size(options, self.options.get('block_size', 4 * 1024))
        LDB.leveldb_options_set_block_restart_interval(options, self.options.get('block_restart_interval', 16))
        LDB.leveldb_options_set_compression(options, self.options.get('compression', True))
        LDB.leveldb_options_set_cache(options, self.cache)
        LDB.leveldb_options_set_filter_policy(options, self.filter_policy)
        LDB.leveldb_options_set_env(options, self.environment)
        #LDB.leveldb_options_set_info_log(options, self.options.get('info_log')) just use default
        #LDB.leveldb_options_set_comparator(options, self.options.get('comparator'))
        return options

    def open(self):
        ''' Creates the neccessary data structures and opens the
        underlying database. To tune the underlying options for the
        database read the following:

        http://docs.basho.com/riak/1.0.0/tutorials/choosing-a-backend/LevelDB/

        :throws DatabaseException: If an error has occurred
        '''
        options = self.get_open_options()

        try:
            error = new_native_string()
            self.database = LDB.leveldb_open(options, self.filename, get_reference_to(error))
            self.handle_error(error)
        finally:
            LDB.leveldb_options_destroy(options)

    def get_version(self):
        ''' Retrieve the current version of the underlying library.

        :returns: major.minor version of the library
        '''
        return "{}.{}".format(
            LDB.leveldb_minor_version(),
            LDB.leveldb_major_version())

    def put(self, key, value, synchronous=False):
        ''' Put the supplied value at the given key.

        :param key: The key to store the value at
        :param value: The value to store at the key
        :param synchronous: True to make the put to disk synchronous, False to buffer
        :throws DatabaseException: If an error has occurred
        '''
        options = LDB.leveldb_writeoptions_create()

        try:
            error = new_native_string()
            LDB.leveldb_writeoptions_set_sync(options, synchronous)
            LDB.leveldb_put(self.database, options, key, len(key), value, len(value), get_reference_to(error))
            self.handle_error(error)
        finally:
            if options: LDB.leveldb_writeoptions_destroy(options)

    def new_write_batch(self, synchronous=False):
        ''' Start a new atomic write batch operation::

            with database.new_write_batch() as batch:
                batch.put('new-key', 'new-value')
                batch.delete('old-key')

        :param synchronous: True to make the put to disk synchronous, False to buffer
        :returns: The new batch write context object
        '''
        return WriteBatch(self, synchronous=synchronous)

    def new_snapshot(self):
        ''' Create a read-only snapshot of the database at this
        point in time. This can be used as follows::

            with database.new_snapshot() as snapshot:
                snapshot.get('key')

        :returns: The database snapshot context object.
        '''
        return DatabaseSnapshot(self.database)

    def write(self, write_batch, synchronous=False):
        ''' Apply the write batch to the datbase.

        :param write_batch: The write batch to apply to the database.
        :param synchronous: True to make the put to disk synchronous, False to buffer
        :throws DatabaseException: If an error has occurred
        '''
        options = LDB.leveldb_writeoptions_create()

        try:
            error = new_native_string()
            LDB.leveldb_writeoptions_set_sync(options, synchronous)
            LDB.leveldb_write(self.database, options, write_batch, get_reference_to(error))
            self.handle_error(error)
        finally:
            if options: LDB.leveldb_writeoptions_destroy(options)

    def delete(self, key, synchronous=False):
        ''' Delete the supplied value at the given key.

        :param key: The key to delete the value at
        :param synchronous: True to make the put to disk synchronous, False to buffer
        :throws DatabaseException: If an error has occurred
        '''
        options = LDB.leveldb_writeoptions_create()

        try:
            error = new_native_string()
            LDB.leveldb_writeoptions_set_sync(options, synchronous)
            LDB.leveldb_delete(self.database, options, key, len(key), get_reference_to(error))
            self.handle_error(error)
        finally:
            if options: LDB.leveldb_writeoptions_destroy(options)

    def compact_range(self, start_key, end_key):
        ''' Given a start and end key, attempt to compact the 
        database to save space.

        :param start_key: The start key to compat from.
        :param end_key: The end to to compat to.
        '''
        LDB.leveldb_compact_range(self.database, start_key,
            len(start_key), end_key, len(end_key))

    def get_property(self, name):
        ''' Get an internal property value if it exists. Current
        properties that are supported are:

        * "leveldb.num-files-at-level<N>"
          return the number of files at level <N>, where <N> is an
          ASCII representation of a level number (e.g. "0").

        * "leveldb.stats"
          returns a multi-line string that describes statistics
          about the internal operation of the DB.

        :param name: The name of the property to retrieve
        :returns: The value of the property if it exists, or None
        '''
        result = None
        value  = LDB.leveldb_property_value(self.database, name)
        return as_python_string(value)

    def approximate_size(self, ranges):
        ''' Given a collection of ranges, return the approximate
        size of the database between those ranges.

        :param ranges: dictionary or list of tuple ranges
        :returns: The size of the database between those ranges
        '''
        if isinstance(ranges, dict):
            ranges = ranges.items()

        collection = RangeCollection.create(ranges)
        LDB.leveldb_approximate_sizes(self.database, collection.count,
            collection.start_keys, collection.start_sizes,
            collection.end_keys, collection.end_sizes, collection.sizes)
        return list(collection.sizes)

    def repair(self):
        ''' Attempt to repair a database after setting paranoid checks
        still results in an error.

        :throws DatabaseException: If an error has occurred
        '''
        options = self.get_open_options()

        try:
            error = new_native_string()
            LDB.leveldb_repair_db(options, self.filename, get_reference_to(error))
            self.handle_error(error)
        finally:
            if options:
                LDB.leveldb_options_destroy(options)
            self.close()

    def destroy(self):
        ''' Completely blows away the underlying database. This process
        is not reversable.

        :throws DatabaseException: If an error has occurred
        '''
        options = self.get_open_options()

        try:
            error = new_native_string()
            LDB.leveldb_destroy_db(options, self.filename, get_reference_to(error))
            self.handle_error(error)
        finally:
            if options:
                LDB.leveldb_options_destroy(options)
            self.close()

    def handle_error(self, error):
        ''' Given an underlying error, attempt to handle it.
        By default this will check if the supplied error is set and
        throw the message. It can be overridden to change this behaviour.

        :param error: The error to handle
        :throws DatabaseException: If an error has occurred
        '''
        message = as_python_string(error)
        if message:
            raise DatabaseException(message)

    def __exit__(self, type, value, traceback): self.close()
    #def __iter__(self): return self.new_iterator().seek_to_first()
    def __iter__(self): return self.new_iterator()
    def __enter__(self):
        if not self.database: self.open()
        return self

__all__ = ['Database']

if __name__ == "__main__":

    with Database(filename='temporary') as database:
        print database.get_version()
        database.put("name", "value")
        print database.get("name")
        database.delete('name')
        print database.get("name")

    with Database(filename='temporary') as database:
        database.put("name", "value")
        with database.new_write_batch() as batch:
            batch.put("new-name", "new-value")
            batch.delete("name")
        print database.get("new-name")
        print database.get("name")

    with Database(filename='temporary') as database:
        with database.new_write_batch() as batch:
            batch.put_many({chr(v + 48) : str(v) for v in range(26)})
        print [database.get(chr(v + 48)) for v in range(26)]

        for key, value in database.new_iterator():
            print key,value

    Database(filename='temporary').destroy()
