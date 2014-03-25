#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
The problem is to place N queens on an NxN size chessboard
so that the following constraints are met:

* no two queens can be on the same row
* no two queens can be on the same column
* no two queens can be on the same diagonal
'''

#------------------------------------------------------------
# constants
#------------------------------------------------------------

SIZE     = 8
SOLUTION = set(range(SIZE))

#------------------------------------------------------------
# methods
#------------------------------------------------------------

def get_next_moves(solution):
    ''' Given a current solution, retrieve the next
    possible moves for the board.

    :returns: A list of the next possible moves
    '''
    def is_diagonal(row, ncol, nrow):
        return (solution[row] + row == ncol + nrow
            or  solution[row] - row == ncol - nrow)

    def is_safe_move(nrow, ncol):
        return all(not is_diagonal(r, ncol, nrow) for r in range(nrow))

    row   = len(solution)
    cols  = SOLUTION.difference(solution)
    moves = [col for col in cols if is_safe_move(row, col)]
    return moves

def is_solution(solution):
    ''' Check if the supplied path is a solution to the
    puzzle.

    :param maze: The maze to find a path through
    :param path: The path through the maze
    :param finish: The finishing point of the maze
    :returns: True if this is a valid solution, False otherwise
    '''
    return set(solution) == SOLUTION

def get_solutions(size):
    ''' Get all the solutions the the N-Queen problem.

    :param size: The size the chess board to get a solution for
    :returns: A generator of possible solutions
    '''
    stack = [[x] for x in range(size)]
    while stack:
        solution = stack.pop()
        if is_solution(solution):
            yield solution
        else:
            for move in get_next_moves(solution):
                stack.append(solution + [move])

def print_solution(solution):
    ''' Given a solution, print it using the correct
    formatting.

    :param solution: The solution to print
    '''
    size = len(solution)
    char = lambda r, c: 'Q' if solution[r] == c else '.'
    rows = [''.join(char(r, c) for c in range(size)) for r in range(size)]
    print '\n'.join(rows) + '\n'

#------------------------------------------------------------
# main
#------------------------------------------------------------

if __name__ == '__main__':
    for count, solution in enumerate(get_solutions(SIZE)):
        print "[{}]".format(count)
        print_solution(solution)
