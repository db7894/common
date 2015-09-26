#!/usr/bin/env python
import unittest
from bashwork.encoder import *

class EncodingTest(unittest.TestCase):

    def test_encoder_interface(self):
        encoder = Encoder()
        self.assertRaises(NotImplementedError, lambda: encoder.encode(None))
        self.assertRaises(NotImplementedError, lambda: encoder.decode(None))

    def test_excel_encode(self):
        encoder = ExcelEncoder()
        self.assertEqual('', encoder.encode(0))
        self.assertEqual('Z', encoder.encode(26))
        self.assertEqual('AA', encoder.encode(27))

    def test_excel_decode(self):
        encoder = ExcelEncoder()
        self.assertEqual(26, encoder.decode('Z'))
        self.assertEqual(27, encoder.decode('AA'))

    def test_ellis_gamma_decode(self):
        encoder = EllisGammaEncoder()
        self.assertEqual(123, encoder.decode('0000001111011'))
        self.assertEqual(456, encoder.decode('00000000111001000'))
        self.assertEqual(789, encoder.decode('0000000001100010101'))

    def test_ellis_gamma_encode(self):
        encoder = EllisGammaEncoder()
        self.assertEqual('0000001111011', encoder.encode(123))
        self.assertEqual('00000000111001000', encoder.encode(456))
        self.assertEqual('0000000001100010101', encoder.encode(789))

    def test_ellis_gamma_decode_stream(self):
        encoder = EllisGammaEncoder()
        stream = '0000001111011' + '00000000111001000' + '0000000001100010101'
        actual = encoder.decode_stream(stream)
        expect = [123, 456, 789]
        self.assertEqual(actual, expect)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
