#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a sentence which has two movie titles randomly
interleaved, break the sentence into the two constituient
titles.
'''
from common import Trie, Words

def print_solution(solution):
    ''' Given a solution to the problem, print out the
    final result.

    :param solution: The final solution for the problem
    '''
    print "%s\t-> [%s][%s]" % solution

def find_solutions(lookup, hidden):
    ''' Given an interleaved string, split that string into
    its two parts.

    :param lookup: The dictionary to search for titles in
    :param hidden: The interleaved title to split
    :returns: The resulting solutions (mixed, title1, title2)
    '''
    stack = [('', '', hidden)]
    while stack:
        worda, wordb, search = stack.pop()
        patha = words.get_path(worda)
        pathb = words.get_path(wordb)
        if patha.has_a_word() and pathb.has_a_word():
            return (hidden, worda, wordb)
        else:
            for i in range(0, len(search)):
                if patha.has_path(search[i]):
                    stack.append((worda + search[i], wordb, search[i + 1:]))
                if pathb.has_path(search[i]):
                    stack.append((worda, wordb + search[i], search[i + 1:]))

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------
HIDDEN = [
    'thiecbeigachgiell',
    'betfworilesigunshett',
    'tcrafafrisc',
    'tlawbyirsintther'
]
WORDS  = set(Words.get_word_list('movies'))

if __name__ == "__main__":
    words = Trie()
    words.add_words(WORDS)
    for hidden in HIDDEN:
        solution = find_solutions(words, hidden)
        print_solution(solution)
