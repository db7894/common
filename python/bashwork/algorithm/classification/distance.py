"""
A collection of distance functions.
"""
		
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