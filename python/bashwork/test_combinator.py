#!/usr/bin/env python
import unittest
from bashwork.combinator import *

class CombinatorTest(unittest.TestCase):

    def setUp(self):
        self.value = 22
        self.ignore = None
        def f1(x):
            return lambda y: y + x
        self.f1 = f1
        self.f2 = lambda x: x + 2
        self.f3 = lambda x: x + 6

    def test_identity(self):
        self.assertEqual(self.value, identity(self.value))
        self.assertEqual(self.value, I(self.value))

    def test_identity_with_sk(self):
        ''' The S and K combinators are actually the only
        ones needed as I can be derived by composing those two.
        '''
        self.assertEquals(self.value, S(K)(S)(self.value))
        self.assertEquals(self.value, S(K)(K)(self.value))

    def test_constant(self):
        self.assertEqual(self.value, constant(self.value)(self.ignore))
        self.assertEqual(self.value, K(self.value)(self.ignore))
        self.assertTrue(constant_true(self.ignore))
        self.assertFalse(constant_false(self.ignore))

    def test_compose(self):
        self.assertEqual(30, compose(self.f2, self.f3)(self.value))

    def test_application(self):
        self.assertEqual(50, application(self.f1)(self.f3)(self.value))
        self.assertEqual(50, S(self.f1)(self.f3)(self.value))

    def test_iota_combinator(self):
        ''' With the iota combinator, we can recreate all the other
        combinators by a series of applications of iota to itself.
        '''
        i = lambda x: x(S)(K)
        I2 = i(i)
        K2 = i(i(i(i))) 
        S2 = i(i(i(i(i)))) 

        self.assertEqual(I2(self.value), I(self.value))
        self.assertEqual(S2(self.f1)(self.f3)(self.value), S(self.f1)(self.f3)(self.value))
        self.assertEqual(K2(self.value)(self.ignore), K(self.value)(self.ignore))

    def test_recursion_combinator(self):
        length_of_list = recursion(
            lambda length:
                lambda xs: 0 if not xs else length(xs[1:]) + 1)
        self.assertEqual(length_of_list([1,2,3,4]), 4)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
