#!/usr/bin/env python
# -*- coding: latin-1 -*-

class Trie(object):
    ''' A simple trie using dictionaries
    and terminal entries for complete words.
    '''

    VALUE = object()

    def __init__(self, root=None):
        ''' Initialize a new instance of the trie

        :param root: The current root to initialize with
        '''
        self.root = root or dict()

    def add_words(self, words):
        ''' Add a collection of words to the Trie

        :param words: The words to add to the trie
        '''
        for value, word in enumerate(words):
            self.add_word(word, value)

    def add_word(self, word, value=None):
        ''' Add a single word to the Trie

        :param word: The word to add to the trie
        :param value: The value to store at the word
        '''
        root = self.root
        for letter in word:
            if letter == ' ': continue
            if letter not in root:
                root[letter] = dict()
            root = root[letter]
        root[Trie.VALUE] = value or len(word)

    def remove_word(self, word):
        ''' Remove a single word to the Trie and
        prune any empty paths.

        :param word: The word to add to the trie
        '''
        pass

    def has_path(self, path):
        ''' Test if the supplied path exists in
        the Trie.

        :param path: The path to text for existance of
        :returns: True if the path exists, False otherwise
        '''
        root = self.root
        for letter in path:
            root = root.get(letter, None)
            if not root: return False
        return root != None

    def get_path(self, path):
        ''' Get a new Trie up to the current path

        :param path: The path to get a current path to
        :returns: a new Trie instance of the given path, or None
        '''
        root = self.root
        for letter in path:
            root = root.get(letter, None)
            if not root: return None
        return Trie(root)

    def has_a_word(self):
        ''' Check if the current root level has a
        word entered at its root level.

        :returns: True if a word is at the root level, False otherwise
        '''
        return self.root and self.root.get(Trie.VALUE, False)

    def __contains__(self, word):
        ''' Test if the supplied word exists
        in the Trie (not the path up to a word).

        :param word: The word to test for existance of
        :returns: True if the word exists, False otherwise
        '''
        root = self.root
        for letter in word:
            root = root.get(letter, None)
            if not root: return False
        return root and root.get(Trie.VALUE, False)
