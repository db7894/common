import ctypes
import ctypes.util

#--------------------------------------------------------------------------------
# Python CTypes Wrapper
#--------------------------------------------------------------------------------
# leveldb library
LDB = ctypes.CDLL(ctypes.util.find_library('leveldb'))

# leveldb_open
LDB.leveldb_open.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_void_p]
LDB.leveldb_open.restype  = ctypes.c_void_p

# leveldb_close
LDB.leveldb_close.argtypes = [ctypes.c_void_p]
LDB.leveldb_close.restype  = None

# leveldb_put
LDB.leveldb_put.argtypes = [
    ctypes.c_void_p, ctypes.c_void_p, ctypes.c_void_p, ctypes.c_size_t,
    ctypes.c_void_p, ctypes.c_size_t, ctypes.c_void_p]
LDB.leveldb_put.restype  = None

# leveldb_delete
LDB.leveldb_delete.argtypes = [
    ctypes.c_void_p, ctypes.c_void_p, ctypes.c_void_p, ctypes.c_size_t,
    ctypes.c_void_p]
LDB.leveldb_delete.restype  = None

# leveldb_write
LDB.leveldb_write.argtypes = [
    ctypes.c_void_p, ctypes.c_void_p, ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_write.restype  = None

# leveldb_get
LDB.leveldb_get.argtypes = [
    ctypes.c_void_p, ctypes.c_void_p, ctypes.c_void_p, ctypes.c_size_t,
    ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_get.restype  = ctypes.POINTER(ctypes.c_char)

# leveldb_write_batch
LDB.leveldb_writebatch_create.argtypes = []
LDB.leveldb_writebatch_create.restype  = ctypes.c_void_p

LDB.leveldb_writebatch_destroy.argtypes = [ctypes.c_void_p]
LDB.leveldb_writebatch_destroy.restype  = None

LDB.leveldb_writebatch_clear.argtypes = [ctypes.c_void_p]
LDB.leveldb_writebatch_clear.restype  = None

LDB.leveldb_writebatch_put.argtypes = [
    ctypes.c_void_p, ctypes.c_char_p, ctypes.c_size_t, ctypes.c_char_p,
    ctypes.c_size_t ]
LDB.leveldb_writebatch_put.restype  = None

LDB.leveldb_writebatch_delete.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_size_t]
LDB.leveldb_writebatch_delete.restype  = None

#leveldb_snapshot
LDB.leveldb_create_snapshot.argtypes = [ctypes.c_void_p]
LDB.leveldb_create_snapshot.restype  = ctypes.c_void_p

LDB.leveldb_release_snapshot.argtypes = [ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_release_snapshot.restype  = None

# leveldb_filterpolicy
LDB.leveldb_filterpolicy_create_bloom.argtypes = [ctypes.c_int]
LDB.leveldb_filterpolicy_create_bloom.restype  = ctypes.c_void_p

LDB.leveldb_filterpolicy_destroy.argtypes = [ctypes.c_void_p]
LDB.leveldb_filterpolicy_destroy.restype  = None

# leveldb_iterator
LDB.leveldb_create_iterator.argtypes = [ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_create_iterator.restype = ctypes.c_void_p

LDB.leveldb_iter_valid.argtypes = [ctypes.c_void_p]
LDB.leveldb_iter_valid.restype  = ctypes.c_bool

LDB.leveldb_iter_seek_to_first.argtypes = [ctypes.c_void_p]
LDB.leveldb_iter_seek_to_first.restype  = None

LDB.leveldb_iter_seek_to_last.argtypes = [ctypes.c_void_p]
LDB.leveldb_iter_seek_to_last.restype  = None

LDB.leveldb_iter_seek.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_size_t]
LDB.leveldb_iter_seek.restype  = None

LDB.leveldb_iter_next.argtypes = [ctypes.c_void_p]
LDB.leveldb_iter_next.restype  = None

LDB.leveldb_iter_prev.argtypes = [ctypes.c_void_p]
LDB.leveldb_iter_prev.restype  = None

LDB.leveldb_iter_key.argtypes = [ctypes.c_void_p, ctypes.POINTER(ctypes.c_size_t)]
LDB.leveldb_iter_key.restype  = ctypes.c_void_p

LDB.leveldb_iter_value.argtypes = [ctypes.c_void_p, ctypes.POINTER(ctypes.c_size_t)]
LDB.leveldb_iter_value.restype  = ctypes.c_void_p

LDB.leveldb_iter_get_error.argtypes = [ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_iter_get_error.restype  = None

LDB.leveldb_iter_destroy.argtypes = [ctypes.c_void_p]
LDB.leveldb_iter_destroy.restype  = None

# leveldb_options
LDB.leveldb_options_create.argtypes = []
LDB.leveldb_options_create.restype  = ctypes.c_void_p

LDB.leveldb_options_set_comparator.argtypes = [ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_options_set_comparator.restype  = None

LDB.leveldb_options_set_filter_policy.argtypes = [ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_options_set_filter_policy.restype  = None

LDB.leveldb_options_set_create_if_missing.argtypes = [ctypes.c_void_p, ctypes.c_ubyte]
LDB.leveldb_options_set_create_if_missing.restype  = None

LDB.leveldb_options_set_error_if_exists.argtypes = [ctypes.c_void_p, ctypes.c_ubyte]
LDB.leveldb_options_set_error_if_exists.restype  = None

LDB.leveldb_options_set_paranoid_checks.argtypes = [ctypes.c_void_p, ctypes.c_ubyte]
LDB.leveldb_options_set_paranoid_checks.restype  = None

LDB.leveldb_options_set_env.argtypes = [ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_options_set_env.restype  = None

LDB.leveldb_options_set_info_log.argtypes = [ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_options_set_info_log.restype  = None

LDB.leveldb_options_set_write_buffer_size.argtypes = [ctypes.c_void_p, ctypes.c_size_t]
LDB.leveldb_options_set_write_buffer_size.restype  = None

LDB.leveldb_options_set_max_open_files.argtypes = [ctypes.c_void_p, ctypes.c_int]
LDB.leveldb_options_set_max_open_files.restype  = None

LDB.leveldb_options_set_cache.argtypes = [ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_options_set_cache.restype  = None

LDB.leveldb_options_set_block_size.argtypes = [ctypes.c_void_p, ctypes.c_size_t]
LDB.leveldb_options_set_block_size.restype  = None

LDB.leveldb_options_set_block_restart_interval.argtypes = [ctypes.c_void_p, ctypes.c_int]
LDB.leveldb_options_set_block_restart_interval.restype  = None

LDB.leveldb_options_set_compression.argtypes = [ctypes.c_void_p, ctypes.c_int]
LDB.leveldb_options_set_compression.restype  = None

LDB.leveldb_options_destroy.argtypes = [ctypes.c_void_p]
LDB.leveldb_options_destroy.restype  = None

# leveldb_write_options
LDB.leveldb_writeoptions_create.argtypes = []
LDB.leveldb_writeoptions_create.restype  = ctypes.c_void_p

LDB.leveldb_writeoptions_set_sync.argtypes = [ctypes.c_void_p, ctypes.c_ubyte]
LDB.leveldb_writeoptions_set_sync.restype  = None

LDB.leveldb_writeoptions_destroy.argtypes = [ctypes.c_void_p]
LDB.leveldb_writeoptions_destroy.restype  = None

# leveldb_read_options
LDB.leveldb_readoptions_create.argtypes = []
LDB.leveldb_readoptions_create.restype  = ctypes.c_void_p

LDB.leveldb_readoptions_set_verify_checksums.argtypes = [ctypes.c_void_p, ctypes.c_ubyte]
LDB.leveldb_readoptions_set_verify_checksums.restype  = None

LDB.leveldb_readoptions_set_fill_cache.argtypes = [ctypes.c_void_p, ctypes.c_ubyte]
LDB.leveldb_readoptions_set_fill_cache.restype  = None

LDB.leveldb_readoptions_set_snapshot.argtypes = [ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_readoptions_set_snapshot.restype  = None

LDB.leveldb_readoptions_destroy.argtypes = [ctypes.c_void_p]
LDB.leveldb_readoptions_destroy.restype  = None

# leveldb_cache
LDB.leveldb_cache_create_lru.argtypes = [ctypes.c_size_t]
LDB.leveldb_cache_create_lru.restype  = ctypes.c_void_p

LDB.leveldb_cache_destroy.argtypes = [ctypes.c_void_p]
LDB.leveldb_cache_destroy.restype  = None

# leveldb_env
LDB.leveldb_create_default_env.argtypes = []
LDB.leveldb_create_default_env.restype  = ctypes.c_void_p

LDB.leveldb_env_destroy.argtypes = [ctypes.c_void_p]
LDB.leveldb_env_destroy.restype  = None

# leveldb_compact_range
LDB.leveldb_compact_range.argtypes = [
   ctypes.c_void_p, ctypes.c_char_p, ctypes.c_size_t, ctypes.c_void_p,
   ctypes.c_size_t]
LDB.leveldb_compact_range.restype  = None

# leveldb_property_value
LDB.leveldb_property_value.argtypes = [ctypes.c_void_p, ctypes.c_char_p]
LDB.leveldb_property_value.restype  = ctypes.c_char_p

# leveldb_approximate_size
LDB.leveldb_approximate_sizes.argtypes = [
    ctypes.c_void_p, ctypes.c_int, ctypes.c_void_p, ctypes.c_void_p,
    ctypes.c_void_p, ctypes.c_void_p, ctypes.c_void_p]
LDB.leveldb_approximate_sizes.restype  = None

# leveldb_destroy_db
LDB.leveldb_destroy_db.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_void_p]
LDB.leveldb_destroy_db.restype  = None

# leveldb_repair_db
LDB.leveldb_repair_db.argtypes = [ctypes.c_void_p, ctypes.c_char_p, ctypes.c_void_p]
LDB.leveldb_repair_db.restype  = None

# leveldb_free
LDB.leveldb_free.argtypes = [ctypes.c_void_p]
LDB.leveldb_free.restype  = None

# leveldb_major_version
LDB.leveldb_major_version.argtypes = []
LDB.leveldb_major_version.restype  = ctypes.c_int

# leveldb_minor_version
LDB.leveldb_minor_version.argtypes = []
LDB.leveldb_minor_version.restype  = ctypes.c_int

#--------------------------------------------------------------------------------
# CTypes Utilities
#--------------------------------------------------------------------------------

def get_pointer_to(reference):
    return ctypes.cast(reference, ctypes.c_void_p)

def new_native_string():
    return ctypes.POINTER(ctypes.c_char)()

def new_native_size():
    return ctypes.c_size_t(0)

def as_python_string(pointer, size=None):
    ''' Given a ctypes null terminated char pointer
    or optionally a size, convert that to a python
    managed string.

    :param pointer: The pointer to convert to a python string
    :param size: The size of the string if it is not null terminated.
    :returns: The python string if it exists, or None
    '''
    result = None
    if bool(pointer):
        if size: result = ctypes.string_at(pointer, size.value)
        else: result = ctypes.string_at(pointer)
        LDB.leveldb_free(get_pointer_to(pointer))
    return result

def get_reference_to(instance):
    return ctypes.byref(instance)

class RangeCollection(object):
    ''' A helper utility to calculate the array types
    needed to query for key ranges.
    '''

    @classmethod
    def create(klass, ranges):
        range_count = len(ranges)
        void_type   = ctypes.c_void_p * range_count
        size_type   = ctypes.c_size_t * range_count
        start_keys  = void_type(*[get_pointer_to(entry[0]) for entry in ranges])
        end_keys    = void_type(*[get_pointer_to(entry[1]) for entry in ranges])
        start_sizes = size_type(*[len(entry[0]) for entry in ranges])
        end_sizes   = size_type(*[len(entry[1]) for entry in ranges])
        sizes       = (ctypes.c_uint64 * range_count)()

        return klass(count=range_count,
            start_keys=start_keys, start_sizes=start_sizes,
            end_keys=end_keys, end_sizes=end_sizes, sizes=sizes)

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the RangeCollection.
        '''
        self.count = kwargs.get('count')
        self.start_keys = kwargs.get('start_keys')
        self.start_sizes = kwargs.get('start_sizes')
        self.end_keys = kwargs.get('end_keys')
        self.end_sizes = kwargs.get('end_sizes')
        self.sizes = kwargs.get('sizes')

__all__ = ['LDB']
