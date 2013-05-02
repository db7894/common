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
            self.assertEqual(expected, barray.first_set_bit())

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
            self.assertEqual(expected, barray.first_clear_bit())

    def test_last_set_bit(self):
        ''' Test that the last_set_bit works correctly '''
        cases = {
            0x1F: 4,
            0x2F: 5,
            0x4F: 6,
            0x8F: 7,
        }
        for array, expected in cases.items():
            barray = BitArray(array=[array])
            self.assertEqual(expected, barray.last_set_bit())

    def test_compact(self):
        ''' Test that the bit array is compacted correctly '''
        array = BitArray()
        array.flip(127)
        #print array.to_byte_string()
        self.assertEqual(128, array.length_of_bits())
        array.flip(127)
        self.assertEqual(0,  array.length_of_bits())

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
