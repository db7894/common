from collections import defaultdict

def subsets(xs):
    ''' Generate all the subsets of the supplied set.

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

    :param xs: The collection to get permutations of
    :returns: All permutations of xs
    '''
    perms = [xs.__class__()]
    for x in xs:
        perms = [p[:i] + x + p[i:] for p in perms
                                   for i in range(0, len(p) + 1)]
    return perms


def permutations_recursive(xs):
    ''' Generate all the permutations of the supplied set
    using recursion.

    :param xs: The collection to get permutations of
    :returns: All permutations of xs
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


def subset_sum_zeromod(array):
    ''' Given an array, return a collection
    of subsets of that array that satisfy:
    `sum(xs) % len(array) == 0` We solve this using
    the pigeonhole principal::

        prefix_sum[j] = \sum{i=0}{j} array[i] % n
        case {
          all_unique(prefix_sum) -> 0 is in prefix_sum
          not_unique(prefix_sum) -> match the repeats
        }

    :param array: The array to find solutions in
    :returns: A collection of solutions
    '''
    def result_set(end, start=0):
        ''' given a range, extract those values from the
        initial array
        '''
        return [array[i] for i in range(start, end)]

    N = len(array)
    prefix_mods = reduce(lambda sets, num: sets + [(sets[-1] + num) % N], array, [0])
    prefix_mods = prefix_mods[1:] # drop initial 0
    prefix_sums = [None] * N
    subset_sums = []

    for index, value in enumerate(prefix_mods):
        if value == 0:
            subset_sums.append(result_set(index + 1))
        elif prefix_sums[value]:
            subset = prefix_sums[value] + 1
            subset_sums.append(result_set(index + 1, subset))
        prefix_sums[value] = index
    return subset_sums

def all_phone_combinations(number, mapping=None):
    default = ['0', '1', 'abc', 'def', 'ghi', 'jkl', 'mno', 'pqrs', 'tuv', 'wxyz']
    mapping = mapping if mapping else default
    strings = []
    queue   = [('', 0)]

    while queue:
        string, index = queue.pop()
        if index != len(number):
            for char in mapping[ord(number[index]) - ord('0')]:
                queue.append((string + char, index + 1))
        else: strings.append(string)
    return strings
