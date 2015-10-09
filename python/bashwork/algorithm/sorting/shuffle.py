from random import randint

def shuffle(coll):
    ''' Given a collection, randomize
    it based on the fisher yates randomization
    scheme by reference.

    :param coll: The collection to shuffle
    :returns: The existing list shuffled
    '''
    for curr in range(len(coll) - 1, 0, -1):
        swap = randint(0, curr)
        coll[swap], coll[curr] = coll[curr], coll[swap]
    return coll

def shuffle_copy(coll):
    ''' Given a collection, randomize
    it based on the fisher yates randomization
    scheme by copy.

    :param coll: The collection to shuffle
    :returns: The new shuffled list
    '''
    return shuffle(list(coll))
