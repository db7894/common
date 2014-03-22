'''
.. todo:: wrap these
   - http://docs.scipy.org/doc/numpy/reference/routines.random.html
   - document the distributions (what do they all mean)
   - http://en.wikipedia.org/wiki/List_of_probability_distributions
'''
import random
import numpy as np
from bashwork.statistics.distribution import Distribution

class ContinuousDistribution(Distribution):
    ''' A simple wrapper around some continuous distribution
    generator to match the Distribution interface.
    '''

    def __init__(self, generator):
        ''' Initialize a new instance of the ContinuousDistribution class

        :param generator: The underlying random generator
        '''
        self.generator = generator

    def sample(self, count=1):
        ''' Sample the existing distribution the supplied
        number of times.

        :param count: The number of samples to retrieve (default 1)
        :returns: The requested number of samples
        '''
        samples = [self.generator() for _ in range(count)]
        return samples[0] if count == 1 else samples

    def is_valid(self):
        ''' Check if the underlying distribution correctly
        sums to 1 (we assume it does).

        :returns: always returns True
        '''
        return True

    def get(self, value): raise NotImplemented("get")
    def __str__(self): raise NotImplemented("str")

#------------------------------------------------------------
# Continuous Distributions
#------------------------------------------------------------

def constant(value):
    '''
    :param value: The value to always sample from
    '''
    return Distribution.create([value])

def uniform(low=0.0, high=1.0):
    '''
    :param low: The low value of the distribution
    :param high: The high value of the distribution
    '''
    return ContinuousDistribution(random.uniform(low, high))

def exponential(lam):
    '''
    :param lam: The lambda parameter for the distribution
    '''
    return ContinuousDistribution(random.expovariate(lam))

def gaussian(mu=0.0, sigma=1.0):
    '''
    The distribution is modeled as:

    .. math::

        f(x, \mu, \sigma) = \frac{1}{\sigma\sqrt{2\pi}} e^{ -\frac{(x-\mu)^2}{2\sigma^2} }

    :param mu: The mean of the distribution
    :param sigma: The standard deviation of the distribution
    '''
    return ContinuousDistribution(lambda: random.gauss(mu, sigma))

def vonmises(mu=0.0, kappa=1.0):
    '''

    :param mu: The mean angle in randians between 0 and 2*pi
    :param kappa: The concentration parameter
    '''
    return ContinuousDistribution(lambda: random.vonmisesvariate(mu, kappa))

def log(mu=0.0, sigma=1.0):
    '''

    :param mu: The mean of the distribution
    :param sigma: The standard deviation of the distribution
    '''
    return ContinuousDistribution(lambda: random.lognormvariate(mu, sigma))

def normal(mu=0.0, sigma=1.0):
    '''
    Some features of this distribution:

    * `\sigma^2` represents the variance of the distribution.
    * `\mu` is the mode, median, and mean
    * the curve is symmetrical around `\mu`

    The distribution is modeled as:

    .. math::

        f(x, \mu, \sigma) = \frac{1}{\sigma\sqrt{2\pi}} e^{ -\frac{(x-\mu)^2}{2\sigma^2} }

    :param mu: The mean of the distribution
    :param sigma: The standard deviation of the distribution
    '''
    return ContinuousDistribution(lambda: random.normalvariate(mu, sigma))

def beta(alpha, beta):
    '''

    :param alpha: The alpha parameter
    :param beta: The beta parameter
    '''
    return ContinuousDistribution(lambda: random.betavariate(alpha, beta))

def weibull(alpha, beta):
    '''

    :param alpha: The scale parameter
    :param beta: The shape parameter
    '''
    return ContinuousDistribution(lambda: random.weibullvariate(alpha, beta))

def pareto(alpha):
    '''

    :param alpha: The shape parameter
    '''
    return ContinuousDistribution(lambda: random.paretovariate(alpha))

def triangular(low=0.0, high=1.0, mode=None):
    '''

    :param low: The low value of the distribution (default 0.0)
    :param high: The high value of the distribution (default 1.0)
    :param mode: The mode between these bounds (default midpoint)
    '''
    return ContinuousDistribution(lambda: random.triangular(low, high, mode))

def gamma(alpha, beta):
    ''' The probability distribution function is::

                  x ** (alpha - 1) * math.exp(-x / beta)
        pdf(x) =  --------------------------------------
                      math.gamma(alpha) * beta ** alpha

    :param alpha: The alpha parameter
    :param beta: The beta parameter
    '''
    assert( alpha > 0 and beta > 0 )
    return ContinuousDistribution(lambda: random.gammavariate(alpha, beta))
