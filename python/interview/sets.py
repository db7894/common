from collections import defaultdict

def are_chars_unique(string):
    ''' Given a string, determine if all the characters
    in the string are unique.

    >>> are_chars_unique("abcdefghijklmnop")
    True
    >>> are_chars_unique("abcdafghijklmnop")
    False
    '''
    return len(string) == len(set(string))

def are_chars_unique_array(string):
    ''' Given a string, determine if all the characters
    in the string are unique using a lookup table.

    >>> are_chars_unique_array("abcdefghijklmnop")
    True
    >>> are_chars_unique_array("abcdafghijklmnop")
    False
    '''
    ks = [False for _ in range(255)]
    for c in string:
        if ks[ord(c)]: return False
        ks[ord(c)] = True
    return True


def are_strings_anagrams(xs, ys):
    ''' Given two strings, check if they are anagrams of
    each other.

    >>> are_strings_anagrams("march", "charm")
    True
    >>> are_strings_anagrams("waffle", "laffaw")
    False
    '''
    return sorted(xs) == sorted(ys)

def are_strings_anagrams_array(xs, ys):
    ''' Given two strings, check if they are anagrams
    using a lookup table.

    >>> are_strings_anagrams_array("march", "charm")
    True
    >>> are_strings_anagrams_array("waffle", "laffaw")
    False
    '''
    if len(xs) != len(ys): return False
    ks = [0 for _ in range(255)]
    for i in range(0, len(xs)):
        ks[ord(xs[i])] += 1
        ks[ord(ys[i])] -= 1
    return all(n == 0 for n in ks)

def get_all_anagrams(word, dictionary):
    ''' Given a dictionary of available words
    return all the anagrams of a specified word.

    >>> words = ['charm', 'march', 'arbor']
    >>> get_all_anagrams('charm', words)
    ['charm', 'march']

    :params word: The word to get all the anagrams for
    :params dictionary: All the available words to check
    :returns: All the available anagrams for that word
    '''
    anagrams = defaultdict(list)
    for entry in dictionary:
        anagrams[str(sorted(entry))].append(entry)
    return anagrams[str(sorted(word))]
    
def subsets(xs):
    ''' Generate all the subsets of the supplied set.
    >>> subsets([1, 2, 3])
    [set([]), set([1]), set([2]), set([1, 2]), set([3]), set([1, 3]), set([2, 3]), set([1, 2, 3])]
    '''
    subs = [set()]
    for x in xs:
        temp = [s.copy().union([x]) for s in subs]
        subs.extend(temp)
    return subs

def permutations(xs):
    ''' Generate all the permutations of the supplied set.
    '''
    perms = [xs.__class__()]
    for x in xs:
        perms = [p[:i] + x + p[i:] for p in perms
                                   for i in range(0, len(p) + 1)]
    return perms

def team_matching(players):
    ''' Given a collection of players and their rankings,
    split the teams with even number of players so that the
    rankings are as close as possible.
    ''' 
    ps, pt = sorted(players, reverse=True), len(players) / 2
    xs, xc = [], 0
    ys, yc = [], 0
    for p in ps:
        if xc < yc and len(xs) < pt:
            xs.append(p)
            xc += p
        else:
            ys.append(p)
            yc += p
    return (xs, xc), (ys, yc)

if __name__ == "__main__":
    import doctest
    doctest.testmod()
