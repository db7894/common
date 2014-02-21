#!/usr/bin/env python
# -*- coding: latin-1 -*-
from collections import defaultdict

class Graph(object):
    ''' A simple directed graph
    '''

    def __init__(self, nodes=set(), **attrs):
        ''' Initialize a new instance of the graph

        :param nodes: The initial collection of nodes
        '''
        self.nodes = nodes
        self.edges = defaultdict(list)
        self.attrs = {} # TODO edge and node attributes
        self.attrs.update(attrs)

    def add_node(self, node, **attrs):
        ''' Adds a new node to the graph with the
        supplied attributes.

        :param node: The node to add to the graph
        '''
        self.nodes.add(node)
        #if node not in self.nodes:
        #    self.nodes[node] = {}
        #    self.edges[node] = {}
        #self.nodes[node].update(attrs)

    def add_nodes(self, nodes, **attrs):
        ''' Adds the supplied nodes to the graph with the
        supplied attributes.

        :param nodes: The nodes to add to the graph
        '''
        for node in nodes:
            self.add_node(node, **attrs)

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

    def __key(self):           return (self.nodes, self.edges)
    def __len__(self):         return len(self.nodes)
    def __eq__(self, other):   return other and (other.__key() == self.__key())
    def __ne__(self, other):   return other and (other.__key() != self.__key())
    def __repr__(self):        return self.attr.get('name', '')
    def __str__(self):         return self.attr.get('name', '')
    def __hash__(self):        return hash(self.__key())
    def __contains__(self, v): return (v in self.nodes)
