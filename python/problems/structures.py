import sys

class MinStack(object):
    '''
    >>> s = MinStack()
    >>> assert(s.push(3) == None); assert(s.minimum() == 3)
    >>> assert(s.push(4) == None); assert(s.minimum() == 3)
    >>> assert(s.push(2) == None); assert(s.minimum() == 2)
    >>> assert(s.push(1) == None); assert(s.minimum() == 1)
    >>> assert(s.pop()   == 1);    assert(s.minimum() == 2)
    >>> assert(s.pop()   == 2);    assert(s.minimum() == 3)
    >>> assert(s.push(0) == None); assert(s.minimum() == 0)
    '''

    def __init__(self):
        self.stack = []
        self.small = []

    def push(self, value):
        self.stack.append(value)
        self.small.append(min(self.minimum(), value))

    def pop(self):
        self.small.pop()
        return self.stack.pop()

    def minimum(self):
        return sys.maxint if not self.small else self.small[-1]
