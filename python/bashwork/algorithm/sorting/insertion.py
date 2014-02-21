def sort(coll):
    ''' Given a collection, sort it using the insertion
    sort method (sorted by reference).

    O(n^2) performance
    O(n)   storage

    :param coll: The collection to sort
    :returns: The sorted collection

    >>> coll = [1, 4, 7, 2, 5, 3, 6, 9, 8]
    >>> sort(coll)
    [1, 2, 3, 4, 5, 6, 7, 8, 9]
    '''
    for j in range(1, len(coll)):
        k = coll[j]
        i = j - 1
        while i > 0 and coll[i] > k:
            coll[i + 1] = coll[i]
            i = i - 1
        coll[i + 1] = k
    return coll


def sort_clone(coll):
    ''' Given a collection, sort it using the insertion
    sort method (sorted by copy).

    :param coll: The collection to sort
    :returns: The sorted collection

    >>> coll = [1, 4, 7, 2, 5, 3, 6, 9, 8]
    >>> sort_clone(coll)
    [1, 2, 3, 4, 5, 6, 7, 8, 9]
    '''
    return sort(list(coll))

#------------------------------------------------------------
# test runner
#------------------------------------------------------------
if __name__ == "__main__":
    import doctest
    doctest.testmod()
