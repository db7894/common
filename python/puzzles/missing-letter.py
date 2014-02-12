#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a collection of words where one letter has been
removed and the remaining letters have been scrambled, find
the original word and using the missing letters create and
un-scramble the final word. The extra rules are:

* There are hints to what the word can be, but these don't matter
* There is only one overall solution, but many initial solutions
* The final solution has something to do with the initial clues
'''
from collections import defaultdict
from common import Words

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

def find_final_solution(lookup, solutions):
    ''' Given an original word and the possible solutions,
    find a working solution for the missing word.
    TODO anagram Trie

    :param original: The original word to find a solution for
    :param solutions: The possible solutions for the given word
    '''
    possible = [('', [], [])]
    for scramble in solutions:
        current = []
        for letters, words, scrambles in possible:
            for letter, word in solutions[scramble]:
                current.append((letters + letter, scrambles + [scramble], words + [word]))
        possible = current

    for letters, words, scramble in possible:
        letters = ''.join(sorted(letters))
        if letters in lookup:
            print lookup[letters]
            #yield zip(scramble, letters, words), lookup[letters]
    return []

def print_solution(solution):
    ''' Given an original word and the possible solutions,
    print them out.

    :param solutions: The final solutions for the problem
    '''
    for original, letter, solution in solution[0]:
        print "%s + %s → %s" % (original, letter, solutions)
    print '= %s' % solution[1]

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------
WORDS    = set(Words.get_word_list())
#M_LOOKUP = Words.generate_missing_lookup(WORDS)
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
    import pickle
    #solutions = get_possible_solutions(M_LOOKUP, MISSING)
    print "finished loading cache"
    solutions = pickle.load(open('/tmp/solutions', 'rb'))
    for solution in find_final_solution(W_LOOKUP, solutions):
        print_solution(solution)
