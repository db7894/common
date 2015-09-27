#!/usr/bin/env python
import unittest
from collections import Counter
from bashwork.algebra.monoid import *

class MonoidsTest(unittest.TestCase):

    def test_constant_group(self):
        v, l, vs, vl = 0, '0', [0, 0], ['0', '0']
        z = None

        assert NoneGroup.zero              == z
        assert NoneGroup.plus(v, v)        == None
        assert NoneGroup.lift(l)           == None
        assert NoneGroup.negate(v)         == None
        assert NoneGroup.minus(v)          == None
        assert NoneGroup.is_zero(z)        == True
        assert NoneGroup.is_zero(v)        == False
        assert NoneGroup.sum(vs)           == None
        assert NoneGroup.sum_with_lift(vl) == None

    def test_int_field(self):
        v, l, vs, vl = 2, '2', [2, 2], ['2', '2']
        z, o = 0, 1

        assert IntField.zero              == z
        assert IntField.one               == o
        assert IntField.plus(v, v)        == 4
        assert IntField.lift(l)           == 2
        assert IntField.negate(v)         == -2
        assert IntField.minus(v, v)       == 0
        assert IntField.is_zero(v)        == False
        assert IntField.is_zero(z)        == True
        assert IntField.is_one(z)         == False
        assert IntField.is_one(o)         == True
        assert IntField.sum(vs)           == 4
        assert IntField.sum_with_lift(vl) == 4
        assert IntField.product([])       == 1
        assert IntField.product(vs)       == 4
        self.assertRaises(ZeroDivisionError, lambda: IntField.if_zero_throw(z))

    def test_float_field(self):
        v, l, vs, vl = 2.0, '2.0', [2.0, 2.0], ['2.0', '2.0']
        z, o = 0.0, 1.0

        assert FloatField.zero              == z
        assert FloatField.one               == o
        assert FloatField.plus(v, v)        == 4.0
        assert FloatField.lift(l)           == 2.0
        assert FloatField.negate(v)         == -2.0
        assert FloatField.minus(v, v)       == 0.0
        assert FloatField.is_zero(v)        == False
        assert FloatField.is_zero(z)        == True
        assert FloatField.is_one(z)         == False
        assert FloatField.is_one(o)         == True
        assert FloatField.sum(vs)           == 4.0
        assert FloatField.sum_with_lift(vl) == 4.0
        assert FloatField.product([])       == 1.0
        assert FloatField.product(vs)       == 4.0

    def test_boolean_field(self):
        v, l, vs, vl = True, 'True', [True, False], ['True', 'True']
        z, o = False, True

        assert BooleanField.zero              == z
        assert BooleanField.one               == o
        assert BooleanField.plus(v, v)        == False
        assert BooleanField.lift(l)           == True
        assert BooleanField.negate(v)         == False
        assert BooleanField.minus(v, v)       == False
        assert BooleanField.is_zero(v)        == False
        assert BooleanField.is_zero(z)        == True
        assert BooleanField.is_one(z)         == False
        assert BooleanField.is_one(o)         == True
        assert BooleanField.sum(vs)           == True
        assert BooleanField.sum_with_lift(vl) == False
        assert BooleanField.product([])       == True
        assert BooleanField.product(vs)       == False

    def test_and_monoid(self):
        v, l, vs, vl = False, 'True', [True, False], ['True', 'True']
        z = True

        assert AndMonoid.zero              == z
        assert AndMonoid.plus(v, v)        == False
        assert AndMonoid.lift(l)           == True
        assert AndMonoid.is_zero(v)        == False
        assert AndMonoid.is_zero(z)        == True
        assert AndMonoid.sum(vs)           == False
        assert AndMonoid.sum_with_lift(vl) == True

    def test_or_monoid(self):
        v, l, vs, vl = True, 'True', [True, False], ['True', 'True']
        z = False

        assert OrMonoid.zero                   == z
        assert OrMonoid.plus(v, v)             == True
        assert OrMonoid.lift(l)                == True
        assert OrMonoid.is_zero(v)             == False
        assert OrMonoid.is_zero(z)             == True
        assert OrMonoid.sum(vs)                == True
        assert OrMonoid.sum_with_lift(vl)      == True

    def test_counter_monoid(self):
        v, l, vs, vl = Counter('abc'), 'abc', [Counter('abc')], ['abc', '123']
        z = Counter()

        assert CounterMonoid.zero              == z
        assert CounterMonoid.plus(v, v)        == Counter('abcabc')
        assert CounterMonoid.lift(l)           == Counter('abc')
        assert CounterMonoid.is_zero(v)        == False
        assert CounterMonoid.is_zero(z)        == True
        assert CounterMonoid.sum(vs)           == Counter('abc')
        assert CounterMonoid.sum_with_lift(vl) == Counter('abc123')

    def test_string_monoid(self):
        v, l, vs, vl = 'abc', 123, ['abc', '123'], [123, 456]
        z = ''

        assert StringMonoid.zero               == z
        assert StringMonoid.plus(v, v)         == 'abcabc'
        assert StringMonoid.lift(l)            == '123'
        assert StringMonoid.is_zero(v)         == False
        assert StringMonoid.is_zero(z)         == True
        assert StringMonoid.sum(vs)            == 'abc123'
        assert StringMonoid.sum_with_lift(vl)  == '123456'

    def test_list_monoid(self):
        v, l, vs, vl = ['a'], 'a', [['a', 'b'], ['c']], ['a', 'b', 'c']
        z = []

        assert ListMonoid.zero                 == z
        assert ListMonoid.plus(v, v)           == ['a', 'a']
        assert ListMonoid.lift(l)              == ['a']
        assert ListMonoid.is_zero(v)           == False
        assert ListMonoid.is_zero(z)           == True
        assert ListMonoid.sum(vs)              == ['a', 'b', 'c']
        assert ListMonoid.sum_with_lift(vl)    == ['a', 'b', 'c']

    def test_priority_queue_monoid(self):
        v, l, vs, vl = ['a'], 'a', [['a', 'b'], ['c']], ['a', 'b', 'c', 'd']
        z = []

        monoid = PriorityQueueMonoid(count=3)
        assert monoid.zero                     == z
        assert monoid.plus(v, v)               == ['a', 'a']
        assert monoid.lift(l)                  == ['a']
        assert monoid.is_zero(v)               == False
        assert monoid.is_zero(z)               == True
        assert monoid.sum(vs)                  == ['a', 'b', 'c']
        assert monoid.sum_with_lift(vl)        == ['b', 'd', 'c']

    def test_set_monoid(self):
        v, l, vs, vl = {'a'}, 'a', [{'a', 'b'}, {'c'}], ['a', 'b', 'c']
        z = set()

        assert SetMonoid.zero                  == z
        assert SetMonoid.plus(v, v)            == {'a'}
        assert SetMonoid.lift(l)               == {'a'}
        assert SetMonoid.is_zero(v)            == False
        assert SetMonoid.is_zero(z)            == True
        assert SetMonoid.sum(vs)               == {'a', 'b', 'c'}
        assert SetMonoid.sum_with_lift(vl)     == {'a', 'b', 'c'}

    def test_tuple_monoid(self):
        v, l, vs, vl = ('a',), 'a', [('a', 'b'), ('c',)], ['a', 'b', 'c']
        z = tuple()

        assert TupleMonoid.zero                == z
        assert TupleMonoid.plus(v, v)          == ('a', 'a')
        assert TupleMonoid.lift(l)             == ('a',)
        assert TupleMonoid.is_zero(v)          == False
        assert TupleMonoid.is_zero(z)          == True
        assert TupleMonoid.sum(vs)             == ('a', 'b', 'c')
        assert TupleMonoid.sum_with_lift(vl)   == ('a', 'b', 'c')

    def test_map_monoid(self):
        v, l, vs, vl = {'a': 1}, ('a', 1), [{'a' : 1}, {'b' : 2}], [('a', 1), ('b', 2)]
        z = {}

        assert MapMonoid.zero                  == z
        assert MapMonoid.plus(v, v)            == {'a': 1}
        assert MapMonoid.lift(l)               == {'a': 1}
        assert MapMonoid.is_zero(v)            == False
        assert MapMonoid.is_zero(z)            == True
        assert MapMonoid.sum(vs)               == {'a' : 1, 'b' : 2}
        assert MapMonoid.sum_with_lift(vl)     == {'a' : 1, 'b' : 2}

    def test_mutable_map_monoid(self):
        v, l, vs, vl = {'a': 1}, ('a', 1), [{'a' : 1}, {'b' : 2}], [('a', 1), ('b', 2)]
        z = {}

        assert MutableMapMonoid.zero              == z
        assert MutableMapMonoid.plus(v, v)        == {'a': 1}
        assert MutableMapMonoid.lift(l)           == {'a': 1}
        assert MutableMapMonoid.is_zero(v)        == False
        assert MutableMapMonoid.is_zero(z)        == True
        assert MutableMapMonoid.sum(vs)           == {'a' : 1, 'b' : 2}
        assert MutableMapMonoid.sum_with_lift(vl) == {'a' : 1, 'b' : 2}

    def test_iter_monoid(self):
        v, l, vs, vl = [1], 1, [[1], [2]], [1, 2]
        z = iter([])

        assert list(IterMonoid.zero)              == []
        assert list(IterMonoid.plus(v, v))        == [1, 1]
        assert list(IterMonoid.lift(l))           == [1]
        assert IterMonoid.is_zero(iter(v))        == False
        assert IterMonoid.is_zero(z)              == True
        assert list(IterMonoid.sum(vs))           == [1, 2]
        assert list(IterMonoid.sum_with_lift(vl)) == [1, 2]

    def test_function_monoid(self):
        o  = 1
        v  = lambda x: x + 2
        l  = 2
        vs = [lambda x: x + 2, lambda x: x + 4]
        vl = [2, 4]
        z  = FunctionMonoid.zero

        assert FunctionMonoid.zero(o)              == 1
        assert FunctionMonoid.plus(v, v)(o)        == 5
        assert FunctionMonoid.lift(l)(o)           == 2
        assert FunctionMonoid.is_zero(v)           == False
        assert FunctionMonoid.is_zero(z)           == True
        assert FunctionMonoid.sum(vs)(o)           == 7
        assert FunctionMonoid.sum_with_lift(vl)(o) == 4

    def test_semigroup(self):
        vs = [2, 2]
        vl = ['2', '2']

        semigroup = SemiGroup(lift=int, plus=lambda x, y: x + y)
        assert semigroup.sum(vs)               == 4
        assert semigroup.sum_with_lift(vs)     == 4

    def test_multi_monoid(self):
        v  = (2, {2})
        l  = 2
        vs = [(1, {1}), (2, {2})]
        vl = [2, 3]
        z  = (0, set())

        monoid = MultiMonoid(IntMonoid, SetMonoid)
        assert monoid.zero                     == z
        assert monoid.plus(v, v)               == (4, {2})
        assert monoid.lift(l)                  == (2, {2})
        assert monoid.is_zero(v)               == False
        assert monoid.is_zero(z)               == True
        assert monoid.sum(vs)                  == (3, {1, 2})
        assert monoid.sum_with_lift(vl)        == (5, {2, 3})

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
