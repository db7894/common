import sys

def rain_trap(heights):
    ''' Given a collection of building heights, figure
    out how much water would be trapped if rain fell from
    above and filled in the local minima.

    >>> heights = [0,1,0,2,1,0,1,3,2,1,2,1]
    >>> rain_trap(heights)
    6

    :param heights: An array of building heights
    :returns: The amount of water trapped
    '''
    def running_max(xs):
        rmax = -sys.maxint
        for x in xs:
            rmax = max(rmax, x)
            yield rmax
    rs = running_max(heights)
    ls = running_max(heights[::-1])
    ds = [min(r,l) for r,l in zip(rs, reversed(list(ls)))]
    return sum(d - h for d,h in zip(ds, heights))
