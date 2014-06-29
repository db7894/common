#!/usr/bin/env python
import unittest
from bashwork.monad import *

class MonadsTest(unittest.TestCase):

    def test_maybe(self):
        pass

    def test_maybe_something(self):
        value = 2
        monad = Something(value)
        final = monad.map(lambda x: x * 2)
        self.assertFalse(monad.is_empty())
        self.assertEqual(monad.get(), value)
        self.assertFalse(final.is_empty())
        self.assertEqual(final.get(), value * 2)

    def test_maybe_nothing(self):
        monad = Nothing
        final = monad.map(lambda x: x * 2)
        self.assertTrue(monad.is_empty())
        self.assertThrows(lambda: monad.get())
        self.assertTrue(final.is_empty())
        self.assertThrows(lambda: final.get())

    def test_either(self):
        pass

    def test_either_success(self):
        value = 2
        monad = Success(value)
        final = monad.map(lambda x: x * 2)
        self.assertFalse(monad.is_error())
        self.assertEqual(monad.get(), value)
        self.assertFalse(final.is_error())
        self.assertEqual(final.get(), value * 2)

    def test_either_failure(self):
        value = "exception"
        monad = Failure(value)
        final = monad.map(lambda x: x * 2)
        self.assertTrue(monad.is_error())
        self.assertEqual(monad.get(), value)
        self.assertTrue(final.is_empty())
        self.assertThrows(lambda: final.get())

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
