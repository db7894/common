#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
Given a grid of images, divide the images into
six sections so that each section consists of
six adjacent squares and each section has one
of each of the images.
'''
from __future__ import division

import math
from random import random, shuffle, choice, seed

#------------------------------------------------------------
# utilities
#------------------------------------------------------------

def spaces_to_groups(grid, solution):
    ''' Given a solution, convert it to a form that is
    easy to check the solution. So we go:
    * from { point -> group }
    * to   { group -> ([points], [images]) }

    :param grid: The current grid to search
    :param solution: The current solution to convert
    :returns: The converted solution
    '''
    spaces = {}
    images = {}

    for (x, y), group in solution.items():
        spaces.setdefault(group, set()).add((x, y))
        images.setdefault(group, set()).add(grid[y][x])
    return spaces, images

def largest_group(points):
    ''' Given a collection of points, determine the linear
    score by finding the size of the largest connected group.

    :param points: The points to find the largest group for
    :returns: The size of the largest group
    '''
    groups = []
    for (x, y) in points:
        neighbors = set([(x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)])
        for group in groups:
            if not group.isdisjoint(neighbors):
                group.add((x, y))
                break
        else: groups.append(set([(x, y)]))
    return max(len(group) for group in groups)

#------------------------------------------------------------
# initial solutions
#------------------------------------------------------------

def random_initial_solution(grid):
    ''' Generates an initial solution given the grid
    to operate with.

    :param grid: The grid to build an initial solution with
    :returns: The initial random solution
    '''
    groups = range(1, len(grid[0]) + 1) # 1..6
    xs = range(len(grid[0]))            # 0..5
    ys = range(len(grid))               # 0..5
    group  = [g for g in groups for _ in xs]
    shuffle(group)
    solution = { (x, y) : group.pop() for x in xs for y in ys }
    return solution

def column_initial_solution(grid):
    ''' Generates an initial solution given the grid
    to operate with.

    :param grid: The grid to build an initial solution with
    :returns: The initial random solution
    '''
    groups = range(1, len(grid[0]) + 1) # 1..6
    xs = range(len(grid[0]))            # 0..5
    ys = range(len(grid))               # 0..5
    solution = { (x, y) : x + 1 for x in xs for y in ys }
    return solution

def row_initial_solution(grid):
    ''' Generates an initial solution given the grid
    to operate with.

    :param grid: The grid to build an initial solution with
    :returns: The initial random solution
    '''
    groups = range(1, len(grid[0]) + 1) # 1..6
    xs = range(len(grid[0]))            # 0..5
    ys = range(len(grid))               # 0..5
    solution = { (x, y) : y + 1 for x in xs for y in ys }
    return solution

#------------------------------------------------------------
# neighbors
#------------------------------------------------------------

def get_random_neighbor(grid, solution):
    ''' Compute a new neighbor to this solution
    that is possible better than this solution.

    :param grid: The current grid to search
    :param solution: The current solution to permute
    :returns: A permuted solution
    '''
    points = solution.keys()
    pa, pb = choice(points), choice(points) 
    copy   = dict(solution)
    copy[pa], copy[pb] = copy[pb], copy[pa]
    return copy

def get_valid_neighbor(grid, solution):
    ''' Compute a new neighbor to this solution
    that is possible better than this solution.

    :param grid: The current grid to search
    :param solution: The current solution to permute
    :returns: A permuted solution
    '''
    pa  = choice(solution.keys())
    (x, y) = pa
    pbs    = [(x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)]
    shuffle(pbs)

    for pb in pbs:
        if (pb in solution) and (solution[pb] != solution[pa]):
            break
    else: return solution

    copy = dict(solution)
    copy[pa], copy[pb] = copy[pb], copy[pa]
    return copy

def get_group_neighbor(grid, solution):
    ''' Compute a new neighbor to this solution
    that is possible better than this solution.

    :param grid: The current grid to search
    :param solution: The current solution to permute
    :returns: A permuted solution
    '''
    spaces, images = spaces_to_groups(grid, solution)
    group  = choice(spaces.keys())
    points = []

    for (x, y) in spaces[group]:
        for point in [(x - 1, y), (x + 1, y), (x, y - 1), (x, y + 1)]:
            if point in solution: points.append(point)

    pb = choice(list(spaces[group]))
    pa = choice(points)

    copy = dict(solution)
    copy[pa], copy[pb] = copy[pb], copy[pa]
    return copy

#------------------------------------------------------------
# solver
#------------------------------------------------------------

def compute_maximal_cost(grid, solution):
    ''' Compute a cost estimate of the current solution.

    :param grid: The current grid to search
    :param solution: The current solution to compute the cost of
    :returns: The cost of the current solution
    '''
    spaces, images = spaces_to_groups(grid, solution)
    total  = sum(len(group) for group in images.values())
    total += sum(largest_group(group) for group in spaces.values())
    return total

def compute_minimal_cost(grid, solution):
    ''' Compute a cost estimate of the current solution.

    :param grid: The current grid to search
    :param solution: The current solution to compute the cost of
    :returns: The cost of the current solution
    '''
    spaces, images = spaces_to_groups(grid, solution)
    size   = len(grid)
    total  = sum(size - len(group) for group in images.values())
    total += sum(size - largest_group(group) for group in spaces.values())
    return total

def accept_maximal_solution(old_cost, new_cost, temp):
    ''' An activation function that can be used to
    accept or reject a new solution based on the current
    parameters.

    :param old_cost: The old compute solution cost
    :param new_cost: The newly computed solution cost
    :param temp: The current temperature
    :returns: True if the new solution is accepted, False otherwise
    '''
    probability = ((new_cost - old_cost) / temp) * math.e
    return (probability > random())

def accept_minimal_solution(old_cost, new_cost, temp):
    ''' An activation function that can be used to
    accept or reject a new solution based on the current
    parameters.

    :param old_cost: The old compute solution cost
    :param new_cost: The newly computed solution cost
    :param temp: The current temperature
    :returns: True if the new solution is accepted, False otherwise
    '''
    diff = new_cost - old_cost
    return (diff < 0) or (math.exp(-diff/ temp) > random())

def find_solution(grid, correct):
    ''' Given a grid and a correct group match, attempt to
    solve the speficied problem.

    :param grid: The grid to find a solution to
    :param correct: An example matching set
    :returns: A generator around possible solutions
    '''
    solution   = initial_solution(grid)
    old_cost   = compute_cost(grid, solution)
    temp       = 1.0
    temp_min   = 0.00001
    cooling    = 0.9
    iterations = 200

    while temp > temp_min:
        iteration = 1
        while iteration <= iterations:
            new_solution = get_neighbor(grid, solution)
            new_cost     = compute_cost(grid, new_solution)
            if accept_solution(old_cost, new_cost, temp):
                solution, old_cost = new_solution, new_cost
                print_grid_solution(grid, solution)
                print "temp: {} cost:{}\n".format(temp, old_cost)
            iteration += 1
        temp = temp * cooling
    return solution

#------------------------------------------------------------
# validation
#------------------------------------------------------------

def validate_solution(grid, solution):
    ''' Given the original grid and a possible solution,
    verify if the solution is valid or not.

    :param grid: The original grid
    :param solution: The solution to validate
    :returns: True if valide, false otherwise
    '''
    spaces, images = spaces_to_groups(grid, solution)
    complete = images.values()[0]
    size = len(complete)

    all_assigned  = all(solution.values())
    all_images    = all(group == complete for group in images.values())
    all_neighbors = all(largest_group(group) == size for group in spaces.values())

    return all_assigned and all_images and all_neighbors

def print_grid_solution(grid, solution):
    ''' Given the original grid and a possible solution
    print the solution to the terminal.

    :param grid: The original grid
    :param solution: The solution to print
    '''
    reset  = '\033[0m'
    colors = [None, '\033[31m', '\033[32m', '\033[33m', '\033[34m', '\033[35m', '\033[36m']
    for y, row in enumerate(grid):
        line = []
        for x, image in enumerate(row):
            message = "{}{}{}".format(colors[solution[(x, y)]], image, reset)
            line.append(message)
        print ' '.join(line)

#------------------------------------------------------------
# constants
#------------------------------------------------------------

GRID1 = [
    ['a', 'd', 'c', 'e', 'a', 'b'],
    ['f', 'e', 'f', 'd', 'e', 'f'],
    ['d', 'c', 'e', 'a', 'f', 'b'],
    ['c', 'b', 'a', 'd', 'b', 'c'],
    ['b', 'f', 'e', 'f', 'a', 'd'],
    ['c', 'a', 'c', 'd', 'e', 'b'],
]
GRID2 = [
    ['a', 'c', 'e', 'a', 'd', 'c'],
    ['a', 'e', 'd', 'b', 'c', 'b'],
    ['b', 'c', 'a', 'e', 'b', 'e'],
    ['c', 'd', 'e', 'd', 'c', 'd'],
    ['a', 'd', 'b', 'b', 'a', 'e'],
]
GRID3 = [
    ['a', 'c', 'a', 'e', 'd', 'd'],
    ['b', 'd', 'e', 'b', 'a', 'c'],
    ['c', 'b', 'c', 'b', 'e', 'd'],
    ['d', 'b', 'd', 'e', 'c', 'a'],
    ['c', 'a', 'e', 'a', 'e', 'b'],
]

initial_solution = column_initial_solution
get_neighbor     = get_random_neighbor
accept_solution  = accept_minimal_solution
compute_cost     = compute_minimal_cost
problem          = GRID3
images           = set(image for row in problem for image in row)

#------------------------------------------------------------
# main
#------------------------------------------------------------

if __name__ == '__main__':
    seed()
    solution = find_solution(problem, images)
    if validate_solution(problem, solution):
        print_grid_solution(problem, solution)
    else: print "No valid solution found"
