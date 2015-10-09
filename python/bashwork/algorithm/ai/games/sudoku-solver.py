#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a sudoku puzzle, solve it representing the state
as a value map. This is mostly the article by Norvig:

    http://norvig.com/sudoku.html
'''
import random

# ------------------------------------------------------------
# common utilities
# ------------------------------------------------------------

def cross(xs, ys):
    ''' Given two iterables, return the cross product
    of those two.
    '''
    return [x + y for x in xs for y in ys]

def some(iterable):
    ''' Given a collection of values, if there is a 
    value in the collection that is not False, return it.

    :param iterable: The iterable to process
    :returns: The first not False value in the collection
    '''
    for entry in iterable:
        if entry: return entry
    return False

def from_file(path, seperator='\n'):
    ''' Read a model from the supplied path.

    :param path: The path to the file to read
    :param seperator: The line seperator
    :returns: The contents of the file
    '''
    return file(path).read().strip().split(seperator)

def shuffled(iterable):
    ''' Given an iterable sequence, return a randomly shuffled
    copy of that sequence

    :param iterable: The sequence to shuffle
    :returns: The randomly shuffled input sequence
    '''
    coll = list(iterable)
    random.shuffle(coll)
    return coll

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------

DIGITS   = '123456789'
ROWS     = 'ABCDEFGHI'
COLS     = DIGITS
SIZE     = len(DIGITS)
SQUARES  = cross(ROWS, COLS)
UNITSET  = ( [cross(ROWS, col) for col in COLS]
           + [cross(row, COLS) for row in ROWS]
           + [cross(row, col)  for row in ('ABC', 'DEF', 'GHI') for col in ('123', '456', '789')])
UNITS    = { square : [unit for unit in UNITSET if square in unit] for square in SQUARES }
PEERS    = { square : set(sum(UNITS[square], [])) - set([square]) for square in SQUARES }

# ------------------------------------------------------------
# parsing utility methods
# ------------------------------------------------------------

def lex_grid(string):
    ''' Given a string representing a sudoku grid, lex it into
    tokens that can be converted into a value map.

    :param string: The sudoku grid to parse
    :returns: The lexed tokens
    '''
    chars = [char for char in string if char in DIGITS or char in '0.']
    assert len(chars) == 81
    return dict(zip(SQUARES, chars)).items()

def parse_grid(string):
    ''' Given a string representing a sudoku grid, convert it to
    a value map.

    :param string: The sudoku grid to parse
    :returns: The value map
    '''
    values = { square : DIGITS for square in SQUARES }
    for square, digit in lex_grid(string):
        if digit in DIGITS and not assign(values, square, digit):
            return False
    return values

def display_grid(values):
    ''' Given a current value map, print it as a formatted grid.

    :param values: The values to print as a grid
    '''
    width = 1 + max(len(values[square]) for square in SQUARES)
    line  = '+'.join(['-' * (width * 3)] * 3)
    lines = []

    for row in ROWS:
        lines.append(''.join(values[row + col].center(width)
                      + ('|' if col in '36' else '') for col in COLS))
        if row in 'CF': lines.append(line)
    print('\n'.join(lines) + '\n')

# ------------------------------------------------------------
# solver methods
# ------------------------------------------------------------

def assign(values, square, digit):
    ''' Given a new assignment to a square, propigate the revealed
    constraints to the remaining squares.

    :param values: The value map to update
    :param sqaure: The square to assign a value to
    :param digit: The value to assign to the sqaure
    '''
    remaining  = values[square].replace(digit, '')
    successful = all(eliminate(values, square, d) for d in remaining)
    return values if successful else False

def eliminate(values, square, digit):
    ''' Given a new constraint, propigate it across the other squares.

    :param values: The value map to update
    :param sqaure: The square to apply a constraint to
    :param digit: The value to remove from the sqaure
    '''
    if digit not in values[square]:                    # if we have already removed this digit
        return values                                  # there is no work to be done

    values[square] = values[square].replace(digit, '') # first remove that value from the square
    if len(values[square]) == 0:                       # check that the square has no solutions
        return False

    elif len(values[square]) == 1:                     # if there is only one value for this square
        value = values[square]                         # attempt to apply that constraint to its peers
        if not all(eliminate(values, peer, value) for peer in PEERS[square]):
            return False                               # fail if this produces an invalid result

    for unit in UNITS[square]:                         # check if the unit has a single result
        places = [s for s in unit if digit in values[s]]
        if len(places) == 0:                           # fail if this produces an invalid result
            return False
        
        elif len(places) == 1:                         # if there is only one value for the unit
            if not assign(values, places[0], digit):   # attempt to assign it
                return False                           # fail if this produces an invalid result
    return values                                      # otherwise return the results

def search(values):
    ''' A simple DFS solver that chooses the next path
    with the least choices.

    :param values: The current problem to solve
    :returns: The solution if there is one
    '''
    if values == False: return False                        # we have already been proven False
    if all(len(values[square]) == 1 for square in SQUARES): # we have solved the problem
        return values

    count, square = min((len(values[square]), square)
        for square in SQUARES if len(values[square]) > 1)
    return some(search(assign(values.copy(), square, digit)) for digit in values[square])

def solve(string):
    ''' Given a sudoku grid string, attempt to solve it.

    :param string: The sudoku puzzle to solve
    :returns: The solved sudoku puzzle
    '''
    return search(parse_grid(string))

def is_solved(values):
    ''' Tests whether the supplied problem is solved.

    :param values: The problem to test if it is solved
    :returns: True if solved, False otherwise
    '''
    def is_unit_solved(unit):
        return set(values[square] for square in unit) == set(DIGITS)
    return values is not False and all(is_unit_solved(unit) for unit in UNITSET)

# ------------------------------------------------------------
# puzzle generation
# ------------------------------------------------------------

def random_puzzle(N=17):
    ''' Make a random puzzle with N or more assignments. Restart on contradictions.
    Note the resulting puzzle is not guaranteed to be solvable, but empirically
    about 99.8% of them are solvable. Some have multiple solutions.
    '''
    values = { square: DIGITS for square in SQUARES }
    for square in shuffled(SQUARES):
        if not assign(values, square, random.choice(values[square])):
            break
        digits = [values[s] for s in SQUARES if len(values[s]) == 1]
        if len(digits) >= N and len(set(digits)) >= 8:
            return ''.join(values[s] if len(values[s])==1 else '.' for s in SQUARES)
    return random_puzzle(N) # give up and make a new puzzle

# ------------------------------------------------------------
# tests
# ------------------------------------------------------------

def test_constants():
    assert len(DIGITS)            == SIZE
    assert len(ROWS)              == SIZE
    assert len(COLS)              == SIZE
    assert len(SQUARES)           == 81
    assert len(UNITSET)           == 27
    assert all(len(UNITS[square]) ==  3 for square in SQUARES)
    assert all(len(PEERS[square]) == 20 for square in SQUARES)

    assert UNITS['C2'] == [
        ['A2', 'B2', 'C2', 'D2', 'E2', 'F2', 'G2', 'H2', 'I2'],
        ['C1', 'C2', 'C3', 'C4', 'C5', 'C6', 'C7', 'C8', 'C9'],
        ['A1', 'A2', 'A3', 'B1', 'B2', 'B3', 'C1', 'C2', 'C3']]

    assert PEERS['C2'] == set(
        ['A2', 'B2', 'D2', 'E2', 'F2', 'G2', 'H2', 'I2',
         'C1', 'C3', 'C4', 'C5', 'C6', 'C7', 'C8', 'C9',
         'A1', 'A3', 'B1', 'B3'])

def test_parser():
    grid1 = "4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......"
    grid2 = """
        400000805
        030000000
        000700000
        020000060
        000080400
        000010000
        000603070
        500200000
        104000000"""

    grid3 = """
        4 . . |. . . |8 . 5 
        . 3 . |. . . |. . . 
        . . . |7 . . |. . . 
        ------+------+------
        . 2 . |. . . |. 6 . 
        . . . |. 8 . |4 . . 
        . . . |. 1 . |. . . 
        ------+------+------
        . . . |6 . 3 |. 7 . 
        5 . . |2 . . |. . . 
        1 . 4 |. . . |. . ."""

    assert parse_grid(grid1) == parse_grid(grid2)
    assert parse_grid(grid2) == parse_grid(grid3)

def test_solver():
    grid1        = '003020600 900305001 001806400 008102900 700000008 006708200 002609500 800203009 005010300'
    grid1_answer = '483921657 967345821 251876493 548132976 729564138 136798245 372689514 814253769 695417382'
    assert solve(grid1) == parse_grid(grid1_answer)

# ------------------------------------------------------------
# main
# ------------------------------------------------------------

if __name__ == "__main__":
    test_constants()
    test_parser()
    test_solver()
    puzzle = random_puzzle(10)
    display_grid(parse_grid(puzzle)), display_grid(solve(puzzle))
