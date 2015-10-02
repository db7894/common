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
        string   = "abcdefghijklmnop"
        self.assertTrue(is_rotation(string, string[10:] + string[:10]))
        self.assertTrue(is_rotation(string, string[:10] + string[10:]))
        self.assertFalse(is_rotation(string, ''.join(reversed(string))))

    def test_string_set_cover(self):
        letters = 'abcd'
        string  = 'axkekfbabxxxcdkeckdabyycd'
        actual  = string_set_cover(letters, string)
        expect  = (5, 16)
        self.assertEqual(actual, expect)

    def test_ransom_note(self):
        note = "we have your computer"
        magazines = "comping a theater ticket wearing a very nice puntour"
        actual = ransom_note(note, magazines)
        self.assertTrue(actual)

    def test_string_to_int(self):
        self.assertEqual(12345, string_to_int('12345'))

    def test_int_to_string(self):
        self.assertEqual('12345', int_to_string(12345))

    def test_all_parentesis_sets(self):
        actual = all_parenthesis_sets(3)
        expect = {'((()))', '(()())', '(())()', '()(())', '()()()'}
        self.assertEqual(actual, expect)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
