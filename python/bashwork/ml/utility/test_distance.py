#!/usr/bin/env python
import unittest
import numpy as np
from bashwork.ml.utilities import distance

class DistanceTest(unittest.TestCase):

    def test_manhattan(self):
        a = np.arange( 0, 10)
        b = np.arange(10, 20)
        self.assertEqual(100, distance.manhattan(a, b))

    def test_euclidean(self):
        a = np.arange( 0, 10)
        b = np.arange(10, 20)
        self.assertEqual(31, int(distance.euclidean(a, b)))

    def test_chebyshevn(self):
        a = np.arange( 0, 10)
        b = np.arange(10, 20)
        self.assertEqual(10, distance.chebyshev(a, b))

    def test_minkowski(self):
        a = np.arange( 0, 10)
        b = np.arange(10, 20)
        self.assertEqual(100, distance.minkowski(a, b))

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
