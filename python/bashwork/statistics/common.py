'''
- joint distribution
  - store is np array or pandas table
- joint -> marginal distribution
  - reduce by one to another joint
  - reduce to one as a Distribution
- conditional distribution P(a|b) = P(a,b) / P(b)
  - normalization trick
'''
import random
from collections import Counter
import numpy as np
import pandas as pd
from bashwork.statistics.utilities import *

class TruthTable(object):
    '''
    '''

    def __init__(self, table):
        self.table = table

    def __str__(self):
        pass # print out table

class Distribution(object):
    ''' Models a probability distribution
    of a given set of variables::

        P(X)
        ------------
        P(x1) -> 0.5
        P(x2) -> 0.5
    '''

    @classmethod
    def create(klass, values, name=None):
        ''' Given a collection of values, create a
        distribution of those values with the supplied name
        of the variable.

        :param values: The values to create a distribution of
        :param name: The name of the variable
        :returns: An initialized distribution over those values
        '''
        counts = normalize(Counter(values))
        counts = pd.Series(counts, name=name or 'X')
        return klass(counts)

    def __init__(self, table):
        ''' Initialize a new instance of the Distribution class

        :param table: A mapping of value to its probability
        :param name: The name of the distribution variable
        '''
        self.table  = table
        self.cumsum = self.table.cumsum()

    def sample(self, count=1):
        ''' Sample the existing distribution the supplied
        number of times.

        :param count: The number of samples to retrieve (default 1)
        :returns: The requested number of samples
        '''
        rands = sorted(random.random() for _ in range(count))
        samid, ranid = 0, 0
        results = []
        while len(results) < count:
            if rands[ranid] < self.cumsum[samid]:
                results.append(self.cumsum.index[samid])
                ranid += 1
            else: samid += 1
        return results[0] if count == 1 else results

    def is_valid(self):
        ''' Check if the underlying distribution correctly
        sums to 1.

        :returns: True if the values sum to 1, else False
        '''
        return ((self.table >= 0.0).all()
           and (0.999 <= self.table.sum() <= 1.0))

    def get(self, *args, **kwargs):
        ''' Retrieve the probability of the specified conditions::

            dist.get(1)      # probability that X is 1
            dist.get(1,2,3)  # probability that X is 1, 2, or 3
            dist.get(X=3)    # probability that X is 3

        :param args: The variables to test
        :param kwargs: The named variable to test (only matching name)
        :returns: The joint probability of the variables
        '''
        keys = list(args)
        if self.table.name in kwargs:
            keys.append(kwargs[self.table.name])
        return self.table.ix[keys].sum()

    def __str__(self):
        ''' Output a table representing the probability
        distribution of this variable.
        '''
        output = ['P({})'.format(self.table.name)]
        for key, prob in self.table.iteritems():
            output.append('P({}) -> {:.2f}'.format(key, prob))
        return '\n'.join(output)

    __repr__ = __str__

class JointDistribution(object):
    '''
    '''

    @classmethod
    def create(klass, values, names=None):
        '''
        '''
        count = Counter(values)
        total = float(sum(count.values()))
        value = { k: v / total for k, v in count.items() }
        return klass(value, names)

    def __init__(self, table, names=None):
        ''' Initialize a new instance of the JointDistribution class

        :param table: A mapping of value to its probability
        '''
        #TODO [x][y]...[z]
        self.table = table
        self.count = 0
        self.names = names or [chr(65 + i) for i in range(self.count)]
        self.names = { n:i for i, n in enumerate(names) }

    def is_valid(self):
        ''' Check if the underlying distribution correctly
        sums to 1.

        :returns: True if the values sum to 1, else False
        '''
        # TODO
        return (all(v >= 0 for v in self.table.values())
           and (0.999 <= sum(self.table.values()) <= 1.0))

    def get(self, *args, **kwargs):
        ''' Retrieve the probability of the specified conditions::

            dist.get(1)      # probability that X is 1
            dist.get(1,2,3)  # probability that X is 1, 2, or 3
            dist.get(X=3)    # probability that X is 3

        :param args: The variables to test
        :param kwargs: The named variable to test (only matching name)
        :returns: The joint probability of the variables
        '''
        keys = zip(self.names, args) if args else kwargs
        if self.name in kwargs: keys += (kwargs[self.name],)
        return sum(self.table[k] for k in keys)

    def __str__(self):
        ''' Output a table representing the probability
        distribution of this variable.
        '''
        #TODO [x][y]...[z]
        output = ['P({})'.format(self.name)]
        for key, prob in self.table.items():
            output.append('P({}) -> {:.2f}'.format(key, prob))
        return '\n'.join(output)

    __repr__ = __str__
