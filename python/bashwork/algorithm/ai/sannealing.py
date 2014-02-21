import sys
import math
import heapq
import random
import itertools
from collections import defaultdict

# ------------------------------------------------------------------ #
# helpers
# ------------------------------------------------------------------ #
def flip_coin(probability):
    value = random.random()
    return value < probability

# ------------------------------------------------------------------ #
# Interfaces
# ------------------------------------------------------------------ #
class Problem:

    def get_start(self):
        ''' Gets the initial problem start state

        :returns: The initial problem start state
        '''
        pass

    def is_goal(self, state):
        ''' Checks if the supplied state is the goal

        :param state: The state to test for being the goal
        :returns: True if the goal state, False otherwise
        '''
        pass

    def get_actions(self, state):
        """
          state: Search state

        For a given state, this should return a list of triples,
        (successor, action, stepCost), where 'successor' is a
        successor to the current state, 'action' is the action
        required to get there, and 'stepCost' is the incremental
        cost of expanding to that successor
        """
        pass

    def action_cost(self, actions):
        """
         actions: A list of actions to take

        This method returns the total cost of a particular sequence of actions.  The sequence must
        be composed of legal moves
        """
        pass

    def get_value(self, state):
        ''' Gets the value of the supplied state
        '''
        pass

# ------------------------------------------------------------------ #
# Data Structures
# ------------------------------------------------------------------ #
class Stack(object):

    def __init__(self):
        self.list = []

    def push(self,item):
        "Push 'item' onto the stack"
        self.list.append(item)

    def pop(self):
        "Pop the most recently pushed item from the stack"
        return self.list.pop()

    def is_empty(self):
        "Returns true if the stack is empty"
        return len(self.list) == 0


class LimitedStack(Stack):
    ''' A stack that refuses to accept any new
    items that are longer than the supplied limit
    '''

    def __init__(self, limit):
        ''' Initializes a new instance of the stack

        :param limit: The limit of the search distance
        '''
        Stack.__init__(self)
        self.limit = limit

    def push(self, item):
        ''' Push a new item onto the stack if it is
        under the supplied limit.

        Note, the item must have overloaded __len__
        to return its current lenth.

        :param item: The item to push on the stack
        '''
        "Push 'item' onto the stack"
        if len(item) < self.limit:
            Stack.push(self, item)

class Queue:
    "A container with a first-in-first-out (FIFO) queuing policy."
    def __init__(self):
        self.list = []

    def push(self,item):
        "Enqueue the 'item' into the queue"
        self.list.insert(0,item)

    def pop(self):
        """
          Dequeue the earliest enqueued item still in the queue. This
          operation removes the item from the queue.
        """
        return self.list.pop()

    def is_empty(self):
        "Returns true if the queue is empty"
        return len(self.list) == 0

class PriorityQueue(object):
    """
      Implements a priority queue data structure. Each inserted item
      has a priority associated with it and the client is usually interested
      in quick retrieval of the lowest-priority item in the queue. This
      data structure allows O(1) access to the lowest-priority item.

      Note that this PriorityQueue does not allow you to change the priority
      of an item.  However, you may insert the same item multiple times with
      different priorities.
    """
    def  __init__(self):
        self.heap = []

    def push(self, item, priority):
        pair = (priority, item)
        heapq.heappush(self.heap,pair)

    def pop(self):
        (priority, item) = heapq.heappop(self.heap)
        return item

    def is_empty(self):
        return len(self.heap) == 0

class EvaluatedPriorityQueue(PriorityQueue):
    """
    Implements a priority queue with the same push/pop signature of the
    Queue and the Stack classes. This is designed for drop-in replacement for
    those two classes. The caller has to provide a priority function, which
    extracts each item's priority.
    """
    def  __init__(self, evaluator):
        self.evaluator = evaluator
        PriorityQueue.__init__(self)

    def push(self, item):
        PriorityQueue.push(self, item, self.evaluator(item))

# ------------------------------------------------------------------ #
# Containers
# ------------------------------------------------------------------ #
class Node(object):
    ''' Represents a node path state chain that 
    is used to make the generic search function
    generic.
    '''
    __slots__ = ['position', 'parent', 'action', 'cost', 'problem', 'depth']

    @staticmethod
    def create(problem):
        ''' Given an initial position, create a node

        :param problem: The initial problem to solve
        :returns: The initial starting node
        '''
        initial = problem.get_start()
        return Node(problem, initial, None, "Stop", 0, 0)

    def __init__(self, problem, position, parent, action, cost, depth):
        ''' Initialize a new node instance

        :param problem: The problem state for the heuristic
        :param position: The position of this node
        :param parent: The previous parent node
        :param action: The action taken to get here
        :param cost: The total cost of the path to here
        :param depth: The total depth of the path to here
        '''
        self.position = position
        self.parent   = parent
        self.action   = action
        self.cost     = cost
        self.problem  = problem
        self.depth    = depth

    def append(self, state):    
        ''' Given a new state, create a new path node

        :param state: The new state to append
        :returns: A new state node
        '''
        state, action, cost = state
        cost  = self.cost  + cost
        depth = self.depth + 1
        return Node(self.problem, state, self, action, cost, depth)

    def get_path(self):
        ''' Given a goal, return the path to that goal

        :returns: A path to the given goal
        '''
        path, node = [], self
        while node.parent != None:
            path.insert(0, node.action)
            node = node.parent
        return path

    def get_positions(self):
        ''' Given a goal, return the path of positions
        to that goal.

        :returns: A path of the positions to the given goal
        '''
        states, node = [], self
        while node.parent != None:
            states.insert(0, node.position)
            node = node.parent
        return states

    def contains(self, node):
        ''' Checks if the given state is already in this path

        :param state: The state to check for existence
        :returns: True if in this path, False otherwise
        '''
        # TODO make this O(1) with an instance singleton
        return node.position in self.get_positions()

    def __hash__(self):
        ''' An overload of the hash function

        :returns: A hash of the current state
        '''
        return hash(self.position)

    def __len__(self):
        ''' An overload of the len function

        :returns: The current length of the path
        '''
        return self.depth

    def __str__(self):
        ''' Returns a string representation of this node

        :returns: The representation of this node
        '''
        parent = str(self.parent.position) if self.parent else "(start)"
        params = (parent, self.action, self.cost, str(self.position), self.depth)
        return "node(%s %s(%d) %s) len(%d)" % params


# ------------------------------------------------------------
# heuristics
# ------------------------------------------------------------
def null_heuristic(state, problem=None):
    """
    A heuristic function estimates the cost from the current state to the nearest
    goal in the provided SearchProblem.  This heuristic is trivial.
    """
    return 0

def manhattan_heuristic(position, problem):
    "The Manhattan distance heuristic for a PositionSearchProblem"
    xy1 = position
    xy2 = problem.goal
    return abs(xy1[0] - xy2[0]) + abs(xy1[1] - xy2[1])

def euclidean_heuristic(position, problem):
    "The Euclidean distance heuristic for a PositionSearchProblem"
    xy1 = position
    xy2 = problem.goal
    return ( (xy1[0] - xy2[0]) ** 2 + (xy1[1] - xy2[1]) ** 2 ) ** 0.5

# ------------------------------------------------------------------ #
# Search Implementations
# ------------------------------------------------------------------ #
def genericSearch(problem, frontier):
    ''' a generic framework for searching

    :param problem: The problem to be solved
    :param frontier: The frontier structure to use
    :returns: A path to solve the specified problem
    '''
    start   = Node.create(problem)
    visited = defaultdict(lambda:0)
    frontier.push(start)

    while not frontier.is_empty():
        state = frontier.pop()
        if problem.is_goal(state.position):
            return state.get_path()

        visited[state.position] += 1
        if visited[state.position] > 1: continue
        for action in problem.get_actions(state.position):
            child = state.append(action)
            if child.position not in visited:
                frontier.push(child)

def depth_limited_search(problem, limit):
    ''' Search the deepest nodes in the search tree first
    up to a given limit.
    [2nd Edition: p 75, 3rd Edition: p 87]

    :param problem: The problem to solve
    :returns: A path that solves the specified problem
    '''
    return generic_search(problem, LimitedStack(limit))

def dfs_search(problem):
    ''' Search the deepest nodes in the search tree first
    [2nd Edition: p 75, 3rd Edition: p 87]

    :param problem: The problem to solve
    :returns: A path that solves the specified problem
    '''
    return generic_search(problem, Stack())

def bfs_search(problem):
    ''' Search the shallowest nodes in the search tree first.
    [2nd Edition: p 73, 3rd Edition: p 82]

    :param problem: The problem to solve
    :returns: A path that solves the specified problem
    '''
    return generic_search(problem, Queue())

def ucs_search(problem):
    ''' Search the node that has the lowest combined cost first.

    :param problem: The problem to solve
    :returns: A path that solves the specified problem
    '''
    cost = lambda node: node.cost
    return generic_search(problem, EvaluatedPriorityQueue(cost))

def astar_search(problem, heuristic=null_heuristic):
    ''' Search the node that has the lowest combined cost and heuristic first.

    :param problem: The problem to solve
    :param heuristic: The heuristic to choose the next node with
    :returns: A path that solves the specified problem
    '''
    cost = lambda node: node.cost + heuristic(node.position, node.problem)
    return generic_search(problem, EvaluatedPriorityQueue(cost))



# ------------------------------------------------------------
# example schedules
# ------------------------------------------------------------
def log_schedule(depth=1):
    def schedule(temp):
        return depth / math.log(temp)
    return schedule

def exp_schedule(k=20, lam=0.045, limit=100):
    def schedule(temp):
        if temp < limit:
            return k * math.exp(-lam * temp)
        return 0.0
    return schedule


# ------------------------------------------------------------
# simulated annealing
# ------------------------------------------------------------
def simulated_annealing(problem, schedule=exp_schedule()):
    ''' Performs simulated annealing on the supplied problem

    :param problem: The problem to solve
    :param schedule: The annealing schedule to run with
    :returns: The resulting solution
    '''
    current = problem.get_start()
    for t in itertools.count(0):
        temp = schedule(t)
        if temp == 0.0: return None
        if problem.is_goal(current):
            print "found a goal"
            return current
        actions = problem.get_actions(current)
        action = random.choice(actions)
        delta  = problem.get_value(action) - problem.get_value(current)
        choice = math.exp(delta / temp)
        if (delta < 0) or flip_coin(choice):
            current = action
    return None

# ------------------------------------------------------------
# problems
# ------------------------------------------------------------
class EightQueensProblem(Problem):

    def __init__(self, size=8):
        "Creates a new EightPuzzleSearchProblem which stores search information."
        self.size = size
        values = range(1, size + 1)
        random.shuffle(values)
        self.puzzle = dict((i,v) for i,v in enumerate(values, 1))

    def get_start(self):
        return self.puzzle

    def is_goal(self, state):
        return self.get_value(state) == 0

    def get_actions(self, state):
        weights = {}
        for col, row in state.items():
            weights[col] = self._count_conflicts(state, col, row) 
        values = weights.items()
        values.sort(key=lambda a: a[1], reverse=True)
        choice = random.choice([c for c,v in values if v == values[0][1]])
        return [self._minimize(state, choice)]

    def get_value(self, state):
        count = 0
        for col, row in state.items():
            count += self._count_conflicts(state, col, row) 
        return count

    def _minimize(self, state, col):
        nstate  = dict(state)
        current = (sys.maxint, -1)
        for row in xrange(1, self.size + 1):
            check = self._count_conflicts(nstate, col, row)
            check = (check, row)
            if check < current: current = check
        nstate[col] = current[1]
        return nstate

    def _count_conflicts(self, state, col, row):
        count = 0
        for ncol, nrow in state.items():
            if   ncol == col: continue
            elif row  == nrow: count += 1
            elif row - col == nrow - ncol: count += 1
            elif row + col == nrow + ncol: count += 1
        return count

def print_board(state):
    size = len(state)
    board = {}
    for col, row in sorted(state.items(), key=lambda a:a[1]):
        line = ['Q' if x == row else '-' for x in xrange(size)]
        board[col] = ''.join(line) + '\n'
    for col, line in board.items():
        print line,
        

if __name__ == '__main__':
    solved = None
    while not solved: 
        problem = EightQueensProblem(50)
        solved  = simulated_annealing(problem, exp_schedule(limit=200))
        print '.'
    print solved.values()
    print_board(solved)
