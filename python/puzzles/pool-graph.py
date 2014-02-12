#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a graph of connected pool balls, attempt
to create three connected groups where the balls
equal the same total. The rules are:

* The balls must be touching to be connected
* The total must be 40
'''
from common import Graph

def is_solution(graph, groups, total):
    ''' Check if the supplied solution groups equal
    the solution.

    :param graph: The graph to check for connections
    :param groups: The groups to check the solution of
    :param total: The total the groups should sum to
    '''
    return all(sum(group) == total for group in groups)

def find_pool_groups(graph, total):
    ''' Given a graph of letter connections and a database
    (in this case a trie) of player teams, find which teams
    can be completed using the supplied graph (BFS).

    :param graph: The letter graph to search with
    :param teams: A dictionary of the available teams
    :returns: A generator of the possible solutions
    '''
    queue = graph.get_nodes()
    while queue:
        groups = queue.pop()
        if is_solution(graph, groups, total):
            yield groups
        pass
        #else:
        #    for group in graph.get_edges(word[-1]):
        #        path = word + letter
        #        if teams.has_path(path):
        #            queue.insert(0, path)

def print_pool_groups(groups):
    ''' Given a collection of pool groups, print the
    solution visually.

    :param groups: The groups to print the solution to
    '''
    for group in groups:
        print "%d → %s" % (sum(group), group)

#------------------------------------------------------------
# main
#------------------------------------------------------------
#total = 40
#graph = Graph(nodes=range(1, 16))
#graph.add_edges( 1, 9, 6, 12, 14)
#graph.add_edges( 2, 10, 8)
#graph.add_edges( 3, 15, 6, 7, 10)
#graph.add_edges( 4, 15, 9)
#graph.add_edges( 5, 14, 11)
#graph.add_edges( 6, 15, 9, 1, 12, 7, 3)
#graph.add_edges( 7, 6, 3, 10, 8, 13, 12)
#graph.add_edges( 8, 2, 10, 7, 13)
#graph.add_edges( 9, 4, 15, 6, 1)
#graph.add_edges(10, 3, 7, 8, 2)
#graph.add_edges(11, 5, 14, 12, 13)
#graph.add_edges(12, 1, 6, 7, 13, 11, 14)
#graph.add_edges(13, 11, 12, 7, 8)
#graph.add_edges(14, 5, 11, 12, 1)
#graph.add_edges(15, 4, 9, 6, 3)

if __name__ == '__main__':
    total = 40
    graph = Graph(nodes=range(1, 16))
    graph.add_edges( 1, )
    graph.add_edges( 2, )
    graph.add_edges( 3, )
    graph.add_edges( 4, )
    graph.add_edges( 5, )
    graph.add_edges( 6, )
    graph.add_edges( 7, )
    graph.add_edges( 8, )
    graph.add_edges( 9, )
    graph.add_edges(10, )
    graph.add_edges(11, )
    graph.add_edges(12, )
    graph.add_edges(13, )
    graph.add_edges(14, )
    graph.add_edges(15, )

    for groups in find_pool_groups(graph, total):
        print_pool_groups(groups)
