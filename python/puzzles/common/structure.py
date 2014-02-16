#!/usr/bin/env python
# -*- coding: latin-1 -*-

class Structure(object):
    ''' A simple base class around various data structures
    '''
    def enqueue(self, value):
        ''' Adds the supplied value to the structure

        :param value: The value to add to the structure
        '''
        raise NotImplemented("enqueue")
        
    def dequeue(self):
        ''' Removes and returns the next value in the structure

        :returns: The next value in the structure
        '''
        raise NotImplemented("dequeue")

    def is_empty(self): return len(self.store) == 0
    def __eq__(self, other):   return other and (other.value == self.store)
    def __ne__(self, other):   return other and (other.value != self.store)
    def __lt__(self, other):   return other and (other.value  < self.store)
    def __le__(self, other):   return other and (other.value <= self.store)
    def __gt__(self, other):   return other and (other.value  > self.store)
    def __ge__(self, other):   return other and (other.value >= self.store)
    def __repr__(self):        return str(self.store)
    def __str__(self):         return str(self.store)
    def __contains__(self, v): return v in self.store

class Stack(Structure):
    ''' A simple wrapper around a python list ot
    implement stack properties.
    '''

    def __init__(self, initial=None): self.store = initial or []
    def enqueue(self, value): self.store.append(value)
    def dequeue(self): return self.store.pop()

class Queue(Structure):
    ''' A simple wrapper around a python list ot
    implement queue properties.
    '''

    def __init__(self, initial=None): self.store = initial or []
    def enqueue(self, value): self.store.insert(0, value)
    def dequeue(self): return self.store.pop()

class PriorityQueue(Structure):
    ''' A simple wrapper around a python heap
    '''

    def  __init__(self, function, initial=None):
        '''
        :param function: The function used to generate priority
        :param initial: The initial value of the structure
        '''
        self.store = initial  or []
        self.order = function or (lambda value: 1)

    def enqueue(self, value): heapq.heappush(self.store, (self.order(value), value))
    def dequeue(self): return heapq.heappop(self.store)[1]
