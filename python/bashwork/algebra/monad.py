from bashwork.algebra.monoid import StringMonoid

class Monad(object):

    def map(self, func):
        ''' Given a function of the following signature,
        apply it to the monad and return its result::

            f(A) => B

        :param func: The function to apply to the monad
        :returns: The result of the function application
        '''
        raise NotImplemented("map")

    def unit(self, value):
        ''' Given a value, put that value in the
        minimal context of the current monad.

        :param value: The value to wrap in a monad
        :returns: The minimal monad context for this value
        '''
        raise NotImplemented("unit")

    def flat_map(self, func):
        ''' Given a function of the following signature,
        apply it to the monad and return its result::

            f(A) => Monad[B]

        :param func: The function to apply to the monad
        :returns: The result of the function application
        '''
        raise NotImplemented("flat_map")

    def __rshift__(self, func): return self.flat_map(func)

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
        raise NotImplemented("get")

    def is_empty(self):
        ''' Check if there is a value inside of this Maybe.

        :returns: True if there is a value, false otherwise
        '''
        raise NotImplemented("is_empty")

class Something(Maybe):
    ''' A Maybe that contains an underlying value '''

    __slots__ = ['value']

    def __init__(self, value): self.value = value
    def is_empty(self): return False
    def get(self): return self.value
    def __str__(self): return "Something(%s)" % self.value
    __repr__ = __str__

class _Nothing(Maybe):
    ''' A Maybe without an underlying value '''

    def is_empty(self): return True
    def get(self): raise ValueError("missing value")
    def __str__(self): return "Nothing()"
    __repr__ = __str__

Nothing = _Nothing() # only need a single instance

#------------------------------------------------------------
# Either Monad
#------------------------------------------------------------

class Either(Monad):

    def is_error(self):
        ''' If the underlying Either is an error this will
        return True, otherwise False.

        :returns: If the Either has an error or not.
        '''
        raise NotImplemented("is_error")

    def left(self):
        ''' Returns the error state if there is one

        :returns: The error state if there is one
        '''
        raise NotImplemented("left")

    def right(self):
        ''' Returns the success state if there is one

        :returns: The success state if there is one
        '''
        raise NotImplemented("right")

    def silent(self):
        ''' Ignores the error and converts an either into
        a Maybe monad.

        :returns: The Maybe monad version of this instance
        '''
        return Nothing if is_error else Something(self.right)

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
    def __str__(self): return "Success(%s)" % self.value
    __repr__ = __str__

class Failure(Either):
    ''' An Either that has failed with some error '''

    __slots__ = ['error']

    def __init__(self, error): self.error = error
    def is_error(self): return True
    def right(self): raise ValueError("missing value")
    def left(self): return self.error
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
