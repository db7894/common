#!/usr/bin/env python
import unittest
from bashwork.structure.matrix import *

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class MatrixTest(unittest.TestCase):

    def test_zero_fill_matching_entries(self):
        matrix = [
            [1,1,1,1,1,1,0],
            [1,1,0,1,1,1,1],
            [1,1,0,1,0,1,1],
            [1,1,1,1,1,1,1],
            [1,1,1,1,1,1,1]]

        expect = [
            [0,0,0,0,0,0,0],
            [0,0,0,0,0,0,0],
            [0,0,0,0,0,0,0],
            [1,1,0,1,0,1,0],
            [1,1,0,1,0,1,0]]

        actual = zero_fill_matching_entries(matrix)
        self.assertEqual(actual, expect)

    def test_count_groups(self):
        matrix = [
            [0,0,1,1,0,0,0],
            [1,1,0,0,1,1,1],
            [1,1,0,0,0,1,1],
            [0,0,1,1,0,0,1],
            [0,0,1,1,0,0,1]]

        self.assertEqual(4, count_groups(matrix))

    def test_is_matrix_path(self):
        matrix = [
            [0,0,1,1,0,0,0],
            [1,0,0,0,1,1,1],
            [1,1,0,0,0,1,1],
            [0,0,1,1,0,0,0],
            [0,0,1,1,1,1,0]]

        self.assertTrue(is_matrix_path(matrix))

    def test_rotate_matrix_ninety_degress(self):
        matrix = [
            [1, 2, 3, 4],
            [5, 6, 7, 8],
            [9, 0, 1, 2],
            [3, 4, 5, 6]]

        expect = [
            [3, 9, 5, 1],
            [4, 0, 6, 2],
            [5, 1, 7, 3],
            [6, 2, 8, 4]]

        actual = rotate_matrix_ninety_degress(matrix)
        self.assertEqual(actual, expect)

        matrix = [
            [1, 2],
            [3, 4]]

        expect = [
            [3, 1],
            [4, 2]]

        actual = rotate_matrix_ninety_degress(matrix)
        self.assertEqual(actual, expect)

    def test_find_value_in_sorted_matrix(self):
        matrix = [
            [0,  1,  2, 10],
            [3,  4,  7, 11],
            [5,  6,  8, 12],
            [9, 13, 15, 20]]

        expect = (2, 2)
        actual = find_value_in_sorted_matrix(matrix, 8)
        self.assertEqual(actual, expect)

    def test_find_largest_submatrix_sum(self):
        matrix = [
            [3, 9, 5, 1],
            [4, 0, 6, 2],
            [5, 1, 7, 3],
            [6, 2, 8, 4]]

        expect = (66, (0, 0), (3, 3))
        actual = find_largest_submatrix_sum(matrix)
        self.assertEqual(actual, expect)

        matrix = [
            [3,   9,   5, 1],
            [4, -50,   6, 2],
            [5,   1, -70, 3],
            [6,   2,   8, 4]]

        expect = (20, (3, 0), (3, 3))
        actual = find_largest_submatrix_sum(matrix)
        self.assertEqual(actual, expect)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
