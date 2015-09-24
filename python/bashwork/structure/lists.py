import sys
from itertools import izip_longest

class Node(object):
    ''' A simple linked list node element.
    '''

    def __init__(self, value, link=None):
        self.value = value
        self.link  = link

    def __iter__(self):
        while self != None:
            yield self
            self = self.link

    def next(self):
        if self.link == None:
            raise StopIteration()
        return self.link

    def __eq__(self, that): return (that != None) and (self.value == that.value)
    def __ne__(self, that): return (that == None)  or (self.value != that.value)
    def __hash__(self):     return hash(self.value)
    def __repr__(self):     return str(self.value)
    def __str__(self):      return str(self.value)


def merge_sorted_lists(xs, ys):
    ''' Given two sorted lists, merge them

    >>> xs = [1, 3, 5]
    >>> ys = [2, 4, 6]
    >>> merge_sorted_lists(xs, ys)
    [1, 2, 3, 4, 5, 6]
    '''
    rs = []
    while len(xs) > 0 and len(ys) > 0:
        rs.append(xs.pop(0) if xs[0] < ys[0] else ys.pop(0))
    return rs + xs + ys


def merge_sorted_linked_lists(xs, ys):
    ''' Given two sorted linked lists, merge them

    >>> xs = Node(1, Node(3, Node(5)))
    >>> ys = Node(2, Node(4, Node(6)))
    >>> rs = merge_sorted_linked_lists(xs, ys)
    >>> list(rs)
    [1, 2, 3, 4, 5, 6]
    '''
    head = xs if xs.value < ys.value else ys
    curr = head
    while xs and ys:
        if xs.value < ys.value:
            xs, curr.link = xs.link, xs
        else: ys, curr.link = ys.link, ys
        curr = curr.link

    rest = xs if xs else ys
    while rest:
        rest, curr.link = rest.link, rest
        curr = curr.link
    return head


def reverse_list(xs):
    ''' Given a linked list, reverse it

    >>> xs = Node(1, Node(2, Node(3, Node(4, Node(5)))))
    >>> rs = reverse_list(xs)
    >>> list(rs)
    [5, 4, 3, 2, 1]
    '''
    last, head = xs, None
    while last:
        temp = last.link
        last.link = head
        last, head = temp, last
    return head


def reverse_list_recursive(xs):
    ''' Given a linked list, reverse it

    >>> xs = Node(1, Node(2, Node(3, Node(4, Node(5)))))
    >>> rs = reverse_list_recursive(xs)
    >>> list(rs)
    [5, 4, 3, 2, 1]
    '''
    def recurse(curr, prev):
        link = curr.link
        curr.link = prev
        return curr if link == None else recurse(link, curr)
    return recurse(xs, None)


def find_merge_point_naive(xs, ys):
    ''' Given two linked lists, find the node
    at which they merge:

    - O(xs + ys) time
    - O(xs) space

    >>> me = Node(6, Node(7))
    >>> xs = Node(0, Node(1, Node(3, me)))
    >>> ys = Node(2, Node(4, Node(5, me)))
    >>> rs = find_merge_point_naive(xs, ys)
    >>> list(rs)
    [6, 7]
    '''
    visited = set(xs)
    for y in ys:
        if y in visited: return y
    return None


def find_merge_point(xs, ys):
    ''' Given two linked lists, find the node
    at which they merge:

    - O(max(xs,ys)) time
    - O(max(xs,ys)) space
    
    Solutions:
    - (sd2) add checkpoints and backtrack

    >>> me = Node(6, Node(7))
    >>> xs = Node(0, Node(1, Node(3, me)))
    >>> ys = Node(2, Node(4, Node(5, me)))
    >>> rs = find_merge_point(xs, ys)
    >>> list(rs)
    [6, 7]
    '''
    visited = set()
    for x, y in izip_longest(xs, ys):
        if x == y: return x
        if x in visited: return x
        if y in visited: return y
        if x: visited.add(x)
        if y: visited.add(y)
    return None

def find_loop_match(xs, ys):
    ''' Find a point at which two lists loop
    (not the first loop point, but the chasing
    pointers node).
    '''
    if not xs or not ys:
        return None
    if xs == ys: return ys
    return find_loop_match(xs.link, ys.link.link)

def find_merge_point_2(xs, ys):
    ''' Given two linked lists, find the node
    at which they merge:

    >>> me = Node(6, Node(7, Node(8, Node(9))))
    >>> me.link.link.link = me
    >>> xs = Node(0, Node(1, Node(3, me)))
    >>> ys = Node(2, Node(4, Node(5, me)))
    >>> find_merge_point(xs, ys)
    6
    '''
    ms = find_loop_match(xs, ys)
    if not ms: return None

    while ms != xs:
        ms = ms.link
        xs = xs.link
    return ms

def make_unique_list(head):
    ''' Given a linked list, return the list
    as a unique list and also return the total
    size of the list as well as the number of
    duplicates

    >>> xs = Node(1, Node(2, Node(1, Node(3, Node(4, Node(5, Node(3)))))))
    >>> make_unique_list(xs)
    (1, 5, 2)
    '''
    if not head: return (head, 0, 0)
    seen = set([head.value])
    size, dups = 1, 0
    curr, prev = head.link, head
    while curr:
        if curr.value in seen:
            dups += 1
            prev.link, curr = curr.link, curr.link
        else:
            size += 1;
            seen.add(curr.value)
            prev, curr = prev.link, curr.link
    return (head, size, dups)

def order_edge_nodes(nodes):
    ''' Given a collection of tuples that represent
    the edges in a graph, order them into a their
    linear linked order.

    >>> order_edge_nodes([(1,3), (4,5), (5,1), (3,8)])
    [(4, 5), (5, 1), (1, 3), (3, 8)]

    :param nodes: The nodes to order correctly
    :returns: The linear linked order of the nodes
    '''
    def graph_visitor(graph, node, target):
        while node in graph:
            target(node, graph.get(node))
            node = graph.get(node)

    graphl, graphr = {}, {}
    for l, r in nodes:
        graphl[l], graphr[r] = r, l

    links = [(nodes[0][0], nodes[0][1])]
    graph_visitor(graphr, nodes[0][0], lambda l,r: links.insert(0, (r,l)))
    graph_visitor(graphl, nodes[0][1], lambda r,l: links.append((r,l)))
    return links

def find_missing_number(numbers):
    ''' Given a collection of integer elements, find the
    missing element (in the set of 1..n).

    >>> numbers = [1,2,3,4,5,6,8,9,10]
    >>> find_missing_number(numbers)
    7

    :param numbers: The collection of numbers to search in
    :returns: The missing number from the numbers list
    '''
    sumn, minn, maxn = 0, sys.maxint, -sys.maxint
    for number in numbers:
        sumn += number
        minn = min(number, minn)
        maxn = max(number, maxn)

    size  = maxn - minn + 1 # n(n + 1) / 2: max_sum - min_sum
    total = (size * (size + 1) / 2) - (minn * (minn - 1) / 2)
    return total - sumn

if __name__ == "__main__":
    import doctest
    doctest.testmod()
