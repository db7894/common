#!/usr/bin/env python
from __future__ import division
import sys
import os
import cv2
import numpy as np
from collections import defaultdict
from scipy.spatial.distance import euclidean as distance

#--------------------------------------------------------------------------------
# computer vision utilities
#--------------------------------------------------------------------------------

def get_color_mask(image, bounds):
    ''' Given an image and a collection of color bounds, return
    a mask for that given color that is cleaned. Note, bounds can
    be a single or multiple range::

        bounds = (0, 127)
        bounds = [(0,20), (235, 255)]

    :param image: The image to get a color mask of
    :param bounds: The list of color regions or a single region
    :returns: The cleaned mask
    '''
    kernel = np.ones((7, 7), np.uint8)
    masked = np.zeros(image.shape[:2], np.uint8)
    bounds = [bounds] if isinstance(bounds, tuple) else bounds
    for lower, upper in bounds:
        region = cv2.inRange(image, (lower,  0, 50), (upper, 250, 255))
        masked = cv2.bitwise_or(region, masked)
    masked = cv2.morphologyEx(masked, cv2.MORPH_OPEN, kernel)
    masked = cv2.morphologyEx(masked, cv2.MORPH_CLOSE, kernel)
    return masked

def get_shape_features_morph(region):
    ''' Given a region of an image, convert it to its primary features
    after cleaning it up a bit.

    :param region: The region to get the moments for
    :returns: The features of the given region
    '''
    kernel  = np.ones((3, 3), np.uint8)
    region  = cv2.threshold(region, 175, 255, cv2.THRESH_BINARY_INV)[1]
    region  = cv2.morphologyEx(region, cv2.MORPH_OPEN, kernel)
    region  = cv2.morphologyEx(region, cv2.MORPH_CLOSE, kernel)
    moments = cv2.moments(region)
    cv2.imshow('outline %d' % np.random.randint(1000), region)
    return cv2.HuMoments(moments)

def get_shape_features(region):
    ''' Given a region of an image, convert it to its primary features
    after cleaning it up a bit.

    :param region: The region to get the moments for
    :returns: The features of the given region
    '''
    outline = np.zeros(region.shape, np.uint8)
    region  = cv2.threshold(region, 175, 255, cv2.THRESH_BINARY_INV)[1]
    contours, _ = cv2.findContours(region, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    contours = sorted(contours, key=cv2.contourArea, reverse=True)[0]
    cv2.drawContours(outline, [contours], -1, 255, -1)
    cv2.imshow('outline %d' % np.random.randint(1000), outline)
    moments = cv2.moments(outline)
    return cv2.HuMoments(moments)

#--------------------------------------------------------------------------------
# image dataset generators
#--------------------------------------------------------------------------------

def each_rect_point_set_of(image):
    ''' Produce a generator of each rectangle in the
    given image grid.

    :param image: The image to generate the rectangles of
    :returns: A generator of (y, x), (uy, ux), (ly, lx)
    '''
    Y, X = image.shape[:2]
    ys   = int(Y  / 9)
    xs   = int(X  / 9)

    for ix, x in enumerate(range(5, X - int(xs / 2), xs)):
        for iy, y in enumerate(range(5, Y - int(ys / 2), ys)):
            yield (iy, ix), ((y, x), (y + ys, x + xs))

def each_rect_region_of(image):
    ''' Produce a generator of each rectangle image region
    in the given image grid.

    :param image: The image to generate the rectangles of
    :returns: A generator of (y, x), region
    '''
    for index, ((y1, x1), (y2, x2)) in each_rect_point_set_of(image):
        yield index, image[y1:y2, x1:x2]

def each_grid_center_point_of(image):
    ''' Produce a generator of each center of rectangle
    in the given image grid.

    :param image: The image to generate the rectangle centers
    :returns: A generator of (y, x), (cy, cx)
    '''
    for index, ((y1, x1), (y2, x2)) in each_rect_point_set_of(image):
        yield index, (int((y2 + y1) / 2), int((x2 + x1) / 2))

def get_color_lookup(colors):
    ''' Given a collection of color masks for an image
    maze, convert them into a grid of matching colors.

    :param colors: The color mask database to build with
    :returns: The color lookup grid
    '''
    lookup = defaultdict(str)
    for name, color in colors.items():
        for index, region in each_rect_region_of(color):
            if cv2.countNonZero(region) > (region.size * 0.10):
                lookup[index] = name
    return lookup

def get_shape_lookup(image, shapes):
    ''' Given an image and a collection of shape moments,
    convert the image maze into a grid of matching shapes.

    :param image: The image maze to convert to a grid
    :param shapes: The shape moments database to compare with
    :returns: The shape lookup grid
    '''
    lookup = defaultdict(str)
    for index, region in each_rect_region_of(image):
        moments = get_shape_features(region)
        matched = min((distance(moments, fs), name) for name, fs in shapes.items())
        lookup[index] = matched[1]
    return lookup

def get_start_and_finish(image, bounds):
    ''' Given an image maze and a background color bound, find
    the start and end points of the maze.

    :param image: The image to get the start and end points for
    :param bounds: The bounds of the color image
    :returns: The two start and end points
    '''
    points = []
    lower, upper = bounds
    for index, region in each_rect_region_of(image):
        region = cv2.inRange(region, (lower, 50, 200), (upper, 250, 255))
        if cv2.countNonZero(region) > (region.size * 0.30):
            points.append(index)
    return points[:2]

#--------------------------------------------------------------------------------
# image graphical debugging
#--------------------------------------------------------------------------------

def draw_grid_center_points(image):
    ''' Given an image, draw the grid center points
    and display the resulting image.

    :param image: The image to get the center points of
    '''
    clone = image.copy()
    for _, (y, x) in each_grid_center_point_of(image):
        cv2.circle(clone, (x, y), 10, (0, 255, 0), 5)
    cv2.imshow('maze center points', clone)

def draw_grid_rectangles(image):
    ''' Given an image, draw the rectangles over the
    given grid and display the resulting image.

    :param image: The image to get the rectangle points of
    '''
    clone = image.copy()
    for _, (ul, lr) in each_rect_point_set_of(image):
        cv2.rectangle(clone, ul, lr, (0, 255, 0), 5)
    cv2.imshow('maze rectangle points', clone)

def print_color_lookup(colors, size=9):
    ''' Given a collection of color masks for an image
    maze, print the grid of colors that was decoded.

    :param colors: The color mask database to build with
    '''
    lookup = [['x'] * size for _ in range(size)]
    for name, color in colors.items():
        for (y, x), region in each_rect_region_of(color):
            if cv2.countNonZero(region) > (region.size * 0.10):
                lookup[y][x] = name

    for line in lookup:
        print line
    print ""

def print_shape_lookup(image, shapes, size=9):
    ''' Given an image and a collection of shape moments,
    print the grid of shapes that was decoded.

    :param image: The image maze to convert to a grid
    :param shapes: The shape moments database to compare with
    '''
    lookup = [['x'] * size for _ in range(size)]
    for (y, x), region in each_rect_region_of(image):
        moments = get_shape_features(region)
        matched = min((distance(moments, ms), name) for name, ms in shapes.items())
        lookup[y][x] = matched[1]

    for line in lookup:
        print line
    print ""

def assert_grid_to_lookup_grid(grid):
    ''' Given a grid of nested lists, convert it to
    a grid of point to value dictionary.

    :param grid: The grid to convert
    :returns: The converted grid
    '''
    return { (y, x) : value
       for y, values in enumerate(grid)
       for x, value  in enumerate(values) }

def lookup_grid_to_assert_grid(grid):
    ''' Given a grid of point to value dictionary,
    convert it to a grid of nested lists.

    :param grid: The grid to convert
    :returns: The converted grid
    '''
    size = int(len(grid) ^ 0.5)
    lookup = [['x'] * size for _ in range(size)]
    for (y, x), v in grid.items():
        lookup[y][x] = v
    return lookup

#--------------------------------------------------------------------------------
# depth first search of the maze
#--------------------------------------------------------------------------------

def is_solution(path, finish):
    ''' Checks if the path is a solution to the maze.

    :param path: The path through the maze
    :param finish: The finishing point of the maze
    :returns: True if this is a valid solution, False otherwise
    '''
    # validate solution
    return path[-1] == finish

def get_next_moves(grids, path):
    ''' Given the current path in the maze and the possible
    next move feature grids, generate the set of next moves
    based on the rules of the game.

    :param grids: The feature grids of possible moves
    :param path: The current path in the maze
    :returns: A list of the next possible moves
    '''
    point = tuple(path[-1][:2])
    y, x  = point
    Y, X  = 8, 8
    moves = set()

    for name, grid in grids.items():
        prev = grid[point] # what is our current state in this grid
        if x < X and grid[(y, x + 1)] == prev: moves.add((y, x + 1))
        if y < Y and grid[(y + 1, x)] == prev: moves.add((y + 1, x))
        if y > 0 and grid[(y - 1, x)] == prev: moves.add((y - 1, x))
        if x > 0 and grid[(y, x - 1)] == prev: moves.add((y, x - 1))
    return moves.difference(path) # do not visit same space twice

def maze_search(grids, start, finish):
    ''' Given a maze, attempt to find a path from the starting
    position to the ending position.

    :param grids: The grids to find moves in
    :param start: The starting position of the maze
    :param finish: The ending position of the maze
    :returns: A generator of possible solutions
    '''
    global image
    stack = [[start]]
    while stack:
        path = stack.pop()
        if is_solution(path, finish):
            yield path
        else:
            for move in get_next_moves(grids, path):
                stack.append(path + [move])

def print_solution(image, solution):
    ''' Given a solution, show the result on the orignal image.

    :param image: The image to overlay the result on
    :param solution: The solution to print
    '''
    clone  = image.copy()
    py, px = solution[0]
    sy, sx = int(image.shape[0] / 9), int(image.shape[1] / 9)
    shift  = lambda x, y: (x * sx + int(sx / 2), y * sy + int(sy / 2))
    for cy, cx in solution[1:]:
        cv2.line(clone, shift(px, py), shift(cx, cy), (0, 255, 0), 10)
        py, px = cy, cx
    cv2.imshow(str(solution), clone)

#--------------------------------------------------------------------------------
# constants
#--------------------------------------------------------------------------------
# TODO get the colors dynamically with a histogram
# TODO get the maze edges and affine to correct warp
# TODO prime the rectangle generator automatically
# TODO get the matching images by clustering
#--------------------------------------------------------------------------------

filename      = sys.argv[1] if len(sys.argv) > 1 else 'images/image-maze.jpg'
image         = cv2.imread(os.path.abspath(filename))
image_hsv     = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)
image_blk     = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
#draw_grid_rectangles(image)

blue_shapes   = get_color_mask(image_hsv, ( 75, 135))
purple_shapes = get_color_mask(image_hsv, (140, 171))
red_shapes    = get_color_mask(image_hsv, [(0, 10), (175, 200)])
colors        = { 'b': blue_shapes, 'p': purple_shapes, 'r': red_shapes }
color_grid    = get_color_lookup(colors)
#print_color_lookup(colors)

star_shape    = image_blk[120:220, 120:220]
moon_shape    = image_blk[225:325, 232:332]
planet_shape  = image_blk[120:220, 227:327]
shapes        = { 's': star_shape, 'm': moon_shape, 'p': planet_shape }
shape_moments = { name: get_shape_features(shape) for name, shape in shapes.items() }
shape_grid    = get_shape_lookup(image_blk, shape_moments)
#print_shape_lookup(image_blk, shape_moments)

grids         = { 'color': color_grid, 'shape': shape_grid }
finish, start = get_start_and_finish(image_hsv, (0, 50))

#------------------------------------------------------------
# correct values
#------------------------------------------------------------

correct_start  = (8, 8)
correct_finish = (0, 0)

correct_shape_grid = [
    ['s', 'm', 'm', 'm', 'p', 's', 's', 'p', 'm'],
    ['s', 's', 'p', 's', 's', 'p', 'p', 'm', 'p'],
    ['p', 'm', 'm', 'p', 'm', 's', 's', 'm', 's'],
    ['p', 'p', 's', 's', 'm', 'm', 's', 'm', 'p'],
    ['s', 'p', 'm', 'p', 's', 's', 'm', 'm', 'p'],
    ['m', 'm', 's', 'p', 's', 'm', 'p', 's', 's'],
    ['s', 's', 'p', 'p', 'm', 'm', 'p', 'm', 's'],
    ['p', 's', 'm', 's', 'p', 's', 'm', 'p', 'p'],
    ['p', 'm', 's', 's', 'm', 's', 'p', 'm', 'm'],
]

['s', 'm', 'm', 'm', 'p', 's', 's', 'p', 'm']
['s', 's', 'p', 's', 's', 'p', 'p', 'm', 'p']
['p', 'm', 'm', 'p', 'm', 's', 's', 'm', 's']
['p', 'p', 's', 's', 'm', 'm', 'X', 'X', 'p']
['s', 'p', 'm', 'p', 's', 's', 'm', 'X', 'p']
['X', 'm', 's', 'p', 's', 'm', 'p', 'X', 'X']
['s', 's', 'p', 'p', 'm', 'm', 'p', 'X', 's']
['p', 's', 'X', 's', 'p', 's', 'X', 'p', 'p']
['p', 'X', 's', 's', 'X', 's', 'p', 'X', 'X']


correct_color_grid = [
    ['b', 'b', 'p', 'r', 'r', 'b', 'r', 'r', 'r'],
    ['p', 'r', 'r', 'p', 'r', 'b', 'p', 'p', 'p'],
    ['p', 'p', 'b', 'p', 'b', 'b', 'r', 'b', 'p'],
    ['b', 'r', 'b', 'r', 'p', 'r', 'p', 'r', 'r'],
    ['r', 'p', 'p', 'r', 'b', 'r', 'b', 'p', 'b'],
    ['r', 'b', 'b', 'b', 'p', 'b', 'r', 'r', 'b'],
    ['p', 'b', 'p', 'r', 'p', 'r', 'b', 'b', 'p'],
    ['p', 'r', 'r', 'p', 'p', 'b', 'b', 'r', 'p'],
    ['b', 'b', 'b', 'r', 'r', 'p', 'p', 'p', 'r'],
]

correct_grids = {
    'color': assert_grid_to_lookup_grid(correct_color_grid),
    'shape': assert_grid_to_lookup_grid(correct_shape_grid),
}

assert( start  == correct_start )
assert( finish == correct_finish )
#assert( shape_grid == correct_shape_grid )
#assert( color_grid == correct_color_grid )

#------------------------------------------------------------
# main
#------------------------------------------------------------

if __name__ == '__main__':
    for solution in maze_search(grids, start, finish):
        print_solution(image, solution)

    while 0xFF & cv2.waitKey(30) != 27:
        pass
    cv2.destroyAllWindows()
