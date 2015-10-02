def identity(value):
    ''' A function that given a value, will
    return that value unmodified.

    :param value: The value to return from the function
    :returns: The supplied value
    '''
    return value

def constant(value):
    ''' A function that given a value, will
    return a function that will return the original value
    regardless of its input

    :param value: The value to return from the function
    :returns: A function that returns that value
    '''
    return lambda _: value

def constant2(value):
    ''' A function that given a value, will
    return a function that will return the original value
    regardless of its arity-2 input

    :param value: The value to return from the function
    :returns: A function that returns that value
    '''
    return lambda x, y: value

constant_true  = constant(True)
constant_false = constant(False)

def compose(*args):
    ''' A function that takes N functions
    and composes them such that a new function of
    the following is created::

       x = f2 (f1 x) 

    :param args: The functions to compose
    :returns: A function composed of the two functions
    '''
    return reduce(lambda t, n: lambda x: n(t(x)), args)

def application(f):
    ''' A function that can be used to form composition
    of resulting functions like::

        S f g x = f x (g x)

    :param f: The original function to wrap
    :returns: The next curried function handler
    '''
    def wrapper(g):
        return lambda x: f(x)(g(x))
    return wrapper

#------------------------------------------------------------
# SKI Aliases
#------------------------------------------------------------

I = identity
K = constant
S = application

#------------------------------------------------------------
# Currying
#------------------------------------------------------------

def curry(function, argc=None):
    ''' Given a function of N-arity, convert it into a
    collection of functions each with 1-arity.

    :param function: The function to curry
    :param argc: The argument count to curry
    '''
    if argc is None:
        argc = function.__code__.co_argcount

    def curry_wrapper(*args, **kwargs):
        if len(args) + len(kwargs) == argc:
            return function(*args, **kwargs)

        def thunk(*args2, **kwargs2):
            return function(*(args + args2), **dict(kwargs, **kwargs2))

        return curry(thunk, argc - len(args) - len(kwargs))
    return curry_wrapper
