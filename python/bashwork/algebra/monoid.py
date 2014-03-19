import operator
from collections import Counter
from bashwork import combinator


#------------------------------------------------------------
# Type Classes
#------------------------------------------------------------
class SemiGroup(object):

    def plus(self, left, right):
        ''' Given an existing value of this monoid, add
        the right value to this monoid.

        :param left: The current monoid to append to
        :param right: The new value to append to the monoid
        :returns: The new joined values
        '''
        raise NotImplemented("zero")

    def sum(self, values):
        ''' Given a collection of values, reduce the values to
        a single value based on the monoid's operations. It should
        be noted that there is no initial value of the reduction
        as a SemiGroup does not define a zero value.

        :param values: The values to reduce to a single value
        :returns: The final reduced value
        '''
        return reduce(self.plus, values)

class Monoid(SemiGroup):

    def __init__(self, zero, lift, plus):
        ''' Creates a new instance of the Monoid class

        :param zero: The zero value for this monoid
        :param left: The method used to lift a value into the monoid
        :param plus: The associative plus function for the monoid
        '''
        self.zero = zero
        self.lift = lift
        self.plus = plus

    def is_zero(self, value):
        ''' Checks if the current value is the additive identity
        element.

        :returns: True if this is the identity, False otherwise
        '''
        return value == self.zero

    def if_zero_throw(self, value, default=None):
        ''' If the supplied value is zero, throw else return
        the supplied default value.

        :param value: The value to heck for zero
        :param default: A value to optionally return if not zero (default None)
        :returns: The supplied default value
        '''
        if value == self.zero:
            raise ZeroDivisionError('algebra values must not be zero')
        return defult

    def sum(self, values):
        ''' Given a collection of values, reduce the values to
        a single value based on the monoid plus operation.

        :param values: The values to reduce to a single value
        :returns: The final reduced value
        '''
        if not values: return self.zero
        return reduce(self.plus, values, self.zero)

class Group(Monoid):

    def __init__(self, negate=None, minus=None, **kwargs):
        ''' Initializes a new instance of the Group class

        :param negate: The method used to negate a value
        :param minus: The method used to subtract two values
        '''
        super(Group, self).__init__(**kwargs)
        assert(negate or minus) # must have one or the other
        self.negate = negate or (lambda v: minus(self.zero, v))
        self.minus  = minus  or (lambda l, r: self.plus(l, negate(r)))

class Ring(Group):

    def __init__(self, one, times, **kwargs):
        ''' Initializes a new instance of the Group class

        :param one: The multiplicative identity operator
        :param times: The multiplicative operation
        '''
        super(Ring, self).__init__(**kwargs)
        self.one   = one
        self.times = times

    def is_one(self, value):
        ''' Checks if the current value is the multiplicative
        identity element.

        :returns: True if this is the identity, False otherwise
        '''
        return value == self.one

    def product(self, values):
        ''' Given a collection of values, reduce the values to
        a single value based on the ring times operation.

        :param values: The values to reduce to a single value
        :returns: The final reduced value
        '''
        if not values: return self.one
        return reduce(self.times, values, self.one)

class Ring(Group):

    def __init__(self, one, times, **kwargs):
        ''' Initializes a new instance of the Group class

        :param one: The multiplicative identity operator
        :param times: The multiplicative operation
        '''
        super(Ring, self).__init__(**kwargs)
        self.one   = one
        self.times = times

    def is_one(self, value):
        ''' Checks if the current value is the multiplicative
        identity element.

        :returns: True if this is the identity, False otherwise
        '''
        return value == self.one

    def product(self, values):
        ''' Given a collection of values, reduce the values to
        a single value based on the ring times operation.

        :param values: The values to reduce to a single value
        :returns: The final reduced value
        '''
        if not values: return self.one
        return reduce(self.times, values, self.one)

class Field(Ring):

    def __init__(self, inverse=None, divide=None, **kwargs):
        ''' Initializes a new instance of the Field class

        :param inverse: The inversion operator (1 / n)
        :param divide: The division operation
        '''
        super(Field, self).__init__(**kwargs)
        assert(inverse or divide)
        self.inverse = inverse or (lambda v: divide(self.one, v))
        self.divide  = divide  or (lambda l, r: self.times(l, inverse(r)))

#------------------------------------------------------------
# Helper methods for creating monoids
#------------------------------------------------------------
def __map_monoid_plus(l, r):
    clone = l.copy()
    clone.update(*r)
    return clone

def __counter_monoid_plus(l, r):
    l[r] + 1
    return l

#------------------------------------------------------------
# The Algebras
#------------------------------------------------------------
# TODO define the group methods of these
# TODO rings, fields
# https://github.com/twitter/algebird/tree/develop/algebird-core/src/main/scala/com/twitter/algebird
#------------------------------------------------------------
AndGroup = Group(**{
    'zero':    True,
    'lift':    bool,
    'plus':    operator.and_,
    'negate':  operator.not_})

OrGroup = Group(**{
    'zero':    False,
    'lift':    bool,
    'plus':    operator.or_,
    'negate':  operator.not_})

BooleanField = Field(**{
    'zero':    False,
    'one':     True,
    'lift':    bool,
    'plus':    operator.xor,
    'minus':   operator.xor,
    'times':   operator.and_,
    'inverse': lambda x: self.if_zero_throw(x, True),
    'divide':  lambda l, r: self.if_zero_throw(r, l)})

AdditionGroup = Group(**{
    'zero':   0,
    'lift':   combinator.identity,
    'plus':   operator.add,
    'negate': operator.neg,
    'minus':  operator.sub})

AdditionMonoid = Group(zero=0, lift=lambda x: x, plus=operator.add, negate=operator.neg, minus=operator.sub)
ProductMonoid  = Group(zero=1, lift=lambda x: x, plus=operator.mul, negate=operator.neg, minus=operator.div)
MapMonoid      = Monoid(zero={}, lift=lambda x: x, plus=__map_monoid_plus)
CounterMonoid  = Monoid(zero=Counter(), lift=lambda x: x, plus=__counter_monoid_plus)
StringMonoid   = Monoid(zero='', lift=lambda x: str(x), plus=lambda l,r: l + str(r))
ListMonoid     = Monoid(zero=list(), lift=lambda x: [x], plus=lambda l,r: l + [r])
TupleMonoid    = Monoid(zero=tuple(), lift=lambda x: (x,), plus=lambda l,r: l + (r,))
SetMonoid      = Monoid(zero=set(), lift=lambda x: set(x), plus=lambda l,r: l.union(r))
FunctionMonoid = Monoid(zero=combinator.identity, lift=combinator.constant, plus=combinator.compose)

#------------------------------------------------------------
# Aliases
#------------------------------------------------------------
#AndMonoid, AndSemiGroup             = [AndGroup] * 2
#OrMonoid, OrSemiGroup               = [OrGroup] * 2
#AdditionMonoid, AdditionSemiGroup   = [AdditionGroup] * 2
#ProductMonoid, ProductSemiGroup     = [ProductGroup] * 2
#MapMonoid, MapSemiGroup             = [MapGroup] * 2
#CounterMonoid, CounterSemiGroup     = [CounterGroup] * 2
#StringMonoid, StringSemiGroup       = [StringGroup] * 2
#ListMonoid, ListSemiGroup           = [ListGroup] * 2
#TupleMonoid, TupleSemiGroup         = [TupleGroup] * 2
#SetMonoid, SetSemiGroup             = [SetGroup] * 2
#FunctionMonoid, FunctionSemiGroup   = [FunctionGroup] * 2
