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
    lookup = [[0] * (len(items) + 1) for _ in range(capacity + 1)]
    for index, item in enumerate(items, start=1):
        for weight in range(1, capacity + 1):
            lookup[weight][index] = lookup[weight][index - 1]
            if item.weight <= weight:
                possible = item.value + lookup[weight - item.weight][index - 1]
                lookup[weight][index] = max(lookup[weight][index], possible)

    weight   = capacity 
    value    = lookup[weight][len(items)]
    selected = []

    for index in range(len(items), -1, -1):
        if lookup[weight][index] != lookup[weight][index - 1]:
            selected.append(items[index - 1])
            weight = items[index - 1].weight
    return value, selected

def greedy_knapsack(capacity, items):
    ''' A trivial greedy algorithm for filling the knapsack
    it takes items in-order until the knapsack is full. This
    uses the value density (value / weight) as a heuristic.

        class Item(object):
            value : Int
            weight: Int

    :param capacity: The total capacity of the knapsack
    :param items: The possible items to choose from
    :returns: A tuple of (total_value, [selected items])
    '''
    value    = 0
    weight   = 0
    selected = []
    by_value = sorted(items, key=lambda i: float(i.value) / i.weight, reverse=True)

    for item in by_value:
        if (weight + item.weight) <= capacity:
            selected.append(item)
            value  += item.value
            weight += item.weight
    return value, selected
