#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a headline with a clue, collapse the word
down into a stream of characters without punctuation,
and then using some of the letters in order, find a
solution to the clue. Rules:

- not all letters are used
- the letters used must be in order
- the solution must match the clue
'''
from common import Trie, Words

# ------------------------------------------------------------
# utility methods
# ------------------------------------------------------------
def print_solution(clue, solution):
    '''

    :param clue: The clue to search for the solution in
    :param solution: A solution to the supplied clue
    '''
    print "%s -> %s" % (clue, solution)

def prepare_clue(clue):
    ''' Given a clue, remove all spaces and punctuation
    making the word searchable with a trie.

    :param clue: The clue to prepare
    :returns: The prepared clue
    '''
    skipped = set(list(' .,!"\''))
    letters = [l for l in clue if l not in skipped]
    return ''.join(letters)

# ------------------------------------------------------------
# solution
# ------------------------------------------------------------
def find_hidden_words(words, clue):
    ''' bfs, DP

    :param words: A words Trie to search for words in
    :param clue: The clue to search for the solution in
    :returns: A generator of possible solutions
    '''
    stack = [('', clue)]
    while stack:
        word, hint = stack.pop()
        path = words.get_path(word)
        if path.has_a_word():
            yield word
        else:
            for i in range(len(hint) - 1, -1, -1):
                if path.has_path(hint[i]):
                    stack.append((word + hint[i], hint[i + 1:]))

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------
WORDS    = set(Words.get_word_list('celebrities'))
HEADLINE1 = "celeb singer did pornos"
HEADLINE2 = "stardom mythical for designer"

if __name__ == "__main__":
    clue  = prepare_clue(HEADLINE2)
    words = Trie()
    words.add_words(WORDS)

    for solution in find_hidden_words(words, clue):
        print_solution(clue, solution)
