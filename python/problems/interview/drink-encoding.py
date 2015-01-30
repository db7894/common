#!/usr/bin/env python
# -*- coding: latin-1 -*-
''' Given a collection of drink recipies where each letter
has been replaced by one of five symbols each of which is
associated with a collection of letter, convert them back
to the original recipies.
'''
from bashwork.utility.lookup import Words
from bashwork.structure.trie import Trie

# ------------------------------------------------------------
# solver
# ------------------------------------------------------------

def find_drink_name(drink, letters, drinks):
    ''' Given a drink, the lookup letters, and a collection
    of valid drinks, attempt to find the drink in question.

    To make this more performant, store the node in the trie
    that we are currently in instead of starting from the
    root everytime.

    :param drink: The drink to decode
    :param letters: The letter lookup map
    :param drinks: The drink database trie
    '''
    drink = [letter for word in drink for letter in word]
    queue = [letter for letter in letters[drink[0]]]

    while queue:
        word = queue.pop()
        if word in drinks:
            yield word
        else:
            for letter in letters[drink[len(word)]]:
                path = word + letter
                if drinks.has_path(path):
                    queue.insert(0, path)

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------

WORDS      = set(Words.get_word_list('drinks'))
DICTIONARY = Trie.create(WORDS)
LETTERS    = {
    1: 'bdfhklt',
    2: 'cmnrsvwxz',
    3: 'aeiou',
    4: 'gjpq',
    5: 'y',
}
DRINKS     = [
    [(1,3,4,3,3,1,3), (2,3,2,2,3,2,3)],
    [(1,1,3,3,1,5), (2,3,2,5)],
    [(4,3,2), (3,2,1), (1,3,2,3,2)],
    [(4,3,2,3), (2,3,1,3,1,3)],
    [(2,3,2,1), (4,3,1,3,4)],
]

# ------------------------------------------------------------
# main runner
# ------------------------------------------------------------

if __name__ == "__main__":
    for drink in DRINKS:
        for solution in find_drink_name(drink, LETTERS, DICTIONARY):
            print "{} -> {}".format(drink, solution)
