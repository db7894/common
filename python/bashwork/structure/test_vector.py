#!/usr/bin/env python
import unittest
from bashwork.structure.vector import *

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class VectorTest(unittest.TestCase):

    def test_vector_interface(self):
        expect = [1, 2, 3, 4, 5]
        vector = Vector.create(expect)
        self.assertEqual(vector.to_list(), expect)
        self.assertEqual(list(vector), expect)
        self.assertEqual(str(vector), str(expect))
        self.assertEqual(repr(vector), repr(expect))
        self.assertEqual(len(vector), len(expect))
        self.assertTrue(bool(vector))
        self.assertFalse(vector.is_empty())
        self.assertTrue(5 in vector)
        self.assertEqual(vector.first, 1)
        self.assertEqual(vector.last, 5)
        self.assertEqual(vector.find(3), 2)
        self.assertEqual(vector.find(6), None)

    def test_vector_clear(self):
        expect = [1, 2, 3, 4, 5]
        vector = Vector.create(expect)
        self.assertFalse(vector.is_empty())

        vector.clear()
        self.assertTrue(vector.is_empty())
        # TODO first / last should throw here

    def test_partition_around_median(self):
        index  = 5 # 4
        vector = [5, 2, 6, 1, 2, 4, 7, 4, 8, 1, 0]
        expect = [0, 2, 1, 1, 2, 4, 4, 8, 7, 6, 5]
        actual = partition_around_median(vector, index)
        self.assertEqual(actual, expect)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
