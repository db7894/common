#!/usr/bin/env python
# -*- coding: latin-1 -*-
import os
from collections import defaultdict
from bashwork.decorator.cache_result import pickle_cache_result

# ------------------------------------------------------------------ #
# Word Lookup Sources
# ------------------------------------------------------------------ #

class Words(object):
    ''' A collection of utility methods to generate
    various word lists and lookup tables.
    '''
    @staticmethod
    def get_word_list_lazy(path='/usr/share/dict/words'):
        ''' Get a list of words to use to build lookup tables

        :param path: The path to the file to open
        :return: A generator for the words in the list
        '''
        with open(path, 'r') as handle:
            for line in handle:
                yield line.strip().lower()

    @staticmethod
    def get_word_list(path='/usr/share/dict/words'):
        ''' Get a list of words to use to build lookup tables

        :param path: The path to the file to open
        :return: A generator for the words in the list
        '''
        return list(Words.get_word_list_lazy(path))

    @staticmethod
    @pickle_cache_result(path='/tmp/anagram-lookup.pickle')
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
    @pickle_cache_result(path='/tmp/missing-lookup.pickle')
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

    @staticmethod
    @pickle_cache_result(path='/tmp/single-letter-lookup.pickle')
    def generate_single_letter_lookup(words):
        ''' Given a collection of words, generate a lookoup table
        of all the words with one letter changed that are also words.

        :param words: The collection of words to build a lookup for
        :returns: The lookup table of the single letter changes
        '''
        lookup  = { word : set() for word in words }
        letters = [chr(ord('a') + l) for l in range(0, 26)]

        for word in words:
            for letter in letters:
                values = (word[:i] + letter + word[i+1:] for i in range(len(word)))
                lookup[word].update(w for w in values if w in words)
        return lookup
