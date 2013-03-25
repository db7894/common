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


def ransom_note(note, magazines):
    ''' Given a ransom note to write and a number of
    magazines, check if we can use the magazines to
    write the note.

    >>> note = "we have your computer"
    >>> magazines = "comping a theater ticket wearing a very nice puntour"
    >>> ransom_note(note, magazines)
    True

    :params note: The note we need to write
    :params magazines: The letters we have available
    :returns: True if you can write the magazine, False otherwise
    '''
    counts  = {chr(a):0 for a in range(256)}
    letters = iter(magazines)
    for entry in note:
        if counts.get(entry) > 0:
            counts[entry] -= 1
            continue
        try:
            while True:
                letter = letters.next()
                if letter == entry: break
                else: counts[letter] += 1
        except StopIteration: return False
    return True


if __name__ == "__main__":
    import doctest
    doctest.testmod()
