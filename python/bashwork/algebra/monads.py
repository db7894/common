from operator import add 
from bashwork.combinator import constant
from bashwork.algebra.monad import Monad
from bashwork.algebra.monoid import StringMonoid


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

    def flat_map(self, function):
        result = function(self.value)
        diary  = self.monoid.plus(self.diary, result.diary)
        return Writer(result.value, diary, self.monoid)

    @classmethod
    def unit(klass, value, monoid=StringMonoid):
        return klass(value, monoid=monoid)

    def map(self, function):
        return Writer(function(self.value), self.diary, self.monoid)

    def apply(self, functor):
        value = self.value(functor.value)
        return Writer(value, functor.diary, self.monoid)

#------------------------------------------------------------
# Reader Monad TODO
#------------------------------------------------------------

class Reader(Monad):
    ''' A Writer that contains an underlying value as well as
    an accumulated diary of events.
    '''

    __slots__ = ['value']

    def __init__(self, value):
        if callable(value):
            self.value = value
        else: self.value = constant(value)

    def flat_map(self, function):
        return Reader(lambda x: function(self.value()(x))(x))

    @classmethod
    def unit(klass, value):
        return klass(constant(value))

    def map(self, function):
        return Reader(lambda x: function(self.value()(x)))

    def apply(self, functor):
        return Reader(lambda x: self(x)(functor(x)))

    def __call__(self, *args):
        pass# TODO

#------------------------------------------------------------
# Continuation Monad TODO
#------------------------------------------------------------

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

    __slots__ = ['value']

    def __init__(self, value):
        ''' Initialize a new instance of the State monad::

            value: S => (A, S)

        :param value: The initial state function 
        '''
        self.value = value

    def get_result(self, state):
        ''' Get the result of applying the supplied state

        :param state: The current state to apply
        :returns: The result of applying the state
        '''
        return self.value(state)[0]

    def get_state(self, state):
        ''' Get the new state after applying the supplied state

        :param state: The current state to apply
        :returns: The resulting state from applying this state
        '''
        return self.value(state)[1]

    def run(self, state):
        ''' Get the side-effects of applying the supplied state.

        :param state: The current state to apply
        :returns: The resulting side-effects (result, state)
        '''
        return self.value(state)

    def flat_map(self, function):
        def flat_map_state(state):
            result, new_state = self.run(state)
            return function(result).run(new_state)
        return State(flat_map_state)

    @classmethod
    def unit(klass, value):
        return klass(lambda state: (value, state))

    def apply(self, functor):
        def apply_state(state):
            function, _ = self.run(state)
            result, _   = functor.map(state)
            return (function(result), state)
        return State(apply_state)

    def map(self, function):
        def map_state(state):
            result, new_state = self.run(state)
            return (function(result), state)
        return State(map_state)

    def __call__(self, state): return self.run(state)

class StateStack(object):
    ''' An example of creating a stateful stack by using
    the state monad. What follows is an example of its use:

    >>> stack  = StateState
    >>> monad  = stack.push(4) >> (lambda _: stack.pop()) >> (lambda _: stack.pop())
    >>> result = monad([1,2,3])
    >>> assert result == (1, [2, 3])
    '''

    @staticmethod
    def push(x):
        return State(lambda xs: ((), [x] + xs))

    @staticmethod
    def pop():
        return State(lambda xs: (xs[0], xs[1:]))
