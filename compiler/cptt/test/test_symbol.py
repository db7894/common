#!/usr/bin/env python
import unittest
from cptt.symbol import Environment

class CpttEnvironmentTest(unittest.TestCase):

    def testSimpleLookup(self):
        head = Environment()
        head.put("key", "value");

        self.assertEqual("value", head.get("key"))

    def testHierarchyLookup(self):
        base = Environment()
        head = Environment(base)
        base.put("key", "value");

        self.assertEqual("value", head.get("key"))

    def testHiddenHierarchyLookup(self):
        base = Environment()
        head = Environment(base)
        base.put("key", "value1");
        head.put("key", "value2");

        self.assertEqual("value1", base.get("key"))
        self.assertEqual("value2", head.get("key"))

#---------------------------------------------------------------------------#
# Main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
