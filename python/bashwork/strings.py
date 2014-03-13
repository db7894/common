from collections import Counter

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

def string_set_cover(letters, string):
    ''' Given a set of letters and a string, find the
    minimum range in the string that covers the set of
    letters.

    >>> letters = 'abcd'
    >>> string  = 'axkekfbabxxxcdkeckdabyycd'
    >>> string_set_cover(letters, string)
    (5, 16)

    :param letters: The set of letters to cover
    :param string: The string to cover with the letter set
    :returns: The smallest range of (index, length)
    '''
    letters  = set(letters)         # set to cover
    counter  = Counter(string[0])   # letter counter state
    answer   = (float('inf'), None) # current best answer
    l, r     = (0, 0)               # left, right pointers
    violated = set(letters)         # current violations

    while r < len(string):
        if not violated:
            answer = min(answer, (r - l + 1, l))
            counter.subtract(string[l])
            if string[l] in letters and counter[string[l]] == 0:
                violated.add(string[l])
            l += 1
        else:
            r += 1
            if r < len(string):
                counter.update(string[r])
                if string[r] in violated: violated.remove(string[r])
    return answer

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
