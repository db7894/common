#!/usr/bin/python
# -*- coding: utf-8 -*-

def knapsack(capacity, items):
    ''' Given a capacity and a collection of items, solve
    the knapsack problem using a dynamic programming approach.
    Items should support the following interface::

        class Item(object):
            value : Int
            weight: Int

    :param capacity: The total capacity of the knapsack
    :param items: The possible items to choose from
    :returns: A tuple of (total_value, [selected items])
    '''
    lookup = [[0] * len(items) for _ in range(capacity)]
    for itemid, item in enumerate(items):
        for weight in range(1, capacity):
            lookup[weight][itemid] = lookup[weight][itemid - 1]
            if item.weight <= weight:
                possible = item.value + lookup[weight - item.weight][itemid - 1]
                lookup[weight][itemid] = max(lookup[weight][itemid], possible)

    weight   = capacity - 1
    value    = lookup[weight][len(items) - 1]
    selected = []

    for itemid in range(len(items) - 1, -1, -1):
        if lookup[weight][itemid] != lookup[weight][itemid - 1]:
            selected.append(items[itemid])
            weight = items[itemid].weight
    return value, selected

from collections import namedtuple
Item = namedtuple("Item", ['index', 'value', 'weight'])
