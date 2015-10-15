#!/usr/bin/env python
import unittest
import copy
from bashwork.structure.crdt.register import *

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#

class RegisterTest(unittest.TestCase):

    def test_lwwregister(self):
        register1 = LWWRegister()
        register1.set(10)
        
        self.assertEqual(10, register1.value())

        register2 = LWWRegister()
        register2.set(15)
        register3 = register1.merge(register2)
        register4 = LWWRegister.merge(register1, register2)

        self.assertEqual(15, register2.value())
        self.assertFalse(register2.compare(register1))
        self.assertEqual(15, register3.value())
        self.assertTrue(register1.compare(register3))
        self.assertEqual(register4.value(), register4.value())

    def test_lwwregister_serialization(self):
        register1 = LWWRegister()
        register1.set(10)

        payload   = register1.to_json()
        register2 = LWWRegister.from_json(payload)
        register3 = copy.copy(register1)

        self.assertEqual(register1.value(), register2.value())
        self.assertEqual(register1.value(), register3.value())

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
