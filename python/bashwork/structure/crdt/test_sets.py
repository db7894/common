#!/usr/bin/env python
import unittest
import copy
from bashwork.structure.crdt import CRDTLaws
from bashwork.structure.crdt.sets import *

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class SetsTest(unittest.TestCase):

    def test_gset(self):
        set1 = GSet()
        set1.add('a')
        
        self.assertEqual({'a'}, set1.value())

        set2 = GSet()
        set2.add('b')
        set3 = set1.merge(set2)
        set4 = GSet.merge(set1, set2)

        self.assertEqual({'b'}, set2.value())
        self.assertTrue('a' in set1)
        self.assertFalse(set1.compare(set2))
        self.assertEqual({'a', 'b'}, set3.value())
        self.assertTrue(set1.compare(set3))
        self.assertEqual(set3.value(), set4.value())
        CRDTLaws.test(set1, set2, set3)

    def test_gset_serialization(self):
        set1 = GSet()
        set1 += 'a'

        payload  = set1.to_json()
        set2 = GSet.from_json(payload)
        set3 = copy.copy(set1)

        self.assertEqual(set1.value(), set2.value())
        self.assertEqual(set1.value(), set3.value())

    def test_twophaseset(self):
        self.assert_set_operations(TwoPhaseSet)
        self.assert_set_serialization(TwoPhaseSet)

    def test_lwwset(self):
        self.assert_set_operations(LWWSet)
        self.assert_set_serialization(LWWSet)

    def test_pnset(self):
        self.assert_set_operations(PNSet)
        self.assert_set_serialization(PNSet)

    def test_orset(self):
        self.assert_set_operations(ORSet)
        self.assert_set_serialization(ORSet)

    def assert_set_operations(self, klass):
        set1 = klass()
        for x in 'abc': set1.add(x)
        set1.remove('b')
        
        self.assertEqual({'a', 'c'}, set1.value())

        set2  = klass()
        set2 += 'a'
        set2 -= 'a'
        set3  = set1.merge(set2)
        set4  = klass.merge(set1, set2)

        self.assertEqual(set(), set2.value())
        self.assertFalse(set1.compare(set2))
        self.assertEqual({'c'}, set3.value())
        self.assertEqual(set3.value(), set4.value())
        CRDTLaws.test(set1, set2, set3)

    def assert_set_serialization(self, klass):
        set1  = klass()
        set1 += 'a'
        set1 += 'b'
        set1 -= 'b'

        payload  = set1.to_json()
        set2 = klass.from_json(payload)
        set3 = copy.copy(set1)

        self.assertTrue('a' in set1)
        self.assertFalse('b' in set1)
        self.assertEqual(set1.value(), set2.value())
        self.assertEqual(set1.value(), set3.value())

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
