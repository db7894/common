#!/usr/bin/env python
import unittest
from bashwork.utility import distance

class DistanceTest(unittest.TestCase):

    def test_manhattan(self):
        a = range( 0, 10)
        b = range(10, 20)
        self.assertEqual(100, distance.manhattan(a, b))

    def test_euclidean(self):
        a = range( 0, 10)
        b = range(10, 20)
        self.assertEqual(31, int(distance.euclidean(a, b)))

    def test_chebyshevn(self):
        a = range( 0, 10)
        b = range(10, 20)
        self.assertEqual(10, distance.chebyshev(a, b))

    def test_minkowski(self):
        a = range( 0, 10)
        b = range(10, 20)
        self.assertEqual(100, distance.minkowski(a, b))

    def test_hamming(self):
        a = range(0, 10)
        b = range(5, 20)
        self.assertEqual(0, distance.hamming(a, b))

    def test_overlap(self):
        a = (0, 10)
        b = (5, 15)
        self.assertEqual(5, distance.overlap(a, b))

    def test_does_overlap(self):
        a = (0, 10)
        b = (5, 15)
        self.assertEqual(True, distance.does_overlap(a, b))

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
