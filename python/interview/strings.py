def is_rotation(string, rotation):
    ''' Given two strings, check if one is a rotation
    of the other.
    '''
    if not string and not rotation: return True
    if not string or not rotation: return False
    if len(string) != len(rotation): return False
    return rotation in (string + string)
