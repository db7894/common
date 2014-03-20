import random
from collections import Counter

def histogram(iterable):
    ''' Given an iterable, create a histogram of
    the values in the iterable.
    
    :param iterable: The iterable to create a histogram of
    :returns: A counter histogram of the iterable
    '''
    return Counter(iterable)

def coin_flip(probability):
    ''' Given a probability, flip a coin and
    see if it comes up heads.

    :param probability: The probability of heads
    :returns: True if heads, False otherwise
    '''
    return random.random() < probability

def normalize(mapping):
    ''' Given a mapping, normalize all the values
    so that they all sum to 1.

    :param mapping: The mapping to normalize
    :returns: The normalized mapping
    '''
    total = float(sum(mapping.values()))
    assert ( total > 0.0 )
    return { k: v / total for k, v in mapping.items() }
