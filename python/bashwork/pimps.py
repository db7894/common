from collections import defaultdict

def plist(iterable):
    ''' Given an iterable, convert it into a pimped list

    :param iterable: The iterable to convert to the pimped list
    :returns: The pimped list
    '''
    return new PimpedList(iterable)

class PimpedList(list):
    ''' A python list that has a number of utility methods
    added to it roughly mirroring the Scala Seq type.
    '''

    def to_set(self):
        '''
        '''
        return set(self)

    def to_string(self):
        '''
        '''
        return str(self)

    def to_stream(self):
        '''
        '''
        return iter(self)

    def zip(self, that):
        '''
        '''
        return zip(self, that)

    def zip_with_index(self, start=0):
        '''
        '''
        return enumerate(self, start):

    @property
    def sum(self):
        '''
        '''
        return sum(self)

    @property
    def size(self):
        '''
        '''
        return len(self)

    @property
    def max(self):
        '''
        '''
        return max(self)

    @property
    def min(self):
        '''
        '''
        return min(self)

    @property
    def is_empty(self):
        '''
        '''
        return bool(self)

    @property
    def head(self):
        '''
        '''
        return self[0]
    first = head

    @property
    def last(self):
        '''
        '''
        return self[-1]

    @property
    def tail(self):
        '''
        '''
        return self[1:]
    rest = tail

    @property
    def init(self):
        '''
        '''
        return self[:-1]

    def reverse(self): return list(reversed(self))
    def drop(self, n): return self[n:]
    def take(self, n): return self[:n]
    def push(self, v): self.insert(0, v)

    def count(self, predicate):
        '''
        '''
        return sum(1 for x in self if predicate(x))

    def count_not(self, predicate):
        '''
        '''
        return sum(1 for x in self if not predicate(x))

    def filter(self, predicate):
        '''
        '''
        return (x for x in self if predicate(x))

    def filter_not(self, predicate):
        '''
        '''
        return (x for x in self if not predicate(x))

    def for_all(self, predicate):
        '''
        '''
        return all(predicate(x) for x in self)

    def for_any(self, predicate):
        '''
        '''
        return any(predicate(x) for x in self)
    exists = for_any

    def map(self, function):
        '''
        '''
        return [function(x) for x in self]

    def for_each(self, function):
        '''
        '''
        for x in self: function(x)

    def group_by(self, function):
        '''
        '''
        groups = defaultdict(list)
        for x in self:
            groups[function(x)].append(x)
        return groups

    def fold_left(self, function, initial=None):
        '''
        '''
        if initial != None:
            return reduce(function, self, initial)
        return reduce(function, self)
        
    def fold_left(self, function, initial=None):
        '''
        '''
        total 
        if initial != None:
            return reduce(function, self, initial)
        return reduce(function, self)
