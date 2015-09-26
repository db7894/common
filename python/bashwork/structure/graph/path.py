#!/usr/bin/env python
# -*- coding: latin-1 -*-
from bashwork.structure.graph.common import Color, Graph
from bashwork.structure.graph.traversal import GraphVisitor
from bashwork.structure.graph.traversal import graph_dfs_visit

class GraphPathExistsVisitor(GraphVisitor):
    ''' A simple illustration of checking if a path exists from
    source to sink in a graph.
    '''

    @classmethod
    def visit(klass, graph, source, sink):
        ''' Given a graph, run the path search
        on the supplied graph and return the results.

        :param graph: The graph to perform a path search on
        :param source: The starting point of the search
        :param sink: The ending point of the search
        :returns: The results of the topological sort
        '''
        visitor = klass(sink)
        graph_dfs_visit(graph, visitor, source)
        return visitor.path_exists

    def __init__(self, sink):
        self.sink = sink
        self.path_exists = False

    def on_discover_vertex(self, node):
        self.path_exists |= (node == self.sink)

def graph_dfs_path_exists(graph, source, sink):
    ''' A simple dfs search to check of a path exists
    between a source and a sink.

    :param graph: The graph to search
    :param source: The source to search from
    :param sink: The sink to search to
    :returns: True if there is a path, False otherwise
    '''
    stack   = [source]
    visited = set()
    while stack:
        node = stack.pop()
        if node == sink:
            return True

        for edge in graph.get_edges(node):
            if edge not in visited:
                stack.append(edge)
                visited.add(edge)
    return False
