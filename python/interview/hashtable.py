class Node(object):

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
            self.buckets[idx] = Node(key, value)
            self.length += 1
            return

        prev, bucket = None, self.buckets[idx]
        while bucket:
            if bucket.key == key:
                bucket.value = value
                return
            prev, bucket = bucket, bucket.link
        prev.link = Node(key, value)
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
