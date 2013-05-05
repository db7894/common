import math

#------------------------------------------------------------
# helper methods
#------------------------------------------------------------
def generate_bit_count_table(size):
    ''' Generates a bit count table of 0x00..0xFF
    '''
    t = [0x00 for _ in xrange(size)]
    for i in xrange(size):
        t[i] = (i & 1) + t[i / 2]
    return t


def generate_bit_index_table(size):
    ''' Generates a bit range table that can
    be used to mask off the specified bit
    given the supplied index.
    '''
    return [1 << i for i in xrange(size)]


def generate_bit_mask_table(size):
    ''' Generates a bit mask table of 0x00..0xFF
    '''
    t = [0x01 for _ in xrange(size)]
    for i in xrange(1, size):
        t[i] = (1 << i) | t[i - 1]
    return [0xFF] + t


#------------------------------------------------------------
# classes
#------------------------------------------------------------
class BitArray(object):
    ''' This is a convenience wrapper to manage all the
    neccessary bitwise operations on a collection of bits.

    It should be noted that all the magic dunder methods are
    implemented in terms of the bit operations.
    '''

    __msk_to_cnt = generate_bit_count_table(256)
    __off_to_bit = generate_bit_index_table(64)
    __off_to_msk = generate_bit_mask_table(64)

    def __init__(self, size=64, block=64, array=None):
        ''' Initialize a new instance of the bit array

        :param size: The initial size of the bit array (default 64 bits)
        :param block: The size of each array block (default byte)
        :param array: The initial array values (default None)
        '''
        self.__blk = max(8, block)             # need at least a byte
        self.__set = (2 << self.__blk - 1) - 1 # 0xff..ff
        self.__cls = 0x00L                     # 0x00..00
        self.__bpw = int(math.log(self.__blk, 2))
        self.array = array or [self.__cls] * (max(self.__blk, size) // self.__blk)

    #------------------------------------------------------------
    # internal helpers
    #------------------------------------------------------------
    def __word_index(self, bit):
        if bit < 0:
            raise IndexError("bit position < 0: %d" % bit)
        return bit >> self.__bpw, bit % self.__blk

    def __check_range(self, start, end):
        if start < 0: raise IndexError("start position < 0: %d" % start)
        if end < 0: raise IndexError("end position < 0: %d" % end)
        if start > end: raise IndexError("start[%d] > end[%d]" % (start, end))

    def __expand_array(self, size):
        if size >= len(self.array):
            size = max(len(self.array), size + 1 - len(self.array))
            self.array.extend([self.__cls] * size)

    def __compact(self):
        ''' Compact the underlying array removing the tail
        end cleared bits.
        '''
        idx = len(self.array)
        if self.array[idx - 1]: return    # cannot be compacted
        for word in reversed(self.array): # we expand to the right
            if word: break; idx -= 1      # until we find a non 0x00
        if idx != len(self.array):        # if we need to shrink
            self.array = self.array[:idx] # then shrink the right side

    #------------------------------------------------------------
    # information methods
    #------------------------------------------------------------
    def first_set_bit(self):
        ''' Return the index of the next set bit.

        :returns: The index of the next set bit.
        '''
        for idx, bits in enumerate(self.array):
            if bits:
                bit = int(math.log(bits & -bits, 2))
                return bit + (idx * self.__blk)
        return None # no bits are set

    def last_set_bit(self):
        ''' Return the index of the last set bit.

        :returns: The index of the last set bit.
        '''
        for idx in reversed(xrange(len(self.array))):
            bits = self.array[idx]
            if bits:
                bit = int(math.log(bits & -bits, 2))
                return bit + (idx * self.__blk)
        return None # no bits are set
        # TODO

    def first_clear_bit(self):
        ''' Return the index of the first cleared bit.

        :returns: The index of the first cleared bit.
        '''
        for idx, bits in enumerate(self.array):
            if bits != self.__set:
                bit = int(math.log(~bits & bits + 1, 2))
                return bit + (idx * self.__blk)
        return None # no bits are cleared

    def cardinality(self):
        ''' Returns the cardinality of the bit array.

        :returns: The number of set bits in the bit array.
        '''
        return sum(self.__msk_to_cnt[x] for x in self.array)

    def parity(self):
        ''' Return the parity of the bit array.

        - Even parity will be 0x0 
        - Odd  parity will be 0x1.

        :returns: The parity of the bit array
        '''
        return self.array[0] & 0x1

    def length_of_bits(self):
        ''' Returns the length of the currently used
        bit space (the last set bit)

        :returns: The current bit length
        '''
        bit = self.last_set_bit()
        return bit + 1 if bit else 0

    def length_of_bytes(self):
        ''' Returns the length of the currently
        allocated number of bytes.

        :returns: The current bytes length
        '''
        return len(self.array)

    def length_of_buffer(self):
        ''' Returns the length of the currently
        allocated buffer.

        :returns: The current buffer length
        '''
        return len(self.array) * self.__blk

    #------------------------------------------------------------
    # set methods
    #------------------------------------------------------------
    def set(self, pos):
        ''' Set the value at the specified bit
        position to 1.

        :param pos: The position to set the value for
        '''
        idx, off = self.__word_index(pos)
        self.__expand_array(idx)
        self.array[idx] |= self.__off_to_bit[off]

    def set_range(self, start, end):
        ''' Set the value at the specified bit
        position to 1.

        :param start: The position to start from
        :param end: The position to end at
        '''
        sidx, soff = divmod(start, self.__blk)
        eidx, eoff = divmod(end, self.__blk)
        if eidx >= len(self.array):
            self.array.extend([self.__set] * (eidx - len(self.array)))
        self.array[idx] |= self.__masks[off]
        #TODO

    def set_all(self):
        ''' Sets the value of all the underlying bits to 1.
        '''
        self.array = [self.__set] * len(self.array)

    #------------------------------------------------------------
    # clear methods
    #------------------------------------------------------------
    def clear(self, pos):
        ''' Set the value at the specified bit
        position to 0.

        :param pos: The position to set the value for
        '''
        idx, off = self.__word_index(pos)
        self.__expand_array(idx)
        self.array[idx] &= ~self.__off_to_bit[off] & self.__set
        self.__compact()

    def clear_range(self, start, end):
        ''' Sets the value of the bits in range to 0.

        :param start: The position to start from
        :param end: The position to end at
        '''
        sidx, soff = divmod(start, self.block)
        eidx, eoff = divmod(end, self.block)
        if eidx >= len(self.array):
            self.array.extend([self.__cls] * (eidx + 1 - len(self.array)))
        #TODO

    def clear_all(self):
        ''' Sets the value of all the underlying bits to 0.
        '''
        self.array = [self.__cls] # already compacted

    #------------------------------------------------------------
    # flip methods
    #------------------------------------------------------------
    def flip(self, pos):
        ''' Flip the value at the specified bit
        position.

        :param pos: The position to flip the value for
        '''
        idx, off = self.__word_index(pos)
        self.__expand_array(idx)
        self.array[idx] ^= self.__off_to_bit[off]
        self.__compact()

    def flip_range(self, start, end):
        ''' Flip the values in the specified bit range.

        :param start: The position to start from
        :param end: The position to end at
        '''
        self.__check_range(start, end)
        sidx, soff = self.__word_index(start)
        eidx, eoff = self.__word_index(end)
        self.__expand_array(eidx)

        smask = self.__off_to_msk[soff]            # mask for the first word
        smask = ~smask & self.__set                # mask for the first word
        emask = self.__off_to_msk[eoff]            # mask for the final word
        #print hex(smask), hex(emask)
        if sidx != eidx:                           # multiply multiple words
            self.array[sidx] ^= smask              # modify first word
            for idx in xrange(sidx + 1, eidx):     # skip first and last word
                self.array[idx] ^= self.__set      # modify all bits in between
            self.array[eidx] ^= emask              # modify last word
        else: self.array[sidx] ^= (smask & emask)  # modify single word
        self.__compact()
        #TODO

    def flip_all(self):
        ''' Flip the value of all the underlying bits.
        '''
        for idx in xrange(len(self.array)):
            self.array[idx] ^= self.__set
        self.__compact()

    #------------------------------------------------------------
    # get methods
    #------------------------------------------------------------
    def get(self, pos):
        ''' Get the value at the specified bit
        position.

        :param pos: The position to get the value for
        :returns: True if the value is set, false otherwise
        '''
        idx, off = self.__word_index(pos)
        val = self.array[idx] & self.__masks[off]
        return (val != self.__cls)

    def get_range(self, start, end):
        ''' Get the value at the specified bit
        position.

        :param start: The position to start from
        :param end: The position to end at
        :returns: The bit values in the specified range.
        '''
        sidx, soff = divmod(start, self.block)
        eidx, eoff = divmod(end, self.block)
        if sidx >= len(self.array): return None
        res = self.array[sidx] & self.__idx_to_msk[soff]
        res >>= soff
        end = min(len(self.array), eidx if not eoff else eidx - 1)
        for idx in xrange(sidx + 1, end):
            res  |= (self.array[idx] << soff)
            soff += self.block
        if eoff:
            res |= ((self.array[eidx] & self.__idx_to_msk[eoff]) << soff)
        return res
        #TODO

    #------------------------------------------------------------
    # format methods
    #------------------------------------------------------------
    def to_byte_string(self):
        ''' Return the bit array represented as a hex string

        :returns: The bit array represented as a hex string
        '''
        return '0x' + ''.join("{0:016X}".format(x) for x in
            reversed(self.array))

    def to_bit_string(self):
        ''' Return the bit array represented as a bit string

        :returns: The bit array represented as a bit string
        '''
        return '0b' + ''.join("{0:064b}".format(x) for x in
            reversed(self.array))

    def to_byte_list(self):
        ''' Return the bit array represented as a hex string

        :returns: The bit array represented as a hex string
        '''
        return list(self.array)

    def to_bit_list(self):
        ''' Return the bit array represented as a bit string

        :returns: The bit array represented as a bit string
        '''
        return list(iter_by_bit())

    def iter_by_byte(self):
        ''' Return the bit array represented as a hex string

        :returns: The bit array represented as a hex string
        '''
        return iter(self.array)

    def iter_by_bit(self):
        ''' Return the bit array represented as a bit string

        :returns: The bit array represented as a bit string
        '''
        for i in len(self): yield self.get(i)

    #------------------------------------------------------------
    # magic methods
    #------------------------------------------------------------
    def __iter__(self):           return self.iter_by_bit()
    def __reversed__(self):       return reversed(self.iter_by_bit())
    def __contains__(self, byte): return byte in self.array
    def __nonzero__(self):        return any(self.array)
    def __hash__(self):           return hash(str(self.array))
    def __repr__(self):           return self.to_byte_string()
    def __str__(self):            return self.to_byte_string()
    def __len__(self):            return self.length_of_bits()
    def __eq__(self, other):      return other and (other.array == self.array)
    def __ne__(self, other):      return other and (other.array != self.array)
    def __lt__(self, other):      return other and (other.array  < self.array)
    def __le__(self, other):      return other and (other.array <= self.array)
    def __gt__(self, other):      return other and (other.array  > self.array)
    def __ge__(self, other):      return other and (other.array >= self.array)
    def __and__(self, other):     return other and BitArray(self.__blk, array=[x & y for x,y in zip(self.array, other.array)]) 
    def __xor__(self, other):     return other and BitArray(self.__blk, array=[x ^ y for x,y in zip(self.array, other.array)]) 
    def __or__(self, other):      return other and BitArray(self.__blk, array=[x | y for x,y in zip(self.array, other.array)]) 
    def __invert__(self):         return BitArray(self.__blk, array=[~x & self.__set for x in self.array]) 
    def __neg__(self):            return BitArray(self.__blk, array=[-x & self.__set for x in self.array]) 
    def __pos__(self):            return self
