#!/usr/bin/env python
import unittest
import pandas as pd
from bashwork.statistics.distribution.discrete import *

class DiscreteDistributionTest(unittest.TestCase):

    def test_is_valid(self):
        dist = Distribution.create(range(10))
        self.assertTrue(dist.is_valid())

        prob = pd.Series({'a': 0.2, 'b': 0.2, 'c': 0.2, 'd': 0.2, 'e': 0.2})
        self.assertTrue(Distribution(prob).is_valid())

        prob = pd.Series({'a': 0.2, 'b': 0.2, 'c': 0.2, 'd': 0.4, 'e': 0})
        self.assertTrue(Distribution(prob).is_valid())

        prob = pd.Series({'a': 0.2, 'b': 0.2, 'c': 0.2, 'd': 0.2, 'e': 0})
        self.assertFalse(Distribution(prob).is_valid())

        prob = pd.Series({'a': 0.2, 'b': 0.2, 'c': 0.2, 'd': 0.6, 'e': -0.2})
        self.assertFalse(Distribution(prob).is_valid())

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
