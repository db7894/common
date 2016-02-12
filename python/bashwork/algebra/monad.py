from operator import add 
from bashwork.combinator import identity
from bashwork.algebra.applicative import Applicative
from bashwork.algebra.monoid import StringMonoid


#------------------------------------------------------------
# Monad
#------------------------------------------------------------

class Monad(Applicative):

    @classmethod
    def unit(klass, value):
        ''' Given a value, put that value in the
        minimal context of the current monad::

            unit(a: A): M[A]

        :param value: The value to wrap in a monad
        :returns: The minimal monad context for this value
        '''
        raise NotImplementedError("unit")

    def flat_map(self, func):
        ''' Given a function of the following signature,
        apply it to the monad and return its result::

            flat_map(ma: M[A], f: A => M[B]): M[B]

        :param func: The function to apply to the monad
        :returns: The result of the function application
        '''
        raise NotImplementedError("flat_map")

    def __rshift__(self, func): return self.flat_map(func)

class MonadLaws(object):
    ''' A collection of laws that can be used to test
    if the supplied monad is indeed valid.
    '''

    @staticmethod
    def validate_left_identity(monad):
        '''
        '''
        value  = 2
        method = lambda x: x * 2
        actual = monad.unit(value) >> method
        expect = method(value)
        assert (actual == expect), "monad fails left identity"

    @staticmethod
    def validate_right_identity(monad):
        '''
        '''
        value  = 2
        expect = monad.unit(value) 
        actual = expect >> monad.unit
        assert (actual == expect), "monad fails right identity"

    @staticmethod
    def validate_associativity(monad):
        '''
        '''
        value   = monad.unit(2)
        method1 = lambda x: monad.unit(x * 2)
        method2 = lambda x: monad.unit(x + 2)
        expect  = value >> (lambda x: method1(x) >> method2)
        actual  = value >> method1 >> method2
        assert (actual == expect), "monad fails associativity"

    @staticmethod
    def validate(monad):
        ''' Validate that the supplied monad obeys the monad
        laws.

        :param monad: The monad to validate
        '''
        laws = MonadLaws
        laws.validate_left_identity(monad)
        laws.validate_right_identity(monad)
        laws.validate_associativity(monad)

#------------------------------------------------------------
# Free Methods
#------------------------------------------------------------

def liftM2(func, ma, mb):
    '''
    '''
    return ma.flat_map(
        lambda a: mb.map(lambda b: func(a, b)))


def pure(klass, value):
    ''' Given a typeclass, return the unit value or minimal
    context for that type for the supplied value.

    :param klass: The typeclass to wrap the value in
    :param value: The value to put in the minimal context
    :returns: The value in the minimal context
    '''
    return klass.unit(value)


def point(klass, value):
    ''' Given a typeclass, return the unit value or minimal
    context for that type for the supplied value.

    :param klass: The typeclass to wrap the value in
    :param value: The value to put in the minimal context
    :returns: The value in the minimal context
    '''
    return klass.unit(value)
