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
        raise NotImplementedError("encode")

    def decode(self, string):
        ''' Given a string encoding, decode it
        back to its integer representation.

        :param string: The encoded string to decode
        :returns: The decoded integer representation of that string
        '''
        raise NotImplementedError("decode")

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

class RunLengthEncoder(Encoder):
    ''' An encoder that will attempt to shorten a given
    string by putting a numeric prefix in front of repeated
    letters.
    '''

    def encode(self, string):
        count, encoded = 1, []
        for i in range(1, len(string)):
            if string[i] != string[i - 1]:
                if count != 1:
                    encoded.append(str(count))
                    count = 1
                encoded.append(string[i - 1])
            else: count += 1
        encoded.append(string[-1])    
        return ''.join(encoded)

    def decode(self, string):
        count, decoded = 0, []
        for char in string:
            if '0' <= char <= '9':
                count = (count * 10) + (ord(char) - ord('0'))
            else:
                decoded.extend([char] * (count if count > 0 else 1))
                count = 0
        return ''.join(decoded)
