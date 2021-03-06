#!/usr/bin/env python
# -*- coding: latin-1 -*-
from bashwork.structure.graph.common import Color

class GraphVisitor(object):
    ''' A visitor base class for creating a custom
    graph BFS visitor.

    TODO list of graph recorders
    - distance recorder
    - time stamper
    - predecessor recorder
    '''

    def on_initialize_vertex(self, node):
        ''' Invoked on every vertex before the start of
        the search.

        :param node: The node that is being initialized
        '''
        pass

    def on_discover_vertex(self, node):
        ''' Invoked on every vertex the first time they
        are seen in the search.

        :param node: The node that is being initialized
        '''
        pass

    def on_examine_vertex(self, node):
        ''' Invoked on every vertex before its edges are
        to be examined.

        :param node: The node that is being examined
        '''
        pass

    def on_finish_vertex(self, node):
        ''' Invoked on every vertex after its edges have
        been examined.

        :param node: The node that is being finished
        '''
        pass

    def on_examine_edge(self, node, edge):
        ''' Invoked on every edge before its sink is
        to be examined.

        :param node: The source node of the edge
        :param edge: The destination node of the edge
        '''
        pass

    def on_tree_edge(self, node, edge):
        ''' Invoked if the supplied edge is a tree edge.

        :param node: The source node of the edge
        :param edge: The destination node of the edge
        '''
        pass

    def on_non_tree_edge(self, node, edge):
        ''' Invoked if the supplied edge is not a tree edge.

        :param node: The source node of the edge
        :param edge: The destination node of the edge
        '''
        pass

    def on_gray_edge(self, node, edge):
        ''' Invoked if the target vertex is colored gray
        at the time of examination.

        :param node: The source node of the edge
        :param edge: The destination node of the edge
        '''
        pass

    def on_black_edge(self, node, edge):
        ''' Invoked if the target vertex is colored black
        at the time of examination.

        :param node: The source node of the edge
        :param edge: The destination node of the edge
        '''
        pass

    def on_back_edge(self, node, edge):
        ''' Invoked if the target vertex has an edge
        pointing back (indicates a cycle).

        :param node: The source node of the edge
        :param edge: The destination node of the edge
        '''
        pass

    def on_forward_or_cross_edge(self, node, edge):
        ''' Invoked if the target vertex has an edge
        pointing forward or across.

        :param node: The source node of the edge
        :param edge: The destination node of the edge
        '''
        pass

def graph_bfs_visit(graph, visitor, root):
    ''' Given a source graph and a root node
    to start exploring from, perform a bfs
    traversal of the graph while emitting events
    to the supplied visitor.

    :param graph: The graph to perform a traversal of
    :param visitor: The visitor to send events to
    :param root: The root node to start exploring from
    '''
    queue = [ root ]
    color = {}

    for node in graph.get_nodes():
        color[node] = Color.White;            visitor.on_initialize_vertex(node)
    color[root] = Color.Gray;                 visitor.on_discover_vertex(root)

    while queue:
        node = queue.pop();                   visitor.on_examine_vertex(node)
        for edge in graph.get_edges(node):  
            visitor.on_examine_edge(node, edge)
            if color[edge] == Color.White:   
                visitor.on_tree_edge(node, edge)
                color[edge] = Color.Gray;     visitor.on_discover_vertex(edge)
                queue.insert(0, edge)
            else:                            
                visitor.on_non_tree_edge(node, edge)
                if color[edge] == Color.Gray: visitor.on_gray_edge(node, edge)
                else:                         visitor.on_black_edge(node, edge)
        color[node] = Color.Black;            visitor.on_finish_vertex(node)

def graph_dfs_visit(graph, visitor, root=None):
    ''' Given a source graph and a root node
    to start exploring from, perform a dfs
    traversal of the graph while emitting events
    to the supplied visitor. If a root is not defined, then
    the graph will attempt to follow all nodes until they
    are all explored.

    :param graph: The graph to perform a traversal of
    :param visitor: The visitor to send events to
    :param root: The root node to start exploring from
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
                edge = next(edges);               visitor.on_examine_edge(node, edge)
                if color[edge] == Color.White: 
                    visitor.on_tree_edge(node, edge)
                    color[edge] = Color.Gray;     visitor.on_discover_vertex(edge)
                    stack.append((edge, iter(graph.get_edges(edge))))
                elif color[edge] == Color.Gray:   visitor.on_back_edge(node, edge)
                else:                             visitor.on_forward_or_cross_edge(node, edge)
            except StopIteration:
                node, _ = stack.pop()
                color[node] = Color.Black;        visitor.on_finish_vertex(node)

def graph_topo_visit_recur(graph, root, visitor):
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
    color  = {}
    result = []

    for node in graph.get_nodes():
        color[node] = Color.White;                visitor.on_initialize_vertex(node)

    def topo_search(node):
        color[root] = Color.Gray;                 visitor.on_discover_vertex(node)
        for edge in graph.get_edges(node):
            if color[root] == Color.White:
                topo_search(edge)
            else:                                 visitor.on_back_edge(edge)
        color[node] = Color.Black;                visitor.on_finish_vertex(node)
        result.append(node)

    for node in color.keys():
        if color[root] == Color.White:
            topo_search(node)

    while result:
        visitor.on__vertex(result.pop())
