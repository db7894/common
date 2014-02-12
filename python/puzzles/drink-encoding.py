#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a collection of drink recipies where each letter
has been replaced by one of five symbols each of which is
associated with a collection of letter, convert them back
to the original recipies.
'''
from common import Words

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------
WORDS   = set(Words.get_word_list('drinks'))
PREFIX  = set(word[:i] for word in WORDS for i in range(1, len(word) + 1))
LETTERS = {
    1: 'bdfhklt',
    2: 'cmnrsvwxz',
    3: 'aeiou',
    4: 'gjpq',
    5: 'y',
}
DRINKS  = [
    [(1,3,4,3,3,1,3), (2,3,2,2,3,2,3)],
    [(1,1,3,3,1,5), (2,3,2,5)],
    [(4,3,2), (3,2,1), (1,3,2,3,2)],
    [(4,3,2,3), (2,3,1,3,1,3)],
    [(2,3,2,1), (4,3,1,3,4)],
]

# ------------------------------------------------------------
# solution
# ------------------------------------------------------------
def word_search(suggest):
    '''
    '''
    total = len(suggest)
    queue = [x for x in suggest[0]]
    while queue:
        word = queue.pop()
        size = len(word)

        if size == total and word in WORDS:
            yield word

        if size != total and word in PREFIX:
            queue = [word + l for l in suggest[size]] + queue

def combinations(solutions):
    '''
    '''
    for solution in solutions:
        pieces = ['']
        for words in solution:
            pieces = [piece.lstrip() + ' ' + word for piece in pieces for word in words]
        yield pieces

def size_encoding(letters, drinks):
    '''
    '''
    suggestions = []
    for drink in drinks:
        suggest = []
        for word in drink:
            suggest.append([letters[letter] for letter in word])
        suggestions.append(suggest)

    guesses = []
    for suggestion in suggestions:
        guesses.append([list(word_search(word)) for word in suggestion])
    return combinations(guesses)

if __name__ == "__main__":
    for solution in size_encoding(LETTERS, DRINKS):
        print solution
