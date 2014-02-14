#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
Given a grid of dominos, move three domino positions and
attempt to find a solution such that each row and column
contain the same set of elements without repeats.

* you can move only three dominos
* the swaps do not have to be 1 to 1 
* each row and column must have the same set of elements
* there can be no repeats of elements
'''

#------------------------------------------------------------
# utilities
#------------------------------------------------------------
def print_solution(grid, solution):
    ''' Given a solution to the problem, print it in a
    formatted way.

    :param grid: The original grid of the problem
    :param solution: A solution to the original problem
    '''
    layout = lambda row: ''.join(DICE[value] for value, group in row)
    for grid_row, solved_row in zip(grid, solution):
        print "%s -> %s" % (layout(grid_row), layout(solved_row))

#------------------------------------------------------------
# solution
#------------------------------------------------------------
def is_solution(grid):
    ''' Given a possible solution, see if it solves the problem.

    >>> is_solution(GRID1)
    False

    >>> is_solution(GRID_SOLVED)
    True

    :param grid: The grid to check if is a solution
    :returns: True if a solution, False otherwise
    '''
    solution = COUNTS
    X, Y = range(0, len(grid)), range(0, len(grid[0]))
    rows_solved = all(set(value for value,_ in row) == solution for row in grid)
    cols_solved = all(set(grid[x][y][0] for x in X) == solution for y in Y)
    return rows_solved and cols_solved

def get_next_moves(grid):
    '''
    '''
    pass

def find_solutions(grid):
    ''' Given a problem grid, attempt to find all the
    possible solutions to the problem.

    :param grid: The grid to find a solution for
    :returns: A generator of solutions
    '''
    queue = [(grid, [])]
    while queue:
        curr, moves = queue.pop()
        if is_solution(curr):
            yield curr
        elif len(moves) < 3:
            for path, move in get_next_moves(curr):
                queue.insert(0, (path, moves + move))

#------------------------------------------------------------
# constants
#------------------------------------------------------------
DICE  = [unichr(9744)] + [unichr(9856 + i) for i in range(0, 6)]
GRID1 = [
    [(4,1),(2,1),(2,8),(1,7)],
    [(3,2),(1,2),(4,8),(2,7)],
    [(4,3),(3,4),(1,4),(4,6)],
    [(3,3),(1,5),(2,5),(3,6)],
]
GRID_SOLVED = [ # for testing
    [(4,1),(1,1),(2,2),(3,2)],
    [(3,3),(2,4),(1,5),(4,7)],
    [(2,3),(3,4),(4,5),(1,7)],
    [(1,8),(4,8),(3,6),(2,6)],
]
GRID2 = [
    [(1,1),(2,1),(3,2),(4,2)],
    [(3,4),(3,5),(3,3),(4,3)],
    [(2,4),(1,5),(2,7),(4,8)],
    [(3,6),(4,6),(1,7),(1,8)],
]
GRID   = GRID1
GROUPS = set(group for row in GRID1 for value, group in row)
COUNTS = set(value for row in GRID1 for value, group in row)

#------------------------------------------------------------
# main
#------------------------------------------------------------

if __name__ == '__main__':
    print_solution(GRID, GRID_SOLVED)
    for solution in find_solution(GRID):
        print_solution(GRID, solution)
