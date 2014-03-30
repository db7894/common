import numpy as np

def euclidean(l, r):
    '''
    :param l: The left value to compare
    :param r: The right value to compare
    :return: The distance between the two
    '''
    return np.sqrt(np.power(l - r, 2).sum())
	
def manhattan(l, r):
    '''
    :param l: The left value to compare
    :param r: The right value to compare
    :return: The distance between the two
    '''
    return np.abs(l - r).sum()
	
def chebyshev(l, r):
    '''
    :param l: The left value to compare
    :param r: The right value to compare
    :return: The distance between the two
    '''
    return np.abs(l - r).max()

def minkowski(l, r, p=1):
    '''
    :param l: The left value to compare
    :param r: The right value to compare
    :param p: The order of the equation (default 1)
    :return: The distance between the two
    '''
    return np.power(np.power(np.abs(l - r), p).sum(), 1.0/p)
