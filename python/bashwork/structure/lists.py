import sys
from itertools import izip_longest, count

class Node(object):
    ''' A simple linked list node element.
    '''

    @classmethod
    def create(klass, values):
        ''' Given an iterable of values, convert them
        to a linked list collection.

        :param values: The values to convert to a linked list
        :returns: The head of the new list
        '''
        vals = iter(values)
        head = klass(vals.next())
        node = head

        for val in vals:
            node = node.append(val)
        return head

    def __init__(self, value, link=None):
        ''' Create a new instance of a linked list node

        :param value: The value of this node
        :param link: The next node in the list
        '''
        self.value = value
        self.link  = link

    def find(self, value):
        ''' Given a value, attempt to find the node
        containing that value.

        :param value: The value to find in the list
        :returns: The found node or None if it is missing
        '''
        while self != None:
            if self.value == value:
                return self
            self = self.link
        return None

    def insert(self, value):
        ''' Given a new value, insert it at the current
        position in the list.

        :param value: The value to insert into the list
        :returns: The newly created node
        '''
        node = Node(value, self.link)
        self.link = node
        return node

    def append(self, value):
        ''' Given a new value, add it as the new last value in
        the list.

        :param value: The value to append onto the list
        :returns: The newly created node
        '''
        node = Node(value)
        last = self.last()
        last.link = node
        return node

    def last(self):
        ''' Return the node at the end of the list

        :returns: The last node in the list
        '''
        while self.link != None:
            self = self.link
        return self

    def next(self):
        ''' Return the next link in the list

        :returns: The next node in the list
        '''
        if self.link == None:
            raise StopIteration()
        return self.link

    def to_list(self):
        ''' Return the linked list as a python list.

        :returns: The linked list as a python list
        '''
        return list(self)

    def __iter__(self):
        ''' Return an iterator around the list at this
        current point.

        :returns: An iterator around the list
        '''
        while self != None:
            yield self
            self = self.link

    def __eq__(self, that): return (that != None) and (self.value == that.value)
    def __ne__(self, that): return (that == None)  or (self.value != that.value)
    def __hash__(self):     return hash(self.value)
    def __repr__(self):     return str(self.value)
    def __str__(self):      return str(self.value)
    def __repr__(self):     return str(self.value)


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


def find_list_middle(xs):
    ''' Given a linked list, find the middle point in one pass

    >>> xs = Node(1, Node(2, Node(3, Node(4, Node(5)))))
    >>> find_list_middle(xs)
    3
    '''
    mid, end = xs, xs
    for i in count(1):
        end = end.link
        if i % 2 == 0: mid = mid.link
        if end == None: break
    return mid


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
