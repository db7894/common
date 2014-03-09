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
    cyper = {}
    src, dst = list(symbols), list(symbols)
    while src and dst:
        srci, dsti = randint(0, len(src) - 1), randint(0, len(dst) - 1)
        cyper[src.pop(srci)] = dst.pop(dsti)
    return cyper

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

def ordering_to_cypher(graph):


if __name__ == "__main__":
    decrypted = Words.getWordList('words')
    encrypt   = generate_caeser_cypher()
    encrypted = encrypt_word_list(decrypted, entrypt)
    graph     = build_char_ordering_graph(encrypted)
    decrypt   = ordering_to_cypher(graph)
    solution  = encrypt_word_list(encrypted, decrypt)
    assert decrypted == solution

    #import doctest
    #doctest.testmod()
