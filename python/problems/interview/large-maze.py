#!/usr/bin/env python
from Queue import Queue
from PIL import Image

#------------------------------------------------------------
# constants
#------------------------------------------------------------

class Color(object):
    ''' A collection of colors on the maze
    '''
    Red   = (255,   0,   0)
    Gray  = (127, 127, 127)
    White = (255, 255, 255)

#------------------------------------------------------------
# utilities
#------------------------------------------------------------

def maze_path_search(start, end, maze):
    ''' Given a start point, an end point, and a maze,
    perform a breadth first search to find a path through
    the maze.

    :param start: The starting point of the maze
    :param end: The ending point of the maze
    :param maze: The maze to find a path through
    :returns: A solution to the maze
    '''
    get_moves     = lambda x, y: [(x-1, y), (x, y-1), (x+1, y), (x, y+1)]
    is_solution   = lambda point: point == end
    is_valid_move = lambda x, y: maze[x, y] > Color.Gray

    queue = Queue()
    queue.put([start])
    while not queue.empty():
        current = queue.get() 
        if is_solution(current[-1]):
            return current

        for move in get_moves(*current[-1]):
            if is_valid_move(*move):
                maze[move[0], move[1]] = Color.Gray
                queue.put(current + [move])
    raise Exception("No solution to the maze!")

def save_maze_solution(name, solution):
    ''' Given a maze solution, draw the solution
    on the maze and then save the final result.

    :param name: The filename of the maze to draw on
    :param solution: The solution to the maze
    '''
    maze = Image.open(name)
    data = maze.load()
    for x, y in solution:
        data[x, y] = Color.Red
    #maze.save('solved-' + name)
    maze.show()

if __name__ == '__main__':
    start, end  = (400, 984), (398, 25)
    name     = 'large-maze.jpg'
    maze     = Image.open(name).convert("RGB")
    data     = maze.load()
    solution = maze_path_search(start, end, data)
    save_maze_solution(name, solution)
