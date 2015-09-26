#!/usr/bin/env python
import unittest
from bashwork.structure.strings import *

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class StringsTest(unittest.TestCase):

    def test_reverse_string(self):
        self.assertEqual('fedcba', reverse_string('abcdef'))
        self.assertEqual('edcba', reverse_string('abcde'))
        self.assertEqual('ba', reverse_string('ab'))
        self.assertEqual('a', reverse_string('a'))

    def test_are_chars_unique(self):
        self.assertTrue(are_chars_unique("abcdefghijklmnop"))
        self.assertFalse(are_chars_unique("abcdafghijklmnop"))
        self.assertTrue(are_chars_unique_array("abcdefghijklmnop"))
        self.assertFalse(are_chars_unique_array("abcdafghijklmnop"))

    def test_are_chars_unique_no_space(self):
        self.assertTrue(are_chars_unique_no_space("abcdefghijklmnop"))
        self.assertFalse(are_chars_unique_no_space("abcdafghijklmnop"))
        self.assertTrue(are_chars_unique_no_space2("abcdefghijklmnop"))
        self.assertFalse(are_chars_unique_no_space2("abcdafghijklmnop"))

    def test_are_strings_anagrams(self):
        self.assertTrue(are_strings_anagrams("march", "charm"))
        self.assertFalse(are_strings_anagrams("waffle", "laffaw"))
        self.assertTrue(are_strings_anagrams_array("march", "charm"))
        self.assertFalse(are_strings_anagrams_array("waffle", "laffaw"))

    def test_get_all_anagrams(self):
        words  = ['charm', 'march', 'arbor']
        actual = get_all_anagrams('charm', words)
        expect = ['charm', 'march']
        self.assertEqual(actual, expect)

    def test_is_rotation(self):
        self.assertTrue(is_rotation('hello', 'lohel'))
        self.assertFalse(is_rotation('hello', 'olhel'))

    def test_string_to_int(self):
        self.assertEqual(12345, string_to_int('12345'))

    def test_int_to_string(self):
        self.assertEqual('12345', int_to_string(12345))

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
