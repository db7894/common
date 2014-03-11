from random import randint, seed


def generate_caeser_cypher(symbols):
    ''' Randomly generate a substitution cypher of the supplied
    symbols.

    >>> seed(0)
    >>> letters = 'abcdefghijklmnopqrstuvwxyz'
    >>> generate_caeser_cypher(letters)
    {'a': 'f', 'c': 's', 'b': 'j', 'e': 'r', 'd': 'w', 'g': 'o', 'f': 'v', 'i': 'b', 'h': 'm', 'k': 'g', 'j': 'u', 'm': 'a', 'l': 'p', 'o': 'c', 'n': 'k', 'q': 'i', 'p': 'e', 's': 'x', 'r': 'l', 'u': 'h', 't': 'y', 'w': 'q', 'v': 't', 'y': 'n', 'x': 'z', 'z': 'd'}

    :param symbols: The symbols to generate a cypher for
    :returns: The cypher mapping of (decrypted -> encrypted)
    '''
    cypher = {}
    src, dst = list(symbols), list(symbols)
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
    encrypted = []
    for word in words:
        encrypted.append(''.join(cypher[char] for char in word))
    return encrypted

def build_char_ordering_graph(words):
    '''
    '''
    pass

def ordering_to_cypher(letters, graph):
    ''' This is a simple topological sort of
    the resulting graph. We then link it to a given alphabet.

    :param letters: The letters to build a lookup to
    :param graph: The graph to get the ordering from
    :returns: A cypher map between the old and new letters
    '''
    ordering = []
    return dict(zip(ordering, letters))


if __name__ == "__main__":
    letters   = 'abcdefghijklmnopqrstuvwxyz'
    decrypted = Words.getWordList('words')
    encrypt   = generate_caeser_cypher(letters)
    encrypted = encrypt_word_list(decrypted, entrypt)
    graph     = build_char_ordering_graph(encrypted)
    decrypt   = ordering_to_cypher(letters, graph)
    solution  = encrypt_word_list(encrypted, decrypt)
    assert decrypted == solution

    #import doctest
    #doctest.testmod()
