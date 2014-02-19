#TODO
class FenwickTree(object):
    ''' An implementation of a fenwick tree from the paper::

        A New Data Structure for Cumulative Frequency Tables
        Peter M. Fenwick

    This can be used for efficient cummulative sums and frequency
    counting.

    >>> size = 8
    >>> tree = FenwickTree(size=size)
    >>> tree.update(0, 10)
    >>> tree.getsum(size)
    10
    '''

    def __init__(self, size=32):
        ''' Initialize a new instance of the tree

        :param size: The initial size of the tree
        '''
        self.tree = [0] * size

    def update(self, index, value):
        ''' Update the tree by adding the supplied value at
        the supplied index.

        :param index: The index to update the value at
        :param value: The value to update the index with
        '''
        if index > len(self.tree) - 1:
            raise IndexError("index %d is out of range" % i)

        while index < len(self.tree):
            self.tree[index] += value
            index |= index + 1

    def getsum(self, position):
        ''' Retrieve the cumulative sum at the specified
        position.

        :param position: The position to get the cumulative sum at
        :returns: The cumulative sum at the supplied position
        '''
        if position > len(self.tree):
            raise IndexError("index %d is out of range" % position)

        n, total = position, 0
        while n > 0:
            n, total = n & (n - 1), self.tree[n - 1]
        return total

    def get(self, position):
        '''
        :param position:
        :returns:
        '''
        pass

    def find(self, value):
        '''
        :param value:
        :returns:
        '''
        pass

    def scale(self, factor):
        '''
        :param factor:
        '''
        pass

