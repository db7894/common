#!/usr/bin/env python
import unittest
from bashwork.structure.graph import Graph
from bashwork.structure.graph.sorting import graph_toposort_recur
from bashwork.structure.graph.sorting import GraphToposortVisitor

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class GraphSortingTest(unittest.TestCase):
    ''' Code to test that the graph sorting is correct.
    '''

    def test_graph_toposort_recur(self):
        graph = Graph(range(6))
        graph.add_edges(5, 2, 0)
        graph.add_edges(4, 0, 1)
        graph.add_edge(2, 3)
        graph.add_edge(3, 1)
        actual = graph_toposort_recur(graph)
        expect = [5, 4, 2, 3, 1, 0]
        self.assertEquals(actual, expect)

    def test_graph_toposort_visitor(self):
        graph = Graph(range(6))
        graph.add_edges(5, 2, 0)
        graph.add_edges(4, 0, 1)
        graph.add_edge(2, 3)
        graph.add_edge(3, 1)
        actual = GraphToposortVisitor.visit(graph)
        expect = [5, 4, 2, 3, 1, 0]
        self.assertEquals(actual, expect)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
