#!/usr/bin/env python
import re
import argparse
import logging
from random import randint
from collections import namedtuple

logger = logging.getLogger("picobot")

# ------------------------------------------------------------
# utilities
# ------------------------------------------------------------

def file_iterator(path):
    ''' Given a path, return an iterator around
    the lines in the file.

    :param path: The path to the file
    :returns: an iterator around the lines in the file
    '''
    with open(path, 'r') as handle:
        for line in handle:
            yield line.strip()

# ------------------------------------------------------------
# game compilation
# ------------------------------------------------------------

Rule = namedtuple('Rule', ['rule', 'state', 'move', 'text'])

def compile_rule(walls, move, state):
    ''' Given the wall pattern, the next state, and the
    next move, compile a single rule instance.

    :param walls: The wall pattern to match with
    :param move: The next move to make
    :param state: The state to enter
    :returns: A new rule instance
    '''
    rule = walls.replace('*', '.')
    rule = re.compile(rule)
    return Rule(rule=rule, move=move, state=state, text=walls)

def compile_rules(path):
    ''' Given a path to the bot rules files,
    compile the rules that the bot will operate with.

    :param path: The path to the bot rules file
    :returns: The rules that the bot will operate with
    '''
    rules = {}
    for line in file_iterator(path):
        if not line or line.startswith('#'):
            continue

        line = line.split('#')[0] # remove comments
        parts = line.split()
        prev, wall, _, move, curr = parts[:5] 
        # TODO check if matching rule exists
        rule = compile_rule(wall, move, curr)
        rules.setdefault(prev, []).append(rule)
        logger.debug("added rule %s %s -> %s %s", prev, rule.rule.pattern, move, curr) 
    return rules

def compile_maze(path):
    ''' Given a path to a maze, compile the maze
    into an object that the picobot can manipulate.

    :param path: The path to the maze
    :returns: The compiled maze object
    '''
    maze = []
    for line in file_iterator(path):
        maze.append(list(line))

    Y, X = len(maze) - 1 , len(maze[0]) - 1
    for y in range(0, Y + 1):
        if maze[y][0] != 'x' or maze[y][X] != 'x':
            raise Exception("The supplied maze is invalid as it has wall gaps")

    for x in range(0, X + 1):
        if maze[0][x] != 'x' or maze[Y][x] != 'x':
            raise Exception("The supplied maze is invalid as it has wall gaps")

    return maze

def print_maze(maze):
    ''' Given a picobot maze, print it to console.

    :param maze: The maze to print to console
    '''
    if logger.isEnabledFor(logging.DEBUG):
        for line in maze:
            print ' '.join(line)
        print

# ------------------------------------------------------------
# game logic
# ------------------------------------------------------------

def get_starting_point(maze):
    ''' Get a random starting point for the bot
    in the supplied maze. The only rule is that
    the bot cannot start on a wall.

    :param maze: The maze to get a random starting point for
    :returns: The initialize starting point for the bot
    '''
    Y, X = len(maze) - 2, len(maze[0]) - 2
    y, x = 0, 0

    while maze[y][x] == 'x':
        y = randint(1, Y)
        x = randint(1, X)
    return y, x

def get_current_walls(maze, point):
    ''' Given the current point of the bot in the maze
    and the current state of the maze, return the current
    wall status to be applied to the rules.

    :param maze: The current state of the maze
    :param point: The current point of the bot in the maze
    :returns: The current wall state to apply a rule to
    '''
    y, x = point
    N = 'N' if (maze[y - 1][x] == 'x') else 'x'
    E = 'E' if (maze[y][x + 1] == 'x') else 'x'
    W = 'W' if (maze[y][x - 1] == 'x') else 'x'
    S = 'S' if (maze[y + 1][x] == 'x') else 'x'
    return ''.join([N, E, W, S])

def is_solved(maze):
    ''' Check if the maze has been solved, i.e. all
    the spaces have been visited in the maze.

    :param maze: The current state of the maze
    :returns: True if the maze is solved, False otherwise
    '''
    return all(' ' not in line for line in maze)

def update_state(rule, maze, point):
    ''' Given the rule to apply, the current maze state, and
    the current position, update the state of the world and
    return the new state.

    :param rule: The rule to apply to the world state
    :param maze: The current state of the maze
    :param point: The current point of the bot in the maze
    :returns: The new (state, point)
    '''
    y, x = point

    if   rule.move == 'N': y -= 1
    elif rule.move == 'E': x += 1
    elif rule.move == 'W': x -= 1
    elif rule.move == 'S': y += 1

    maze[y][x] = '.' # mark the spot as visited
    print_maze(maze)

    return rule.state, (y, x)

def run_picobot(maze, rules):
    ''' Run the picobot until success or failure
    on the supplied maze with the supplied rules.

    :param maze: The maze to run the picobot on
    :param rules: The rules to drive the picobot
    '''
    state = '0'
    point = get_starting_point(maze)

    while not is_solved(maze):
        walls = get_current_walls(maze, point)
        for rule in rules[state]:
            if rule.rule.match(walls):
                state, point = update_state(rule, maze, point)
                break
        else:
            raise Exception("No matching rules for {}:'{}' @ {}".format(state, walls, point))

# ------------------------------------------------------------
# setup
# ------------------------------------------------------------

def compile_options():
    ''' Process the command line arguments
    '''
    parser = argparse.ArgumentParser(description="Picobot Runner")
    parser.add_argument('-m', '--maze', dest="maze",
        default=None, help="The path to the maze to operate with")
    parser.add_argument('-r', '--rules', dest="rules",
        default=None, help="The path to the rules to operate with")
    parser.add_argument('-s', '--start', dest="start",
        default=None, help="The starting point for the bot in the maze")
    parser.add_argument('-d', '--debug', dest="debug", action='store_true',
        default=False, help="To enable debut tracing of the program")
    options = parser.parse_args()

    if not options.rules: parser.error("must supply a path to the rules")
    if not options.maze: parser.error("must supply a path to the maze")

    return options

def main():
    ''' Run the picobot with the supplied arguments
    '''
    options = compile_options()

    if options.debug:
        logging.basicConfig(level=logging.DEBUG)

    rules   = compile_rules(options.rules)
    maze    = compile_maze(options.maze)
    run_picobot(maze, rules)
    print "solved the supplied maze"

if __name__ == "__main__":
    main()
