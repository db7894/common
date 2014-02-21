#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
Given a matrix of arrows, try to find a 2x2 box containing
one of up, down, left, and right in any order.
'''

class Direction(object):
    ''' An enumeration that defines the direction
    of the arrows in the Matrix
    '''
    Empty    = 0x0000
    Left     = 0x0001
    Up       = 0x0010
    Right    = 0x0100
    Down     = 0x1000
    Combined = 0x1111

def get_all_moves(matrix):
    ''' Given a matrix, generate a stream of the possible
    2x2 boxes.

    :param maze: The matrix to search for boxes in
    :returns: A generator of the next possible moves
    '''
    X, Y = len(matrix) - 2, len(matrix[0]) - 2
    for r in range(0, X):
        for c in range(0,Y):
            yield [(r, c), (r + 1, c), (r, c + 1), (r + 1, c + 1)]

def is_solution(matrix, entry):
    ''' Check if the supplied box is a solution to the
    puzzle.

    :param matrix: The matrix to find a solution in
    :param entry: The entry to test as a solution
    :returns: True if this is a valid solution, False otherwise
    '''
    result = Direction.Empty
    for x, y in entry:
        result |= matrix[x][y]
    return result == Direction.Combined

def matrix_search(matrix):
    ''' Given a matrix, try to find all the solutions to
    the supplied problem.

    :param matrix: The matrix to search for solutions in
    :returns: A generator of possible solutions
    '''
    for move in get_all_moves(matrix):
        if is_solution(matrix, move):
            yield move

def print_solution(matrix, solution):
    ''' Given a solution, print it using the correct
    formatting.

    :param matrix: The matrix to get the points from
    :param solution: The solution to print
    '''
    print ' '.join("%s %s" % (MOVES[matrix[x][y]], (x, y)) for x,y in solution)

def print_matrix(matrix):
    ''' Given an encoded matrix, print out its textual
    version.

    :param matrix: The matrix to print to screen
    '''
    for row in matrix:
        print ''.join(MOVES[move] for move in row)

def build_matrix():
    ''' Given a block of text, generate a matrix out
    of the suppiled values.
    '''
    lines = '''
        lurdlurrd
        dlrrulldu
        uldurrudr
        lrldludrl
        drudrldur
        uudrldrlu
        rllrulddd
        drdludrlr
        rulrrdurd
    '''.split()

    moves   = {
        'u': Direction.Up,
        'd': Direction.Down,
        'l': Direction.Left,
        'r': Direction.Right
    }
    return [[moves[move] for move in line] for line in lines]

#------------------------------------------------------------
# constants
#------------------------------------------------------------

MOVES  = {
    Direction.Left  : '←',
    Direction.Up    : '↑',
    Direction.Right : '→',
    Direction.Down  : '↓',
}
MATRIX = build_matrix()
X, Y   = len(MATRIX) - 1, len(MATRIX[0]) - 1

if __name__ == '__main__':
    for solution in matrix_search(MATRIX):
        print_solution(MATRIX, solution)
