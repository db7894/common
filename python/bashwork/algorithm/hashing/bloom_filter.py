'''
implement hashes
http://burtleburtle.net/bob/hash/doobs.html
'''
class AbstractBloomFilter(object):
    '''
    Best choice of hash count is 4 for small differences
    and 3 for larger sizes with the crossover around 128.

    Keep an in memory heap of pure/empty buckets which
    gets updated on the fly as bucket counts are updated
    during unraveling.
    '''
    def __init__(self, **kwargs):
        self.hashers = []

    def get_hash_keys(self, value):
        '''
        '''
        for hasher in self.hashers:
            yield hasher

class BloomFilter(AbstractBloomFilter):

    def insert(self, value):
        '''
        '''
        for bit in self.get_hash_keys(value):
            self.bits[bit] = 0x1

    def contains(self, value):
        '''
        '''
        return all(self.bits[bit] for bit in self.get_hash_keys(value))


    def remove(self, value):
        ''' Removes the bits specified by the
        given input.

        ..note:: This is not a safe operation!

        :param value: The value to remove from the table
        '''
        for bit in self.get_hashes(value):
            self.bits[bit] &= bit

class CountingBloomFilter(AbstractBloomFilter):

    def insert(self, value):
        '''
        '''
        for bit in self.get_hash_keys(value):
            self.bits[bit] += 1

    def remove(self, value):
        ''' Removes the bits specified by the
        given input.

        ..note:: This is not a safe operation!

        :param value: The value to remove from the table
        '''
        for bit in self.get_hashes(value):
            self.bits[bit] -= 1

class InvertibleBloomFilter(AbstractBloomFilter):

    # [(count, key_sum, value_sum)] -> word per field

    def get(self, key):
        pass

    def list(self):
        pass

    def insert(self, value):
        '''
        '''
        for bit in self.get_hash_keys(value):
            self.bits[bit] += 1

    def remove(self, value):
        ''' Removes the bits specified by the
        given input.

        ..note:: This is not a safe operation!

        :param value: The value to remove from the table
        '''
        for bit in self.get_hashes(value):
            self.bits[bit] -= 1

# cardinality estimation based on 0s like bitcoin
# send bloom and cardinality
# service returns diff against their bloom filter