import sys
from bashwork.sample.sampling import largest_difference
import heapq

def largest_stock_payout(stocks):
    ''' { 'stock-name': [stock-values] } '''
    return { name: largest_difference(vs)
       for name, vs in stocks.items() }

def robot_battery_capacity(coordinates):
    ''' [(x,y,z)] '''
    return largest_difference((z for _,_,z in coordinates))

def first_unique_character(string):
    ''' Given a string, find the first non-repeating
    character and return its index.

    :param string: The string to find the unique character in
    :returns: (index, character) of first unique character
    '''
    chars = {}
    for index, char in enumerate(string):    # get index of every character
        if char in chars: chars[char] = None # if we have seen a char twice
        else: chars[char] = index            # if we have seen a char once
    return min((b, a) for a, b in a.items()) # missing items will not appear

def min_of_stream_window(stream, size=3):
    '''
    >>> stream = [4, 35, 8, 19, 31, 17]
    >>> list(min_of_stream_window(stream))
    [4, 8, 8, 17]
    '''
    queue = []                                # queue to store (min ...rest)
    for index, entry in enumerate(stream, 1): # enumerate the stream
        while queue and entry < queue[-1][0]: # while there is a new min
            queue.pop()                       # empty the queue of largers
        if (queue
            and (index > size)                # ignore the warmup
            and queue[0][1] <= (index - size)): # if the current min is old
            queue.pop(0)                      # then remove the old min
        queue.append((entry, index))          # add this new entry to the end
        if (index >= size): yield queue[0][0] # if we have a window, yeild current min

def max_sum_of_sub_arrays(stream):
    '''
    f(xs, 0) = xs[0]
    f(xs, n) = max(xs[n], xs[n] + f(xs, n - 1))

    >>> stream = [1, -2, 3, 10, -4, 7, 2, -5]
    >>> max_sum_of_sub_arrays(stream)
    18
    '''
    sums = 0
    best = -sys.maxint
    for value in stream:
        sums = max(sums + value, value)
        best = max(sums, best)
    return best

def streaming_median(stream):
    ''' Given an umlimited stream of numbers, compute
    the exact median of the values::

    >>> streaming_median(range(101))
    50

    :param stream: The stream of incoming numbers
    :returns: The median of the given stream
    '''
    max_heap, min_heap = [], []
    for el in stream:
        if min_heap and el > min_heap[0]:
            heapq.heappush(min_heap, el)
        else: heapq.heappush(max_heap, -el)

        if abs(len(min_heap) - len(max_heap)) > 1:
            if len(min_heap) > len(max_heap):
                heapq.heappush(max_heap, -heapq.heappop(min_heap))
            else:
                heapq.heappush(min_heap, -heapq.heappop(max_heap))

    if   len(min_heap) > len(max_heap): return min_heap[0]
    elif len(max_heap) > len(max_heap): return -max_heap[0]
    else: return (min_heap[0] - max_heap[0]) / 2
