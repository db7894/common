#!/usr/bin/env python
import unittest
from bashwork.security.encryption import *

class EncryptionTest(unittest.TestCase):

    def test_hmac_sign(self):
        phrase = "this is the secret key"
        string = "this is the secret message"
        actual = hmac_sign(phrase, string)
        expect = '8d\x94\xfb\xfd\xde-\xd2\x19\xea0\x9f\x89o\xeaV'
        self.assertEqual(actual, expect)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
