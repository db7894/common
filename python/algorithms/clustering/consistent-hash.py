import hashlib
import bisect


class ConsistentHash(object):
    ''' A simple implementation of a consistent hash
    manager of some node type.
    '''

    def __init__(self, replicas=3, hasher=None):
        ''' Initialize a new instance of the circle

        :param replicas: The number of virtual replicas to use
        :param hasher: The hash method to use
        '''
        self.index = list()
        self.nodes = dict()
        self.replicas = max(1, replicas)
        self.hasher = hasher or (lambda x: hashlib.sha256(x).digest())

    def add_node(self, node):
        ''' Given a node, add it to the circle

        :param node: The node to add to the circle
        '''
        for i in range(self.replicas):
            key = self.hasher("{}:{}".format(node, i))
            self.nodes[key] = node
            bisect.insort(self.index, key)

    def del_node(self, node):
        ''' Given a node, deleted it from the circle

        :param node: The node to remove from the circle
        '''
        for i in range(self.replicas):
            key = self.hasher("{}:{}".format(node, i))
            del self.nodex[key]
            self.index.remove(key)

    def get_node(self, key):
        ''' Given a key, return the node this key belongs to

        :param key: The key to find the best node for
        :returns: The node the key should attatch to or None
        '''
        if len(self.index) == 0:
            return None

        key = self.hasher(key)
        if key in self.nodes:
            return self.nodes[key]

        end = bisect.bisect_right(self.index, key)
        end = 0 if end >= len(self.index) else end
        return self.nodes[self.index[end]]


# ------------------------------------------------------------
# load testing example
# ------------------------------------------------------------
if __name__ == "__main__":
    from collections import defaultdict
    import os

    s = 100000
    m = ConsistentHash()
    for i in range(10):
        m.add_node("cache{}.host.com".format(i))

    hits = defaultdict(int)
    for i in range(s):
        hits[m.get_node(os.urandom(15))] += 1
    for k,v in hits.items():
        print "{}\t{}".format(k, v * 1.0 / s)
