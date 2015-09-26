import sys
from itertools import izip_longest, count

class Node(object):
    ''' A simple linked list node element.
    '''

    @classmethod
    def create(klass, *values):
        ''' Given an iterable of values, convert them
        to a linked list collection.

        :param values: The values to convert to a linked list
        :returns: The head of the new list
        '''
        if isinstance(values[0], list):
            vals = iter(values[0])
        else: vals = iter(values)

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
            yield self.value
            self = self.link

    def __eq__(self, that): return (that != None) and (self.value == that.value)
    def __ne__(self, that): return (that == None)  or (self.value != that.value)
    def __hash__(self):     return hash(self.value)
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


def find_from_list_end(xs, n=3):
    ''' Given a linked list, find the nth to last
    element in the list.

    :param xs: The list to search in
    :param n: Which node from the end of the list to get
    :returns: The requested list node
    '''
    prev, curr = xs, xs
    for _ in range(n):
        curr = curr.link

    while curr != None:
        curr = curr.link
        prev = prev.link
    return prev


def find_list_middle(xs):
    ''' Given a linked list, find the middle point in one pass

    :param xs: The list to process
    :returns: The middle of the list
    '''
    mid, end = xs, xs
    for i in count(1):
        end = end.link
        if i % 2 == 0: mid = mid.link
        if end == None: break
    return mid


def reverse_list(xs):
    ''' Given a linked list, reverse it.

    :param xs: The list to reverse
    :returns: The new head pointer
    '''
    last, head = xs, None
    while last:
        temp = last.link
        last.link = head
        last, head = temp, last
    return head


def reverse_list_recursive(xs):
    ''' Given a linked list, reverse it

    :param xs: The list to reverse
    :returns: The new head pointer
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

def find_merge_point_pointers(xs, ys):
    ''' Given two linked lists, find the node
    at which they merge. The idea here is to use
    the chasing pointers.
    
    If they meet, that means that the fast pointer has
    a head start of K.  If we reset it to the start and
    have both pointers walk at the same speed, they will
    meet at the loop point (K steps).

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

    :param head: The head of the list
    :returns: A list with only unique values
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

def make_unique_list_no_storage(head):
    ''' Given a linked list, return the list
    as a unique list and also return the total
    size of the list as well as the number of
    duplicates

    :param head: The head of the list
    :returns: A list with only unique values
    '''
    curr = head
    while curr != None:
        prev, step = curr, curr.link
        while step != None:
            if step.value == curr.value:
                prev.link = step.link
                step = prev.link
            else:
                prev = step
                step = step.link
        curr = curr.link
    return head

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


def delete_middle_node(node):
    ''' Given a random middle node in a linked list, delete
    that node without a reference to the head.

    :param node: The node to delete
    :returns: The new middle node
    '''
    node.value = node.link.value
    node.link  = node.link.link
    return node

def sum_integer_nodes(number1, number2):
    ''' Given two linked lists where each node
    represents a number in big endian, add the
    two numbers (with carry) and produce the result
    as a new list.

    :param number1: The first number to add
    :param number2: The second number to add
    :returns: The resulting number as a list
    '''
    def recurse(xs, ys):
        if not xs and not ys:
            return (0, None)

        carry, leaf = recurse(xs.link, ys.link)
        value = xs.value + ys.value + carry
        return (value / 10, Node(value % 10, leaf))

    (carry, head) = recurse(number1, number2)
    if carry > 0: head = Node(carry, head)
    return head

def sum_integer_nodes_little_endian(number1, number2):
    ''' Given two linked lists where each node
    represents a number in little endian, add the
    two numbers (with carry) and produce the result
    as a new list.

    :param number1: The first number to add
    :param number2: The second number to add
    :returns: The resulting number as a list
    '''
    def recurse(xs, ys, carry=0):
        if not xs and not ys:
            return Node(carry) if carry > 0 else None

        total = carry
        total += xs.value if xs else 0
        total += ys.value if ys else 0
        carry = total / 10
        node = Node(total % 10)
        node.link = recurse(xs and xs.link, ys and ys.link, carry)
        return node

    return recurse(number1, number2)

if __name__ == "__main__":
    import doctest
    doctest.testmod()
