#!/usr/bin/env python
import unittest
import numpy as np
from bashwork.ml.utilities import sample

class SampleTest(unittest.TestCase):

    def test_sample_everything(self):
        a = np.arange(0, 10)
        actual = sample.everything(a)
        expect = a
        self.assertTrue(np.array_equal(actual, expect))

    def test_sample_random(self):
        a = np.arange(0, 10)
        actual = sample.random(a, 5)
        expect = a
        self.assertFalse(np.array_equal(actual, expect))

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
