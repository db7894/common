import sys
from collections import defaultdict
from longest_paths import *
from common import Graph

#------------------------------------------------------------
# longest common subsequence
#------------------------------------------------------------

def longest_common_subsequence(this, that):
    index = {}
    n, m = len(this), len(that)
    for i in range(n + 1):
        for j in range(m + 1):
            if i == 0 or j == 0:
                index[i, j] = 0
            elif this[i - 1] == that[j - 1]:
                index[i, j] = 1 + index[i - 1, j - 1]
            else: index[i, j] = max(index[i - 1, j], index[i, j - 1])

    def rebuild(l, r):
        if l == 0 or r == 0:
            return [] 
        elif this[l - 1] == that[r - 1]:
            return rebuild(l - 1, r - 1) + [this[l - 1]]
        elif index[l - 1, r] > index[l, r -1]:
            return rebuild(l - 1, r)
        else: return rebuild(l, r - 1)
    
    return rebuild(n, m)

def dp_lcs_test():
    this = list('human')
    that = list('chimpanzee')
    print longest_common_subsequence(this, that)

#------------------------------------------------------------
# longest increasing subsequence
#------------------------------------------------------------

def longest_increasing_subsequence_lcs(coll):
    this, that = coll, sorted(coll)
    return longest_common_subsequence(this, that)

def longest_increasing_subsequence_path(coll):
    graph = Graph(min(coll))
    for i, a in enumerate(coll):
        for b in coll[i:]:
            if a < b: graph.add_edge(a, b, 1)
    return longest_paths(graph)

#------------------------------------------------------------
# tests
#------------------------------------------------------------
if __name__ == "__main__":
    coll = [5, 2, 8, 6, 3, 6, 9, 7]
    print longest_increasing_subsequence_lcs(coll)
    print longest_increasing_subsequence_path(coll)

