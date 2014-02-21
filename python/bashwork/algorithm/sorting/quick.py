#TODO
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
        pass

    def internal(xs, lo, hi):
        if lo >= hi: return xs
        split = partition(xs, lo, hi)
        internal(xs, lo, split - 1)
        internal(xs, split + 1, hi)
    return internal(coll, 0, len(coll) - 1)

#------------------------------------------------------------
# test runner
#------------------------------------------------------------
if __name__ == "__main__":
    import doctest
    doctest.testmod()
