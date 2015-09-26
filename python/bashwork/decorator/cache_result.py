#!/usr/bin/env python
# -*- coding: latin-1 -*-
import os
import functools

try:
    import cPickle as pickle
except ImportError:
    import pickle

def cache_result(path, test, load, dump):
    ''' Given a computationally intensive operation,
    cache the result to file after it is completed and
    check if the cached version exists before doing the
    calculation again. If the cache exists, load and return
    that.

    :param path: The path to cache the result to
    :param test: The function to test if data is in cache
    :param load: The function to load data from cache
    :param dump: The function to dump data to the cache
    :returns: A cache decorated function
    '''
    def method_catch(method):
        @functools.wraps(method)
        def wrapper(*parameters):
            if test(path):
                return load(path)
            result = method(*parameters)
            dump(path, result)
            return result
        return wrapper
    return method_catch

def memory_cache_result(path):
    ''' Given a computationally intensive operation,
    cache the result to memory after it is completed and
    check if the cached version exists before doing the
    calculation again. If the cache exists, load and return
    that.

    :param path: The path to cache the result to
    :returns: A cache decorated function
    '''
    store = None
    def test(path): return store != None
    def load(path): return store
    def dump(path, data): store = data

    return cache_result(path, test, load, dump) 

def pickle_cache_result(path):
    ''' Given a computationally intensive operation,
    cache the result to file after it is completed and
    check if the cached version exists before doing the
    calculation again. If the cache exists, load and return
    that.

    :param path: The path to cache the result to
    :returns: A cache decorated function
    '''
    def test(path):
        return os.path.exists(path)

    def load(path):
        with open(p, 'rb') as handle:
            return pickle.load(handle)

    def dump(path, data):
        with open(path, 'wb') as handle:
            pickle.dump(data, handle)

    return cache_result(path, test, load, dump) 
