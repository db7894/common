#!/usr/bin/env python
# -*- coding: latin-1 -*-
from collections import defaultdict
from copy import copy as clone

class Graph(object):
    ''' A directed graph that allows adding properties on
    edges and nodes as well as the graph itself.
    '''

    def __init__(self, nodes=None, **attrs):
        ''' Initialize a new instance of the graph

        :param nodes: The initial collection of nodes
        '''
        self.nodes = defaultdict(dict)
        self.edges = defaultdict(lambda: defaultdict(dict))
        self.attrs = {}
        self.attrs.update(attrs)
        self.add_nodes(nodes or [])

    def add_node(self, node, **attrs):
        ''' Adds a new node to the graph with the
        supplied attributes.

        :param node: The node to add to the graph
        '''
        self.nodes[node].update(attrs)

    def add_nodes(self, nodes, **attrs):
        ''' Adds the supplied nodes to the graph with the
        supplied attributes.

        :param nodes: The nodes to add to the graph
        '''
        for node in nodes:
            self.add_node(node, **attrs)

    def add_edge(self, node, sink, **attrs):
        ''' Adds a directed edge between the first
        node and the supplied sink along with the supplied
        attributes.

        :param node: The node to add an edge from
        :param sink: The node to add an edge to
        '''
        if node not in self.nodes:
            raise KeyError("source node %s does not exist" % source)

        if sink not in self.nodes:
            raise KeyError("sink node %s does not exist" % sink)

        self.edges[node][sink].update(attrs)

    def add_edges(self, node, *nodes, **attrs):
        ''' Adds a directed edge between the first
        node and the following nodes.

        :param node: The node to add an edge from
        :param nodes: The nodes to add an edge to
        '''
        for sink in list(nodes):
            self.add_edge(node, sink, **attrs)

    def get_node(self, node):
        ''' Retrieve the attributes for the supplied node

        :param node: The node to retrieve attributes for
        :returns: The supplied node's attributes
        '''
        return self.nodes[node]

    def get_nodes(self):
        ''' Retrieve a list of the current nodes

        :returns: A copy of the current list of nodes
        '''
        return self.nodes.keys()

    def get_edge(self, node, sink):
        ''' Retrieve the attributes for the supplied edge
        between the node and sink.

        :param node: The source node for the requested edge
        :param sink: The sink node for the requested edge
        :returns: The supplied edge's attributes
        '''
        return self.edges[node][sink]
    
    def get_edges(self, node):
        ''' Returns the outgoing edges from the supplied
        source node.

        :param node: The node to get the edges for
        :returns: A list of the edges out of a node
        '''
        return self.edges[node].keys()

    def __key(self):             return tuple((k, tuple(v.keys())) for k,v in self.edges.items())
    def __len__(self):           return len(self.nodes)
    def __eq__(self, other):     return other and (other.__key() == self.__key())
    def __ne__(self, other):     return other and (other.__key() != self.__key())
    def __repr__(self):          return self.attrs.get('name', '')
    def __str__(self):           return self.attrs.get('name', '')
    def __hash__(self):          return hash(self.__key())
    def __getitem__(self, k):    return self.attrs.get(k, None)
    def __setitem__(self, k, v): self.attrs[k] = v
    def __contains__(self, v):   return (v in self.nodes)


def copy_graph(source, root):
    ''' Given a source graph and an initial root node
    to start exploring from, create and return a clone
    of the graph.

    :param source: The source graph to create a clone of
    :param root: The root node to start exploration from
    :returns: The cloned graph
    '''
    graph    = Graph()
    node_map = {}
    queue    = [root] 

    def get_node_clone(node):
        if node not in node_map:
            node_map[node] = clone(node) 
            graph.add_node(node_map[node])
        return node_map[node]

    while queue:
        node = queue.pop()
        for edge in source.get_edges(node):
            graph.add_edge(get_node_clone(node), get_node_clone(edge))
            queue.insert(0, get_node_clone(edge))
    return graph

class Color(object):
    ''' An enumeration that defines the current
    visited status of a node in a graph. These 

    * White - The node has yet to be visited
    * Gray  - The node has been seen, but not explored
    * Black - The node has been fully explored
    '''
    White = 0
    Gray  = 1
    Black = 2

class BfsGraphVisitor(object):
    ''' A visitor base class for creating a custom
    graph BFS visitor.

    TODO list of graph recorders
    - distance recorder
    - time stamper
    - predecessor recorder
    '''

    def on_initialize_vertex(self, node): pass
    def on_discover_vertex(self, node): pass
    def on_examine_vertex(self, node): pass
    def on_finish_vertex(self, node): pass
    def on_tree_edge(self, node, edge): pass
    def on_non_tree_edge(self, node, edge): pass
    def on_gray_edge(self, node, edge): pass
    def on_black_edge(self, node, edge): pass

def graph_bfs_visit(graph, root, visitor):
    ''' Given a source graph and a root node
    to start exploring from, perform a bfs
    traversal of the graph while emitting events
    to the supplied visitor.

    :param graph: The graph to perform a traversal of
    :param root: The root node to start exploring from
    :param visitor: The visitor to send events to
    '''
    queue = [ root ]
    color = {}

    for node in graph.get_nodes():
        color[node] = Color.White;            visitor.on_initialize_vertex(node)
    color[root] = Color.Gray;                 visitor.on_discover_vertex(root)

    while queue:
        node = queue.pop();                   visitor.on_examine_vertex(node)
        for edge in graph.get_edges(node):    visitor.on_examine_edge(node, edge);
            if color[edge] == Color.White:    visitor.on_tree_edge(node, edge);
                color[edge] = Color.Gray;     visitor.on_discover_vertex(edge);
                queue.insert(0, edge)
            else:                             visitor.on_non_tree_edge(node, edge);
                if color[edge] == Color.Gray: visitor.on_gray_edge(node, edge);
                else:                         visitor.on_black_edge(node, edge)
        color[node] = Color.Black;            visitor.on_finish_vertex(node)

def graph_dfs_visit(graph, root, visitor):
    ''' Given a source graph and a root node
    to start exploring from, perform a dfs
    traversal of the graph while emitting events
    to the supplied visitor. If a root is not defined, then
    the graph will attempt to follow all nodes until they
    are all explored.

    :param graph: The graph to perform a traversal of
    :param root: The root node to start exploring from
    :param visitor: The visitor to send events to
    '''
    roots = [ root ] if root else graph.get_nodes()
    color = {}

    for node in graph.get_nodes():
        color[node] = Color.White;                visitor.on_initialize_vertex(node)

    for root in roots:
        if color[root] != Color.White:
            continue
        stack = [ (root, iter(graph.get_edges(root))) ]
        color[root] = Color.Gray;                 visitor.on_discover_vertex(root)

        while stack:
            try:
                node, edges = stack[-1];          visitor.on_examine_vertex(node)
                edge = next(edges);               visitor.on_examine_edge(node, edge);
                if color[edge] == Color.White:    visitor.on_tree_edge(node, edge);
                    color[edge] = Color.Gray;     visitor.on_discover_vertex(edge);
                    stack.append((edge, iter(graph.get_edges(edge))))
                elif color[edge] == Color.Gray:   visitor.on_back_edge(node, edge);
                else:                             visitor.on_forward_or_cross_edge(node, edge)
            except StopIteration:
                node, _ = stack.pop()
                color[node] = Color.Black;        visitor.on_finish_vertex(node)
