#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
Given a maze that may have some obstructions, find a path between
the starting point and the ending point by visiting every space
along the way at least once.

* you cannot land on the same spot twice
* you cannot exceed the bounds of the grid
* you must land on the last space perfectly
* you must visit every spot
* you cannot move into blocked spots (marked with a 1)
'''

class Direction(object):
    ''' An enumeration that defines the direction
    of movement in the grid.
    '''
    Left  = 0
    Up    = 1
    Right = 2
    Down  = 3


def get_next_moves(maze, path):
    ''' Given a maze and the current path in the maze,
    generate the set of next moves by using the rules
    of the game.

    :param maze: The maze to search for next moves in
    :param path: The current path in the maze
    :returns: A list of the next possible moves
    '''
    X,Y   = len(maze) - 1, len(maze[0]) - 1
    x,y   = path[-1]
    moves = set()

    if x > 0 and not maze[x - 1][y]: moves.add((x - 1, y))
    if x < X and not maze[x + 1][y]: moves.add((x + 1, y))
    if y > 0 and not maze[x][y - 1]: moves.add((x, y - 1))
    if y < Y and not maze[x][y + 1]: moves.add((x, y + 1))
    return moves.difference(path) # cannot visit same spot twice

def is_solution(maze, path, finish):
    ''' Check if the supplied path is a solution to the
    puzzle.

    :param maze: The maze to find a path through
    :param path: The path through the maze
    :param finish: The finishing point of the maze
    :returns: True if this is a valid solution, False otherwise
    '''
    # are we at the end point
    if path[-1] != finish: return False

    # have we made enough steps
    moves = set(path)
    if len(moves) != STEPS:
        return False

    # did we step on any bad spots
    for x,y in moves:
        if maze[x][y]: return False
    return True

def maze_search(maze, start=(0, 0), finish=(-1,-1)):
    ''' Given a maze, attempt to find a path from the starting
    position to the ending position.

    :param maze: The maze to find a path in
    :param start: The starting position of the maze
    :param finish: The ending position of the maze
    :returns: A generator of possible solutions
    '''
    stack = [[start]]
    while stack:
        path = stack.pop()
        if is_solution(maze, path, finish):
            yield path
        else:
            for move in get_next_moves(maze, path):
                stack.append(path + [move])

def print_solution(maze, solution):
    ''' Given a solution, print it using the correct
    formatting.

    :param maze: The maze to get the points from
    :param solution: The solution to print
    '''
    px, py = solution[0]
    moves = []
    for x, y in solution[1:]:
        if x != px and x > px: moves.append(Direction.Down)
        if x != px and x < px: moves.append(Direction.Up)
        if y != py and y > py: moves.append(Direction.Right)
        if y != py and y < py: moves.append(Direction.Left)
        px, py = x, y
    print ' '.join(MOVES[move] for move in moves)


#------------------------------------------------------------
# constants
#------------------------------------------------------------

MOVES = ['←', '↑', '→', '↓']
MAZE  = [
    [0,0,0,0,0,0],
    [0,1,0,0,0,0],
    [0,0,0,0,0,0],
    [0,0,1,1,0,0],
    [0,0,0,0,0,0],
    [1,0,0,0,0,0],
]
STEPS = sum(1 for row in MAZE for val in row if not val)
X, Y  = len(MAZE) - 1, len(MAZE[0]) - 1

if __name__ == '__main__':
    for solution in maze_search(MAZE, start=(1, 4), finish=(0, 4)):
        print_solution(MAZE, solution)
