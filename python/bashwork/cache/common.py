import os
import json
import pandas as pd
import msgpack as msgpack

try:
    import cPickle as pickle
except ImportError:
    import pickle


#-----------------------------------------------------------
# Logging
#-----------------------------------------------------------

import logging
log = logging.getLogger(__name__)

#-----------------------------------------------------------
# Datasets
#-----------------------------------------------------------

class Cache(object):
    ''' A base class cache that can be used to supply
    methods to the higher level caches.
    '''

    def save(self, path, data):
        ''' Given a path and some data to cache, cache
        that data.

        :param path: The path to store the data at
        :param data: The data to store at the supplied path
        '''
        raise NotImplemented("save")

    def load(self, path):
        ''' Given a path, attempt to load the supplied
        data at the path.

        :param path: The path to load from the cache
        :returns: The data (if it exists) at the cache
        '''
        raise NotImplemented("load")


class FileCache(Cache):
    ''' A base class cache that can be used to supply
    methods to the higher level caches.
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the cache.
        '''
        self.root = kwargs.get('root', 'cache')
        self.ext  = kwargs.get('ext', '')

    def exists(self, path):
        ''' Given a path, determine if the supplied cache
        exists.

        :patah path: The path to test for existance
        :return: True if exists False otherwise
        '''
        path = os.path.join(self.root, path + self.ext)
        return os.path.exists(path)

    @staticmethod
    def convert(cache_a, cache_b):
        ''' Given two cache functions, convert
        all the cached files from one caching format
        to the other.

        :param cache_a: The cache to convert from
        :param cache_b: The cache to convert to
        '''
        name_a = cache_a.__class__.__name__
        name_b = cache_b.__class__.__name__
        for path in os.listdir(cache_a.root):
            log.debug("converting file %s from %s to %s", path, name_a, name_b)
            data = cache_a.load(path)
            cache_b.save(path, data)


class PickleFileCache(FileCache):
    ''' A cache that will persist the supplied data to a
    pickle data store.
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the cache.
        '''
        super(PickelFileCache, self).__init__(ext=".pickle", **kwargs)

    def save(self, path, data):
        ''' Given a path and some data to cache, cache
        that data.

        :param path: The path to store the data at
        :param data: The data to store at the supplied path
        '''
        path = os.path.join(self.root, path + self.ext)
        with open(path, 'w') as handle:
            pickle.dump(data, handle)

    def load(self, path):
        ''' Given a path, attempt to load the supplied
        data at the path.

        :param path: The path to load from the cache
        :returns: The data (if it exists) at the cache
        '''
        path = os.path.join(self.root, path + self.ext)
        with open(path, 'r') as handle:
            return pickle.load(handle)


class JsonFileCache(FileCache):
    ''' A cache that persists the supplied data as a json
    blob to disk.
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the cache.
        '''
        super(JsonFileCache, self).__init__(ext=".json", **kwargs)
    
    def save(self, path, data):
        ''' Given a path and some data to cache, cache
        that data.

        :param path: The path to store the data at
        :param data: The data to store at the supplied path
        '''
        path = os.path.join(self.root, path + self.ext)
        with open(path, 'w') as handle:
            json.dump(data, handle)

    def load(self, path):
        ''' Given a path, attempt to load the supplied
        data at the path.

        :param path: The path to load from the cache
        :returns: The data (if it exists) at the cache
        '''
        path = os.path.join(self.root, path + self.ext)
        with open(path, 'r') as handle:
            return json.load(handle)

class MsgPackFileCache(FileCache):
    ''' A cache that persists the supplied data as a msgpack
    blob to disk.
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the cache.
        '''
        super(MsgPackFileCache, self).__init__(ext=".pack", **kwargs)
    
    def save(self, path, data):
        ''' Given a path and some data to cache, cache
        that data.

        :param path: The path to store the data at
        :param data: The data to store at the supplied path
        '''
        path = os.path.join(self.root, path + self.ext)
        with open(path, 'w') as handle:
            msgpack.packb(data, use_bin_type=True)

    def load(self, path):
        ''' Given a path, attempt to load the supplied
        data at the path.

        :param path: The path to load from the cache
        :returns: The data (if it exists) at the cache
        '''
        path = os.path.join(self.root, path + self.ext)
        with open(path, 'r') as handle:
            return msgpack.unpackb(handle)


class PandasFileCache(FileCache):
    ''' A cache that persists the supplied pandas data as
    a pandas hdf store to disk.
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the cache.
        '''
        super(PandasFileCache, self).__init__(ext=".hdf", **kwargs)
    
    def save(self, path, data):
        ''' Given a path and some data to cache, cache
        that data.

        :param path: The path to store the data at
        :param data: The data to store at the supplied path
        '''
        path = os.path.join(self.root, path + self.ext)
        data.to_hdf(path)

    def load(self, path):
        ''' Given a path, attempt to load the supplied
        data at the path.

        :param path: The path to load from the cache
        :returns: The data (if it exists) at the cache
        '''
        path = os.path.join(self.root, path + self.ext)
        return pd.read_hdf(path)

def cacheable(key_format):
    ''' A helper method to cache expensive methods
    of a class method. The key format should use all
    the params of the input to the function.
 
    :param key_format: The format of the key to cache
    :returns: A decorated class method
    '''
    def _cacheable(func):
        def wrapper(self, *args):
            key = key_format.format(*args)
            if not self.cache.exists(key):
                result = func(self, *args)
                self.cache.save(key, result)
                return result
            return self.cache.load(key)
        return wrapper
    return _cacheable
