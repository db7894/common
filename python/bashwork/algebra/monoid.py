'''
.. todo::
   - monad based monoids (maybe, either, etc)
   - Map[Monoid] (monoid for how to add values)
   - count min sketch
   - SpaceSaver
   - topK
   - hyperloglog
   - bloom filter
   - sliding window
   - tuple of (monoida, monoidb, monoidc)
   - https://github.com/twitter/algebird/blob/develop/algebird-core/src/main/scala/com/twitter/algebird/DecayedValue.scala
   - the other utilities of algebird (summing queue)
'''
import operator
import itertools
import heapq
from collections import Counter
from bashwork import combinator

#------------------------------------------------------------
# Type Classes
#------------------------------------------------------------

class SemiGroup(object):
    ''' A semigroup is an algebraic structure consisting of
    a set together with an associative binary operation. A
    semigroup generalizes a monoid in that a semigroup need
    not have an identity element.
    '''

    def __init__(self, lift, plus=None):
        ''' Creates a new instance of the SemiGroup class

        :param lift: The method used to lift a value into the monoid
        :param plus: The associative plus function for the monoid
        '''
        self.lift = lift
        self.plus = plus or operator.add

    def sum(self, values):
        ''' Given a collection of values, reduce the values to
        a single value based on the monoid's operations. It should
        be noted that there is no initial value of the reduction
        as a SemiGroup does not define a zero value.

        :param values: The values to reduce to a single value
        :returns: The final reduced value
        '''
        return reduce(self.plus, values)

    def sum_with_lift(self, values):
        ''' Given a collection of values, reduce the values to
        a single value based on the monoid plus operation. This
        method runs lift on all the supplied values to give them
        a minimal context.

        :param values: The values to reduce to a single value
        :returns: The final reduced value
        '''
        return reduce(self.plus, (self.lift(v) for v in values))

class Monoid(SemiGroup):

    def __init__(self, zero, **kwargs):
        ''' Creates a new instance of the Monoid class

        :param zero: The zero value for this monoid
        '''
        super(Monoid, self).__init__(**kwargs)
        self.zero = zero

    def is_zero(self, value):
        ''' Checks if the current value is the additive identity
        element.

        :returns: True if this is the identity, False otherwise
        '''
        return value == self.zero

    def if_zero_throw(self, value):
        ''' If the supplied value is zero, throw else return
        the supplied default value.

        :param value: The value to heck for zero
        '''
        if value == self.zero:
            raise ZeroDivisionError('algebra values must not be zero')

    def sum(self, values):
        ''' Given a collection of values, reduce the values to
        a single value based on the monoid plus operation.

        :param values: The values to reduce to a single value
        :returns: The final reduced value
        '''
        if not values: return self.zero
        return reduce(self.plus, values, self.zero)

    def sum_with_lift(self, values):
        ''' Given a collection of values, reduce the values to
        a single value based on the monoid plus operation. This
        method runs lift on all the supplied values to give them
        a minimal context.

        :param values: The values to reduce to a single value
        :returns: The final reduced value
        '''
        if not values: return self.zero
        return reduce(self.plus, (self.lift(v) for v in values), self.zero)

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

    def __init__(self, one, times=None, **kwargs):
        ''' Initializes a new instance of the Group class

        :param one: The multiplicative identity operator
        :param times: The multiplicative operation
        '''
        super(Ring, self).__init__(**kwargs)
        self.one   = one
        self.times = times or operator.mul

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
        # TODO wrap with if_zero_throw
        self.inverse = inverse or (lambda v: divide(self.one, v))
        self.divide  = divide  or (lambda l, r: self.times(l, inverse(r)))

#------------------------------------------------------------
# Fields
#------------------------------------------------------------

BooleanField = Field(**{
    'zero':    False,
    'one':     True,
    'lift':    bool,
    'plus':    operator.xor,
    'minus':   operator.xor,
    'times':   operator.and_,
    'negate':  operator.not_,
    'inverse': combinator.constant_true,
    'divide':  lambda l, r: l,
})

IntField = Field(**{
    'zero':    0,
    'one':     1,
    'lift':    int,
    'plus':    operator.add,
    'negate':  operator.neg,
    'minus':   operator.sub,
    'times':   operator.mul,
    'divide':  operator.div,
})

FloatField = Field(**{
    'zero':    0.0,
    'one':     1.0,
    'lift':    float,
    'plus':    operator.add,
    'negate':  operator.neg,
    'minus':   operator.sub,
    'times':   operator.mul,
    'divide':  operator.div,
})

#------------------------------------------------------------
# Rings
#------------------------------------------------------------

IntRing        = IntField
FloatRing      = FloatField
BooleanRing    = BooleanField

#------------------------------------------------------------
# Groups
#------------------------------------------------------------

class ConstantGroup(Group):
    ''' A group that only represents a single constant
    value. Basically a singleton group. All operations
    are basically the operator::

      S constant _ = constant
    '''

    def __init__(self, value):
        ''' Initialize a new instance of the ConstantGroup

        :param value: The constant value to operate with
        '''
        super(ConstantGroup, self).__init__(**{
            'zero':    value,
            'lift':    combinator.constant(value),
            'plus':    combinator.constant(value),
            'negate':  combinator.constant(value),
            'minus':   combinator.constant(value),
        })

NoneGroup      = ConstantGroup(None)
IntGroup       = IntField
FloatGroup     = FloatField
BooleanGroup   = BooleanField

#------------------------------------------------------------
# Monoid Helper Methods
#------------------------------------------------------------

def __map_monoid_plus(l, r):
    clone = l.copy()
    copy.update(r)
    return copy

def __mutable_map_monoid_plus(l, r):
    l.update(r)
    return l

#------------------------------------------------------------
# Monoids
#------------------------------------------------------------
class PriorityQueueMonoid(Monoid):

    def __init__(self, count):
        ''' Creates a new instance of the PrioirtyQueueMonoid class

        :param count: The count of items to keep
        '''
        self.count = max(1, count)

    def zero(self, v): return []
    def lift(self, v): return [v]
    def plus(self, l, r):
        heap = list(heqpq.merge(l, r))
        while len(heap) > self.count:
            heapq.heappop(heap)
        return heap


NoneMonoid     = NoneGroup
IntMonoid      = IntField
FloatMonoid    = FloatField
BooleanMonoid  = BooleanField

PriorityQueueMonoid = Monoid(**{
    'zero': [],
    'lift': lambda x: [x],
    'plus': operator.and_,
})

AndMonoid = Monoid(**{
    'zero': True,
    'lift': bool,
    'plus': operator.and_,
})

OrMonoid = Monoid(**{
    'zero': False,
    'lift': bool,
    'plus': operator.or_,
})

MapMonoid = Monoid(**{
    'zero': {},
    'lift': combinator.identity,
    'plus': __map_monoid_plus,
})

MutableMapMonoid = Monoid(**{
    'zero': {},
    'lift': combinator.identity,
    'plus': __mutable_map_monoid_plus,
})

CounterMonoid = Monoid(**{
    'zero': Counter(),
    'lift': combinator.identity,
    'plus': operator.add,
})

StringMonoid = Monoid(**{
    'zero': '',
    'lift': str,
    'plus': operator.add,
})

ListMonoid = Monoid(**{
    'zero': [],
    'lift': lambda x: [x],
    'plus': operator.add,
})

IterMonoid = Monoid(**{
    'zero': iter([]),
    'lift': lambda x: iter([x]),
    'plus': itertools.chain,
})

SetMonoid = Monoid(**{
    'zero': set(),
    'lift': lambda x: {x},
    'plus': lambda l,r: l.union(r),
})

TupleMonoid = Monoid(**{
    'zero': tuple(),
    'lift': lambda x: (x,),
    'plus': operator.add,
})

FunctionMonoid = Monoid(**{
    'zero': combinator.identity,
    'lift': combinator.constant,
    'plus': combinator.compose,
})

#------------------------------------------------------------
# SemiGroups
#------------------------------------------------------------

NoneSemiGroup       = NoneGroup
IntSemiGroup        = IntField
FloatSemiGroup      = FloatField
BooleanSemiGroup    = BooleanField
AndSemiGroup        = AndMonoid
OrSemiGroup         = OrMonoid
MapSemiGroup        = MapMonoid
MutableMapSemiGroup = MutableMapMonoid
CounterSemiGroup    = CounterMonoid
StringSemiGroup     = StringMonoid
IterSemiGroup       = IterMonoid
ListSemiGroup       = ListMonoid
SetSemiGroup        = SetMonoid
TupleSemiGroup      = TupleMonoid
FunctionSemiGroup   = FunctionMonoid
