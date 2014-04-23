#!/usr/bin/env python
import unittest
from bashwork.structure.sparse import SparseVector

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class SparseVectorTest(unittest.TestCase):
    ''' Code to validate that the sparse vector implementation is correct.
    '''

    def test_get_and_set_item(self):
        ''' Test that the bit Vector is inited correctly '''
        vector = SparseVector(default='z')
        vector[1] = 'a'
        vector[3] = 'b'
        vector[5] = 'x'
        self.assertEqual('z', vector[0])
        self.assertEqual('a', vector[1])
        self.assertEqual('z', vector[2])
        self.assertEqual('b', vector[3])

    def test_iter(self):
        vector = SparseVector(default='z')
        vector[1] = 'a'
        vector[3] = 'b'
        vector[5] = 'x'
        expect = ['z', 'a', 'z', 'b', 'z', 'x']
        actual = list(vector)
        self.assertEqual(expect, actual)


#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
