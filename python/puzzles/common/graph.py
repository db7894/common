#!/usr/bin/env python
# -*- coding: latin-1 -*-
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
