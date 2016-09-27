import random

class Entry(object):

    def __init__(self, key, value, link=None):
        self.key   = key
        self.value = value
        self.link  = link

class HashTable(object):

    def __init__(self, size=10):
        self.factor  = min(1, size)
        self.length  = 0
        self.buckets = [None for i in range(size)]

    def get(self, key):
        if not key: raise KeyError("invalid key")
        idx = hash(key) % self.factor
        bucket = self.buckets[idx]
        while (bucket != None) and (bucket.key != key):
            bucket = bucket.link
        return bucket.value if bucket else bucket

    def put(self, key, value):
        if not key: raise KeyError("invalid key")
        idx = hash(key) % self.factor
        if not self.buckets[idx]:
            self.buckets[idx] = Entry(key, value)
            self.length += 1
            return

        prev, bucket = None, self.buckets[idx]
        while bucket:
            if bucket.key == key:
                bucket.value = value
                return
            prev, bucket = bucket, bucket.link
        prev.link = Entry(key, value)
        self.length += 1
    
    def remove(self, key):
        if not key: raise KeyError("invalid key")
        idx = hash(key) % self.factor
        if not self.buckets[idx]: return

        prev, bucket = None, self.buckets[idx]
        while bucket:
            if bucket.key == key:
                if not prev:
                    self.buckets[idx] = None
                else: prev.link = bucket.link
                self.length -= 1
            prev, bucket = bucket, bucket.link

    def empty(self):
        return (self.length == 0)

    def __len__(self):
        return self.length

class RandomHashTable(object):
    ''' Implement a hash table with methods get, put, remove, and
    a new method random which will return a uniformally random
    value from the hash table all in amortized constant time.
    '''

    def __init__(self):
        self.lookup = dict()                 # key   -> values[index]
        self.values = []                     # index -> (key, value)

    def get(self, key):
        index = self.lookup[key]             # get the location of the value
        return self.values[index][1]         # return the underlying value

    def put(self, key, value):
        self.lookup[key] = len(self.values)  # store our index location
        self.values.append((key, value))     # always add to the end of the values

    def remove(self, key):
        index  = self.lookup.pop(key)        # remove index of old value
        _, value = self.values[index]        # get value for removed key
        self.values[index] = self.values[-1] # fill in slot with last value
        key, _ = self.values.pop()           # remove duplicate last value in list
        self.lookup[key] = index             # populate the lookup index
        return value                         # return removed value

    def random(self):
        index = random.randint(0, len(self.values) - 1) # uniformly choose an entry
        return self.values[index][1]         # return the underlying value

    def __str__(self): return str(self.values)
    def __iter__(self): return iter(self.values)
    def __len__(self): return len(self.values)
    def __contains__(self, key): return key in self.lookup

    __delitem__ = remove
    __setitem__ = put
    __getitem__ = get
    __repr__ = __str__
    __unicode__ = __str__

