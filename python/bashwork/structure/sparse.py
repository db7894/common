import math
from bisect import bisect_left

class SparseArray(object):
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
