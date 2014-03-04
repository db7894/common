#!/usr/bin/env python
# -*- coding: latin-1 -*-

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
