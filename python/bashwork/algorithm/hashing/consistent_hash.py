'''
Consistent Hash Circle
------------------------------------------------------------

This is a simple implementation of David Karger's idea of hashing
to an imagined circle in order to spread a key distribution and to
add and remove nodes without affecting existing clients. In order
for this to happen, both the client and server keys are hashed
modulous a fixed size and the correct node is the closest match on the
client side. To prevent hot spots, "virtual" locations can be added
on the circle by hashing again with a replication count.

The choice of the partition key is particularlly important so that
hot spots are not created. For example, say a user has a marketplace
id, a customer id, and an application id. If the partition was just
on marketplace id, then the US node would be hot. If the key was just
the application id, then the most popular mobile phone node would be
hot. It makes sense to blend these keys into composite keys so that
there is a good mixture in the circle::

    circle = ConsistentHash(replicas=10)
    for node in nodes:
        circle.add_node(node)

    key  = "{}:{}:{}".format(mid, aid, cid)
    node = circle.get_node(key)
    return node
'''
from hashlib import sha256
from bisect import bisect_right, insort_right


class ConsistentHash(object):
    ''' A simple implementation of a consistent hash
    manager of some node type.
    '''

    def __init__(self, replicas=3, hasher=None):
        ''' Initialize a new instance of the circle

        :param replicas: The number of virtual replicas to use
        :param hasher: The hash method to use
        '''
        self.circle   = list() # sorted hash keys
        self.mapping  = dict() # hash-key => node
        self.replicas = max(1, replicas)
        self.hasher   = hasher or (lambda x: sha256(x).digest())

    def add_node(self, node):
        ''' Given a node, add it to the circle

        :param node: The node to add to the circle
        '''
        for idx in range(self.replicas):
            key = self.hasher("{}:{}".format(node, idx))
            self.mapping[key] = node
            insort_right(self.circle, key)

    def del_node(self, node):
        ''' Given a node, deleted it from the circle

        :param node: The node to remove from the circle
        '''
        for idx in range(self.replicas):
            key = self.hasher("{}:{}".format(node, idx))
            if key in self.mapping:
                del self.mapping[key]
                self.circle.remove(key)

    def get_node(self, key):
        ''' Given a key, return the node this key belongs to

        :param key: The key to find the best node for
        :returns: The node the key should attatch to or None
        '''
        if len(self.circle) == 0:
            return None

        key = self.hasher(key)
        if key in self.mapping:
            return self.mapping[key]

        end = bisect_right(self.circle, key)
        end = 0 if end >= len(self.circle) else end
        return self.mapping[self.circle[end]]


# ------------------------------------------------------------
# load testing example
# ------------------------------------------------------------
if __name__ == "__main__":
    from collections import defaultdict
    from os import urandom

    c = 10
    s = 100000
    m = ConsistentHash(replicas=5)
    for i in range(c):
        m.add_node("cache{}.host.com".format(i))

    hits = defaultdict(int)
    for i in range(s):
        hits[m.get_node(urandom(15))] += 1
    hits = { k : v/float(s) for k,v in hits.items() }
    for k,v in hits.items():
        print "{}\t{}".format(k, v)

    mean = sum(hits.values()) / c
    vary = sum((v - mean)**2 for v in hits.values()) / c 
    stdv = vary**.5

    print "-" * 30
    print "mean\t\t{}".format(mean)
    print "variance\t{}".format(vary)
    print "std-deviation\t{}".format(stdv)
