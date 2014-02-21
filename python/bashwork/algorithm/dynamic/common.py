from collections import defaultdict

def get_dictionary(path='/usr/share/dict/words'):
    ''' Given a wordlist path, return a dictionary

    :param path: The path to the wordlist
    :returns: The initialized dictionary map
    '''
    with open(path, 'r') as handle:
       return {w.strip():1 for w in handle}

def get_anagram_dictionary(path='/usr/share/dict/words'):
    ''' Given a wordlist path, return a listing of anagrams

    :param path: The path to the wordlist
    :returns: The initialized anagram map {anagram -> [words]}
    '''
    anagrams = defaultdict(list)
    with open(path, 'r') as handle:
        for word in handle:
            anagrams[sorted(word)].append(word)
    return anagrams

class Graph(object):
    ''' A simple graph implementation '''

    def __init__(self, head):
        self.head  = head
        self._nodes = set([head])
        self._edges = defaultdict(dict)

    def nodes(self):
        return iter(self._nodes)

    def edge(self, a, b):
        return self._edges[a][b] 

    def siblings(self, a):
        return self._edges[a].keys()

    def add_edge(self, a, b, weight):
       self._nodes.add(a) 
       self._nodes.add(b) 
       self._edges[a][b] = weight


class Queue(object):
    ''' a simple queue '''
    def __init__(self): self.value = []
    def enqueue(self, value): self.value.insert(0, value)
    def dequeue(self): return self.value.pop()
    def is_empty(self): return not any(self.value)
