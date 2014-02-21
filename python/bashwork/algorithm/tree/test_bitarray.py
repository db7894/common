#!/usr/bin/env python
import unittest
from bitarray import BitArray

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class BitArrayTest(unittest.TestCase):
    ''' Code to validate that the bitarray implementation is correct.
    '''

    #def test_init(self):
    #    ''' Test that the bit array is inited correctly '''
    #    self.assertEqual(8, BitArray(block=4).block)

    def test_magic_methods(self):
        ''' Test that the magic methods work correctly '''
        barray = BitArray(array=[0x12, 0x34])
        narray = BitArray(array=[0x34, 0x12])
        self.assertTrue(0x12 in barray)
        self.assertTrue(0x56 not in barray)
        self.assertEqual(-2861452931799617962, hash(barray))
        self.assertEqual(70, len(barray))
        self.assertEqual(list(iter(barray)), barray.to_bit_list())
        self.assertEqual(list(reversed(barray)), list(reversed(barray.to_bit_list())))
        self.assertTrue(barray  == barray)
        self.assertFalse(barray == narray)
        self.assertTrue(barray  != narray)
        self.assertFalse(barray != barray)
        self.assertTrue(barray  >= barray)
        self.assertTrue(barray  <= barray)
        self.assertTrue(barray   > narray)
        self.assertTrue(narray   < barray)
        self.assertEqual(narray  & barray, BitArray(array=[0x10, 0x10]))
        self.assertEqual(narray  ^ barray, BitArray(array=[0x26, 0x26]))
        self.assertEqual(narray  | barray, BitArray(array=[0x36, 0x36]))
        self.assertEqual(-barray, BitArray(array=[0xFFFFFFFFFFFFFFEE, 0xFFFFFFFFFFFFFFCC]))
        self.assertEqual(~barray, BitArray(array=[0xFFFFFFFFFFFFFFED, 0xFFFFFFFFFFFFFFCB]))
        self.assertEqual(+barray, BitArray(array=[0x12, 0x34]))

    #------------------------------------------------------------
    # test information operations
    #------------------------------------------------------------
    def test_first_set_bit(self):
        ''' Test that the first_set_bit works correctly '''
        cases = {
            0xF1: 0,
            0xF2: 1,
            0xF4: 2,
            0xF8: 3,
        }
        for array, expected in cases.items():
            barray = BitArray(array=[array])
            self.assertEqual(expected, barray.first_set_bit)

    def test_first_clear_bit(self):
        ''' Test that the first_clear_bit works correctly '''
        cases = {
            0xF0: 0,
            0xF1: 1,
            0xF3: 2,
            0xF7: 3,
        }
        for array, expected in cases.items():
            barray = BitArray(array=[array])
            self.assertEqual(expected, barray.first_clear_bit)

    def test_last_set_bit(self):
        ''' Test that the last_set_bit works correctly '''
        cases = {
            0x00: None,
            0x1F: 4,
            0x2F: 5,
            0x4F: 6,
            0x8F: 7,
        }
        for array, expected in cases.items():
            barray = BitArray(array=[array])
            self.assertEqual(expected, barray.last_set_bit)

    def test_cardinality(self):
        ''' Test that the cardinality works correctly '''
        cases = {
            0x1F: 5,
            0x3F: 6,
            0x7F: 7,
            0xFF: 8,
        }
        for array, expected in cases.items():
            barray = BitArray(array=[array])
            self.assertEqual(expected, barray.cardinality)

    def test_parity(self):
        ''' Test that the parity works correctly '''
        cases = {
            0x1: 1,
            0x0: 0,
        }
        for array, expected in cases.items():
            barray = BitArray(array=[array])
            self.assertEqual(expected, barray.parity)

    def test_compact(self):
        ''' Test that the bit array is compacted correctly '''
        array = BitArray()
        array.flip(127)
        #print array.to_byte_string()
        self.assertEqual(128, array.length_of_bits)
        array.flip(127)
        self.assertEqual(0,  array.length_of_bits)

    #------------------------------------------------------------
    # test length operations
    #------------------------------------------------------------
    def test_length_of_bits(self):
        ''' Test that the length_of_bits works correctly '''
        for size in range(1, 10):
            barray = BitArray(array=[0x01] * size)
            expected = 1 + (size - 1) * 64
            self.assertEqual(expected, barray.length_of_bits)

    def test_length_of_bytes(self):
        ''' Test that the length_of_bytes works correctly '''
        for size in range(1, 10):
            barray = BitArray(array=[0x01]*size)
            self.assertEqual(size, barray.length_of_bytes)

    def test_length_of_buffer(self):
        ''' Test that the length_of_buffer works correctly '''
        for size in range(1, 10):
            barray = BitArray(array=[0x01]*size)
            self.assertEqual(size * 64, barray.length_of_buffer)

    #------------------------------------------------------------
    # test flip operations
    #------------------------------------------------------------
    def test_flip(self):
        ''' Test that the flip bit operation is correct '''
        barray = BitArray(array=[0xEC])
        for pos in [0, 1, 4]: barray.flip(pos)
        self.assertEqual([0xFF], barray.to_byte_list())

        barray = BitArray(array=[0xFF])
        for pos in [0, 1, 4]: barray.flip(pos)
        self.assertEqual([0xEC], barray.to_byte_list())

    def test_flip_range(self):
        ''' Test that the flip_all bit operation is correct '''
        barray = BitArray(array=[0xF00F])
        barray.flip_range(4, 12)
        self.assertEqual([0xFFFF], barray.to_byte_list())

        barray = BitArray(array=[0xF00000000000000F])
        barray.flip_range(4, 60)
        self.assertEqual('0xFFFFFFFFFFFFFFFF', barray.to_byte_string())

        barray = BitArray(array=[0x00, 0x00])
        barray.flip_range(60, 127)
        #self.assertEqual('0xFFFFFFFFFFFFFFFF', barray.to_hex_string())

        barray = BitArray(array=[0x00, 0x00, 0x00])
        barray.flip_range(60, 128)
        self.assertEqual('0xFFFFFFFFFFFFFFFF', barray.to_byte_string())

    def test_flip_all(self):
        ''' Test that the flip_all bit operation is correct '''
        cases = {
            0xEC: [0xFFFFFFFFFFFFFF13],
            0xFFFFFFFFFFFFFF13: [0xEC],
        }
        for array, expected in cases.items():
            barray = BitArray(array=[array])
            barray.flip_all()
            self.assertEqual(expected, barray.to_byte_list())

    #------------------------------------------------------------
    # test clear operations
    #------------------------------------------------------------
    def test_clear(self):
        ''' Test that the clear bit operation is correct '''
        barray = BitArray(array=[0xFF])
        for pos in [0, 1, 4]: barray.clear(pos)
        self.assertEqual([0xEC], barray.to_byte_list())

        barray = BitArray(array=[0x13])
        for pos in [0, 1, 4]: barray.clear(pos)
        self.assertEqual([0x00], barray.to_byte_list())

    def test_clear_all(self):
        ''' Test that the clear_all bit operation is correct '''
        cases = {
            0x00: [0x00],
            0xEC: [0x00],
            0xFFFFFFFFFFFFFF13: [0x00],
        }
        for array, expected in cases.items():
            barray = BitArray(array=[array])
            barray.clear_all()
            self.assertEqual(expected, barray.to_byte_list())

    #------------------------------------------------------------
    # test set operations
    #------------------------------------------------------------
    def test_set(self):
        ''' Test that the set bit operation is correct '''
        barray = BitArray(array=[0xEC])
        for pos in [0, 1, 4]: barray.set(pos)
        self.assertEqual([0xFF], barray.to_byte_list())

        barray = BitArray(array=[0x00])
        for pos in [0, 1, 4]: barray.set(pos)
        self.assertEqual([0x13], barray.to_byte_list())

    def test_set_all(self):
        ''' Test that the set_all bit operation is correct '''
        cases = {
            0x00: [0xFFFFFFFFFFFFFFFF],
            0x12: [0xFFFFFFFFFFFFFFFF],
            0x34: [0xFFFFFFFFFFFFFFFF],
        }
        for array, expected in cases.items():
            barray = BitArray(array=[array])
            barray.set_all()
            self.assertEqual(expected, barray.to_byte_list())

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
