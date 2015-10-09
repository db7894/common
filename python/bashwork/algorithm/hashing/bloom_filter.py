class AbstractBloomFilter(object):
    '''
    Best choice of hash count is 4 for small differences
    and 3 for larger sizes with the crossover around 128.

    Keep an in memory heap of pure/empty buckets which
    gets updated on the fly as bucket counts are updated
    during unraveling.
    '''

    def __init__(self, **kwargs):
        ''' Initialize the bloom filter instance.

        :param hashers: The collection of hashers to hash with
        '''
        self.hashers = kwargs.get('hashers', [])
        self.keys    = []

    def get_hash_keys(self, value):
        ''' Given a new value, return the resulting hash
        codes for that value.

        :param value: The value to get hash keys for
        :returns: The N hash keys for that value
        '''
        for hasher in self.hashers:
            yield hasher.hash(value)

class BloomFilter(AbstractBloomFilter):

    def insert(self, value):
        '''
        '''
        for key in self.get_hash_keys(value):
            self.keys[key] = 0x1

    def contains(self, value):
        '''
        '''
        return all(self.keys[key] for key in self.get_hash_keys(value))


    def remove(self, value):
        ''' Removes the bits specified by the
        given input.

        ..note:: This is not a safe operation!

        :param value: The value to remove from the table
        '''
        for key in self.get_hashes(value):
            self.keys[key] &= key

class CountingBloomFilter(AbstractBloomFilter):

    def insert(self, value):
        '''
        '''
        for key in self.get_hash_keys(value):
            self.keys[key] += 1

    def remove(self, value):
        ''' Removes the bits specified by the
        given input.

        ..note:: This is not a safe operation!

        :param value: The value to remove from the table
        '''
        for key in self.get_hash_keys(value):
            self.keys[key] -= 1

class InvertibleBloomFilter(AbstractBloomFilter):

    # [(count, key_sum, value_sum)] -> word per field

    def get(self, key):
        pass

    def list(self):
        pass

    def insert(self, value):
        '''
        '''
        for key in self.get_hash_keys(value):
            self.keys[key] += 1

    def remove(self, value):
        ''' Removes the bits specified by the
        given input.

        ..note:: This is not a safe operation!

        :param value: The value to remove from the table
        '''
        for key in self.get_hashes(value):
            self.keys[key] -= 1

# cardinality estimation based on 0s like bitcoin
# send bloom and cardinality
# service returns diff against their bloom filter
