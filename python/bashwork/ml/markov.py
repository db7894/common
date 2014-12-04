import sys
import random
from collections import defaultdict

def recursive_dict(default, level=1):
    ''' Create a two level defaultdict where
    the second level defaults to the supplied value::

        cache = recursive_dict(default=list, level=2)
        print cache['service']['hosts']

    :param default: The final default value to return
    :param level: The number of levels to recurse
    :returns: The default dict mapper
    '''
    def recurse(n):
        if n <= 0:
            if callable(default):
                method = default
            else: method = lambda: default
        else: method = lambda: recurse(n - 1)
        return defaultdict(method)
    return recurse(level)

class MarkovChain(object):

    ROOT_NODE = ''
    ROOT_PATH = (ROOT_NODE, )

    @classmethod
    def train(klass, sources, size=2):
        ''' Given a source of training data, build a database
        of possible transitions based on the supplied number
        of previous states.

        :param sources: The source of data to train with (list of list)
        :param size: The number of previous data to use
        :returns: A trained markov chain
        '''
        def generate_source_paths():
            for line in sources:                          # for each line in the source
                if not line: continue                     # as long as that source has data
                yield (klass.ROOT_PATH, line[0])          # emit the first word transition
                for n in range(1, size + 1):              # then for each order of size
                    if len(line) < n: continue            # if the source line is long enough
                    for w in range(len(line) - n):        # generate all the path -> node combos
                        yield (tuple(line[w:w + n]), line[w + n]) # tuple(path) -> word
                    yield (tuple(line[len(line)-n:]), klass.ROOT_NODE) # and then emit the end of line combo

        moves = recursive_dict(default=0.0, level=1)      # build a two level defaultdict of 0 value
        moves[klass.ROOT_PATH][klass.ROOT_NODE] = 0.0     # set the default root values
        for path, node in generate_source_paths():        # for all our possible path -> node pairs
            moves[path][node] += 1.0                      # count the occurrences

        for path in moves:                                # for each path to a node
            total = sum(v for v in moves[path].values())  # get the total sum of possible paths
            total = max(1.0, total)                       # prevent a divide by zero
            for node in moves[path]:                      # then for each node in that path
                moves[path][node] /= total                # normalize its probability
        return klass(moves)                               # return an initialized database

    def __init__(self, moves):
        ''' Initialize a new instance of the MarkovChain
        class with the supplied trained dataset.

        :param moves: The pre-trained transition database
        '''
        self.moves = moves

    def generate(self, seed=None, size=sys.maxint):
        ''' Generate a random sentence of the supplied size
        (default until the path ends) starting from the supplied
        seed or the path root.

        :param seed: The seed to start the path with (default ROOT_PATH)
        :param size: The maximum size of the generated path (default max)
        :returns: The genrated path of nodes
        '''
        path = list(seed) if seed else []
        seed = seed or self.ROOT_PATH
        if seed not in self.moves:
            raise Exception("cannot create a path without a valid starting node")

        node = self.next_node(seed)
        while node and (size > 0):
            path.append(node)
            node = self.next_node(path)
            size = size - 1
        return path

    def next_node(self, seed):
        ''' Given a seed, return the next possible node from the
        moves database.

        :param seed: The seed to pull the next possible node from
        :returns: The next selected node from the moves database
        '''
        seed = tuple(seed)               # only tuples hash
        while seed not in self.moves:    # make sure we have a possible entry
            seed = seed[1:]              # if not, try and get a shorter path
            if not seed: return None     # otherwise we have no possible next node

        nodes  = self.moves[seed]        # get the possible next moves
        sample = random.random()         # sample to choose a node with
        max_prob, max_node = 0.0, None   # initialize default returns

        for node, prob in nodes.items(): # try all items in order
            if prob > max_prob:          # set the default return values
                max_prob = prob
                max_node = node
            if sample > prob:            # otherwise change probability
                sample -= prob           # reduce probability over time
            else: return node            # or choose the selected node
        return max_node                  # otherwise return highest probability node

def chain_from_text(text):
    ''' Given a collection of text, generate a
    markov chain from the contents.

    :param text: The text to train with
    :returns: The resulting markov chain
    '''
    from nltk import work_tokenize, sent_tokenize

    data = [word_tokenize(sent) for sent in sent_tokenize(text)]
    return MarkovChain.train(data)

def chain_from_file(path):
    ''' Given a file path, open it and genrate a
    markov chain from the contents.

    :param path: The path to the file to train with
    :returns: The resulting markov chain
    '''
    text = open(path, 'r').read()
    return chain_from_text(text)
