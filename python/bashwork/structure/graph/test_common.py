#!/usr/bin/env python
import unittest
from bashwork.structure.graph import Graph
from bashwork.structure.graph.common import copy_graph

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

    def test_graph_copy(self):
        source = Graph(['a', 'b', 'c', 'd', 'e'])
        source.add_edge('a', 'b')
        source.add_edge('a', 'c')
        source.add_edge('b', 'c')
        source.add_edge('b', 'd')
        source.add_edge('b', 'e')
        source.add_edge('d', 'e')
        output = copy_graph(source, 'a')

        expect = [ (s, d.keys()) for s,d in source.edges.items() if d.values()]
        actual = [ (s, d.keys()) for s,d in output.edges.items() if d.values()]
        self.assertEqual(expect, actual)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
