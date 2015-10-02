def bit_length(value):
    ''' Given a value, determine the
    length of the bits

    >>> bit_length(0x80)
    8
    >>> bit_length(0x11)
    5
    '''
    v, l = value, 0
    while v:
        v >>= 1
        l += 1
    return l

def bit_count_sub(v):
    ''' Given a value, determine the
    number of bits set on it

    >>> bit_count_sub(0xff)
    8
    >>> bit_count_sub(0x11)
    2
    '''
    c = 0
    while v:
        v &= v - 1
        c += 1
    return c

def bit_parity(v):
    ''' Given a value, determine if it has
    even or odd parity.

    >>> bit_parity(0xff)
    0
    >>> bit_parity(0x10)
    1
    '''
    return bit_count_sub(v) & 1

def lowest_bit(v):
    ''' Given a value, determine the lowest
    bit set.

    >>> lowest_bit(0x00)
    0
    >>> lowest_bit(0xff)
    1
    >>> lowest_bit(0x10)
    5
    '''
    return bit_length(v & -v)

def bit_count_shift(v):
    ''' Given a value, determine the
    number of bits set on it

    >>> bit_count_shift(0xff)
    8
    >>> bit_count_shift(0x11)
    2
    '''
    c = 0
    while v:
        c += (v & 0x01)
        v >>= 1
    return c

def bit_count_table(v):
    ''' Given a value, determine the
    number of bits set on it

    >>> bit_count_table(0xff)
    8
    >>> bit_count_table(0x11)
    2
    '''
    t = [0x00 for _ in range(256)]
    for i in range(256):
        t[i] = (i & 1) + t[i / 2]
    return t[v]

def swap_even_and_odd_bits(value):
    ''' Given a value, swap the even and odd bits
    in that value.

    >>> value = swap_even_and_odd_bits(0x01020304)
    >>> hex(value)
    '0x2010308'
    
    :param value: The value to swap the bits of
    :returns: The value with swapped bits.
    '''
    return (((value & 0xAAAAAAAA) >> 1)
          | ((value & 0x55555555) << 1))

def find_missing_integer(array):
    ''' Given a collection of integers, attempt to find
    the missing integer if one is only allowed to look at
    a single bit each time they look at a single integer.

    >>> array = range(101)
    >>> _ = array.pop(50)
    >>> find_missing_integer(array)
    50

    :param array: The array of integers to examine
    :returns: The missing integer
    '''
    def recurse(coll, bit):
        if not coll: return 0 # no more values

        mask, es, os = 0x1 << bit, [], [] # mask, evens, odds

        for bits in coll:                              # for the current collection
            (os if (mask & bits) else es).append(bits) # if the low bit is set, add to odds

        coll, step = (es, 0) if len(os) >= len(es) else (os, 1)
        return (recurse(coll, bit + 1) << 1) | step    # choose the smaller collection and next bit
    return recurse(array, 0)                           # start with bit 0 and the whole collection

#------------------------------------------------------------
# common bit operations
#------------------------------------------------------------

class Bit(object):

    @staticmethod
    def test(value, offset):
        return value & (1 << offset)

    @staticmethod
    def set(value, offset):
        return value | (1 << offset)

    @staticmethod
    def unset(value, offset):
        return value & ~(1 << offset)

    @staticmethod
    def toggle(value, offset):
        return value ^ (1 << offset)

if __name__ == "__main__":
    import doctest
    doctest.testmod()
