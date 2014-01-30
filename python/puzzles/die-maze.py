#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
Given a maze of dice where you are started at a given position
(say upper left), attempt to find a path to the ending position
(say bottom right). The rules are:

* on your first move, you can move vertical or horizontal
* on every following move you must alter the direction
* on each move, you must move the number of spaces as indicated
  by the previous die
* you cannot land on the same spot twice
* you cannot exceed the bounds of the grid
* you must land on the last space perfectly
'''

class Axis(object):
    ''' An enumeration that defines the direction
    of movement in the grid.
    '''
    Vertical   = 0x01
    Horizontal = 0x10
    Both       = 0x11


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
    steps = maze[x][y]
    moves = set()

    # unless this is the first move, we have to alternate
    # direction of the moves between horizontal and vertical
    if len(path) > 1:
        (a, b), (c, d) = path[-2:]
        axis = Axis.Horizontal if a == c else Axis.Vertical
    else: axis = Axis.Both

    if Axis.Horizontal & axis:
        if x - steps >= 0: moves.add((x - steps, y))
        if x + steps <= X: moves.add((x + steps, y))
    if Axis.Vertical & axis:
        if y - steps >= 0: moves.add((x, y - steps))
        if y + steps <= Y: moves.add((x, y + steps))
    return moves.difference(path) # cannot visit same spot twice


def maze_search(maze, start=(0, 0), finish=(-1,-1)):
    ''' Given a maze, attempt to find a path from the starting
    position to the ending position.

    :param maze: The maze to find a path in
    :param start: The starting position of the maze
    :param finish: The ending position of the maze
    :returns: A generator of possible solutions
    '''
    queue = [[start]]
    while queue:
        path = queue.pop()
        if path[-1] == finish:
            yield path
        else:
            for move in get_next_moves(maze, path):
                queue.insert(0, path + [move])


def print_solution(maze, solution):
    ''' Given a solution, print it using the correct
    formatting.

    :param maze: The maze to get the points from
    :param solution: The solution to print
    '''
    dice = [str(maze[x][y]) for x, y in solution]
    #dice = [DICE[maze[x][y]] for x, y in solution]
    print ' → '.join(dice)


#------------------------------------------------------------
# constants
#------------------------------------------------------------
DICE = [unichr(9744)] + [unichr(9856 + i) for i in range(0, 6)]
MAZE = [
    [1,3,2,4,1,5,2,6],
    [6,1,2,5,5,6,3,2],
    [4,3,1,4,6,4,3,5],
    [2,4,6,5,2,1,6,3],
    [6,4,3,4,3,5,4,6],
    [3,1,6,1,4,3,5,2],
    [2,2,5,3,4,6,1,3],
    [5,6,1,5,3,1,4,6],
]
X, Y = len(MAZE) - 1, len(MAZE[0]) - 1

if __name__ == '__main__':
    for solution in maze_search(MAZE, finish=(X, Y)):
        print_solution(MAZE, solution)
