import sys

class MinStack(object):
    ''' This works by keeping a seperate stack that
    records the current minimum. If a new minimum
    value is added, then it is pushed to the stack.
    '''

    def __init__(self):
        ''' Initialize a new instance of the MinStack
        '''
        self.stack = []
        self.small = []

    def push(self, value):
        ''' Push a new value to the stack.

        :param value: The value to push to the stack
        '''
        self.stack.append(value)
        if value <= self.minimum():
            self.small.append(value)

    def pop(self):
        ''' Pop the top value off the stack.

        :returns: The top value off the stack
        '''
        value = self.stack.pop()
        if value == self.minimum():
            self.small.pop()
        return value

    def minimum(self):
        ''' Get the current minimum value in the stack.

        :returns: The current minimum value in the stack or maxint
        '''
        return sys.maxint if not self.small else self.small[-1]


class StackSet(object):
    ''' This is a stack that has a number of sub stacks such
    that if a single stack gets too big, a new stack is
    created to handle the load based on some threshold.
    '''

    def __init__(self, threshold=5):
        ''' Initialize a new instance of the StackSet

        :param threshold: The splitting threshold
        '''
        self.stacks = []
        self.threshold = threshold

    def push(self, value):
        ''' Push a new value to the stack.

        :param value: The value to push to the stack
        '''
        if self.is_last_full():
            self.stacks.append([value])
        else: self.last_stack.append(value)

    def pop(self):
        ''' Pop the top value off the stack.

        :returns: The top value off the stack
        '''
        value = self.last_stack.pop()
        if not self.last_stack:
            self.stacks.pop()

        return value

    def pop_at(self, index):
        ''' Pop a value off of the requested stack
        index.

        :param index: The index of the stack to pop from
        :returns: The top value off that stack
        '''
        if index >= len(self.stacks):
            raise IndexError("Index out of stack range")

        value = self.stacks[index].pop()
        if not self.stacks[index]:
            self.stacks.pop(index)

        return value

    @property
    def last_stack(self):
        ''' Get a handle to the last stack if it exists

        :returns: A handle to the last stack
        '''
        if not self.stacks:
            raise IndexError("all stacks are empty")
        return self.stacks[-1]

    def is_last_full(self):
        ''' Check if the current stack is full

        :returns: True if the stack is full, False otherwise
        '''
        return (not self.stacks
             or len(self.stacks[-1]) >= self.threshold)

class StackQueue(object):
    ''' Implementation of a queue using two stacks. The
    general idea is to enqueue to one stack and dequeue
    from the other.
    '''

    def __init__(self):
        ''' Initialize a new instance of the StackQueue
        '''
        self.stack1 = []
        self.stack2 = []

    def enqueue(self, value):
        ''' Enqueue the supplied value to the queue.

        :param value: The value to enqueue
        '''
        self.stack1.append(value) # append == push

    def dequeue(self):
        ''' Dequeue the last value from the queue.

        :returns: The last value from the queue
        ''' 
        if not self.stack2:
            while self.stack1:
                self.stack2.append(self.stack1.pop())

        assert(len(self.stack2) > 0)
        return self.stack2.pop()

def stack_sort(stack):
    ''' Sort the supplied stack using only stack
    operations.

    :param stack: The stack to sort
    :returns: The sorted stack
    '''
    sort = [stack.pop()]
    while stack:
        temp = stack.pop()
        while temp > sort[-1]:
            stack.append(sort.pop())
        sort.append(temp)
    return sort

def towers_of_hanoi(size=5): # pragma: no cover
    ''' Play towers of hanoi for the given size of
    rings.

    :param size: The size of the rings to play with
    '''
    def transfer(n, src, dst, tmp):
        if n <= 0: return
        transfer(n - 1, src, tmp, dst)
        dst.append(src.pop())
        print("moved {} from {} to {} by {}".format(dst[-1], src, dst, tmp))
        transfer(n - 1, tmp, dst, src)

    s1, s2, s3 = range(1, size + 1), [], []
    transfer(size, s1, s2, s3)
