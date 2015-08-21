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

class QueueStack(object):
    '''
    >>> q = QueueStack()
    >>> assert(q.enqueue(3) == None)
    >>> assert(q.enqueue(4) == None)
    >>> assert(q.enqueue(2) == None)
    >>> assert(q.dequeue()  == 3)     
    >>> assert(q.dequeue()  == 4)     
    >>> assert(q.enqueue(1) == None)
    >>> assert(q.dequeue()  == 2)     
    >>> assert(q.dequeue()  == 1)     
    '''

    def __init__(self):
        self.stack1 = []
        self.stack2 = []

    def enqueue(self, value):
        self.stack1.append(value) # append == push

    def dequeue(self):
        if not self.stack2:
            while self.stack1:
                self.stack2.append(self.stack1.pop())

        assert(len(self.stack2) > 0)
        return self.stack2.pop()
