#!/usr/bin/env python
import unittest
import copy
from bashwork.structure.crdt.counter import *

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class CounterTest(unittest.TestCase):

    def test_gcounter(self):
        counter1 = GCounter(name="counter1")
        counter1.increment(10)
        
        self.assertEqual(10, counter1.value())

        counter2 = GCounter(name="counter2")
        counter2.increment(5)
        counter3 = counter1.merge(counter2)
        counter4 = GCounter.merge(counter1, counter2)

        self.assertEqual(5, counter2.value())
        self.assertFalse(counter1.compare(counter2))
        self.assertEqual(15, counter3.value())
        self.assertTrue(counter1.compare(counter3))
        self.assertEqual(counter3.value(), counter4.value())

    def test_gcounter_serialization(self):
        counter1 = GCounter(name="counter1")
        counter1 += 10

        payload  = counter1.to_json()
        counter2 = GCounter.from_json(payload)
        counter3 = copy.copy(counter1)

        self.assertEqual(counter1.value(), counter2.value())
        self.assertEqual(counter1.value(), counter3.value())

    def test_pncounter(self):
        counter1 = PNCounter(name="counter1")
        counter1.increment(10)
        counter1.decrement(5)
        
        self.assertEqual(5, counter1.value())

        counter2 = PNCounter(name="counter2")
        counter2.increment(5)
        counter2.decrement(7)
        counter3 = counter1.merge(counter2)
        counter4 = PNCounter.merge(counter1, counter2)

        self.assertEqual(-2, counter2.value())
        self.assertFalse(counter1.compare(counter2))
        self.assertEqual(3, counter3.value())
        self.assertTrue(counter1.compare(counter3))
        self.assertEqual(counter3.value(), counter4.value())

    def test_pncounter_serialization(self):
        counter1  = PNCounter(name="counter1")
        counter1 += 10
        counter1 -= 5

        payload  = counter1.to_json()
        counter2 = PNCounter.from_json(payload)
        counter3 = copy.copy(counter1)

        self.assertEqual(counter1.value(), counter2.value())
        self.assertEqual(counter1.value(), counter3.value())

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
