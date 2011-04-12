"""
A collection of helper utilities for the various
algorithms.
"""
from math import log
from collections import defaultdict

#------------------------------------------------------------#
# classes
#------------------------------------------------------------#
class Entry(object):

    def __init__(self, label, values):
        self.label  = label
        self.values = tuple(values)

    def __str__(self):
        return "%s[%s]" % (self.values, self.label)

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

def gain(dataset, field):
    ''' gain(D,a) = entropy(D)-entropy(Da)

    @param dataset The dataset to test
    @param field The field to split on
    @return The information gain at the split
    '''
    return entropy(dataset) - entropy(field-split)

def best_gain(dataset, fields):
    ''' Picks the best gain split from the given fields

    @param dataset The dataset to test
    @param fields The fields to split on
    @return The best field to split on
    '''
    gains = [(gain(dataset, field), field) for field in fields]
    return max(gains)[1]

#------------------------------------------------------------#
# other helpers
#------------------------------------------------------------#
def count_groups(dataset, extract=lambda e:e.label):
    ''' Given a dataset of entries, count the values
    and return the most common label.

    @param dataset The dataset to count
    @param extract The label extractor
    @return ('max label', count-dict)
    '''
    d = defaultdict(int)
    for entry in dataset:
        d[extract(entry)] += 1
    return (max(d.items(), key=lambda a:a[1])[0], d)
    
