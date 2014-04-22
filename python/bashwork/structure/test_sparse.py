#!/usr/bin/env python
import unittest
from bashwork.structure.sparse import SparseArray

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class SparseArrayTest(unittest.TestCase):
    ''' Code to validate that the sparse array implementation is correct.
    '''

    def test_get_and_set_item(self):
        ''' Test that the bit array is inited correctly '''
        array = SparseArray(default='z')
        array[1] = 'a'
        array[3] = 'b'
        array[5] = 'x'
        self.assertEqual('z', array[0])
        self.assertEqual('a', array[1])
        self.assertEqual('z', array[2])
        self.assertEqual('b', array[3])

    def test_iter(self):
        array = SparseArray(default='z')
        array[1] = 'a'
        array[3] = 'b'
        array[5] = 'x'
        expect = ['z', 'a', 'z', 'b', 'z', 'x']
        actual = list(array)
        self.assertEqual(expect, actual)


#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
