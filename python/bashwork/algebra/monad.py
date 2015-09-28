from bashwork.algebra.monoid import StringMonoid

#------------------------------------------------------------
# Functor
#------------------------------------------------------------

class Functor(object):

    def map(self, func):
        ''' Given a function of the following signature,
        apply it to the functor and return its result::

            map(fa: F[A], f: A => B): F[B]

        :param func: The function to apply to the functor
        :returns: The result of the function application
        '''
        raise NotImplementedError("map")

    def __mul__(self, func): return self.map(func)

class FunctorLaws(object):
    ''' A collection of laws that can be used to test
    if the supplied functor is indeed valid.
    '''

    @staticmethod
    def validate_identity(functor):
        '''
        '''
        value  = 2
        method = lambda x: x
        expect = functor.unit(value)
        actual = expect.map(method)
        assert (actual == expect), "functor fails identity"

    @staticmethod
    def validate(functor):
        ''' Validate that the supplied functor obeys the
        functor laws.

        :param functor: The functor to validate
        '''
        laws = FunctorLaws
        laws.validate_identity(functor)

#------------------------------------------------------------
# Applicative Functor
#------------------------------------------------------------

class Applicative(Functor):

    def pure(self, value):
        ''' Given a value, put that value in the
        minimal context of the current applicative::

            unit(b: B): A[B]

        :param value: The value to wrap in a applicative
        :returns: The minimal applicative context for this value
        '''
        raise NotImplementedError("pure")

    def apply(self, func):
        ''' Given a function of the following signature,
        apply it to the applicative and return its result::

            map(fa: F[A], f: => F[A => B]): F[B]

        :param func: The function to apply to the applicative
        :returns: The result of the function application
        '''
        raise NotImplementedError("apply")


class ApplicativeLaws(object):
    ''' A collection of laws that can be used to test
    if the supplied applicative functor is indeed valid.
    '''

    @staticmethod
    def validate_identity(applicative):
        pass # pure id <*> v = v

    @staticmethod
    def validate_composition(applicative):
        pass # pure (.) <*> u <*> v <*> w = u <*> (v <*> w)

    @staticmethod
    def validate_homomorphism(applicative):
        pass # pure f <*> pure x = pure (f x)

    @staticmethod
    def validate_interchange(applicative):
        pass # u <*> pure y = pure ($ y) <*> u

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

    def map(self, func):
        ''' Given a function of the following signature,
        apply it to the monad and return its result::

            map(fa: M[A], f: A => B): M[B]

        :param func: The function to apply to the monad
        :returns: The result of the function application
        '''
        raise NotImplementedError("map")

    def unit(self, value):
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

def liftM2(func, ma, mb):
    '''
    '''
    return ma.flat_map(
        lambda a: mb.map(lambda b: func(a, b)))

#------------------------------------------------------------
# Option Monad
#------------------------------------------------------------

class Maybe(Monad):
    ''' A monad that can optionally hold a value or not hold
    a value. This can be used instead of using `None` in a
    program.
    '''

    def flat_map(self, func):
        ''' Given a function of the following signature,
        apply it to the Maybe and return its result::

            f(A) => Option[B]

        :param func: The function to apply to the Maybe
        :returns: The result of the function application
        '''
        return Nothing if self.is_empty() else func(self.get())

    @staticmethod
    def unit(value):
        ''' Given a value, put that value in the
        minimal context of the current Maybe.

        :param value: The value to wrap in a Maybe
        :returns: The minimal Maybe context for this value
        '''
        return Something(value)

    def map(self, func):
        ''' Given a function of the following signature,
        apply it to the Maybe and return its result::

            f(A) => B

        :param func: The function to apply to the Maybe
        :returns: The result of the function application
        '''
        return Nothing if self.is_empty() else Something(func(self.get()))

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

    def flat_map(self, func):
        ''' Given a function of the following signature,
        apply it to the Either and return its result::

            f(A) => Either[A, B]

        :param func: The function to apply to the Either
        :returns: The result of the function application
        '''
        return self if self.is_error() else func(self.right())

    @staticmethod
    def unit(value):
        ''' Given a value, put that value in the
        minimal context of the current Either.

        :param value: The value to wrap in a Either
        :returns: The minimal Either context for this value
        '''
        return Success(value)

    def map(self, func):
        ''' Given a function of the following signature,
        apply it to the Either and return its result::

            f(A) => B

        :param func: The function to apply to the Either
        :returns: The result of the function application
        '''
        return self if self.is_error() else Success(func(self.right()))

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

    def get_or_else(self, default):
        ''' Retrieve the underlying value if one exists,
        otherwise return the default value.

        :param default: The default value to return
        :returns: The underlying value if it exists or the default otherwise
        '''
        return default if self.is_error() else self.right()

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
        ''' Given a function of the following signature,
        apply it to the Writer and return its result::

            f(A) => Writer[A, B]

        :param func: The function to apply to the Maybe
        :returns: The result of the function application
        '''
        result = func(self.value)
        diary  = self.monoid.plus(self.diary, result.diary)
        return Writer(result.value, diary, self.monoid)

    @classmethod
    def unit(klass, value, monoid=StringMonoid):
        ''' Given a value, put that value in the
        minimal context of the current Writer.

        :param value: The value to wrap in a Writer
        :returns: The minimal Writer context for this value
        '''
        return klass(value, monoid=monoid)

    def map(self, func):
        ''' Given a function of the following signature,
        apply it to the Writer and return its result::

            f(A) => B

        :param func: The function to apply to the Maybe
        :returns: The result of the function application
        '''
        return Writer(func(self.value), self.diary, self.monoid)
