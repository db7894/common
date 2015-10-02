#!/usr/bin/env python
import unittest
from bashwork.algebra.functor import Functor
from bashwork.algebra.applicative import Applicative
from bashwork.algebra.monad import Monad

class MonadsTest(unittest.TestCase):

    def test_functor_interface(self):
        functor = Functor()
        self.assertRaises(NotImplementedError, lambda: functor.map(None))

    def test_applicative_interface(self):
        applicative = Applicative()
        self.assertRaises(NotImplementedError, lambda: applicative.apply(None))

    def test_monad_interface(self):
        monad = Monad()
        self.assertRaises(NotImplementedError, lambda: monad.map(None))
        self.assertRaises(NotImplementedError, lambda: monad.unit(None))
        self.assertRaises(NotImplementedError, lambda: monad.flat_map(None))

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
