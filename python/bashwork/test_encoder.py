#!/usr/bin/env python
import unittest
from bashwork.encoder import *

class EncodingTest(unittest.TestCase):

    def test_excel_encode(self):
        encoder = ExcelEncoder()
        self.assertEqual('', encoder.encode(0))
        self.assertEqual('Z', encoder.encode(26))
        self.assertEqual('AA', encoder.encode(27))

    def test_excel_decode(self):
        encoder = ExcelEncoder()
        self.assertEqual(26, encoder.decode('Z'))
        self.assertEqual(27, encoder.decode('AA'))

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
