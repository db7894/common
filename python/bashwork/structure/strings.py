from collections import defaultdict
from collections import Counter


def reverse_string(string):
    ''' Reverse a string treating it like a c-style
    string (null terminated array). We simulate this
    by converting the immutable string to a python
    list and then back to a string after reversing.

    :param string: The string to reverse
    :returns: The reversed string
    '''
    sp, ep, xs = 0, len(string) - 1, list(string)
    while sp < ep:
        xs[sp], xs[ep] = xs[ep], xs[sp]
        sp, ep = sp + 1, ep - 1
    return ''.join(xs)


def are_chars_unique(string):
    ''' Given a string, determine if all the characters
    in the string are unique.

    :param string: The string to test.
    :returns: True if the characters are unique, False otherwise
    '''
    return len(string) == len(set(string))

def are_chars_unique_no_space(string):
    ''' Given a string, determine if all the characters
    in the string are unique.

    :param string: The string to test.
    :returns: True if the characters are unique, False otherwise
    '''
    for i in range(len(string)):
        for j in range(i + 1, len(string)):
            if string[i] == string[j]:
                return False
    return True


def are_chars_unique_no_space2(string):
    ''' Given a string, determine if all the characters
    in the string are unique.

    :param string: The string to test.
    :returns: True if the characters are unique, False otherwise
    '''
    string = sorted(string)
    stream = zip(iter(string), iter(string[1:]))

    for prev, curr in stream:
        if prev == curr:
            return False
    return True


def are_chars_unique_array(string):
    ''' Given a string, determine if all the characters
    in the string are unique using a lookup table.

    :param string: The string to test.
    :returns: True if the characters are unique, False otherwise
    '''
    chars = [False for _ in range(255)]
    for char in string:
        if chars[ord(char)]:
            return False
        chars[ord(char)] = True
    return True


def are_strings_anagrams(string1, string2):
    ''' Given two strings, check if they are anagrams of
    each other.

    :param string1: The first string to test.
    :param string2: The second string to test.
    :returns: True if the strings are anagrams, False otherwise
    '''
    return sorted(string1) == sorted(string2)


def are_strings_anagrams_array(string1, string2):
    ''' Given two strings, check if they are anagrams
    using a lookup table.

    :param string1: The first string to test.
    :param string2: The second string to test.
    :returns: True if the strings are anagrams, False otherwise
    '''
    if len(string1) != len(string2):
        return False

    chars = [0 for _ in range(255)]
    for idx in range(0, len(string1)):
        chars[ord(string1[idx])] += 1
        chars[ord(string2[idx])] -= 1
    return all(char == 0 for char in chars)


def get_all_anagrams(word, dictionary):
    ''' Given a dictionary of available words
    return all the anagrams of a specified word.

    :params word: The word to get all the anagrams for
    :params dictionary: All the available words to check
    :returns: All the available anagrams for that word
    '''
    anagrams = defaultdict(list)
    for entry in dictionary:
        anagrams[str(sorted(entry))].append(entry)
    return anagrams[str(sorted(word))]


def is_rotation(string, rotation):
    ''' Given two strings, check if one is a rotation
    of the other.

    :param string: The original string to source from
    :param rotation: The string to check as a rotation
    :returns: True if it is a rotation, False otherwise
    '''
    if not string and not rotation: return True
    if not string or not rotation: return False
    if len(string) != len(rotation): return False
    return rotation in (string + string)


def string_set_cover(letters, string):
    ''' Given a set of letters and a string, find the
    minimum range in the string that covers the set of
    letters.

    :param letters: The set of letters to cover
    :param string: The string to cover with the letter set
    :returns: The smallest range of (index, length)
    '''
    letters  = set(letters)                      # set to cover
    counter  = Counter(string[0])                # current letter counter state
    answer   = (float('inf'), None)              # current best answer
    l, r     = (0, 0)                            # left, right pointers
    violated = set(letters)                      # current uncovered letters

    while r < len(string):                       # while our range is less than the word
        if not violated:                         # if we are corrently covered
            answer = min(answer, (r - l + 1, l)) # see if we are the smallest cover range
            counter.subtract(string[l])          # remove this character from our counter
            if string[l] in letters and counter[string[l]] == 0: # if our range no longer covers the set
                violated.add(string[l])          # add this char to the set of uncovered
            l += 1                               # increaes the left hand range
        else:                                    # otherwise we have to cover the set
            r += 1                               # keep moving right until
            if r < len(string):                  # we are longer than the string
                counter.update(string[r])        # keep a count of how many of char has been seen
                if string[r] in violated:        # if this char is currently not covered
                    violated.remove(string[r])   # remove it as it now is
    return answer


def ransom_note(note, magazines):
    ''' Given a ransom note to write and a number of
    magazines, check if we can use the magazines to
    write the note.

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


def front_zero_pad(string, length):
    ''' Given a numbeic string, pad it with zeros
    until it is the requested length. Note, a handy
    way to do this in java is::

       String zeros = "000000";
       return zeros.substring(s.length()) + s;

    :param string: The string to front pad with zeros
    :param length: The resulting length of the string
    :returns: The string front-padded with zeros
    '''
    return "0" * (length - len(string)) + string


def string_to_int(string, radix=10):
    ''' Given a string, convert that into a number.

    :param radix: The radix of the number
    :param string: The string to convert to a number.
    :returns: The string as an integer
    '''
    curr, sign = 0, -1 if string[0] == '-' else 1
    if string[0] == '-': string = string[1:]

    for c in string:
        #curr = (curr * radix) + (ord(c) - 48)
        curr = (curr * radix) + int(c, radix)
    return sign * curr


def int_to_string(value, radix=10):
    ''' Given an integer value, convert that number
    to a string.

    :param radix: The current base of the integer
    :param value: The value to convert to a string
    :returns: The value as a string
    '''
    string, negative = [], (value < 0)
    value = abs(value)

    while value:
        string.insert(0, chr(48 + value % radix))
        value /= radix

    if negative: string.insert(0, '-')
    return ''.join(string)


def all_parenthesis_sets(size=3):
    ''' Given a size, produce all the valid sets
    of open and closed parenthesis.

    :param size: The number of paren sets allowed
    :returns: A set of all the valid sets
    '''
    if size == 0: return set([''])
    sets = all_parenthesis_sets(size - 1)
    news = [['(' + x + ')', '()' + x, x + '()'] for x in sets]
    sets = set(entry for new in news for entry in new)
    return sets

#def all_parenthesis_sets_backtrack(size=3):
#    ''' Given a size, produce all the valid sets
#    of open and closed parenthesis. This version reduces
#    the amount of storage needed by backtracking and
#    yielding using the same list (strings are immutable).
#
#    :param size: The number of paren sets allowed
#    :returns: A set of all the valid sets
#    '''
#    def recurse(os, cs, string, index):
#        if os == 0 and cs == 0:
#            yield string
#        else:
#            if os > 0:
#                string[index] = '('
#                yield from recurse(os - 1, cs, string, index + 1)
#            if cs > os:
#                string[index] = ')'
#                yield from recurse(os, cs - 1, string, index + 1)
#
#    recurse(size, size, [''] * size * 2, 0)
