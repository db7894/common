#!/usr/bin/env python
import unittest
import copy
from bashwork.structure.crdt import *

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#

class CRDTTest(unittest.TestCase):

    def test_crdt_methods(self):
        crdt = CRDT()
        self.assertRaises(TypeError, lambda: CRDT.merge(crdt, crdt))

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()

