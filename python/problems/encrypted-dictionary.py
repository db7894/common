import doctest
from random import randint, seed
from bashwork.utility.lookup import Words
from bashwork.structure.graph import Graph
from bashwork.structure.graph.sorting import graph_toposort_recur

def get_clean_word_list(letters):
    ''' Retrieve a clean word list that only has words matching
    the supplied letter set.

    :params letters: The letter set to filter by
    :returns: The cleaned word list
    '''
    return [word.lower()
        for word in Words.get_word_list('/usr/share/dict/words')]

def generate_caeser_cypher(letters):
    ''' Randomly generate a substitution cypher of the supplied
    letters.

    >>> seed(0)
    >>> letters = 'abcd'
    >>> generate_caeser_cypher(letters)
    {'a': 'c', 'c': 'b', 'b': 'a', 'd': 'd'}

    :param letters: The letters to generate a cypher for
    :returns: The cypher mapping of (decrypted -> encrypted)
    '''
    cypher = {}
    src, dst = list(letters), list(letters)
    while src and dst:
        src_chr, dst_chr = randint(0, len(src) - 1), randint(0, len(dst) - 1)
        cypher[src.pop(src_chr)] = dst.pop(dst_chr)
    return cypher

def encrypt_word_list(words, cypher):
    ''' Given a list of words, encrypt them with the
    supplied cypher.

    >>> cypher = { 'a':'c', 't': 'a', 'c':'t' }
    >>> words  = [ 'act', 'cat', 'tac' ]
    >>> encrypt_word_list(words, cypher)
    ['cta', 'tca', 'act']

    :param words: The word list to encrypt
    :param cypher: The cypher to encrypt the words with
    :returns: The original list of words encrypted
    '''
    return [''.join(cypher[char] for char in word) for word in words]

def build_letter_ordering_graph(letters, words):
    ''' Given a list of words, build a difference graph
    such that the the first differing letter between
    one word and the next represents a dependency edge
    in the graph.

    :param letters: The letters to build edges between
    :param words: The words to build a dependency graph of
    :returns: A dependency graph of the letter edges
    '''
    prev, graph = words[0], Graph(letters)
    for curr in words[1:]:
        for p, c in [(p,c) for p,c in zip(prev, curr) if p != c][:1]:
            graph.add_edge(p, c)
        prev = curr
    return graph

def order_graph_to_cypher(letters, graph):
    ''' This is a simple topological sort of
    the resulting graph. We then link it to a given alphabet.

    :param letters: The letters to build a lookup to
    :param graph: The graph to get the ordering from
    :returns: A cypher map between the old and new letters
    '''
    ordered_letters = graph_toposort_recur(graph)
    return dict(zip(ordered_letters, letters))

def check_letter_edges(letters, graph):
    ''' Given a collection of letters and a graph,
    check that the letter followers are correct.

    This can be used to check that the wordlist
    has been encoded into the graph correctly without
    cycles.

    >>> letters = 'abcd'
    >>> graph = Graph(letters)
    >>> graph.add_edges('a', 'b', 'c', 'd')
    >>> graph.add_edges('b', 'c', 'd')
    >>> graph.add_edges('c', 'd')
    >>> check_letter_edges(letters, graph)
    True

    :param letters: The letters in order to check
    :param graph: The edges to check the order of
    '''
    for index in range(0, len(letters)):
        letter = letters[index]
        expect = set(letters[index:])
        actual = set(graph.get_edges(letter))
        differ = actual.difference(expect)
        if not actual.issubset(expect): return False
        #    print "ERROR: {} : {}".format(letter, differ)
    return True

def print_encryption_chain(encrypt, decrypt):
    ''' Given the encryption and decryption chain,
    print the mapping of orignal to encrypted to
    decrypted.

    :param encrypt: The encryption map
    :param decrypt: The decryption map
    '''
    for src, dst in encrypt.items():
        print "{} -> {} -> {}".format(src, dst, decrypt[dst])

if __name__ == "__main__":
    doctest.testmod() # run unit tests first

    letters   = 'abcdefghijklmnopqrstuvwxyz'
    decrypted = get_clean_word_list(letters)
    encrypt   = generate_caeser_cypher(letters)
    encrypted = encrypt_word_list(decrypted, encrypt)
    graph     = build_letter_ordering_graph(letters, encrypted)
    decrypt   = order_graph_to_cypher(letters, graph)
    solution  = encrypt_word_list(encrypted, decrypt)
    assert (decrypted == solution)
