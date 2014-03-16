#!/usr/bin/env python
import unittest
from bashwork.strings import *

class StringsTest(unittest.TestCase):

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

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
