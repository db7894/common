#!/usr/bin/env python
import unittest
from bashwork.statistics.common import *

class StatisticsDistributionTest(unittest.TestCase):

    def test_is_valid(self):
        dist = Distribution.create(range(10))
        self.assertTrue(dist.is_valid())

        dist = Distribution({'a': 0.2, 'b': 0.2, 'c': 0.2, 'd': 0.2, 'e': 0.2})
        self.assertTrue(dist.is_valid())

        dist = Distribution({'a': 0.2, 'b': 0.2, 'c': 0.2, 'd': 0.4, 'e': 0})
        self.assertTrue(dist.is_valid())

        dist = Distribution({'a': 0.2, 'b': 0.2, 'c': 0.2, 'd': 0.2, 'e': 0})
        self.assertFalse(dist.is_valid())

        dist = Distribution({'a': 0.2, 'b': 0.2, 'c': 0.2, 'd': 0.6, 'e': -0.2})
        self.assertFalse(dist.is_valid())

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
