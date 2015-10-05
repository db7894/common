#!/usr/bin/env python
import unittest
from bashwork.structure.sets import *

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class SetsTest(unittest.TestCase):

    def test_subsets(self):
        array  = [1, 2, 3]
        expect = [set([]), set([1]), set([2]), set([1, 2]), set([3]), set([1, 3]), set([2, 3]), set([1, 2, 3])]
        actual = subsets(array)
        self.assertEqual(actual, expect)

    def test_subsets_recursive(self):
        array  = [1, 2, 3]
        expect = [set([]), set([3]), set([2]), set([2, 3]), set([1]), set([1, 3]), set([1, 2]), set([1, 2, 3])]
        actual = subsets_recursive(array)
        self.assertEqual(actual, expect)

    def test_permutations(self):
        word = "abc"
        expect = ['cba', 'bca', 'bac', 'cab', 'acb', 'abc']
        actual = permutations(word)
        self.assertEqual(actual, expect)

    def test_permutations_recursive(self):
        word = "abc"
        expect = ['abc', 'bac', 'bca', 'acb', 'cab', 'cba']
        actual = list(permutations_recursive(word))
        self.assertEqual(actual, expect)

    def test_subset_sum_zeromod(self):
        array  = [429, 334, 62, 711, 704, 763, 98, 733, 721, 995]
        expect = [
            [429, 334, 62, 711, 704],
            [62, 711, 704, 763],
            [711, 704, 763, 98, 733, 721],
            [429, 334, 62, 711, 704, 763, 98, 733, 721, 995]
        ]
        actual = subset_sum_zeromod(array)
        self.assertEqual(actual, expect)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
