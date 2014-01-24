#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a graph of connected letters (directed),
starting at any point in the graph attempt to walk
the graph and spell a National Football League team
name.
'''
from collections import defaultdict


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


def find_team_names(graph, teams):
    ''' Given a graph of letter connections and a database
    (in this case a trie) of player teams, find which teams
    can be completed using the supplied graph (BFS).

    :param graph: The letter graph to search with
    :param teams: A dictionary of the available teams
    :returns: A generator of the possible solutions
    '''
    queue = graph.get_nodes()
    while queue:
        word = queue.pop()
        if word in teams:
            yield word
        else:
            for letter in graph.get_edges(word[-1]):
                path = word + letter
                if teams.has_path(path):
                    queue.insert(0, path)


def print_team_name(name):
    ''' Given a team name, print it using the
    correct spacing.

    :param name: The name to find the correct spacing of
    '''
    for team in TEAMS:
        if team.replace(' ', '') == name:
            print team
            break
    else: print "no matching team name for: " + team

#------------------------------------------------------------
# constants
#------------------------------------------------------------

TEAMS = [
    'arizona cardinals',
    'atlanta falcons',
    'baltimore ravens',
    'buffalo bills',
    'carolina panthers',
    'chicago bears',
    'cincinnati bengals',
    'cleveland browns',
    'dallas cowboys',
    'denver broncos',
    'detroit lions',
    'green bay packers',
    'houston texans',
    'indianapolis colts',
    'jacksonville jaguars',
    'kansas city chiefs',
    'miami dolphins',
    'minnesotta vikings',
    'new england patriots',
    'new orleans saints',
    'new york jets',
    'oakland raiders',
    'philadelphia eagles',
    'pittsburgh steelers',
    'san diego chargers',
    'san francisco forty niners',
    'seattle seahawks',
    'tampa bay buccaneers',
    'tennessee titans',
    'washington redskins',
]


if __name__ == '__main__':
    teams = Trie()
    teams.add_words(TEAMS)

    graph = Graph(nodes=set('ihgalpsed'))
    graph.add_edges('i', 'h', 'l', 'a')
    graph.add_edges('h', 'i', 'p')
    graph.add_edges('g', 'a', 'l')
    graph.add_edges('a', 'i', 'g', 'l', 'e', 'd')
    graph.add_edges('l', 'i', 'g', 'a', 'e', 'p')
    graph.add_edges('p', 'h', 'l')
    graph.add_edges('s', 'e')
    graph.add_edges('e', 'l', 'a', 's', 'd')
    graph.add_edges('d', 'e', 'a')

    for name in find_team_names(graph, teams):
        print_team_name(name)


