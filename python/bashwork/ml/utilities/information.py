'''
.. todo:: just inline the various pieces
'''
from math import log
from collections import Counter

def count_groups(labels):
    ''' Given a dataset of entries, count the values
    and return the most common label.
    
    :param labels: The labels to count
    :returns: ('max label', counter)
    '''
    counts = Counter(labels)
    return (counts.most_common(1)[0][0], counts)

def entropy(labels):
    ''' Computes the entropy of the labels::

        sum { -Pr(i) log2 Pr(i) }

    :param labels: The labels to calculate the entropy of
    :returns: The entropy of the labels
    '''
    c = len(labels)
    d = count_groups(labels)[1].values()
    return sum(((-1.0 * v) / c) * log((1.0 * v) / c, 2) for v in d)

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
