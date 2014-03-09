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

class Event(object):
    ''' A collection of visitor events that are emitted
    from the various graph visitors. Vertex and edge events
    return the following messages:

    * vertex - (event, vertex)
    * edge   - (event, source, sink)
    '''
    InitializeVertex   = 0x01
    DiscoverVertex     = 0x02
    ExamineVertex      = 0x03
    FinishVertex       = 0x04
    TreeEdge           = 0x10
    NonTreeEdge        = 0x20
    ExamineEdge        = 0x30
    GrayEdge           = 0x40
    BlackEdge          = 0x50
    ForwardOrCrossEdge = 0x60
    BackEdge           = 0x70

    @staticmethod
    def is_vertex_event(event):
        return bool(event & 0x0F)

    @staticmethod
    def is_edge_event(event):
        return bool(event & 0xF0)

    @classmethod
    def get_name(klass, event):
        values = dict((v, k) for k, v in klass.__dict__.iteritems()
            if not k.startswith('__') and not callable(v))
        return values.get(event, None)

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
        color[node] = Color.White;            
        visitor.send((Event.InitializeVertex, node))

    color[root] = Color.Gray;
    visitor.send((Event.DiscoverVertex, root))

    while queue:
        node = queue.pop();
        visitor.send((Event.ExamineVertex, node))
        for edge in graph.get_edges(node):
            visitor.send((Event.ExamineEdge, node, edge))
            if color[edge] == Color.White:
                color[edge] = Color.Gray;
                queue.insert(0, edge)
                visitor.send((Event.TreeEdge, node, edge))
                visitor.send((Event.DiscoverVertex, edge))
            else:
                visitor.send((Event.NonTreeEdge, node, edge))
                if color[edge] == Color.Gray:
                    visitor.send((Event.GrayEdge, node, edge))
                else: visitor.send((Event.BlackEdge, node, edge))
        color[node] = Color.Black;
        visitor.send((Event.FinishVertex, node))

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
        color[node] = Color.White
        visitor.send((Event.InitializeVertex, node))

    for root in roots:
        if color[root] != Color.White:
            continue
        stack = [ (root, iter(graph.get_edges(root))) ]
        color[root] = Color.Gray
        visitor.send((Event.DiscoverVertex, root))

        while stack:
            try:
                node, edges = stack[-1]
                edge = next(edges)
                visitor.send((Event.ExamineVertex, node))
                visitor.send((Event.ExamineEdge, node, edge))
                if color[edge] == Color.White:
                    color[edge] = Color.Gray
                    stack.append((edge, iter(graph.get_edges(edge))))
                    visitor.send((Event.TreeEdge, node, edge))
                    visitor.send((Event.DiscoverVertex, edge))
                elif color[edge] == Color.Gray:
                    visitor.send((Event.BackEdge, node, edge))
                else: visitor.send((Event.ForwardOrCrossEdge, node, edge))
            except StopIteration:
                node, _ = stack.pop()
                color[node] = Color.Black
                visitor.send((Event.FinishVertex, node))

def graph_topo_visit(graph, visitor):
    ''' Given a source graph, perform a topological
    traversal of the graph while emitting events
    to the supplied visitor.

    :param graph: The graph to perform a traversal of
    :param visitor: The visitor to send events to
    '''
    color = {}

    for node in graph.get_nodes():
        color[node] = Color.White
        visitor.send((Event.InitializeVertex, node))

    for node in color.keys():
        if color[root] != Color.White:
            continue

        stack = [ (node, iter(graph.get_edges(node))) ]
        color[node] = Color.Gray
        visitor.send((Event.DiscoverVertex, node))

        while stack:
            try:
                node, edges = stack[-1]
                edge = next(edges)
                visitor.send((Event.ExamineVertex, node))
                visitor.send((Event.ExamineEdge, node, edge))

                if color[edge] == Color.White:
                    color[edge] = Color.Gray
                    stack.append((edge, iter(graph.get_edges(edge))))
                    visitor.send((Event.TreeEdge, node, edge))
                    visitor.send((Event.DiscoverVertex, edge))
                elif color[edge] == Color.Gray:
                    visitor.send((Event.BackEdge, node, edge))

            except StopIteration:
                node, _ = stack.pop()
                color[node] = Color.Black
                visitor.send((Event.FinishVertex, node))

if __name__ == "__main__":
    from common import Graph
    def print_visitor(graph):
        while True:
            message = yield
            print Event.get_name(message[0]),
            if Event.is_vertex_event(message[0]):
                print "\t[{}]".format(message[1])
            else: print "\t[{}]->[{}]".format(message[1], message[2])

    graph = Graph(['a', 'b', 'c', 'd', 'e', 'f'])
    graph.add_edges('a', 'b', 'c')
    graph.add_edges('b', 'c', 'e')
    graph.add_edges('c', 'd', 'f')
    graph.add_edges('d', 'e')
    graph.add_edges('e', 'f')
    visitor = print_visitor(graph)
    visitor.next()
    graph_dfs_visit(graph, 'a', visitor)
