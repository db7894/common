"""
A collection of helper utilities for the various
algorithms.
"""

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
# helpers
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
		
def euclidean_distance(left, right):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
	total = sum(pow(l - r, 2) for (l,r) in zip(left, right))
	return pow(total, 0.5)
	
def manhattan_distance(left, right):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
	return sum(abs(l - r) for (l,r) in zip(left, right))
	
def chebyshev_distance(left, right):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
	return max(abs(l - r) for (l,r) in zip(left, right))

def minkowski_distance(left, right, p=1):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
	total = sum(pow(abs(l - r), p) for (l,r) in zip(left, right))
	return pow(total, 1/p)
