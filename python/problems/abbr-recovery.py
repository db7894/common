#!/usr/bin/env python
'''
Given an abbreviation and a list of possible
words, return the word before it was abbreviated.
'''
import sys

#------------------------------------------------------------
# solvers
#------------------------------------------------------------

def edit_distance(this, that):
    ''' Given a word and a possible candidate, find
    the edit distance of the two words by only considering
    deletions from the candidate.

    :param this: The word to find a match for
    :param that: The possible match
    :returns: The edit distance from the two words
    '''
    n, m = len(this) - 1, len(that) - 1
    edits = {}
    for i in range(n + 1): edits[i,0] = i
    for j in range(m + 1): edits[0,j] = j
    for i in range(1, n + 1):
        for j in range(1, m + 1):
            d = 0 if this[i] == that[j] else 1
            edits[i,j] = min(1 + edits[i - 1, j], d + edits[i - 1, j - 1])
    return edits[n,m]

def get_best_words(short, dictionary):
    ''' Given a shortened word and a dictionary, attempt to
    find the best original word for the shortened word.

    :param short: The shortened word
    :param dictionary: The dictionary of words to check
    :returns: A collection of the best possible words
    '''
    words, value = [], sys.maxint
    for possible in dictionary:
        new_value = edit_distance(short, possible)
        if new_value < value: value, words = new_value, [possible]
        elif new_value == value: words.append(possible)
    return words

#------------------------------------------------------------
# constants
#------------------------------------------------------------

WORD = "apl jce"
WORDS = [
    "apple juice",
    "apple joist",
    "appaling jokes",
    "apalit jacee",
]

if __name__ == "__main__":
    for word in get_best_words(WORD, WORDS):
        print "{} -> {}".format(WORD, word)
