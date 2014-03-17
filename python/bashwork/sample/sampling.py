import heapq
from sys import maxint
from collections import defaultdict
from random import randint, seed

def popular_exact(coll, size):
    ''' Given a collection, sample the N most
    common elements in a single pass. This uses
    the SpaceSaving algorithm by Metwally.

    :param coll: The collection to sample
    :param size: The number of common elements to sample
    :returns: A collection of the most common elements
    '''
    sample = defaultdict(int)
    for elem in coll:
        sample[elem] += 1
    return heapq.nlargest(size, sample, key=lambda el:sample[el])

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

def random(coll, size):
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
