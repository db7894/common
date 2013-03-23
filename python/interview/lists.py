class Node(object):
    ''' A simple linked list node element.
    '''

    def __init__(self, value, link=None):
        self.value = value
        self.link  = link

    def __repr__(self):
        return "{}->{}".format(self.value, str(self.link))
    __str__ = __repr__

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
    >>> merge_sorted_linked_lists(xs, ys)
    1->2->3->4->5->6->None
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
    >>> reverse_list(xs)
    5->4->3->2->1->None
    '''
    last, head = xs, None
    while last:
        temp = last.link
        last.link = head
        last, head = temp, last
    return head

def make_unique_list(head):
    ''' Given a linked list, return the list
    as a unique list and also return the total
    size of the list as well as the number of
    duplicates

    >>> xs = Node(1, Node(2, Node(1, Node(3, Node(4, Node(5, Node(3)))))))
    >>> make_unique_list(xs)
    (1->2->3->4->5->None, 5, 2)
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

if __name__ == "__main__":
    import doctest
    doctest.testmod()
