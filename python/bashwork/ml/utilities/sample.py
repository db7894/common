import numpy as np

def everything(xs):
    ''' Given a collection of values,
    return all of the values as the sample.

    :param xs: The dataset to sample from
    :returns: The entire dataset
    '''
    return xs

def random(xs, count):
    ''' Given a collection of values,
    return all of the values as the sample.

    :param xs: The dataset to sample from
    :returns: The entire dataset
    '''
    return xs[np.random.random_integers(0, len(xs) - 1, count)]
