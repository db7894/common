#!/usr/bin/env python
"""
quick implementation of k-nearest neighbors
"""
from common import euclidean_distance as distance
from common import Entry

#------------------------------------------------------------#
# classes
#------------------------------------------------------------#
class Leaf(object):

    def __init__(self, label):
        self.label = label

    def __str__(self):
        return "[%s]" % (self.label)

class Tree(object):

    def __init__(self, **kwargs):
        self.field = kwargs.get('field', 0)
        self.value = kwargs.get('value', 0)
        self.left  = kwargs.get('left',  Leaf("x"))
        self.right = kwargs.get('right', Leaf("x"))

    def evaluate(self, entry):
        root = self
        while True:
            root = root.right if entry.values[root.field] > root.value else root.left
            if isinstance(root, Leaf):
                return root.label

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

def tree_accuracy(tree, testing):
    ''' Test the accurracy of the given tree using the
    provided testing set.
    
    @param tree The tree to evaluate
    @param testing The testing set to use
    @return The accurracy of the given tree
    '''
    results = [(entry.label, tree.evaluate(entry)) for entry in training]
    count   = sum(1 for i in results if i[0] == i[1])
    return float(count)/len(testing)
    

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
    print [tree.evaluate(entry) for entry in training[0:3]]
    print "accuracy:",
    print tree_accuracy(tree, training)
    print "rules:"
    for rule in tree_to_rules(tree, attrs): print rule

