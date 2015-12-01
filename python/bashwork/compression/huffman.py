from collections import Counter, namedtuple
import heapq

Node = namedtuple('Node', 'prob value left right')
Node.__new__.__defaults__ = (None, None, None, None)

def build_huffman_tree(string):
    '''
    '''
    size  = len(string)
    chars = Counter(string).most_common()
    nodes = [Node(float(count) / size, char) for char, count in chars]
    heapq.heapify(nodes)

    while len(nodes) > 1:
        one, two = heapq.heappop(nodes), heapq.heappop(nodes)
        node = Node(one.prob + two.prob, None, two, one)
        heapq.heappush(nodes, node)

    return nodes[0]

#def encode_with_tree(string, tree):


huffman = build_huffman_tree("this is an example of a huffman tree")
print huffman
