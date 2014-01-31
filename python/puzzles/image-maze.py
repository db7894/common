#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
Given a maze of images where you are started at a given position
(say upper left), attempt to find a path to the ending position
(say bottom right). The rules are:

* you can only move horizontally or vertically
* you can only move to a next square that shares the same color
  or the same image
* you cannot land on the same spot twice
* you cannot exceed the bounds of the grid
* you must land on the last space perfectly

.. note:: The next thing to do is the ocr(puzzle) -> encoding step
   so I don't have to do it manually. This can help:

   - http://stackoverflow.com/questions/9413216/simple-digit-recognition-ocr-in-opencv-python
'''

#------------------------------------------------------------
# Solution
#------------------------------------------------------------

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
    image = maze[x][y]
    moves = set()

    if x > 0 and maze[x - 1][y] & image: moves.add((x - 1, y))
    if x < X and maze[x + 1][y] & image: moves.add((x + 1, y))
    if y > 0 and maze[x][y - 1] & image: moves.add((x, y - 1))
    if y < Y and maze[x][y + 1] & image: moves.add((x, y + 1))
    return moves.difference(path) # cannot visit same spot twice

def maze_search(maze, start=(-1, -1), finish=(0, 0)):
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
        if path[-1] == finish:
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
    colors = [COLORS[maze[x][y] & 0xF0] for x, y in solution]
    images = [IMAGES[maze[x][y] & 0x0F] for x, y in solution]
    string = '\033[1;%sm%s\033[1;m' 
    print ' → '.join(string % (c, i) for c, i in zip(colors, images))

#------------------------------------------------------------
# Puzzle OCR
#------------------------------------------------------------

def image_to_maze(path):
    '''
    '''

#------------------------------------------------------------
# Constants
#------------------------------------------------------------

COLORS = { 0x10: '34', 0x20: '31', 0x40: '35' }
IMAGES = { 0x01:  '✹', 0x02:  '☾', 0x04:  '★' }
MAZE   = [
    [ 0x11, 0x21, 0x22, 0x21, 0x11, 0x44, 0x22, 0x42, 0x12 ],
    [ 0x12, 0x24, 0x42, 0x12, 0x21, 0x41, 0x24, 0x11, 0x41 ],
    [ 0x42, 0x14, 0x11, 0x41, 0x42, 0x24, 0x22, 0x12, 0x44 ],
    [ 0x44, 0x41, 0x12, 0x44, 0x22, 0x44, 0x11, 0x42, 0x22 ],
    [ 0x12, 0x22, 0x21, 0x24, 0x11, 0x14, 0x12, 0x44, 0x12 ],
    [ 0x14, 0x41, 0x44, 0x14, 0x21, 0x22, 0x44, 0x41, 0x44 ],
    [ 0x24, 0x22, 0x12, 0x42, 0x41, 0x11, 0x24, 0x12, 0x14 ],
    [ 0x21, 0x42, 0x41, 0x11, 0x44, 0x22, 0x21, 0x44, 0x41 ],
    [ 0x24, 0x14, 0x12, 0x21, 0x14, 0x42, 0x44, 0x14, 0x21 ],
]
X, Y = len(MAZE) - 1, len(MAZE[0]) - 1

#------------------------------------------------------------
# Main
#------------------------------------------------------------

if __name__ == '__main__':
    for solution in maze_search(MAZE, start=(X, Y)):
        print_solution(MAZE, solution)
