#!/usr/bin/env python
import unittest
from bashwork.algorithm.sorting.shuffle import *

class ShuffleTest(unittest.TestCase):

    def test_shuffle(self):
        coll = range(0, 10)
        shuf = shuffle(coll)
        self.assertNotEqual(coll, sorted(shuf)) # reference

    def test_shuffle_clone(self):
        coll = range(0, 10)
        shuf = shuffle_copy(coll)
        self.assertNotEqual(coll, shuf)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
