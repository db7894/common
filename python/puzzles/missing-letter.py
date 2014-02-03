#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a collection of drink recipies where each letter
has been replaced by one of five symbols each of which is
associated with a collection of letter, convert them back
to the original recipies.
'''
from collections import defaultdict
from common import Trie, Words

def get_possible_solutions(lookup, words):
    ''' Given a lookup table and a collection of words,
    attempt to find all the possible solutions to the
    given problem.

    :param lookup: The lookup table to find solutions from
    :param words: The words to find solutions for
    :returns: A generator of (original, solutions)
    '''
    results = {}
    for word in words:
        results[word] = lookup[''.join(sorted(word))]
    return results

def find_working_solution(lookup, solutions):
    ''' Given an original word and the possible solutions,
    print them out.

    :param original: The original word to find a solution for
    :param solutions: The possible solutions for the given word
    '''
    print "%s → %s" % (original, solutions)

def print_solution(original, solutions):
    ''' Given an original word and the possible solutions,
    print them out.

    :param original: The original word to find a solution for
    :param solutions: The possible solutions for the given word
    '''
    print "%s → %s" % (original, solutions)

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------
WORDS    = set(Words.get_word_list())
M_LOOKUP = Words.generate_missing_lookup(WORDS)
W_LOOKUP = Words.generate_anagram_lookup(WORDS)
MISSING  = [
    'sarong',
    'gown',
    'shirt',
    'raincoat',
    'apron',
    'sweater',
    'cape'
]

if __name__ == "__main__":
    solutions = get_possible_solutions(M_LOOKUP, MISSING):
    find_working_solutions(W_LOOKUP, solutions)
    #print_solution(original, solutions)
