#!/usr/bin/env python
"""
quick implementation of k-nearest neighbors
"""
from common import Entry, count_groups, best_gain
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
        self.name  = kwargs.get('name', 'x')
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

    def __str__(self):
        return "{name:%s, field:%s, value:%d}" % (self.name, self.field, self.value)

#------------------------------------------------------------#
# helper methods
#------------------------------------------------------------#
def tree_build(dataset, attributes, threshold=0.01):
    ''' Implementation of k-nearest-neighbor
    
    @param dataset The training dataset
    @param attributes The attribute names
    '''
    attrs = attributes.keys() # just the indexes
    def _inner(db):
        # if groups are pure or no more attributes
        m, groups = count_groups(db)
        if (len(groups) == 1) or (len(attrs) == 0):
            return Leaf(m)

        # calculate entropies and gain
        (gain, value, field) = best_gain(db, attrs)
        _logger.debug("g:%f, f:%s, v:%f" % (gain, attributes[field], value))
        if gain < threshold:
           return Leaf(m)

        # build next tree level
        tree  = Tree(field=field, value=value, name=attributes[field])
        _logger.debug(tree)
        left  = [e for e in db if e.values[field] <= value]  # left  group
        right = [e for e in db if e.values[field] >  value]  # right group
        if len(left)  > 0: tree.left  = _inner(left)
        if len(right) > 0: tree.right = _inner(right)
        return tree
    return _inner(dataset)

def tree_to_rules(tree):
    ''' Helper method to convert a tree to rules
    
    @param tree The tree to test against
    @return A list of rules
    '''
    def _inner(root, rule, rules):
        if isinstance(root, Leaf):
            return rules.append(rule[0:-2] + " -> " + root.label)
        _inner(root.left,  rule + "%s <= %f, " % (root.name, root.value), rules)
        _inner(root.right, rule + "%s >  %f, " % (root.name, root.value), rules)
        return rules
    return _inner(tree, "", [])

def tree_to_graphviz(tree):
    ''' Helper method to convert a tree to a graphviz
    document.
    
    @param tree The tree to process
    @return A graphviz representation of the tree
    '''
    def _listnodes(root, c):
        if isinstance(root, Leaf):
            return (c, '\tnode%d [label ="%s"];\n' % (c, root.label))
        else:
            g = '\tnode%d [label ="%s"];\n' % (c, root.name)
            (c, l) = _listnodes(root.left,  c+1)
            (c, r) = _listnodes(root.right, c+1)
            return (c, g + l + r)

    def _linklinks(root, count):
        if isinstance(root, Leaf): return (count, "")
        g  = '\tnode%d -> node%d [label="<= %0.2f"]\n' % (count, count+1, root.value)
        (c, l) = _linklinks(root.left,  count+1)
        g += '\tnode%d -> node%d [label="> %0.2f"]\n'  % (count, c+1, root.value)
        (c, r) = _linklinks(root.right, c+1)
        return (c, g + l + r)

    graph  = "digraph G\n{\n\tnode  [shape = diamond];\n"
    graph += '\tedge  [color="#2554c7"];\n'
    graph += _listnodes(tree, 0)[1]
    graph += "\n\n"
    graph += _linklinks(tree, 0)[1]

    return graph + "}"
    
#------------------------------------------------------------#
# example run
#------------------------------------------------------------#
if __name__ == "__main__":
    from common import load_arff

    #logging.basicConfig()
    #_logger.setLevel(logging.DEBUG)

    attributes = {0:"temp", 1:"humid", 2:"light", 3:"cloud"}
    training   = list(load_arff("data/training.arff"))
    testing    = list(load_arff("data/testing.arff"))
    tree       = tree_build(training, attributes)

    print "# first 3 examples:",
    print [tree.classify(entry) for entry in testing[0:3]]
    print "#\n# training accuracy:",
    print tree.accuracy(training)
    print "# testing accuracy: ",
    print tree.accuracy(testing)
    print "#\n# rules:"
    for rule in tree_to_rules(tree): print "#    " + rule
    print "#\n# tree:"
    print tree_to_graphviz(tree)

