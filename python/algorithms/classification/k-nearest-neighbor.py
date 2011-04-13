#!/usr/bin/env python
"""
quick implementation of k-nearest neighbors
"""
from common import count_groups
from distance  import manhattan_distance as distance

#------------------------------------------------------------#
# implementation
#------------------------------------------------------------#
def knn(dataset, entry, k):
    ''' Implementation of k-nearest-neighbor
    
    @param dataset The training dataset
    @param entry The entry to label
    @param k The number of neighbors to test
    '''
    weights = sorted((distance(d.values, entry.values), d) for d in dataset)
    return count_groups(weights[0:k], lambda e:e[1].label)[0]

#------------------------------------------------------------#
# example run
#------------------------------------------------------------#
if __name__ == "__main__":
    from common import load_arff

    tests    = 3
    database = list(load_arff("data/training.arff"))
    training = list(load_arff("data/testing.arff"))

    print "1-nearest-neighbor",
    print [knn(database, entry, 1) for entry in training[0:tests]]
    print "5-nearest-neighbor",
    print [knn(database, entry, 5) for entry in training[0:tests]]

