#!/usr/bin/env python
# -*- coding: latin-1 -*-
import os
import functools
from collections import defaultdict

try:
    import cPickle as pickle
except ImportError:
    import pickle

# ------------------------------------------------------------------ #
# Utility Methods
# ------------------------------------------------------------------ #

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
                    print "loading cache"
                    return pickle.load(handle)

            result = method(*parameters)

            with open(path, 'wb') as handle:
                pickle.dump(result, handle)
            return result
        return wrapper
    return method_catch

# ------------------------------------------------------------------ #
# Word Lookup Sources
# ------------------------------------------------------------------ #

class Words(object):
    ''' A collection of utility methods to generate
    various word lists and lookup tables.
    '''

    @staticmethod
    def get_word_list(path='/usr/share/dict/words'):
        ''' Get a list of words to use to build lookup tables

        :param path: The path to the file to open
        :return: A generator for the words in the list
        '''
        with open(path, 'r') as handle:
            for line in handle:
                yield line.strip().lower()

    @staticmethod
    @cache_result(path='/tmp/anagram-lookup.pickle')
    def generate_anagram_lookup(words):
        ''' Given a collection of words, generate a lookoup table
        of the words to find all anagrams.

        :param words: The collection of words to build a lookup for
        :returns: The lookup table of sorted words
        '''

        lookup = defaultdict(list)
        for word in words:
            lookup[''.join(sorted(word))].append(word)
        return lookup

    @staticmethod
    @cache_result(path='/tmp/missing-lookup.pickle')
    def generate_missing_lookup(words):
        ''' Given a collection of words, generate a lookoup table
        of all the words missing one letter that hash sorted to the
        same instance.

        :param words: The collection of words to build a lookup for
        :returns: The lookup table of sorted words
        '''
        lookup = defaultdict(list)
        for word in words:
            missing = [(word[i], word[:i] + word[i+1:]) for i in range(len(word))]
            for letter, entry in missing:
                sorted_word = ''.join(sorted(entry))
                lookup[sorted_word].append((letter, word))
        return lookup
