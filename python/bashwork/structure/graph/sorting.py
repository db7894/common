#!/usr/bin/env python
# -*- coding: latin-1 -*-
from bashwork.structure.graph.exceptions import GraphCycleException
from bashwork.structure.graph.common import Color, Graph
from bashwork.structure.graph.traversal import GraphVisitor
from bashwork.structure.graph.traversal import graph_dfs_visit

class GraphToposortVisitor(GraphVisitor):
    ''' A simple illustration of performing a topological
    sort using a DFS visitor.
    '''

    @classmethod
    def visit(klass, graph):
        ''' Given a graph, run a topological sort
        on the supplied graph and return the results.

        :param graph: The graph to perform a topological sort on
        :returns: The results of the topological sort
        '''
        visitor = klass()
        graph_dfs_visit(graph, visitor)
        return visitor.result

    def __init__(self): self.result = []
    def on_finish_vertex(self, node): self.result.insert(0, node)
    def on_back_edge(self, node, edge):
        raise GraphCycleException()

def graph_toposort_recur(graph):
    ''' Given a source graph, perform a topological
    sort of the graph.

    :param graph: The graph to perform a traversal of
    :returns: The topological sorting of the graph
    '''
    colors = { node: Color.White for node in graph.get_nodes() }
    result = []

    def topological_search(node):
        colors[node] = Color.Gray
        for edge in graph.get_edges(node):
            if colors[edge] == Color.White:
                topological_search(edge)
            elif colors[edge] == Color.Gray:
                raise GraphCycleException()
            else: pass # visited already

        colors[node] = Color.Black
        result.insert(0, node)

    for node in colors.keys():
        if colors[node] == Color.White:
            topological_search(node)
    return result
