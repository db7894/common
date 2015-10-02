from collections import defaultdict

def subsets(xs):
    ''' Generate all the subsets of the supplied set.

    >>> subsets([1, 2, 3])
    [set([]), set([1]), set([2]), set([1, 2]), set([3]), set([1, 3]), set([2, 3]), set([1, 2, 3])]

    :param xs: The collection to get the subsets of
    :returns: The collection of subsets.
    '''
    subs = [set()]
    for x in xs:
        temp = [s.copy().union([x]) for s in subs]
        subs.extend(temp)
    return subs


def subsets_recursive(coll):
    ''' Generate all the subsets of the supplied set.

    >>> subsets_recursive([1, 2, 3])
    [set([]), set([3]), set([2]), set([2, 3]), set([1]), set([1, 3]), set([1, 2]), set([1, 2, 3])]

    :param coll: The collection to get the subsets of
    :returns: The collection of subsets.
    '''
    if not coll: return [set()]
    curr, rest = coll[0], coll[1:]
    sets = subsets_recursive(rest)
    news = [s.copy().union([curr]) for s in sets]
    sets.extend(news)
    return sets


def permutations(xs):
    ''' Generate all the permutations of the supplied set.

    >>> word = "abc"
    >>> permutations(word)
    ['cba', 'bca', 'bac', 'cab', 'acb', 'abc']
    '''
    perms = [xs.__class__()]
    for x in xs:
        perms = [p[:i] + x + p[i:] for p in perms
                                   for i in range(0, len(p) + 1)]
    return perms


def permutations_recursive(xs):
    ''' Generate all the permutations of the supplied set
    using recursion.

    >>> word = "abc"
    >>> list(permutations_recursive(word))
    ['abc', 'bac', 'bca', 'acb', 'cab', 'cba']
    '''
    if len(xs) > 1:
        for perm in permutations_recursive(xs[1:]):
            for i in range(len(xs)):
                yield perm[:i] + xs[0:1] + perm[i:]
    else: yield xs


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
