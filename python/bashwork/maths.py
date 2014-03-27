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
    return (a * b) / gcd(a, b)

