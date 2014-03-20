'''
.. todo:: wrap these
   - http://docs.scipy.org/doc/numpy/reference/routines.random.html
   - http://docs.python.org/2/library/random.html
'''
import random
import numpy as np
from bashwork.statistics.common import Distribution

class GenericDistribution(Distribution):
    ''' A simple wrapper around some random generator
    to match the Distribution interface.
    '''

    def __init__(self, method):
        ''' Initialize a new instance of the GenericDistribution class

        :param method: The underlying random generator
        '''
        self.method = method

    def sample(self, count=1):
        ''' Sample the existing distribution the supplied
        number of times.

        :param count: The number of samples to retrieve (default 1)
        :returns: The requested number of samples
        '''
        samples = [self.method() for _ in range(count)]
        return samples[0] if count == 1 else samples

    def is_valid(self):
        ''' Check if the underlying distribution correctly
        sums to 1 (we assume it does).

        :returns: always returns True
        '''
        return True


class Distributions(object):
    ''' A collection of factory methods for creating
    a number of common distributions.
    '''

    @staticmethod
    def always(value):
        return Distribution.create([value])

    @staticmethod
    def uniform():
        return GenericDistribution(random.random)

    @staticmethod
    def gaussian(mu=0.0, sigma=1.0):
        '''

        :param mu: The mean of the distribution
        :param sigma: The standard deviation of the distribution
        '''
        return GenericDistribution(lambda: random.gauss(mu, sigma))

    @staticmethod
    def normal(mu=0.0, sigma=1.0):
        '''

        :param mu: The mean of the distribution
        :param sigma: The standard deviation of the distribution
        '''
        return GenericDistribution(lambda: random.normalvariate(mu, sigma))

    @staticmethod
    def beta(alpha, beta):
        '''

        :param alpha: The alpha parameter
        :param beta: The beta parameter
        '''
        return GenericDistribution(lambda: random.betavariate(alpha, beta))
