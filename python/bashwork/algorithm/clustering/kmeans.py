#!/usr/bin/env python
"""
quick implementation of k-nearest neighbors
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
def random_initialize(database, k):
    '''
    '''
    length = len(database)
    values = [database[randint(0, length)] for _ in range(k)]
    return [Entry(id, value) for (id, value) in enumerate(values)]

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

def kmeans(database, k, initialize=random_initialize, terminate=None):
	''' Implementation of kmeans

	@param database The training database
	@param k The number of clusters to find
	'''
    centroids = initialize(database, k)
    terminate = terminate or count_terminate(10)
    while True:
        for entry in database:
            weights = [(distance(entry.values, c.values), c) for c in centroids]
            entry.label = sorted(weights)[0][1]
        groups = groupby(database, lambda (a,b): 
        centroids = [compute_centroid(group) for group in groups]
        if terminate(): break;
    return centroids

#------------------------------------------------------------#
# example run
#------------------------------------------------------------#
if __name__ == "__main__":
    from common import load_arff

    database = list(load_arff("training.arff"))
    #training = list(load_arff("testing.arff"))

    print "5-means",
    print kmeans(database, 5)
