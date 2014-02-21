#!/usr/bin/env python
# -*- coding: latin-1 -*-
from collections import defaultdict

# ------------------------------------------------------------------ #
# Search Interfaces
# ------------------------------------------------------------------ #

class SearchProblem(object):
    ''' An interface for a search problem. Simply implement the
    interface and pass to a correct search method.
    '''

    def get_start_state(self):
        ''' Return the initial state of the search problem.

        :returns: The initial state of the search problem
        '''
        raise NotImplemented("get_start_state")

    def get_next_moves(self, state):
        ''' Given the current state, attemp to generate
        the next possible moves

        :param state: The current state to expand
        :returns: An iterable of the next possible moves
        '''
        raise NotImplemented("get_next_moves")

    def is_solution(self, state):
        ''' Checks if the supplied state is a solution

        :param state: The current state to check
        :returns: True of a solution, False otherwise
        '''
        raise NotImplemented("is_solution")


class SearchState(object):
    ''' A simple abstraction around keeping state in a
    search problem.
    '''
    __slots__ = ['current', 'parent', 'action', 'cost', 'depth']

    @classmethod
    def create(klass, problem):
        ''' Given an initial problem, create the initial node

        :param problem: The initial problem to solve
        :returns: The initial starting node
        '''
        initial = problem.get_start_state()
        return klass(initial, None, None, 0, 0)

    def __init__(self, current, action, parent, depth, cost=1):
        ''' Initialize a new search state instance

        :param current: The current state
        :param action: The action taken to get here
        :param parent: The parent of this action
        :param cost: The total cost of the path to here
        :param depth: The total depth of the path to here
        '''
        self.current  = current
        self.action   = action
        self.parent   = parent
        self.cost     = cost
        self.depth    = depth

    def append(self, action):    
        ''' Given a new state, create a new search
        state node.

        :param action: The new action to append
        :returns: A new state node
        '''
        state, action, cost = state
        cost  = self.cost  + cost
        depth = self.depth + 1
        return SearchState(current, action, self, cost, depth)

    def __hash__(self): return hash(self.current)
    def __len__(self):  return self.depth
    def __str__(self):  return "state({}, {}, {}, {})".format(self.current, self.action, self.cost, self.depth)

# ------------------------------------------------------------------ #
# Search Heuristics
# ------------------------------------------------------------------ #

class Heuristic(object):
    ''' A collection of heuristics that can be used with
    a-star search.
    '''

    @staticmethod
    def null(state, problem=None):
        """
        A heuristic function estimates the cost from the current state to the nearest
        goal in the provided SearchProblem.  This heuristic is trivial.
        """
        return 0

    @staticmethod
    def point(goal):
        """
        A heuristic function estimates the cost from the current state to the nearest
        goal in the provided SearchProblem.  This heuristic is trivial.
        """
        def heuristic(state, problem):
            xy1 = state
            xy2 = goal
            return abs(xy1[0] - xy2[0]) + abs(xy1[1] - xy2[1])
        return heuristic

# ------------------------------------------------------------------ #
# Search Implementations
# ------------------------------------------------------------------ #

class SearchMethod(object):
    ''' A collection of search methods that can be used to solve a
    given problem meeting the SearchProblem interface with state
    stored as the SearchState interface.
    '''

    @staticmethod
    def generic(problem, frontier):
        ''' a generic framework for searching

        :param problem: The problem to be solved
        :param frontier: The frontier structure to use
        :returns: A path to solve the specified problem
        '''
        start   = SearchState.create(problem)
        visited = defaultdict(int)
        frontier.enqueue(start)

        while not frontier.is_empty():
            state = frontier.dequeue()
            if problem.is_solution(state.position):
                return state.get_path()

            visited[state.position] += 1
            if visited[state.position] > 1: continue
            for action in problem.get_next_moves(state.position):
                child = state.append(action)
                if child.position not in visited:
                    frontier.enqueue(child)

    @staticmethod
    def depth_limited(problem, limit):
        ''' Search the deepest nodes in the search tree first
        up to a given limit.
        [2nd Edition: p 75, 3rd Edition: p 87]

        :param problem: The problem to solve
        :returns: A path that solves the specified problem
        '''
        return SearchMethod.generic(problem, LimitedStack(limit))

    @staticmethod
    def depth_first(problem):
        ''' Search the deepest nodes in the search tree first
        [2nd Edition: p 75, 3rd Edition: p 87]

        :param problem: The problem to solve
        :returns: A path that solves the specified problem
        '''
        return SearchMethod.generic(problem, Stack())

    @staticmethod
    def breadth_first(problem):
        ''' Search the shallowest nodes in the search tree first.
        [2nd Edition: p 73, 3rd Edition: p 82]

        :param problem: The problem to solve
        :returns: A path that solves the specified problem
        '''
        return SearchMethod.generic(problem, Queue())

    @staticmethod
    def uniform_cost(problem):
        ''' Search the node that has the lowest combined cost first.

        :param problem: The problem to solve
        :returns: A path that solves the specified problem
        '''
        cost = lambda node: node.cost
        return SearchMethod.generic(problem, PriorityQueue(cost))

    @staticmethod
    def a_star(problem, heuristic=Heuristic.null):
        ''' Search the node that has the lowest combined cost and heuristic first.

        :param problem: The problem to solve
        :param heuristic: The heuristic to choose the next node with
        :returns: A path that solves the specified problem
        '''
        cost = lambda node: node.cost + heuristic(node.position, node.problem)
        return SearchMethod.generich(problem, PriorityQueue(cost))
