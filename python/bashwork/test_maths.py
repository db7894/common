#!/usr/bin/env python
import unittest
from bashwork.maths import *

class MathsTest(unittest.TestCase):

    def test_add_maths(self):
        self.assertEqual(  7, AddMaths.add(3,  4))
        self.assertEqual( -1, AddMaths.add(3, -4))
        self.assertEqual( -1, AddMaths.sub(3,  4))
        self.assertEqual(  1, AddMaths.sub(4,  3))
        self.assertEqual(  3, AddMaths.abs(3))
        self.assertEqual(  3, AddMaths.abs(-3))
        self.assertEqual( -3, AddMaths.negate(3))
        self.assertEqual(  3, AddMaths.negate(-3))
        self.assertEqual( 30, AddMaths.mul(3, 10))
        self.assertEqual(-30, AddMaths.mul(-3, 10))
        self.assertEqual( 40, AddMaths.mul(20, 2))
        self.assertEqual(-40, AddMaths.mul(-20, 2))

        self.assertFalse(AddMaths.diff_signs(3, 3))
        self.assertTrue(AddMaths.diff_signs(-3, 3))
        self.assertTrue(AddMaths.diff_signs(3, -3))
        self.assertFalse(AddMaths.diff_signs(-3, -3))

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
