#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' TODO make this a generic memoize
- to memory
- to file
- serialization type
'''
import functools

try:
    import cPickle as pickle
except ImportError:
    import pickle

def cache_result(path):
    ''' Given a computationally intensive operation,
    cache the result to file after it is completed and
    check if the cached version exists before doing the
    calculation again. If the cache exists, load and return
    that.

    :param path: The path to cache the result to
    :returns: A cache decorated function
    '''
    def method_catch(method):
        @functools.wraps(method)
        def wrapper(*parameters):
            if os.path.exists(path):
                with open(path, 'rb') as handle:
                    return pickle.load(handle)

            result = method(*parameters)

            with open(path, 'wb') as handle:
                pickle.dump(result, handle)
            return result
        return wrapper
    return method_catch
