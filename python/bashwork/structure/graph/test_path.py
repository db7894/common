#!/usr/bin/env python
import unittest
from bashwork.structure.graph import Graph
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

    def test_graph_dfs_path_exists(self):
        self.assertTrue(graph_dfs_path_exists(self.graph, 'a', 'e'))
        self.assertFalse(graph_dfs_path_exists(self.graph, 'b', 'a'))

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
