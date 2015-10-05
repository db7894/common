'''
.. todo::
   - all the fun math problems
'''

def gcd(a, b):
    ''' GCD implemented using Euclid's algorithm.
    '''
    while b != 0:
        a, b = b, a % b
    return a

def gcd_rec(a, b):
    ''' GCD implemented using Euclid's algorithm
    and recursion
    '''
    if b == 0: return a
    return gcd(b, a % b)

def gcd_sub(a, b):
    ''' GCD implemented using Euclid's algorithm
    and only the sub operation.
    '''
    while a != b:
        if a > b: a -= b
        else: b -= a
    return a

def lcm(a, b):
    ''' Least common multiple based on the gcd
    '''
    return (a * b) / gcd(a, b)

class AddMaths(object):
    ''' A collection of math operations formed by only
    using the addition operator.
    '''

    @staticmethod
    def add(a, b):
        return a + b

    @staticmethod
    def diff_signs(a, b):
        return (a < 0 and b > 0) or (a > 0 and b < 0)

    @staticmethod
    def abs(a):
        return a if a >= 0 else AddMaths.negate(a)

    @staticmethod
    def negate(a):
        neg, pad = 0, 1 if a < 0 else -1
        while a != 0:
            neg, a = neg + pad, a + pad
        return neg

    @staticmethod
    def sub(a, b):
        return a + AddMaths.negate(b)

    @staticmethod
    def div(a, b):
        if b == 0:
            raise ZeroDivisionError("the supplied dividend is 0")

        quotient, dividend = 0, AddMaths.abs(a)
        divisor = AddMaths.negate(AddMaths.abs(b))

        while dividend > 0:
            dividend += divisor
            quotient += 1

        return AddMaths.negate(quotient) if AddMaths.diff_signs(a, b) else quotient

    @staticmethod
    def mul(a, b):
        value = sum(a for _ in range(AddMaths.abs(b)))
        return value if b >= 0 else AddMaths.negate(value)
