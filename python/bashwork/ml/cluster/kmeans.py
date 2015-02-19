#!/usr/bin/env python
"""
quick implementation of k-nearest neighbors
http://datasciencelab.wordpress.com/2013/12/27/finding-the-k-in-k-means-clustering/
"""
from operator import add
from random import randint
from itertools import groupby

from common import Entry
from distance import manhattan_distance as distance

#------------------------------------------------------------#
# initialize strategies
# - 
#------------------------------------------------------------#
def random_initialize(database, count):
    '''
    '''
    length = len(database)
    values = [database[randint(0, length)] for _ in range(count)]
    return [Entry(index, value) for index, value in enumerate(values)]

#------------------------------------------------------------#
# terminate strategies
# - no/min re-assignment of points
# - no/min re-assignment of clusters
# - min decrease of SSE
#------------------------------------------------------------#
def count_terminate(rounds):
    count = rounds
    def implementation():
        count -= 1
        return count == 0
    return implementation

#------------------------------------------------------------#
# implementation
#------------------------------------------------------------#
def compute_centroid(dataset):
    ''' Helper method to recompute the centroids given new
    dataset entries.
    '''
    id = dataset[0].label.label
    data = (entry.values for entry in dataset)
    values = map(add, *data)
    return Entry(id, values)

def kmeans(database, count, **kwargs):
	''' Implementation of kmeans

    :param database: The training database
    :param count: The number of clusters to find
	'''
    initialize = kwargs.get('initialize', random_initialize)
    terminate  = kwargs.get('terminate', count_terminate(10))
    centroids  = initialize(database, count)

    while not terminate():
        for entry in database:
            weights = [(distance(entry.values, c.values), c) for c in centroids]
            entry.label = sorted(weights)[0][1]
        groups = groupby(database, lambda (a,b): 
        centroids = [compute_centroid(group) for group in groups]
    return centroids

