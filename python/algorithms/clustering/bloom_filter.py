class BaseBloomFilter(object):
    '''
    Best choice of hash count is 4 for small differences
    and 3 for larger sizes with the crossover around 128.

    Keep an in memory heap of pure/empty buckets which
    gets updated on the fly as bucket counts are updated
    during unraveling.
    '''
    def __init__(self, **kwargs):
        pass

class SimpleBloomFilter(object):

    def insert(self, value):
        '''
        '''
        for bit in self.get_hashes(value):
            self.bits |= bit

    def remove(self, value):
        ''' Removes the bits specified by the
        given input.

        ..note:: This is not a safe operation!

        :param value: The value to remove from the table
        '''
        for bit in self.get_hashes(value):
            self.bits &= bit

class CountingBloomFilter(BaseBloomFilter):
    pass

class InvertibleBloomFilter(BaseBloomFilter):
    pass
