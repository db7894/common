#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
Given a matrix of elements, partition the matrix
into N groups of equal sized sets of elements with
no repeats. Rules:

* elements in a partition must be horizontal or vertical
  neighbors (no diagonal).
* each group must have one of each element
* there must not be any remaining elements not in a set

:keywords: DP, knapsack, partition-problem, subset-sum

* http://en.wikipedia.org/wiki/Partition_problem
* http://en.wikipedia.org/wiki/Bin_packing_problem
* http://en.wikipedia.org/wiki/Guillotine_problem
* http://en.wikipedia.org/wiki/Packing_problem
* http://en.wikipedia.org/wiki/Clique_(graph_theory)
'''
from itertools import combinations

#------------------------------------------------------------
# Constants
#------------------------------------------------------------
IMAGES = { 1: '☙', 2: '☫', 3: '☸', 4: '♼', 5: '❄' }
MATRIX = [
    [ 1, 2, 2, 3, 1 ],
    [ 3, 4, 5, 2, 4 ],
    [ 5, 4, 1, 5, 1 ],
    [ 2, 2, 4, 3, 3 ],
    [ 3, 4, 1, 3, 5 ],
    [ 5, 2, 4, 1, 5 ],
]
VALUES  = set(x for xs in MATRIX for x in xs)
TOTALS  = sum(x for xs in MATRIX for x in xs)
COUNTS  = len(VALUES)
LENGTH  = len(MATRIX)
X, Y    = len(MATRIX) - 1, len(MATRIX[0]) - 1
ENTRIES = COUNTS * LENGTH

assert COUNTS == 5
assert TOTALS == sum(range(min(VALUES), max(VALUES) + 1)) * LENGTH

#------------------------------------------------------------
# Helper Methods
#------------------------------------------------------------

def get_all_starts(matrix):
    ''' Given a matrix, get all the possible starting
    points of the same minimum element.

    >>> get_all_starts(MATRIX)
    [(1, 0, 0), (1, 0, 4), (1, 2, 2), (1, 2, 4), (1, 4, 2), (1, 5, 3)]

    :param matrix: The matrix to get the elements from
    :returns: A list of positions of the starting points
    '''
    start  = min(VALUES)
    starts = [(val,x,y) for x, row in enumerate(matrix)
                        for y, val in enumerate(row) if val == start]
    return starts

def is_solution(groups):
    ''' Check if the supplied collection of paths
    is a valid solution. Groups is a collection of group
    elements::

        group  = [(v,x,y)] * 5
        groups = [group]   * 6

    >>> groups = [[(i, j, i) for i in range(1, 6)] for j in range(1, 7)]
    >>> is_solution(groups)
    True

    >>> groups = groups[0] + groups[:-1]
    >>> is_solution(groups)
    False

    :param groups: The collection of groups as a solution
    :returns: True if a valid solution, False otherwise
    '''
    lengths = [len(group) == COUNTS for group in groups]
    entries = set(entry for group in groups for entry in group)

    if not all(lengths): return False
    if len(entries) != ENTRIES : return False

    solution = set(v for v,x,y in iter(groups).next())
    for group in groups:
        if set(v for v,x,y in group) != solution:
            return False
    return True

def print_solution(matrix, solution):
    ''' Given a solution, print it using the correct
    formatting.

    :param matrix: The matrix to get the points from
    :param solution: The solution to print
    '''
    for count, group in enumerate(solution):
        result = [IMAGES[matrix[x][y]] for x,y in group]
        print "%d: %s" % (count, ' '.join(result))

#------------------------------------------------------------
# All Permutations Solution
#------------------------------------------------------------

def get_next_step(matrix, entry):
    ''' Given a matrix and a single current entry,
    return all the possible elements that the entry
    can extend to.

    >>> entry = set([(4, 1, 1)])
    >>> get_next_step(MATRIX, entry)
    set([(3, 1, 0), (2, 0, 1), (5, 1, 2)])

    :param matrix: The matrix to look up elements in
    :param entry: The entry to extend elements to
    :returns: All the possible elements that can be extended
    '''
    poss   = set()
    values = set(v for v,x,y in entry)
    X,Y    = len(matrix) - 1, len(matrix[0]) - 1
    for v,x,y in entry:
        if x > 0 and matrix[x-1][y] not in values: poss.add((matrix[x-1][y], x-1, y))
        if y > 0 and matrix[x][y-1] not in values: poss.add((matrix[x][y-1], x, y-1))
        if x < X and matrix[x+1][y] not in values: poss.add((matrix[x+1][y], x+1, y))
        if y < Y and matrix[x][y+1] not in values: poss.add((matrix[x][y+1], x, y+1))
    return poss - entry

def get_next_steps(matrix, group):
    ''' Given a matrix and a group of entries,
    return all the possible elements that the entry
    can extend to.

    >>> entry = set([(2, 0, 1), (1, 0, 0), (3, 1, 0)])
    >>> get_next_step(MATRIX, entry)
    set([(4, 1, 1), (5, 2, 0)])

    :param matrix: The matrix to look up elements in
    :param group: The collection of entries to extend elements to
    :returns: All the possible elements that can be extended
    '''
    return set(step for entry in group
                    for step  in get_next_step(matrix, entry))

def get_all_groups(matrix):
    ''' Given a matrix, return all possible
    collection of groups of the same number of elements.

    :param matrix: The matrix to get the elements from
    :returns: A generator of the possible groups
    '''
    queue = [{p} for p in get_all_starts(matrix)]
    while queue:
        path = queue.pop()
        if len(path) == COUNTS:
            yield path
        else:
            for step in get_next_step(matrix, path):
                queue.insert(0, path.union({step}))

def intersecting_entry(group, entry):
    ''' A quick check to see if the next entry intersects
    with the current group.

    >>> group = [(i, j, i) for i in range(1, 3) for j in range(1, 2)]
    >>> intersecting_entry(group, [(4, 4, 4)])
    False

    >>> intersecting_entry(group, [(1, 1, 1)])
    True

    :param group: The current group in the solution
    :param entry: The next possible entry
    :returns: True if the solution intersects, False otherwise
    '''
    group = set(entry for entry in group)
    entry = set(entry)
    return bool(group.intersection(entry))

def solve_brute_force(matrix):
    ''' Given a matrix, solve the puzzle by simply generating
    all possible sub-group solutions and then finding a combination
    that works.

    :param matrix: The matrix to get the elements from
    :returns: A generator of the possible solutions
    '''
    def get_solution(path, search, results):
        if is_solution(path):
            results.append(path)

        if len(path) < number:
            for i in range(0, len(search) - 1):
                if not intersecting_entry(path, search[i]):
                    get_solution(path + [search[i]], search[i:], results)

    number = len(matrix)
    groups = set(tuple(sorted(g)) for g in get_all_groups(matrix))
    groups = list(groups)
    result = []
    for i in range(0, len(groups) - 1):
        get_solution([groups[i]], groups[i:], result)
    return results

#------------------------------------------------------------
# Map Coloring Solution
#------------------------------------------------------------

def expand_groups(matrix, groups):
    '''
    :param matrix: The matrix to get the solutions from
    :param groups: The current groups to expand
    :returns: A generator of the next possible groups
    '''
    moves = [get_next_steps(matrix, group) for group in groups]
    order = sorted((len(move), c) for c, move in enumerate(moves))
    print order


    # for move in sorted(moves, by=len(move), asc)
    #   clone.update(move)
    #   moves.remove(move)
    #   if len(moves) == 0: backtrack

def solve_groups(matrix):
    ''' Given a matrix, return the partition solutions
    that are all equal.

    Each entry in the stack is a list of sets of groups::

        group  = [{1,2,3}, {3,2,1}, {2,1,3}]
        groups = [group, group, group]

    :param matrix: The matrix to get the solutions from
    :returns: A generator of the possible solutions
    '''
    stack = [{p} for p in get_all_starts(matrix)]
    while stack:
        groups = stack.pop()
        if is_solution(groups):
            yield groups
        else:
            for entry in expand_groups(matrix, groups):
                stack.append(entry)

#------------------------------------------------------------
# Main
#------------------------------------------------------------

if __name__ == '__main__':
    #import doctest
    #doctest.testmod()

    for solution in solve_groups(MATRIX):
        print_solution(MATRIX, solution)
