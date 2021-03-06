'''
Sparse datastructures are generally of two camps:

1. those that support efficient modification
2. those that support efficient operations

The common methods of building / creating sparse structures are:

* dictionary of keys (DOK)
* list of lists (LIL)
* coordinate list (COO)

When the sparse structures need to be operated on, the
following are better representations:

* compresses sparse row (CSR)
* compresses sparse column (CSC)
* Yale format
'''
import math
from bisect import bisect_left

class SparseVector(object):
    '''
    '''

    def __init__(self, default=None):
        '''
        :param default: The default value to use for missing keys
        '''
        self.values  = []
        self.index   = []
        self.default = default

    def __setitem__(self, key, value):
        idx = bisect_left(self.index, key)
        self.index.insert(idx, key)
        self.values.insert(idx, value)

    def __getitem__(self, key):
        idx = bisect_left(self.index, key)
        if idx != len(self.index) and self.index[idx] == key:
            return self.values[idx]
        return self.default

    def __iter__(self):
        pre, idx = -1, 0
        while idx < len(self.index):
            while pre < self.index[idx] - 1:
                yield self.default
                pre += 1
            yield self.values[idx]
            idx, pre = idx + 1, self.index[idx]

    iter_dense = __iter__

    def iter_sparse(self):
        return zip(self.index, self.values)

class SparseMatrix(object):
    '''
    .. todo:: complete this
    '''

    def __init__(self, default=None):
        '''
        :param default: The default value to use for missing keys
        '''
        self.values   = []
        self.idx_rows = []
        self.idx_cows = []
        self.default = default

    def __setitem__(self, key, value):
        pass

    def __getitem__(self, key):
        pass

    def __iter__(self):
        pass


def distance(this, that):
    ''' Compute the distance between two sparse arrays.
    math.sqrt(sum(math.pow(x - y, 2) for x, y in zip(this, that)))


    .. todo:: implement this

    :param this: The left array to compute with
    :param that: The right array to compute with
    :returns: The distance between the two arrays
    '''
    total  = 0
    lx, rx = 0, 0

    return math.sqrt(total)
