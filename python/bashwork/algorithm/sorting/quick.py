def sort(coll):
    ''' Given a collection, sort it using the quicksort
    method sorted by reference.

    :param coll: The collection to sort
    :returns: The sorted collection

    >>> coll = [1, 4, 7, 2, 5, 3, 6, 9, 8]
    >>> sort(coll)
    [1, 2, 3, 4, 5, 6, 7, 8, 9]
    '''
    def partition(xs, lo, hi):
        s  = lo + (hi - lo) / 2
        m  = xs[s]
        ls, ms, hs = [], [], []
        for x in xs[lo:hi+1]:
            if   x < m: ls.append(x)
            elif x > m: hs.append(x)
            else: ms.append(x)
        xs[lo:hi+1] = ls + ms + hs
        return s

    def internal(xs, lo, hi):
        if lo >= hi: return
        split = partition(xs, lo, hi)
        internal(xs, lo, split - 1)
        internal(xs, split + 1, hi)
    internal(coll, 0, len(coll) - 1)
    return coll

def sort_clone(xs):
    ''' Given a collection, sort it using the quick
    sort method (sorted by copy).

    :param xs: The collection to sort
    :returns: The sorted collection

    >>> coll = [1, 4, 7, 2, 5, 3, 6, 9, 8]
    >>> sort_clone(coll)
    [1, 2, 3, 4, 5, 6, 7, 8, 9]
    '''
    if not xs: return []
    m = xs[0]
    ls = [x for x in xs[1:] if x <  m]
    hs = [x for x in xs[1:] if x >= m]
    return sort_clone(ls) + [m] + sort_clone(hs)

#------------------------------------------------------------
# test runner
#------------------------------------------------------------
if __name__ == "__main__":
    import doctest
    doctest.testmod()
