#!/usr/bin/env python
import unittest
from bashwork.algorithm.sorting.insertion import *

class InsertionSortTest(unittest.TestCase):

    def test_sort(self):
        coll = [1, 4, 7, 2, 5, 3, 6, 9, 8]
        expect = [1, 2, 3, 4, 5, 6, 7, 8, 9]
        actual = sort(coll)
        self.assertEqual(actual, expect)

    def test_sort_clone(self):
        coll = [1, 4, 7, 2, 5, 3, 6, 9, 8]
        expect = [1, 2, 3, 4, 5, 6, 7, 8, 9]
        actual = sort_clone(coll)
        self.assertEqual(actual, expect)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
