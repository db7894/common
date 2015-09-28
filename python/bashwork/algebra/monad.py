'''
TODO

- https://bitbucket.org/jason_delaat/pymonad/src/cbecd6796cd1488237d2a0f057cefd2a50df753a/pymonad/?at=master
- reader, writer
- state
'''
from operator import add 
from bashwork.combinator import identity, compose
from bashwork.algebra.monoid import StringMonoid

#------------------------------------------------------------
# Functor
#------------------------------------------------------------

class Functor(object):

    def map(self, function):
        ''' Given a function of the following signature,
        apply it to the functor and return its result::

            map(fa: F[A], f: A => B): F[B]

        :param function: The function to apply to the functor
        :returns: The result of the function application
        '''
        raise NotImplementedError("map")

    def __mul__(self, function): return self.map(function)

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

#------------------------------------------------------------
# Applicative Functor
#------------------------------------------------------------

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


#------------------------------------------------------------
# Option Monad
#------------------------------------------------------------

class Maybe(Monad):
    ''' A monad that can optionally hold a value or not hold
    a value. This can be used instead of using `None` in a
    program.
    '''

    def flat_map(self, function):
        return Nothing if self.is_empty() else function(self.get())

    def apply(self, functor):
        return Nothing if self.is_empty() else functor.map(self.get())

    def map(self, function):
        return Nothing if self.is_empty() else Something(function(self.get()))

    @staticmethod
    def unit(value):
        return Something(value)

    @staticmethod
    def something(value):
        ''' Factory method to return a new Maybe with an
        underlying value.

        :param value: The value to wrap in a Maybe
        :returns: The value wrapped in a Maybe
        '''
        return Something(value)

    @staticmethod
    def nothing():
        ''' Factory method to return a new Maybe with out
        an underlying value.

        :returns: A new Maybe without a value
        '''
        return Nothing

    def get_or_else(self, default):
        ''' Retrieve the underlying value if one exists,
        otherwise return the default value.

        :param default: The default value to return
        :returns: The underlying value if it exists or the default otherwise
        '''
        return default if self.is_empty() else self.get()

    def get(self):
        ''' Retrieve the underlying value of the Maybe
        if it exists, otherwise throw.

        :returns: The underlying value of the maybe
        '''
        raise NotImplementedError("get")

    def is_empty(self):
        ''' Check if there is a value inside of this Maybe.

        :returns: True if there is a value, false otherwise
        '''
        raise NotImplementedError("is_empty")

class Something(Maybe):
    ''' A Maybe that contains an underlying value '''

    __slots__ = ['value']

    def __init__(self, value): self.value = value
    def is_empty(self): return False
    def get(self): return self.value
    def __eq__(self, that): return (that != None) and (self.value == that.value)
    def __str__(self): return "Something(%s)" % self.value
    __repr__ = __str__

class _Nothing(Maybe):
    ''' A Maybe without an underlying value '''

    def is_empty(self): return True
    def get(self): raise ValueError("missing value")
    def __str__(self): return "Nothing()"
    def __call__(self, _=None): return self # singleton
    __repr__ = __str__

Nothing = _Nothing() # only need a single instance
Option  = Maybe      # alias if someone likes this name better

#------------------------------------------------------------
# Either Monad
#------------------------------------------------------------

class Either(Monad):

    def flat_map(self, function):
        return self if self.is_error() else function(self.right())

    def apply(self, functor):
        return self if self.is_error() else functor.map(self.right())

    def map(self, function):
        return self if self.is_error() else Success(function(self.right()))

    def is_error(self):
        ''' If the underlying Either is an error this will
        return True, otherwise False.

        :returns: If the Either has an error or not.
        '''
        raise NotImplementedError("is_error")

    def left(self):
        ''' Returns the error state if there is one

        :returns: The error state if there is one
        '''
        raise NotImplementedError("left")

    def right(self):
        ''' Returns the success state if there is one

        :returns: The success state if there is one
        '''
        raise NotImplementedError("right")

    def silent(self):
        ''' Ignores the error and converts an either into
        a Maybe monad.

        :returns: The Maybe monad version of this instance
        '''
        return Nothing if self.is_error() else Something(self.right())

    def get_or_else(self, default):
        ''' Retrieve the underlying value if one exists,
        otherwise return the default value.

        :param default: The default value to return
        :returns: The underlying value if it exists or the default otherwise
        '''
        return default if self.is_error() else self.right()

    @staticmethod
    def unit(value):
        return Success(value)

    @staticmethod
    def success(value):
        ''' Factory method to return a new Either with an
        underlying value.

        :param value: The value to wrap in a Either
        :returns: The value wrapped in a Either
        '''
        return Success(value)

    @staticmethod
    def failure(ex):
        ''' Factory method to return a new Either with a
        failed underlying value.

        :returns: A new Either with a failed value
        '''
        return Failure(ex)

class Success(Either):
    ''' An Either that contains an underlying value '''

    __slots__ = ['value']

    def __init__(self, value): self.value = value
    def is_error(self): return False
    def right(self): return self.value
    def left(self): raise ValueError("there is no error")
    def __eq__(self, that): return ((that != None)
        and (that.is_error() == self.is_error())
        and (that.value == self.value))
    def __str__(self): return "Success(%s)" % self.value
    __repr__ = __str__

class Failure(Either):
    ''' An Either that has failed with some error '''

    __slots__ = ['error']

    def __init__(self, error): self.error = error
    def is_error(self): return True
    def right(self): raise ValueError("missing value")
    def left(self): return self.error
    def __eq__(self, that): return ((that != None)
        and (that.is_error() == self.is_error())
        and (that.error == self.error))
    def __str__(self): return "Failure(%s)" % self.error
    __repr__ = __str__

#------------------------------------------------------------
# Writer Monad
#------------------------------------------------------------

class Writer(Monad):
    ''' A Writer that contains an underlying value as well as
    an accumulated diary of events.
    '''

    __slots__ = ['value', 'diary', 'monoid']

    def __init__(self, value, diary=None, monoid=StringMonoid):
        self.value  = value
        self.monoid = monoid
        self.diary  = diary or monoid.zero

    def get(self): return self.value
    def get_diary(self): return self.diary
    def __str__(self): return "Writer(%s, %s)" % (self.value, self.diary)
    __repr__ = __str__

    def flat_map(self, func):
        result = func(self.value)
        diary  = self.monoid.plus(self.diary, result.diary)
        return Writer(result.value, diary, self.monoid)

    @classmethod
    def unit(klass, value, monoid=StringMonoid):
        return klass(value, monoid=monoid)

    def map(self, func):
        return Writer(func(self.value), self.diary, self.monoid)

#------------------------------------------------------------
# List Monad
#------------------------------------------------------------

class List(Monad, list): # ordered to override list methods
    ''' A List monad represents a computation that can have
    many results.
    '''

    def get(self): return self

    def flat_map(self, function):
        return reduce(add, map(function, self))

    @classmethod
    def unit(klass, value):
        return klass([value])

    def apply(self, functor):
        return reduce(add, [functor.map(x) for x in self])

    def map(self, function):
        return List([function(x) for x in self])

#------------------------------------------------------------
# State Monad
#------------------------------------------------------------

class State(Monad):
    ''' A monad that represents some calculation with stateful
    side effects.
    '''

    def __init__(self, value): self.value = value

    def get_result(self, state):
        return self.value(state)[0]

    def get_state(self, state):
        return self.value(state)[1]

    def flat_map(self, function):
        return reduce(add, map(function, self))

    @classmethod
    def unit(klass, value):
        return klass(lambda state: (value, state))

    def apply(self, functor):
        return reduce(add, [functor.map(x) for x in self])

    def map(self, function):
        return State(lambda state: (function(self(state)[0]), state))

    def __call__(self, state):
        return self.value(state)
