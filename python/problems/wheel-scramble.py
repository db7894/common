#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a sentence, remove all the punctuation and spaces,
break the sentence into equal groups of letters (say 5),
wrap these around randomly (like a wheel), then randomly
scramble the resulting wheel groups.

Given this scrambled collection of wheels, restore the original
sentence.
'''

def print_solution(solution):
    ''' Given a solution to the problem, print out the
    final result.

    :param solution: The final solution for the problem
    '''
    print ''.join(solution)

def find_solutions(wheels):
    pass

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------
WHEELS1 = [
    list('farbes'),
    list('ortsma'),
    list('fiftan'),
    list('spcing'),
    list('preacy'),
    list('ractip'),
    list('manone'),
    list('githin'),
    list('ipinsh'),
    list('thtter'),
]

WHEELS2 = [
    list('thees'),
    list('surei'),
    list('mebod'),
    list('behin'),
    list('ediss'),
    list('tysoi'),
    list('dever'),
    list('rtuno'),
    list('yhadm'),
    list('ppano'),
    list('ilyfa'),
    list('hywis'),
]

if __name__ == "__main__":
    wheels = WHEELS1
    for solution in find_solutions(W_LOOKUP, wheels):
        print_solution(solution)
