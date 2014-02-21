import sys
from common import get_dictionary

def edit_distance(this, that):
    n, m = len(this) - 1, len(that) - 1
    edits = {}
    for i in range(n + 1): edits[i,0] = i
    for j in range(m + 1): edits[0,j] = j
    for i in range(1, n + 1):
        for j in range(1, m + 1):
            d = 0 if this[i] == that[j] else 1
            edits[i,j] = min(1 + edits[i - 1, j], 1 + edits[i, j - 1], d + edits[i - 1, j - 1])
    return edits[n,m]

if __name__ == "__main__":
    dictionary = get_dictionary()
    this = 'evantualie'
    words, value = [], sys.maxint
    for w in dictionary:
        v = edit_distance(this, w)
        if v < value: value, words = v, [w]
        elif  v == value: words.append(w)
    print words
