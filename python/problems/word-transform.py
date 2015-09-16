#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given two words and five steps, change a single letter
at each step to make a new valid word such that at the end
of N steps, you have the final word.
'''
from collections import defaultdict
from bashwork.utility.lookup import Words

def get_solutions(start, end, lookup, steps=5):
    ''' Given a lookup table and the start and end words,
    attempt to find all the possible solutions to the
    given problem.

    :param start: The starting word
    :param end: The final word
    :param lookup: The lookup table to find solutions from
    :param steps: The number of allowed steps
    :returns: A generator of (original, solutions)
    '''
    queue = [[start]]
    while queue:
        current = queue.pop()
        if len(current) > steps:
            continue

        if current[-1] == end:
            yield current

        for word in lookup[current[-1]]:
            if word not in current:
                queue.append(current + [word])

def print_solution(solution):
    ''' Given the original problem and a solution, print
    them out.

    :param solution: The final solution to the problem
    '''
    print " → ".join(solution)

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------
WORDS   = set(Words.get_word_list())
#LOOKUP  = Words.generate_single_letter_lookup(WORDS)
import cPickle as pickle
LOOKUP  = pickle.load(open('/tmp/lookup.pickle'))
PROBLEM = ("lose", "gain") # lost last lait gait gain
PROBLEM = ("cold", "warm") # cold → wold → word → ward → warm

if __name__ == "__main__":
    start, end = PROBLEM
    for solution in get_solutions(start, end, LOOKUP):
        print_solution(solution)
