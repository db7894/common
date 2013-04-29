def __bit_count_table():
    ''' Generates a bit count table of 0x00..0xFF
    '''
    t = [0x00 for _ in range(256)]
    for i in range(256):
        t[i] = (i & 1) + t[i / 2]
    return t

def __bit_mask_table():
    ''' Generates a bit mask table of 0x00..0xFF
    '''
    return [pow(2, i) for i in range(16)]

class BitArray(object):

    __counts = __bit_count_table()
    __masks  = __bit_mask_table()

    def __init__(self, size=64, block=16, array=None):
        '''
        '''
        self.block = block
        self.array = array or [0] * (size // self.block)

    def cardinality(self):
        ''' Returns the cardinality of the bit array.

        :returns: The number of set bits in the bit array.
        '''
        return sum(self.__counts[x] for x in self.array)

    def set(self, pos):
        ''' Set the value at the specified bit
        position to 1.

        :param pos: The position to set the value for
        '''
        idx, off = divmod(pos, self.block)
        if idx > len(self.array):
            self.array.extend([0x00] * (idx - len(self.array)))
        self.array[idx] |= self.__masks[off]

    def set_range(self, pos):
        ''' Set the value at the specified bit
        position to 1.

        :param pos: The position to set the value for
        '''
        idx, off = divmod(pos, self.block)
        if idx > len(self.array):
            self.array.extend([0x00] * (idx - len(self.array)))
        self.array[idx] |= self.__masks[off]

    def clear(self, pos):
        ''' Set the value at the specified bit
        position to 0.

        :param pos: The position to set the value for
        '''
        idx, off = divmod(pos, self.block)
        if idx > len(self.array):
            self.array.extend([0x00] * (idx - len(self.array)))
        self.array[idx] &= ~self.__masks[off]

    def clear_range(self, start, end):
        ''' Sets the value of the bits in range to 0.

        :param start: The position to start from
        :param end: The position to end at
        '''
        sidx, soff = divmod(start, self.block)
        eidx, eoff = divmod(end, self.block)
        if eidx > len(self.array):
            self.array.extend([0x00] * (eidx - len(self.array)))
        pass

    def clear_all(self):
        ''' Sets the value of all the underlying bits to 0.
        '''
        self.array = [0x00] * len(self.array)

    def flip(self, pos):
        ''' Flip the value at the specified bit
        position.

        :param pos: The position to flip the value for
        '''
        idx, off = divmod(pos, self.block)
        if idx > len(self.array):
            self.array.extend([0x00] * (idx - len(self.array)))
        self.array[idx] ^= self.__masks[off]

    def flip_range(self, start, end):
        ''' Flip the values in the specified bit range.

        :param start: The position to start from
        :param end: The position to end at
        '''
        pass

    def get(self, pos):
        ''' Get the value at the specified bit
        position.

        :param pos: The position to get the value for
        :returns: True if the value is set, false otherwise
        '''
        idx, off = divmod(pos, self.block)
        if idx > len(self.array): return 0 # out of range
        v = self.array[idx] & self.__masks[off]
        return (v == 1)

    def get_range(self, start, end):
        ''' Get the value at the specified bit
        position.

        :param start: The position to start from
        :param end: The position to end at
        :returns: The bit values in the specified range.
        '''
        pass

    def __eq__(self, other):   return other and (other.array == self.array)
    def __ne__(self, other):   return other and (other.array != self.array)
    def __lt__(self, other):   return other and (other.array  < self.array)
    def __le__(self, other):   return other and (other.array <= self.array)
    def __gt__(self, other):   return other and (other.array  > self.array)
    def __ge__(self, other):   return other and (other.array >= self.array)
    def __hash__(self):        return hash(self.array)
    def __repr__(self):        return str(self.array)
    def __str__(self):         return str(self.array)
    def __len_(self):          return len(self.array) * self.block
    def __invert__(self):      return BitVector(self.block, [~x for x in self.array]) 
    def __and__(self, other):  return other and BitVector(self.block, [x & y for x,y in zip(self.array, other.array)]) 
    def __xor__(self, other):  return other and BitVector(self.block, [x ^ y for x,y in zip(self.array, other.array)]) 
    def __or__(self, other):   return other and BitVector(self.block, [x | y for x,y in zip(self.array, other.array)]) 
