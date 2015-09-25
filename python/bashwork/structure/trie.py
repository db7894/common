#!/usr/bin/env python
# -*- coding: latin-1 -*-

class Trie(object):
    ''' A simple trie using dictionaries
    and terminal entries for complete words.
    '''

    VALUE = object()
    SKIP  = set(list(' ,.;:-_\'"`'))

    @classmethod
    def create(klass, words):
        ''' Initialize a new trie with the supplied words

        :param words: The words to initialize the trie with
        :returns: The initialize trie
        '''
        trie = klass()
        trie.add_words(words)
        return trie

    def __init__(self, root=None, skip=None):
        ''' Initialize a new instance of the trie

        :param root: The current root to initialize with
        '''
        self.root = root or dict()
        self.skip = skip or Trie.SKIP

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
            if letter in self.skip: continue
            if letter not in root:
                root[letter] = dict()
            root = root[letter]
        root[Trie.VALUE] = value or len(word)

    def compact(self):
        ''' Compact the Trie to make single paths
        resolve down to single hash lookups:
        `a->b->c->` compacted to `abc->`
        '''
        pass # TODO

    def clean(self):
        ''' Remove any empty nodes in the trie.
        '''
        def recurse(root):
            if not root: return False            # if the node doesn't exist, exit
            for key in root:                     # for every key in the ndoe
                if key == Trie.VALUE: continue   # don't consider leaf values
                if recurse(root.get(key, None)): # check if we can clean that node
                    del root[key]                # remove empty keys
            return not bool(root)                # check if we can delete this node

        recurse(self.root)                       # recurse down the trie 

    def remove_word(self, word):
        ''' Remove a single word from the Trie and
        prune any empty paths.

        :param word: The word to remove from the trie
        '''
        def recurse(root, word):
            if not root: return False      # the word does not exist, exit
            if root and not word:          # the word exists and is found
                root.pop(Trie.VALUE, None) # unmark this node as a leaf word
                return not bool(root)      # return if this node is empty

            leaf = root.get(word[0], None) # get next child node
            if recurse(leaf, word[1:]):    # check if we can delete this node
                del root[word[0]]          # if so, delete the unused node
            return not bool(root)          # return if this node is empty

        recurse(self.root, word)           # recurse down the trie

    def has_path(self, path):
        ''' Test if the supplied path exists in the Trie. 
        It should be noted that this doesn't test if a word
        leaf exists in the Trie, just that there is a path
        including at least this word prefix.

        :param path: The path to test for existance of
        :returns: True if the path exists, False otherwise
        '''
        return bool(self.get_path(path).root)

    def get_path(self, path):
        ''' Get a new Trie up to the current path

        :param path: The path to get a current path to
        :returns: a new Trie instance of the given path, or None
        '''
        root = self.root
        for letter in path:
            root = root.get(letter, None)
            if not root: return Trie(None)
        return Trie(root)

    def has_a_word(self):
        ''' Check if the current root level has a
        word entered at its root level or is this a
        possible leaf node.

        :returns: True if a word is at this level, False otherwise
        '''
        return self.root and self.root.get(Trie.VALUE, False)

    def mark_word(self, value):
        ''' Mark this node as having a word or a leaf node.

        :param value: The value to set for this leaf node
        '''
        self.root[Trie.VALUE] = value

    def unmark_word(self):
        ''' Mark this node as not having a word or leaf value

        :returns: The value at that node
        '''
        return self.root.pop(Trie.VALUE, None)

    def __contains__(self, word):
        ''' Test if the supplied word exists
        in the Trie (not the path up to a word).

        :param word: The word to test for existance of
        :returns: True if the word exists, False otherwise
        '''
        return Trie.VALUE in self.get_path(word).root
