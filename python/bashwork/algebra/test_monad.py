#!/usr/bin/env python
import unittest
from bashwork.algebra.monad import *

class MonadsTest(unittest.TestCase):

    def test_applicative_interface(self):
        applicative = Applicative()
        self.assertRaises(NotImplementedError, lambda: applicative.apply(None))

    def test_monad_interface(self):
        monad = Monad()
        self.assertRaises(NotImplementedError, lambda: monad.map(None))
        self.assertRaises(NotImplementedError, lambda: monad.unit(None))
        self.assertRaises(NotImplementedError, lambda: monad.flat_map(None))

    def test_maybe_interface(self):
        maybe = Maybe()
        FunctorLaws.validate(Maybe)
        ApplicativeLaws.validate(Maybe)
        MonadLaws.validate(Maybe)

        self.assertEquals(Maybe.unit(1), Something(1))
        self.assertRaises(NotImplementedError, lambda: maybe.get())
        self.assertRaises(NotImplementedError, lambda: maybe.get_or_else(None))

    def test_maybe_something(self):
        value = 2
        monad = Maybe.something(value)
        final = monad.map(lambda x: x * 2)
        self.assertFalse(monad.is_empty())
        self.assertEqual(monad.get(), value)
        self.assertFalse(final.is_empty())
        self.assertEqual(final.get(), value * 2)
        self.assertEqual(monad.get_or_else(None), value)
        self.assertEqual(monad.flat_map(lambda x: Something(x + 2)), Something(4))

    def test_maybe_nothing(self):
        monad = Maybe.nothing()
        final = monad.map(lambda x: x * 2)
        self.assertTrue(monad.is_empty())
        self.assertRaises(lambda: monad.get())
        self.assertTrue(final.is_empty())
        self.assertRaises(lambda: final.get())
        self.assertEqual(monad.get_or_else(2), 2)
        self.assertEqual(monad.flat_map(lambda x: Something(x + 2)), Nothing)

    def test_either_interface(self):
        either = Either()
        FunctorLaws.validate(Either)
        ApplicativeLaws.validate(Either)
        MonadLaws.validate(Either)

        self.assertEqual(Either.unit(2), Success(2))
        self.assertRaises(NotImplementedError, lambda: either.is_error())
        self.assertRaises(NotImplementedError, lambda: either.left())
        self.assertRaises(NotImplementedError, lambda: either.right())

    def test_either_success(self):
        value = 2
        monad = Either.success(value)
        final = monad.map(lambda x: x * 2)
        self.assertFalse(monad.is_error())
        self.assertEqual(monad.right(), value)
        self.assertFalse(final.is_error())
        self.assertEqual(final.right(), value * 2)
        self.assertEqual(monad.silent(), Something(2))
        self.assertEqual(monad.get_or_else(None), 2)
        self.assertEqual(monad.flat_map(lambda x: Success(x + 2)), Success(4))

    def test_either_failure(self):
        value = "exception"
        monad = Either.failure(value)
        final = monad.map(lambda x: x * 2)
        self.assertTrue(monad.is_error())
        self.assertEqual(monad.left(), value)
        self.assertTrue(final.is_error())
        self.assertRaises(lambda: final.left())
        self.assertEqual(monad.silent(), Nothing)
        self.assertEqual(monad.get_or_else(2), 2)
        self.assertEqual(monad.flat_map(lambda x: Success(x + 2)), Failure(value))

    #def test_writer_interface(self):
    #    FunctorLaws.validate(Writer)
    #    ApplicativeLaws.validate(Writer)
    #    MonadLaws.validate(Writer)

    #def test_reader_interface(self):
    #    FunctorLaws.validate(Reader)
    #    ApplicativeLaws.validate(Reader)
    #    MonadLaws.validate(Reader)

    def test_list_interface(self):
        FunctorLaws.validate(List)
        ApplicativeLaws.validate(List)
        MonadLaws.validate(List)

    # this won't work unless we call `run` at the assert points
    #def test_State_interface(self):
    #    FunctorLaws.validate(State)
    #    ApplicativeLaws.validate(State)
    #    MonadLaws.validate(State)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
