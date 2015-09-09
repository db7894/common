#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a collection of scrambled words, unscrable
them and in the fewest possible letter movements.
'''
from bashwork.utility.lookup import Words

def get_clean_word_list():
    ''' Retrieve a clean word list that only has words matching
    the supplied letter set.

    :returns: The cleaned word list
    '''
    words = Words.get_word_list_lazy()
    return Words.generate_anagram_lookup(words)

def print_solution(scramble, solution):
    ''' Given a solution to the problem, print out the
    final result.

    :param scramble: The scramble that was solved
    :param solution: The final solution for the problem
    '''
    print "{} -> {}".format(scramble, solution)

def find_solutions(scrambles):
    ''' Given a collection of scrambles, find a collection 
    of possible solutions to each.

    :param scrambles: A list of scrambled words
    :returns: An iterator of the possible solutions
    '''
    words = get_clean_word_list()
    for scramble in scrambles:
        key = ''.join(sorted(scramble))
        yield (scramble, words[key])
        # TODO gray coding to find easiest movement

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------

SCRAMBLES = [
    'eunitsyvri',
    'tntiarmop',
    'iairotgtnne',
    'ytordnama',
    'iortinym',
]

if __name__ == "__main__":
    for scramble, solution in find_solutions(SCRAMBLES):
        print_solution(scramble, solution)
