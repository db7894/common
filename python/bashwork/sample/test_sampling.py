#!/usr/bin/env python
import unittest
from bashwork.sample.sampling import *

class SamplingTest(unittest.TestCase):

    def test_popular_exact(self):
        coll = [1,3,2,4,1,5,3,2,6,7,2,4,3,4]
        actual = popular_exact(coll, 3)
        expect = [2, 3, 4]
        self.assertEquals(actual, expect)

    def test_popular(self):
        coll = [1,3,2,4,1,5,3,2,6,7,2,4,3,4]
        actual = popular_exact(coll, 3)
        expect = [2, 3, 4]
        self.assertEquals(actual, expect)

    def test_random(self):
        seed(0) # to force same results
        coll = range(0, 10)
        actual = random(coll, 3)
        expect = [0, 6, 5]
        self.assertEquals(actual, expect)

    def test_largest_difference(self):
        self.assertEquals(8, largest_difference([4,5,6,1,4,7,9]))
        self.assertEquals(0, largest_difference([5,4,3,2,1]))

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
