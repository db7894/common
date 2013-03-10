def are_chars_unique(string):
    ''' Given a string, determine if all the characters
    in the string are unique.
    '''
    return len(string) == len(set(string))

def are_chars_unique_array(string):
    ''' Given a string, determine if all the characters
    in the string are unique using a lookup table.
    '''
    ks = [False for _ in range(255)]
    for c in string:
        if ks[ord(c)]: return False
        ks[ord(c)] = True
    return True


def are_strings_anagrams(xs, ys):
    ''' Given two strings, check if they are anagrams of
    each other.
    '''
    return sorted(xs) == sorted(ys)

def are_strings_anagrams_array(xs, ys):
    ''' Given two strings, check if they are anagrams
    using a lookup table.
    '''
    if len(xs) != len(ys): return False
    ks = [o for _ in range(255)]
    for i in range(0, len(xs)):
        ks[ord(xs[i])] += 1
        ks[ord(ys[i])] -= 1
    return all(n == 0 for n in ks)
    
def subsets(xs):
    ''' Generate all the subsets of the supplied set.
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
