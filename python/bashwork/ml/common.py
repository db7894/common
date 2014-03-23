from math import log
from collections import Counter

#------------------------------------------------------------#
# classes
#------------------------------------------------------------#

class Entry(object):
    ''' Represents a single entry in a model along with
    its label and values.
    '''

    def __init__(self, label, values):
        ''' Initialize a new instance of an Entry class.

        :param label: The label associated with this entry
        :param values: The values associated with this entry
        '''
        self.label  = label
        self.values = tuple(values)

    def __str__(self):  return self.label
    def __repr__(self): return self.label

#------------------------------------------------------------#
# entropy helpers
#------------------------------------------------------------#
# A great deal of these operations can be cached to be
# more performant (as they are recomputed many times). If
# that is an issue, simply move them into a single function
# or use a more performant library.
#------------------------------------------------------------#

def count_groups(dataset, extract=lambda e:e.label):
    ''' Given a dataset of entries, count the values
    and return the most common label.
    
    :param dataset: The dataset to count
    :param extract: The label extractor
    :returns: ('max label', counter)
    '''
    counts = Counter(extract(entry) for entry in dataset)
    return (counts.most_common(1)[0][0], counts)

def entropy(dataset):
    ''' Computes the entropy of the dataset::

        sum { -Pr(i) log2 Pr(i) }

    :param dataset: The dataset to calculate the entropy of
    :returns: The entropy of the dataset
    '''
    c = len(dataset)
    d = count_groups(dataset)[1]
    return sum(((-1.0 * v) / c) * log((1.0 * v) / c, 2) for v in d.values())

def entropy_split(dataset, field, value):
    ''' Computes the entropy of splitting at the selected
    attribute.

    :param dataset: The dataset to calculate the entropy of
    :param field: The field to split on
    :param value: The value to split at
    :returns: The entropy of the dataset after the split
    '''
    c = len(dataset)
    d = sorted((entry.values[field], entry) for entry in dataset)
    l = [entry[1] for entry in d if entry[0] <= value]
    r = [entry[1] for entry in d if entry[0] >  value]
    return sum((float(len(group)) / c) * entropy(group) for group in [l, r] if len(group) > 0)

def gain(dataset, field, value, cache=None):
    ''' Compute the gain by splitting at the specified
    attribute::
    
        gain(D,a) = entropy(D) - entropy(Da)

    :param dataset: The dataset to compute the gain on
    :param field: The field to split on
    :param value: The value to split at
    :returns: The information gain at the split
    '''
    return (cache or entropy(dataset)) - entropy_split(dataset, field, value)

def best_value_gain(dataset, field, cache=None):
    ''' Picks the best gain split threshold value for the
    given continuous attribute.

    :param dataset: The dataset to find the best gain on
    :param field The field to try and split on
    :returns: The best field split value
    '''
    values = sorted(e.values[field] for e in dataset)
    gains  = ((gain(dataset, field, value, cache), value) for value in values)
    return max(gains) # (gain, value)

def best_gain(dataset, fields):
    ''' Picks the best gain split from the given fields

    :param dataset: The dataset to find the best gain on
    :param fields: The fields to split on
    :returns: The best field to split on
    '''
    cache = entropy(dataset)
    gains = (best_value_gain(dataset, field, cache) + (field, ) for field in fields)
    return max(gains) # (gain, value, field)

def check_classifier_accuracy(classifier, dataset):
    ''' Test the accurracy of the given classifier using
    the provided testing set.
   
    :param classifier: The classifier to evaluate
    :param dataset: The dataset to evaluate with
    :returns: The accurracy of the given classifier
    '''
    correct = sum(classifier.classify(e) == e.label for e in dataset)
    return float(correct) / len(dataset)

