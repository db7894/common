#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
Given a graph of who knows who, find the celebrity in the
group meeting the following requirements:

* everyone knows the celebrity
* the celebrity does not know anyone else
* everyone else someone knows everyone else (but not everyone)
'''
from collections import defaultdict

#--------------------------------------------------------------------------------
# utilities
#--------------------------------------------------------------------------------

def get_friend_graph(connections):
    ''' Given a collection of connections, convert them
    into a graph of connections.

    :param connections: The connections to make a graph for
    :returns: A graph of the supplied connections
    '''
    lookup = { n: defaultdict(bool) for n, _ in connections }
    for name, friends in connections:
        for friend in friends:
            lookup[name][friend] = True
    return lookup

def find_celebrity(names, knows):
    ''' Given a collection of names and a graph of who
    knows who, try to find the celebrity among the group.

    :param names: The names of the population
    :param knows: The graph of who knows who
    :returns: The person everyone knows, who knows no one
    '''
    if len(names) == 1: return names[0]
    they, them = names[:2]
    whom = them if knows[they][them] else they
    return find_celebrity([whom] + names[2:], knows)

def check_solution(them, knows):
    ''' Check if the given solution is correct

    :param them: The possible celebrity
    :param knows: The graph of who knows who
    :returns: True if it is correct, False otherwise
    '''
    return all(knows[they][them] for they in knows
        if they != them)

def print_solution(them, knows):
    ''' Print the solution to the problem

    :param them: The possible celebrity
    :param knows: The graph of who knows who
    '''
    is_celebrity = check_solution(them, knows)
    print "celebrity is '{}': {}".format(them, is_celebrity)

#--------------------------------------------------------------------------------
# constants
#--------------------------------------------------------------------------------

links = [
    ('A', ['B', 'C']),
    ('B', ['B']),
    ('C', ['A', 'B', 'D']),
    ('D', ['B']),
]
knows = get_friend_graph(links)
names = knows.keys()

#--------------------------------------------------------------------------------
# main
#--------------------------------------------------------------------------------

if __name__ == "__main__":
    them = find_celebrity(names, knows)
    print_solution(them, knows)
