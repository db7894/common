import math
import sys
import random
from bashwork.security.hashing import Hashing

class CountMinSketch(object):
    ''' A simple count min sketch implementation
    based on: http://en.wikipedia.org/wiki/Count-Min_sketch
    '''

    @classmethod
    def create(klass, epsilon, delta):
        ''' Create a new instance of the CountMinSketch class
        with the supplied epsilon and delta tuning parameters

        :param epsilon: A tuning parameter to control the precision
        :param delta: A tuning parameter to control the precision
        :returns: An initialized CountMinSketch instance
        '''
        assert (0 < epsilon < 1)
        assert (0 < delta   < 1)

        depth = int(math.log(1.0 / delta) + 0.5)
        width = int((math.e / epsilon) + 0.5)
        return klass(width, depth)

    def __init__(self, width, depth):
        ''' Initialize a new instance of the CountMinSketch class

        :param weight:
        :param depth:
        '''
        self.width   = width
        self.depth   = depth
        self.count   = [[0] * width for _ in range(depth)]
        self.hashers = [self.get_hash_method(seed) for seed in range(depth)]

    def update(self, value, count=1):
        ''' Update the cardinality estimate of the supplied value
        by the supplied count.

        :param value: The new value to update with
        :param count: The count to update the value with (default 1)
        '''
        for index, hasher in enumerate(self.hashers):
            hashed = hasher(value) % self.width
            self.count[index][hashed] += count

    def query(self, value):
        '''
        :param value: The value to get the cardinality of
        :returns: The estimated cardinality of the value
        '''
        estimate = sys.maxint
        for index, hasher in enumerate(self.hashers):
            hashed = hasher(value) % self.width
            estimate = min(self.count[index][hashed], estimate)
        return estimate

    def get_hash_method(self, index):
        ''' Supply the CountMinSketch with the hash function at
        the supplied index. This can be overloaded to supply different
        hash functions to tune the system.

        :param index: The index of the hash function to retrieve
        :returns: The hash function at the supplied index
        '''
        hasher = Hashing.murmur3_32()
        hasher.seed = index
        return hasher


class PairWiseCountMinSketch(CountMinSketch):
    ''' An implementation of the count min sketch algorithm
    using N pair-wise independent hashing functions as
    shown in: http://en.wikipedia.org/wiki/Universal_hashing
    '''

    BIG_PRIME = 9223372036854775783

    def get_hash_method(self, index):
        ''' Returns a hash function from a family of
        pairwise-independent hash functions
        '''
        a = random.randrange(0, self.BIG_PRIME - 1)
        b = random.randrange(0, self.BIG_PRIME - 1)
        return lambda v: (a * v + b) % self.BIG_PRIME
