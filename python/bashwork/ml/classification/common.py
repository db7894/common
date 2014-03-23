"""
A collection of helper utilities for the various
algorithms.
"""
from math import log

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
# arff helpers
#------------------------------------------------------------#
def _load_file(path):
	'''
	@param path The file path to load
	@return a generator of the file's lines
	'''
	try:
		with open(path) as stream:
			for line in stream:
				yield line
	except IOError: yield []

def _parse_arff_entry(entry):
	'''
	@param entry The arff entry to parse
	@return The parsed arff entry
	'''
	pieces = entry.split(",")
	label  = pieces.pop().strip()
	values = tuple([float(piece) for piece in pieces])
	return Entry(label, values)
		
def load_arff(path):
	'''
	@param path The file path to load
	@return a generator of the file's lines
	'''
	for line in _load_file(path):
		if not line.startswith("@"):
			yield _parse_arff_entry(line)

#------------------------------------------------------------#
# entropy helpers
#------------------------------------------------------------#
def entropy(dataset):
    ''' sum { -Pr(i) log2 Pr(i) }

    @param dataset The dataset to calculate entropy for
    @return The entropy of the dataset
    '''
    c = len(dataset)
    d = count_groups(dataset)[1]
    return sum(((-1.0*v)/c) * log((1.0*v)/c, 2) for v in d.values())

def entropya(dataset, field, value):
    ''' Computres the attribute entropy

    @param dataset The dataset to calculate entropy for
    @param field The field to split on
    @param value The value to split at
    @return The entropy of the dataset
    '''
    c = len(dataset)
    D = sorted((e.values[field], e) for e in dataset)
    l = [e[1] for e in D if e[0] <= value]  # left  group
    r = [e[1] for e in D if e[0] >  value]  # right group
    return sum((float(len(dj))/c) * entropy(dj) for dj in [l, r] if len(dj) > 0)

def gain(dataset, field, value):
    ''' gain(D,a) = entropy(D)-entropy(Da)

    @param dataset The dataset to test
    @param field The field to split on
    @param value The value to split at
    @return The information gain at the split
    '''
    # memoize entropy here
    return entropy(dataset) - entropya(dataset, field, value)

def best_value_gain(dataset, field):
    ''' Picks the best gain split threshold value for the
    given continuous attribute.

    @param dataset The dataset to test
    @param field The field to try and split on
    @return The best field split value
    '''
    values = sorted(e.values[field] for e in dataset)
    gains  = ((gain(dataset, field, value), value) for value in values)
    return max(gains) # (gain, value)

def best_gain(dataset, fields):
    ''' Picks the best gain split from the given fields

    @param dataset The dataset to test
    @param fields The fields to split on
    @return The best field to split on
    '''
    gains = [best_value_gain(dataset, field) + (field, ) for field in fields]
    return max(gains) # (gain, value, field)

