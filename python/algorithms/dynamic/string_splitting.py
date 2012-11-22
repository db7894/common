from collections import defaultdict
from common import get_dictionary

def split_string(string, dictionary):
    N = len(string)
    W = [None for _ in range(N + 2)]
    for i in range(N - 1, -1, -1):
        for j in range(N - 1, i, -1):
            split = string[i:j + 1]
            if split in dictionary:
                path = split if not W[j + 1] else split + ' ' + W[j + 1]
                if not W[i] or len(path) > len(W[i]): W[i] = path
    return W

def split_string_map(string, dictionary):
    N = len(string)
    W = defaultdict(int)
    for i in range(N - 1, -1, -1):
        for j in range(N - 1, i, -1):
            if string[i:j + 1] in dictionary:
                W[i,j - i] = 1
 
    # initialize the dfs
    starts = defaultdict(list) 
    for s, l in W.keys():
        starts[s].append(l)
    stack = [[string[0:l + 1]] for l in starts[0]]

    # DFS of the words 
    while any(stack):
        words = stack.pop()
        start = sum(len(w) for w in words)
        trails = [words + [string[start:start + length + 1]] for length in starts[start]]
        if not any(trails):
            yield words
        else: stack = stack + trails


#------------------------------------------------------------
# tests
#------------------------------------------------------------
if __name__ == "__main__":
    dictionary = get_dictionary()
    this = "supermagicsunbeamartist"
    for split in split_string_map(this, dictionary):
        print split
