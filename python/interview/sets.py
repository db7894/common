def are_chars_unique(string):
    return len(string) == len(set(string))

def are_chars_unique_array(string):
    ks = [False for _ in range(255)]
    for c in string:
        if ks[ord(c)]: return False
        ks[ord(c)] = True
    return True


def are_strings_anagrams(xs, ys):
    return sorted(xs) == sorted(ys)

def are_strings_anagrams_array(xs, ys):
    if len(xs) != len(ys): return False
    ks = [o for _ in range(255)]
    for i in range(0, len(xs)):
        ks[ord(xs[i])] += 1
        ks[ord(ys[i])] -= 1
    return all(n == 0 for n in ks)
    
def subsets(xs):
    subs = [set()]
    for x in xs:
        temp = [s.copy().union([x]) for s in subs]
        subs.extend(temp)
    return subs

def permutations(xs):
    perms = [xs.__class__()]
    for x in xs:
        perms = [p[:i] + x + p[i:] for p in perms
                                   for i in range(0, len(p) + 1)]
    return perms

def team_matching(players):
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
