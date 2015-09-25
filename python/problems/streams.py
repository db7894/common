import sys
from bashwork.sample.sampling import largest_difference
import heapq

def largest_stock_payout(stocks):
    ''' Given a stream of stocks and their values, return the
    max stock payout that could be achieved by buying and selling
    at any ordered time.
    
    :param stocks: a stream of { 'stock-name': [stock-values] }
    :returns: The max payout of the stock stream
    '''
    return { name: largest_difference(vs)
       for name, vs in stocks.items() }

def robot_battery_capacity(coordinates):
    ''' Given a stream of coordinates for a robot, return
    the maximum battery capacity needed to traverse the landscape
    if it only takes power to move up in the Z direction and we gain
    power by moving down in the Z direction.
    
    :param coordinates: a stream of (x,y,z) coordinates
    :returns: The max battery capacity needed to traverse
    '''
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
    ''' Given a stream of values and a window size,
    return the minimum value of the current window
    for the entire stream.

    :param stream: The stream of values to monitor
    :param size: The size of the window to observe
    :returns: A list of the minimum values for each window

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
    ''' Given a stream of values, find the maximum sum
    of the minimum sub array in the stream::

        f(xs, 0) = xs[0]
        f(xs, n) = max(xs[n], xs[n] + f(xs, n - 1))

    :param stream: The stream of values to observe
    :returns: The max sum of the min sub array

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
    the exact median of the values:

    :param stream: The stream to observe
    :returns: The median of the stream

    >>> streaming_median(range(101))
    50

    :param stream: The stream of incoming numbers
    :returns: The median of the given stream
    '''
    max_heap, min_heap = [], [] # max_heap is a min heap holding the large values
    for el in stream:           # min_heap is a max heap holding the small values
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
