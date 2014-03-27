def dutch_flag_partition(xs, idx):
    '''
    >>> xs = [2,3,4,56,1,5,7,6,8,9]
    >>> dutch_flag_partition(xs, 5)
    [2, 3, 4, 1, 5, 7, 6, 8, 9, 56]
    '''
    def swap(t, f): xs[t], xs[f] = xs[f], xs[t]
    pivot = xs[idx]
    small, equal, large = 0, 0, len(xs) - 1

    while equal <= large:
        if xs[equal] < pivot:
            swap(small, equal)
            small += 1; equal += 1
        elif xs[equal] == pivot:
            equal += 1
        else:
            swap(equal, large)
            large -= 1
    return xs

#------------------------------------------------------------
# main
#------------------------------------------------------------

if __name__ == '__main__':
    import doctest
    doctest.testmod()
