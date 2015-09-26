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

def compose(f1, f2):
    ''' A function that takes two functions
    and composes them such that a new function of
    the following is created::

       x = f2 (f1 x) 

    :param f1: The first function to compose
    :param f2: The second function to compose
    :returns: A function composed of the two functions
    '''
    return lambda x: f2(f1(x))

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
