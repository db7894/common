#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a graph of connected cities, each city has
two paths out: one blue and one red. If we model a
path between cities as a string 'rbrrb', based on which
path they take, can we find a single path such that from
any node, following that path, we will arrive at the same
node (say 1).
'''
#------------------------------------------------------------
# solution
#------------------------------------------------------------

#------------------------------------------------------------
# constants
#------------------------------------------------------------

GRAPH = {
  1: { 'b': 3, 'r': 4 },
  2: { 'b': 3, 'r': 1 },
  3: { 'b': 3, 'r': 2 },
  4: { 'b': 2, 'r': 3 },
}
NODES = GRAPH.keys()

#------------------------------------------------------------
# main
#------------------------------------------------------------
if __name__ == '__main__':
    for solution in find_single_path(GRAPH, final=1):
        print_solution(solution)
