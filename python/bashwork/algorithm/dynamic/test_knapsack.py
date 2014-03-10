#!/usr/bin/python
# -*- coding: utf-8 -*-
import unittest
from collections import namedtuple
from bashwork.algorithm.dynamic.knapsack import knapsack

Item = namedtuple("Item", ['index', 'value', 'weight'])

class KnapsackTest(unittest.TestCase):

    def test_knapsack(self):
        items    = [Item(0, 8, 4), Item(1, 10, 5), Item(2, 15, 8), Item(3, 4, 3)]
        capacity = 11
        value, actual = knapsack(capacity, items)
        expect = [items[1], items[0]]
        self.assertEqual(value, 18)
        self.assertEqual(expect, actual)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
