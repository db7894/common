from bashwork.combinator import identity, compose


class Functor(object):

    def map(self, function):
        ''' Given a function of the following signature,
        apply it to the functor and return its result::

            map(fa: F[A], f: A => B): F[B]

        :param function: The function to apply to the functor
        :returns: The result of the function application
        '''
        raise NotImplementedError("map")

    def __mul__(self, function):  return self.map(function)
    def __rmul__(self, function): return self.map(function)

class FunctorLaws(object):
    ''' A collection of laws that can be used to test
    if the supplied functor is indeed valid.
    '''

    @staticmethod
    def validate_identity(functor):
        ''' The functor must satisfy the following law::

            fmap id fa == id
        '''
        value  = 2
        method = identity
        expect = functor.unit(value)
        actual = expect.map(method)
        assert (actual == expect), "functor fails identity"

    @staticmethod
    def validate_composition(functor):
        ''' The functor must satisfy the following law::

            fmap (f . g) = fmap f . fmap g.
        '''
        value   = functor.unit(2)
        method1 = lambda x: x + 2
        method2 = lambda x: x * 2
        methodc = compose(method1, method2)
        expect  = value * method1 * method2
        actual  = value * methodc
        assert (actual == expect), "functor fails composition"

    @staticmethod
    def validate(functor):
        ''' Validate that the supplied functor obeys the
        functor laws.

        :param functor: The functor to validate
        '''
        laws = FunctorLaws
        laws.validate_identity(functor)
        laws.validate_composition(functor)
