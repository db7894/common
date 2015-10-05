
def __gray_internal(current, size):
    '''
    '''
    size = size - 1
    if size <= 0: return current
    clone = [x[:] for x in reversed(current)]
    for item in current: item.insert(0,0)
    for item in clone: item.insert(0,1)
    return __gray_internal(list(current + clone), size)

def generate_gray(size):
    '''
    '''
    return __gray_internal([[0],[1]], size)

def generate_subsets(items):
    '''
    '''
    for mask in generate_gray(len(items)):
        yield [item for item,flag in zip(items, mask) if flag]

def generate_cond_subsets(items, condition):
    '''
    '''
    result = generate_subsets(items)
    return (r for r in result if condition(r))

def generate_size_subsets(items, size):
    '''
    '''
    return generate_cond_subsets(items,
        lambda s: len(s) == size)

def next_gray_code(value):
    ''' Given a value, return the next gray code
    for that value.

    :param value: The value to manipulate
    :returns: The next gray code for that value
    '''
    return value ^ (value >> 1)
