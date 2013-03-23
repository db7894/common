def is_rotation(string, rotation):
    ''' Given two strings, check if one is a rotation
    of the other.

    >>> is_rotation("helloworld", "worldhello")
    True
    >>> is_rotation("hello world", "world hello")
    False
    '''
    if not string and not rotation: return True
    if not string or not rotation: return False
    if len(string) != len(rotation): return False
    return rotation in (string + string)

if __name__ == "__main__":
    import doctest
    doctest.testmod()
