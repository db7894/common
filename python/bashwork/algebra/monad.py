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

    __rshift__ = flat_map # so we can look like haskell

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
        return Nothing if self.is_empty else func(self.get())

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
        return Nothing if self.is_empty else Something(func(self.get()))

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

class _Nothing(Maybe):
    ''' A Maybe without an underlying value '''

    def is_empty(self): return True
    def get(self): raise ValueError("missing value")

Nothing = _Nothing() # only need a single instance

#------------------------------------------------------------
# Either Monad
#------------------------------------------------------------

class Either(Monad):
    pass
