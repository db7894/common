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
# structure
#------------------------------------------------------------
class Path(object):
    ''' A simple inverted tree that can be used to maintain
    a running path.
    '''
    Red  = 'r'
    Blue = 'b'

    @classmethod
    def expand(klass, graph, parent, action):
        ''' Given a current parent path, expand it on the
        given direction and return a new child path.

        :param graph: The graph to expand with
        :param parent: The parent path to expand
        :param action: The movement to expand to
        :returns: A new child Path node
        '''
        value = set(graph[node][action] for node in parent.value)
        return klass(value, action, parent)

    def __init__(self, value, action=None, parent=None):
        ''' Initialize a new instance of a path node

        :param value: The value of this node
        :param action: The action that was taken to get here
        :param parent: The parent of this node
        '''
        self.value  = value
        self.action = action
        self.parent = parent

#------------------------------------------------------------
# solution
#------------------------------------------------------------
def find_single_path(graph, final):
    ''' Given a graph, find a path from any node such
    that all nodes following that path will arrive
    at the same final node.

    Note, by the pumping lemma, there can be infinite
    solutions to this problem.

    :param graph: The graph to find a path through
    :param final: The final node to arrive at
    :returns: The first solution to the problem
    '''
    frontier = [Path(set(graph.keys()))]
    solution = set([final])
    while frontier:
        node = frontier.pop()
        if node.value == solution:
            return node

        frontier.insert(0, Path.expand(graph, node, Path.Red))
        frontier.insert(0, Path.expand(graph, node, Path.Blue))

def print_solution(solution):
    ''' Given a solution to the problem,
    print out the result:

    :param solution: A solution to the problem
    '''
    node = solution
    path = []
    while node.parent:
        path.append(node.action)
        node = node.parent
    print ' → '.join(reversed(path))

#------------------------------------------------------------
# constants
#------------------------------------------------------------

GRAPH = {
  1: { Path.Blue: 3, Path.Red: 4 },
  2: { Path.Blue: 3, Path.Red: 1 },
  3: { Path.Blue: 3, Path.Red: 2 },
  4: { Path.Blue: 2, Path.Red: 3 },
}
FINAL = 1
NODES = GRAPH.keys()

#------------------------------------------------------------
# main
#------------------------------------------------------------

if __name__ == '__main__':
    solution = find_single_path(GRAPH, FINAL)
    print_solution(solution)
