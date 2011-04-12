"""
A collection of helper utilities for the various
algorithms.
"""
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
	values = [float(piece) for piece in pieces] + [label]
	return tuple(values)
		
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
	left, right = left[0:-1], right[0:-1] # remove class labels
	total = sum(pow(l - r, 2) for (l,r) in zip(left, right))
	return pow(total, 0.5)
	
def manhattan_distance(left, right):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
	left, right = left[0:-1], right[0:-1] # remove class labels
	total = sum(abs(l - r) for (l,r) in zip(left, right))
	return total
	
def chebyshev_distance(left, right):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
	left, right = left[0:-1], right[0:-1] # remove class labels
	total = max(abs(l - r) for (l,r) in zip(left, right))
	return total

def minkowski_distance(left, right, p=1):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
	left, right = left[0:-1], right[0:-1] # remove class labels
	total = sum(pow(abs(l - r), p) for (l,r) in zip(left, right))
	return pow(total, 1/p)
