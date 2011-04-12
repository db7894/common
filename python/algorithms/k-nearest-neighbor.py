#!/usr/bin/env python
"""
quick implementation of k-nearest neighbors
"""
from common import manhattan_distance as distance
from itertools import groupby

#------------------------------------------------------------#
# implementation
#------------------------------------------------------------#
def knn(database, entry, k):
	''' Implementation of k-nearest-neighbor

	@param database The training database
	@param entry The entry to label
	@param k The number of neighbors to test
	'''
	weight = sorted((distance(d.values, entry.values), d.label) for d in database)
	groups = groupby(sorted([d[-1] for d in weight[0:k]]))
	return max(groups, key=lambda(key,group): len(list(group)))[0]

#------------------------------------------------------------#
# example run
#------------------------------------------------------------#
if __name__ == "__main__":
    from common import load_arff

    tests    = 3
    database = list(load_arff("training.arff"))
    training = list(load_arff("testing.arff"))

    print "1-nearest-neighbor",
    print [knn(database, entry, 1) for entry in training[0:tests]]
    print "5-nearest-neighbor",
    print [knn(database, entry, 5) for entry in training[0:tests]]

