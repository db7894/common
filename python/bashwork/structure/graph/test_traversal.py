#!/usr/bin/env python
import unittest
from bashwork.structure.graph import Graph
from bashwork.structure.graph.traversal import *
from bashwork.structure.graph.path import *

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class GraphTraversalTest(unittest.TestCase):

    def setUp(self):
        self.graph = Graph(['a', 'b', 'c', 'd', 'e'])
        self.graph.add_edge('a', 'b')
        self.graph.add_edge('a', 'c')
        self.graph.add_edge('b', 'c')
        self.graph.add_edge('b', 'd')
        self.graph.add_edge('b', 'e')
        self.graph.add_edge('d', 'e')

    def test_graph_bfs_visit(self):
        visitor = GraphPathExistsVisitor('e')
        graph_bfs_visit(self.graph, visitor, 'a')
        self.assertTrue(visitor.path_exists)

        visitor = GraphPathExistsVisitor('a')
        graph_bfs_visit(self.graph, visitor, 'b')
        self.assertFalse(visitor.path_exists)

    def test_graph_dfs_visit(self):
        visitor = GraphPathExistsVisitor('e')
        graph_dfs_visit(self.graph, visitor, 'a')
        self.assertTrue(visitor.path_exists)

        visitor = GraphPathExistsVisitor('a')
        graph_dfs_visit(self.graph, visitor, 'b')
        self.assertFalse(visitor.path_exists)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
