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

    def test_reservoir(self):
        seed(0) # to force same results
        coll = [1,3,2,4,1,5,3,2,6,7,2,4,3,4]
        actual = reservoir(coll, 3)
        expect = [1, 3, 5]
        self.assertEquals(actual, expect)

    def test_random_sample(self):
        seed(0) # to force same results
        coll = range(0, 10)
        actual = random_sample(coll, 3)
        expect = [0, 6, 5]
        self.assertEquals(actual, expect)

    def test_largest_difference(self):
        self.assertEquals(8, largest_difference([4,5,6,1,4,7,9]))
        self.assertEquals(0, largest_difference([5,4,3,2,1]))

    def test_get_largest_product(self):
        xs = [4,6,3,8,-2]
        rs = [6, 8, 4]
        self.assertEquals(get_largest_product(xs, 3), rs)

        xs = [4, 6, 3, -30, 25, 7, -12, 4, 8,-2]
        rs = [-30, -12, 25]
        self.assertEquals(get_largest_product(xs, 3), rs)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
