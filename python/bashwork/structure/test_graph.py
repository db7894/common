#!/usr/bin/env python
import unittest
from bashwork.structure.graph import Graph

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class GraphTest(unittest.TestCase):
    ''' Code to test that the graph is correct.
    '''

    def test_graph_init(self):
        graph = Graph([1,2,3,4], name='graph', weight=1)
        self.assertEqual('graph', graph['name'])
        self.assertEqual(1, graph['weight'])
        self.assertEqual([1,2,3,4], graph.get_nodes())

        graph = Graph(name='graph')
        self.assertEqual('graph', graph['name'])
        self.assertNotEqual(1, graph['weight'])
        self.assertEqual([], graph.get_nodes())

    def test_magic_methods(self):
        grapha = Graph([1,2,3,4], name='graph')
        grapha['type'] = 'weighted'

        graphb = Graph([5,6,7], name='graph')

        self.assertTrue(grapha == grapha)
        self.assertFalse(grapha != graphb)
        self.assertEqual(4, len(grapha))
        self.assertEqual(str(grapha), str(graphb))
        self.assertEqual(repr(grapha), repr(graphb))
        self.assertTrue(4 in grapha)
        self.assertFalse(4 in graphb)
        self.assertEqual('weighted', grapha['type'])

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
