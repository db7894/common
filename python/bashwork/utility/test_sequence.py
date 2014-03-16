#!/usr/bin/env python
import unittest
from itertools import islice
from bashwork.utility.sequence import *

class SequenceTest(unittest.TestCase):

    def test_look_and_say_sequence(self):
        stream = sequence_generator(look_and_say_sequence)
        actual = take_n_of_sequence(stream, 10)
        expect = [0, 11, 21, 1211, 111221, 312211, 13112221, 1113213211, 31131211131221, 13211311123113112211L]
        self.assertEqual(expect, actual)

    def test_lucas_sequence(self):
        stream = sequence_generator(lucas_sequence)
        actual = take_n_of_sequence(stream, 10)
        expect = [0, 1, 2, 3, 5, 8, 13, 21, 34, 55]
        self.assertEqual(expect, actual)

    def test_square_sequence(self):
        stream = sequence_generator(square_sequence)
        actual = take_n_of_sequence(stream, 10)
        expect = [0, 1, 4, 9, 16, 25, 36, 49, 64, 81]
        self.assertEqual(expect, actual)

    def test_cube_sequence(self):
        stream = sequence_generator(cube_sequence)
        actual = take_n_of_sequence(stream, 10)
        expect = [0, 1, 8, 27, 64, 125, 216, 343, 512, 729]
        self.assertEqual(expect, actual)

    def test_fibonacci_sequence(self):
        stream = sequence_generator(fibonacci_sequence)
        actual = take_n_of_sequence(stream, 10)
        expect = [0, 1, 2, 3, 5, 8, 13, 21, 34, 55]
        self.assertEqual(expect, actual)

    def test_triangle_sequence(self):
        stream = sequence_generator(triangle_sequence)
        actual = take_n_of_sequence(stream, 10)
        expect = [0, 1, 3, 6, 10, 15, 21, 28, 36, 45]
        self.assertEqual(expect, actual)

    def test_pentagon_sequence(self):
        stream = sequence_generator(pentagon_sequence)
        actual = take_n_of_sequence(stream, 10)
        expect = [0, 1, 5, 12, 22, 35, 51, 70, 92, 117]
        self.assertEqual(expect, actual)

    def test_hexagon_sequence(self):
        stream = sequence_generator(hexagon_sequence)
        actual = take_n_of_sequence(stream, 10)
        expect = [0, 1, 6, 15, 28, 45, 66, 91, 120, 153]
        self.assertEqual(expect, actual)

    def test_lazy_caterer_sequence(self):
        stream = sequence_generator(lazy_caterer_sequence)
        actual = take_n_of_sequence(stream, 10)
        expect = [0, 2, 4, 7, 11, 16, 22, 29, 37, 46]
        self.assertEqual(expect, actual)

    def test_magic_number_sequence(self):
        stream = sequence_generator(magic_number_sequence)
        actual = take_n_of_sequence(stream, 10)
        expect = [0, 1, 5, 15, 34, 65, 111, 175, 260, 369]
        self.assertEqual(expect, actual)

    def test_catalan_sequence(self):
        stream = sequence_generator(catalan_sequence)
        actual = take_n_of_sequence(stream, 10)
        expect = [0, 1, 2, 5, 14, 42, 132, 429, 1430, 4862]
        self.assertEqual(expect, actual)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
