		
def euclidean(left, right):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
	total = sum(pow(l - r, 2) for l, r in zip(left, right))
	return pow(total, 0.5)
	
def manhattan(left, right):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
	return sum(abs(l - r) for l, r in zip(left, right))
	
def chebyshev(left, right):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
	return max(abs(l - r) for l, r in zip(left, right))

def minkowski(left, right, p=1):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
	total = sum(pow(abs(l - r), p) for l, r in zip(left, right))
	return pow(total, 1/p)

def hamming(left, right):
	'''
	@param left The left value to compare
	@param right The right value to compare
	@return The distance between the two
	'''
    return sum(l == r for l, r in zip(left, right))

def overlap(left, right):
    ''' Given two segments on a 1-D plane, determine if
    the two segements overlap and by how much.

    :param left: The left segment (start, end)
    :param right: The right segment (start, end)
    :returns: The amount the two segments overlap
    '''
    return max(0, min(left[1], right[1]) - max(left[0], right[0]))

def does_overlap(left, right):
    ''' Given two segments on a 1-D plane, determine if
    the two segements overlap and by how much.

    :param left: The left segment (start, end)
    :param right: The right segment (start, end)
    :returns: The amount the two segments overlap
    '''
    return right[1] >= left[0] and left[1] >= right[0]
