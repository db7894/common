from bashwork.combinator import identity, compose
from bashwork.algebra.functor import Functor

class Applicative(Functor):

    def apply(self, function):
        ''' Given a function of the following signature,
        apply it to the applicative and return its result::

            map(fa: F[A], f: => F[A => B]): F[B]

        :param function: The function to apply to the applicative
        :returns: The result of the function application
        '''
        raise NotImplementedError("apply")

    def __and__(self, function): return self.apply(function)


class ApplicativeLaws(object):
    ''' A collection of laws that can be used to test
    if the supplied applicative functor is indeed valid.
    '''

    @staticmethod
    def validate_identity(applicative):
        ''' The applicative must satisfy the following law::

            pure id <*> v == v
        '''
        value  = 2
        method = applicative.unit(identity)
        expect = applicative.unit(value)
        actual = method & expect
        assert (actual == expect), "applicative fails identity"

    @staticmethod
    def validate_composition(applicative):
        ''' The applicative must satisfy the following law::
        
            pure (.) <*> u <*> v <*> w = u <*> (v <*> w)
        '''
        value   = applicative.unit(2)
        method1 = applicative.unit(lambda x: x + 1)
        method2 = applicative.unit(lambda x: x * 2)
        compose = applicative.unit(lambda x: lambda y: lambda z: x(y(z)))
        expect  = (compose & method1 & method2) & value
        actual  = method1 & (method2 & value)
        assert (actual == expect), "applicative fails composition"

    @staticmethod
    def validate_homomorphism(applicative):
        ''' The applicative must satisfy the following law::
        
            pure f <*> pure x = pure (f x)
        '''
        value  = 2
        method = lambda x: x + 2
        expect = applicative.unit(method(value))
        actual = applicative.unit(method) & applicative.unit(value)
        assert (actual == expect), "applicative fails homomorphism"

    @staticmethod
    def validate_interchange(applicative):
        ''' The applicative must satisfy the following law::
        
            u <*> pure y = pure ($ y) <*> u
        '''
        value  = 2
        method = applicative.unit(lambda x: x + 2)
        expect = method & applicative.unit(value)
        actual = applicative.unit(lambda x: x(value)) & method
        assert (actual == expect), "applicative fails interchanged"

    @staticmethod
    def validate(applicative):
        ''' Validate that the supplied applicative functor
        obeys the applicative laws.

        :param applicative: The applicative to validate
        '''
        laws = ApplicativeLaws
        laws.validate_identity(applicative)
        laws.validate_composition(applicative)
        laws.validate_homomorphism(applicative)
        laws.validate_interchange(applicative)
