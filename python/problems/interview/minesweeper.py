#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a sentence, remove all the punctuation and spaces,
break the sentence into equal groups of letters (say 5),
wrap these around randomly (like a wheel), then randomly
scramble the resulting wheel groups.

Given this scrambled collection of wheels, restore the original
sentence.
'''
from clone import deepclone

def is_valid_solution(matrix, solution):
    ''' Given a possible solution to the problem, check
    if it indeed a solution.

    :param matrix: The original matrix to test against
    :param solution: The final solution for the problem
    :returns: True if this is a solution, False otherwise
    '''
    pass

def print_solution(matrix, solution):
    ''' Given a solution to the problem, print out the
    final result.

    :param matrix: The original matrix to test against
    :param solution: The final solution for the problem
    '''
    matrix = deepclone(matrix)
    for x, y in solution:
        matrix[x][y] = '★'
    clean = lambda line: ''.join(map(str, line)).replace('0','_')
    print '\n'.join(clean(line) for line in matrix)

def find_solutions(wheels):
    pass

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------
MATRIX = [
    [0,2,0,0,0,0,0],
    [0,0,0,1,0,0,1],
    [0,1,0,0,0,0,0],
    [3,0,0,0,3,2,0],
    [0,0,2,0,0,0,1],
    [0,2,0,0,1,0,0],
    [0,0,0,0,0,0,0],
]

if __name__ == "__main__":
    matrix = MATRIX
    for solution in find_solutions(matrix):
        print_solution(matrix, solution)
