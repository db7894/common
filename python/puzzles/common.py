#!/usr/bin/env python
# -*- coding: latin-1 -*-
from collections import defaultdict

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


class Graph(object):
    ''' A simple directed graph
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the graph

        :param nodes: The initial collection of nodes
        '''
        self.nodes = kwargs.get('nodes', set())
        self.edges = defaultdict(list)

    def add_edges(self, node, *nodes):
        ''' Adds a directed edge between the first
        node and the following nodes.

        :param node: The node to add an edge from
        :param nodes: The nodes to add an edge to
        '''
        nodes = list(nodes)
        self.nodes.update([node] + nodes)
        self.edges[node].extend(nodes)

    def get_nodes(self):
        ''' Retrieve a list of the current nodes

        :returns: A copy of the current list of nodes
        '''
        return list(self.nodes)
    
    def get_edges(self, node):
        ''' Returns the edges out of the supplied
        node.

        :param node: The node to get the edges for
        :returns: A list of the edges out of a node
        '''
        return self.edges[node]


class Trie(object):
    ''' A simple trie using dictionaries
    and terminal entries for complete words.
    '''

    VALUE = object()

    def __init__(self):
        ''' Initialize a new instance of the trie
        '''
        self.root = dict()

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


class Stack(object):
    '''
    '''

    def __init__(self): self.store = []
    def enqueue(self, value): self.store.append(value)
    def dequeue(self): return self.store.pop()


class Queue(object):
    '''
    '''

    def __init__(self): self.store = []
    def enqueue(self, value): self.store.insert(0, value)
    def dequeue(self): return self.store.pop()
