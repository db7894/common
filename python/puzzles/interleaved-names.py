#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a sentence which has two movie titles randomly
interleaved, break the sentence into the two constituient
titles.
'''
from common import Words

def print_solution(solution):
    ''' Given a solution to the problem, print out the
    final result.

    :param solution: The final solution for the problem
    '''
    print "%s\t-> [%s][%s]".format(solution)

def find_solutions(lookup, words):
    '''

    :param lookup: The dictionary to search for titles in
    :param words: The interleaved titles to split
    :returns: The resulting solutions (mixed, title1, title2)
    '''
    pass

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------
WORDS = [
    'thiecbeigachgiell',
    'betfworilesigunshett',
    'tcrafafrisc',
    'tlawbyirsintther'
]

if __name__ == "__main__":
    for solution in find_solutions(W_LOOKUP, WORDS):
        print_solution(solution)
