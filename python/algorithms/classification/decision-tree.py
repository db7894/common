#!/usr/bin/env python
"""
quick implementation of k-nearest neighbors
"""
from common import Entry
from distance import euclidean_distance as distance

#------------------------------------------------------------#
# Logging
#------------------------------------------------------------#
import logging
_logger = logging.getLogger(__name__)

#------------------------------------------------------------#
# classes
#------------------------------------------------------------#
class Leaf(object):
    ''' Represents the leaf of a tree '''

    def __init__(self, label):
        self.label = label

    def __str__(self):
        return "[%s]" % (self.label)

class Tree(object):
    ''' Represents some level of a given tree '''

    def __init__(self, **kwargs):
        self.field = kwargs.get('field', 0)
        self.value = kwargs.get('value', 0)
        self.left  = kwargs.get('left',  Leaf('x'))
        self.right = kwargs.get('right', Leaf('x'))

    def classify(self, entry):
        ''' Classify a new entry with the current tree
        
        @param entry The entry to classify
        @return The label of the new entry
        '''
        root = self
        while True:
            goright = (entry.values[root.field] > root.value)
            root = root.right if goright else root.left
            if isinstance(root, Leaf):
                return root.label

    def accuracy(self, dataset):
        ''' Test the accurracy of the given tree using the
        provided testing set.
        
        @param dataset The dataset to evaluate
        @return The accurracy of the given tree
        '''
        correct = 0
        for entry in dataset:
            correct += (self.classify(entry) == entry.label)
        return float(correct)/len(dataset)

#------------------------------------------------------------#
# helper methods
#------------------------------------------------------------#
def tree_build(database):
    ''' Implementation of k-nearest-neighbor
    
    @param database The training database
    '''
    return Tree()

def tree_to_rules(tree, attrs):
    ''' Helper method to convert a tree to rules
    
    @param tree The tree to test against
    @param attrs The attribute names lookup list
    @return A list of rules
    '''
    def _inner(root, rule, rules):
        if isinstance(root, Leaf):
            return rules.append(rule[0:-2] + " -> " + root.label)
        _inner(root.left,  rule + "%s <= %f, " % (attrs[root.field], root.value), rules)
        _inner(root.right, rule + "%s >  %f, " % (attrs[root.field], root.value), rules)
        return rules
    return _inner(tree, "", [])
    
#------------------------------------------------------------#
# example run
#------------------------------------------------------------#
if __name__ == "__main__":
    from common import load_arff

    attrs    = ["temp", "humid", "light", "cloud"]
    database = list(load_arff("training.arff"))
    training = list(load_arff("testing.arff"))
    tree     = tree_build(database)

    print "first 3 examples:",
    print [tree.classify(entry) for entry in training[0:3]]
    print "accuracy:",
    print tree.accuracy(training)
    print "rules:"
    for rule in tree_to_rules(tree, attrs): print rule

