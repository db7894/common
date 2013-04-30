#!/usr/bin/env python
import unittest
from bitarray import BitArray

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class BitArrayTest(unittest.TestCase):
    ''' Code to validate that the bitarray implementation is correct.
    '''

    def setUp(self):
        self.array  = BitArray()

    def test_compact(self):
        ''' Test that the kdtree is created correctly '''
        self.array.clear(128)
        self.asssertEqual(128, len(self.array))
        self.array.compact()
        self.asssertEqual(0, len(self.array))


#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
