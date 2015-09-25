import sys

class Vector(object):
    ''' A simple dynamicly size vector or array '''

    @classmethod
    def create(klass, values):
        ''' Given an iterable of values, convert them
        to a linked list collection.

        :param values: The values to convert to a linked list
        :returns: The head of the new list
        '''
        vals = iter(values)
        head = klass()
        node = head

        for val in vals:
            node = node.append(val)
        return head

    def __init__(self, size=8):
        ''' Create a new instance of a vector

        :param size: The initial size of the vector
        '''
        self.clear(size)

    def is_empty(self):
        ''' Returns if the vector is currently empty

        :returns: True if empty, False otherwise
        '''
        return not bool(self.tail)

    def clear(self, size=8):
        ''' Clears the entire vector of all values
        '''
        self.data = [None] * size
        self.tail = size
        self.size = size

    def find(self, value):
        ''' Given a value, attempt to find the node
        containing that value.

        :param value: The value to find in the vector.
        :returns: The found node or None if it is missing
        '''
        for index in range(0, self.tail):
            if self.data[index] == value:
                return index
        return None

    def contains(self, value):
        ''' Given a value, check if the value exists in the
        vector.

        :param value: The value to find in the vector.
        :returns: True if the value exists, False otherwise.
        '''
        return bool(self.find(value))

    def get(self, index):
        ''' Retrieve the value at the supplied index.

        :param index: The index to retrieve the value at.
        :returns: The value at that index.
        '''
        self.check_index(index)
        return self.data[index]

    def remove(self, index):
        ''' Given an index, remove it from the vector.

        :param index: The value to remove from the vector
        :returns: The removed value
        '''
        self.check_index(index)
        self.data = self.data[:index] + self.data[index:] + [None]
        self.tail -= 1

    def set(self, index, value):
        ''' Given a new value, set it at the current
        position in the vector.

        :param index: The index to set the value at.
        :param value: The value to insert into the vector.
        :returns: The newly created node
        '''
        self.check_index(index)
        self.data[index] = value

    def check_index(self, index):
        ''' Given an index, check if it is a valid index
        into the current data store.

        :param index: The index to check in the data store
        :throws: Exception if the index is illegal
        '''
        if not 0 <= index < self.tail:
            raise Exception("index %d out of bounds 0..%d" % (index, self.tail))

    def grow(self):
        ''' Grow the underlying data store by the next
        power of 2.
        '''
        self.data = self.data + [None] * self.size
        self.size = self.size * 2

    def check_and_grow(self):
        ''' Check if we need to grow the data store before
        adding a new element.
        '''
        if self.tail == self.size: self.grow()

    def append_all(self, values):
        ''' Given a collection of values, append them all to
        the vector.

        :param values: The values to append onto the vector
        '''
        for value in values:
            self.append(value)

    def append(self, value):
        ''' Given a new value, add it as the new last value in
        the vector.

        :param value: The value to append onto the vector
        '''
        self.check_and_grow()
        self.data[self.tail] = value
        self.tail += 1

    def prepend_all(self, values):
        ''' Given a collection of values, prepend them all to
        the vector.

        :param values: The values to append onto the vector
        '''
        for value in values:
            self.prepend(value)

    def prepend(self, value):
        ''' Given a new value, add it as the new first value in
        the vector.

        :param value: The value to prepend to the vector
        '''
        self.check_and_grow()
        self.data = [value] + self.data[:self.size - 1]
        self.tail += 1

    @property
    def first(self):
        ''' Return the first element in the vector

        :returns: The first element in the vector
        '''
        return self.data[0]

    @property
    def last(self):
        ''' Return the last element in the vector

        :returns: The last element in the vector
        '''
        return self.data[self.tail - 1]

    def to_list(self):
        ''' Return the vector as a python list.

        :returns: The vector as a python list
        '''
        return list(self)

    def __iter__(self):
        ''' Return an iterator around the vector.

        :returns: An iterator around the vector.
        '''
        return iter(self.data[:self.tail])

    def __repr__(self):          return str(self.data)
    def __str__(self):           return str(self.data)
    def __len__(self):           return self.tail
    def __add__(self, v):        return self.append(value)
    def __eq__(self, that):      return (that != None) and (self.data == that.data)
    def __nonzero__(self):       return bool(self.tail)
    def __getitem__(self, i):    return self.get(i)
    def __setitem__(self, i, v): return self.set(v, i)
    def __delitem__(self, i):    return self.remove(i)
    def __contains__(self, v):   return self.contains(v)
    #def __reversed__(self):      return reversed(iter(self))
