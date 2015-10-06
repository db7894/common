#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a sudoku puzzle, solve it representing the state
as a 9x9 matrix.
'''

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------

SIZE     = 9
SOLUTION = set(range(1, SIZE + 1))
COLUMNS  = range(SIZE)
SUDOKU   = [
    [4, 1, 7, 3, 6, 9, 8, 2, 5],
    [6, 3, 2, 1, 5, 8, 9, 4, 7],
    [9, 5, 8, 7, 2, 4, 3, 1, 6],
    [8, 2, 5, 4, 3, 7, 1, 6, 9],
    [7, 9, 1, 5, 8, 6, 4, 3, 2],
    [3, 4, 6, 9, 1, 2, 7, 5, 8],
    [2, 8, 9, 6, 4, 3, 5, 7, 1],
    [5, 7, 3, 2, 9, 1, 6, 8, 4],
    [1, 6, 4, 8, 7, 5, 2, 9, 3]
]

PARTIAL_SUDOKU   = [
    [4, 1, 7, 3, 6, 9, 8, 2, 5],
    [6, 0, 2, 1, 5, 8, 9, 4, 7],
    [9, 5, 8, 7, 0, 0, 3, 1, 6],
    [8, 2, 5, 4, 3, 7, 1, 6, 9],
    [7, 9, 1, 0, 8, 6, 0, 3, 2],
    [3, 4, 6, 9, 1, 2, 7, 5, 8],
    [2, 8, 9, 6, 4, 3, 5, 7, 1],
    [5, 0, 3, 2, 9, 0, 6, 8, 4],
    [1, 6, 4, 8, 7, 5, 2, 9, 3]
]

# ------------------------------------------------------------
# solver methods
# ------------------------------------------------------------
def is_set_valid(values):
    return set(values) == SOLUTION

def is_partial_set_valid(values):
    exists = { s: False for s in SOLUTION }
    for value in values: 
        if exists.get(value, False):
            return False
        if value != 0: exists[value] = True
    return True

def get_all_rows(sudoku): return (row for row in sudoku)
def get_all_cols(sudoku): return ([row[col] for row in sudoku] for col in COLUMNS)
def get_all_boxes(sudoku): return ([sudoku[row][col]
        for row in range(rows, rows + 3) for col in range(cols, cols + 3)]
        for rows in [0, 3, 6] for cols in [0, 3, 6])

def is_solution(sudoku):
    ''' Given a current sudoku solution, return if the puzzle
    represents a correct solution.

    :param sudoku: The puzzle to validate if valid
    :returns: True if a solution, False otherwise
    '''
    are_rows_valid  = all(map(is_set_valid, get_all_rows(sudoku)))
    are_cols_valid  = all(map(is_set_valid, get_all_cols(sudoku)))
    are_boxes_valid = all(map(is_set_valid, get_all_boxes(sudoku)))
    return are_rows_valid and are_cols_valid and are_boxes_valid

def is_partial_solution(sudoku):
    ''' Given a current sudoku solution, return if the puzzle
    represents a current partial solution (does not invalidate
    any of the rules).

    :param sudoku: The puzzle to validate if valid
    :returns: True if a partial solution, False otherwise
    '''
    are_rows_valid  = all(map(is_partial_set_valid, get_all_rows(sudoku)))
    are_cols_valid  = all(map(is_partial_set_valid, get_all_cols(sudoku)))
    are_boxes_valid = all(map(is_partial_set_valid, get_all_boxes(sudoku)))
    return are_rows_valid and are_cols_valid and are_boxes_valid

def print_solution(solution):
    ''' Given a solution to the problem, print out the
    final result.

    :param solution: The final solution for the problem
    '''
    for row in solution:
        print row

# ------------------------------------------------------------
# main
# ------------------------------------------------------------

if __name__ == "__main__":
    print is_solution(SUDOKU)
    print is_partial_solution(PARTIAL_SUDOKU)
    print_solution(SUDOKU)
