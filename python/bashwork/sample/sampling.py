import heapq
import math
from sys import maxint
from collections import Counter
from random import randint, seed, random
from itertools import islice

def popular_exact(coll, size):
    ''' Given a collection, sample the N most
    common elements in a single pass. This uses
    a simple counter of all the values.

    :param coll: The collection to sample
    :param size: The number of common elements to sample
    :returns: A collection of the most common elements
    '''
    sample = Counter()
    for elem in coll:
        sample[elem] += 1
    return heapq.nlargest(size, sample, key=lambda el:sample[el])

def reservoir(coll, size):
    ''' Given a collection, sample the N most
    common elements in a single pass. This uses
    reservoir sampling to do this task.

    :param coll: The collection to sample
    :param size: The number of common elements to sample
    :returns: A collection of the most common elements
    '''
    c, sample = 0, []
    for elem in coll:
        c += 1
        if len(sample) >= size:
            s = int(random() * c)
            if s < size: sample[s] = elem
        else: sample.append(elem)
    return sample

def popular(coll, size):
    ''' Given a collection, sample the N most
    common elements in a single pass. This uses
    the SpaceSaving algorithm by Metwally.

    :param coll: The collection to sample
    :param size: The number of common elements to sample
    :returns: A collection of the most common elements
    '''
    default  = (0, 0)
    find_min = lambda s: min((c, a, v) for v, (c, a) in s.items())
    sample   = { elem:(1, 0) for elem in coll[:size] } # initialize at least N elements (count, age)
    for age, elem in enumerate(coll[size:]):           # for a single pass through the stream
        if elem in sample or len(sample) < size:       # if we are in array or our sample is small
            count = sample.get(elem, default)[0] + 1   # increment the current count 
            sample[elem] = (count, age)                # and add ourself to the sample list
        else:                                          # otherwise we have to replace the lowest,oldest element
            xcount, xage, xcurr = find_min(sample)     # find out who is the removed candidate
            sample[elem] = (1 + xcount, age)           # add ourself as a new candidate popular element
            del sample[xcurr]                          # remove the old popular element
    return sample.keys()                               # finally return the N most popular elements

def random_sample(coll, size):
    ''' Given a collection, randomly sample with
    equal probability N elements.

    :param coll: The collection to sample
    :param size: The number of common elements to sample
    :returns: The randomly sampled elements of the collection
    '''
    sample = coll[:size] # initialize the sample
    for index, element in enumerate(coll[size:], size):
        choice = randint(0, index)
        if choice < size:
            sample[choice] = element
    return sample

def largest_difference(stream):
    ''' Given a stream of numbers, return the maximum
    difference between some entry at n, and another entry
    at n + m.

    :param stream: A stream of values to find the difference of
    :returns: The maximum difference in the stream
    '''
    minimum = maxint
    maximum = -1
    for entry in stream:
        minimum = min(minimum, entry)
        maximum = max(maximum, entry - minimum)
    return maximum

def gap_sample(coll, prob):
    ''' Given a collection, randomly sample with
    the supplied probability. This works with streams
    and still maintains O(pn) by using the probability
    to skip a gap of elements instead of running a trial
    on each element.

    :param coll: The collection to sample
    :param prob: The probability to sample with
    :returns: The randomly sampled elements of the collection
    '''
    try:
        coll = iter(coll)
        while True:
            size = int(math.log(random()) / (-prob))
            next(islice(coll, size, size), None) # skip N elements
            yield next(coll)
    except StopIteration: pass

def get_largest_product(values, count):
    ''' Given a collection of values, return the
    N values that produce the largets product.

    DP solution:

        f(i, k) = max(A[i]*f(i+1, k-1), f(i+1, k))

    :param values: The values to search
    :param count: The number of values to return
    :return: A list of the values with the largest product
    '''
    if len(values) < count:
        raise Exception("Not enough values for supplied count")

    def product(xs):
        return reduce(lambda x, y: x * y, xs)

    def recurse(xs, k):
        if k == 1: return [xs[-1]]                # odd base case
        if k <= 0: return []                      # even base case

        xs_n, xs_p = xs[:2], xs[-2:]              # the negative and positive values
        pr_n, pr_p = product(xs_n), product(xs_p) # the product of those values
        ys_n, ys_p = xs[2:], xs[:-2]              # the remainding list to recurse on
        pr, xs, ys = max((pr_n, xs_n, ys_n), (pr_p, xs_p, ys_p))

        return xs + recurse(ys, k - 2)

    return recurse(list(sorted(values)), count)
