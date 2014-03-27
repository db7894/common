class Encoder(object):
    ''' Interface for an encoder that converts an
    integer into a string and a string back into an
    integer.
    '''

    def encode(self, value):
        ''' Given an integer value, encode it to 
        the underlying string representation.

        :param value: The value to encode
        :returns: The underlying encoding of that value
        '''
        raise NotImplemented("encode")

    def decode(self, string):
        ''' Given a string encoding, decode it
        back to its integer representation.

        :param string: The encoded string to decode
        :returns: The decoded integer representation of that string
        '''
        raise NotImplemented("decode")

class ExcelEncoder(Encoder):
    ''' An encoder that converts to and from
    the excel column encoding (base 26 with a twist).
    '''

    def encode(self, value):
        encoded = []
        while value:
            value, digit = divmod(value, 26)
            if digit == 0:
                encoded.insert(0, 'Z')
                value = value - 1
            else: encoded.insert(0, chr(64 + digit))
        return ''.join(encoded)

    def decode(self, string):
        convert = lambda t, n: (t * 26) + (ord(n) - ord('A') + 1)
        return reduce(convert, string, 0)

class EllisGammaEncoder(Encoder):

    def encode(self, value):
        string = bin(value)[2:]
        return ('0' * (len(string) - 1)) + string

    def decode(self, string):
        v = string.find('1')
        d = string[v:v + v + 1]
        return int(d, 2)

    def decode_stream(self, string):
        idx, values = 0, []
        while idx < len(string):
            v = string.find('1', idx)
            d = string[v:v + (v - idx) + 1]
            values.append(int(d, 2))
            idx += len(d) * 2 - 1
        return values
