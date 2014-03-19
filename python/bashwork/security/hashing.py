import hashlib
import zlib
import struct
try:
    import pyhash
except ImportError: pass

#------------------------------------------------------------
# Hash Utility Classes
#------------------------------------------------------------

class HashCode(object):
    ''' A convenience wrapper around a resulting hash digest
    that provides a number of magic methods and utility methods
    to work with the underlying digest.
    '''

    __slots__ = ['digest', 'digits']

    def __init__(self, digest):
        ''' Create a new instane of a HashCode. It is assumed
        that the supplied digest is a binary packed string.

        :param digest: The underlying digest of the hash
        '''
        if isinstance(digest, long):
            self.digest = struct.pack("L", digest)
            self.digits = digest
        else:
            self.digest = digest
            self.digits = sum(self.to_bytes())

    def to_b64_string(self): return self.digest.encode('base64')
    def to_hex_string(self): return self.digest.encode('hex')
    def to_string(self):     return self.digest
    def to_bytes(self):      return [ord(x) for x in self.digest]
    def to_hex_bytes(self):  return [x.encode('hex') for x in self.digest]
    def to_long(self):       return self.digits

    def __len__(this):       return len(this.digest)
    def __str__(this):       return this.digest
    def __repr__(this):      return this.digest
    def __eq__(this, that):  return this.digest == that.digest
    def __ne__(this, that):  return this.digest != that.digest
    def __lt__(this, that):  return this.digest  < that.digest
    def __le__(this, that):  return this.digest <= that.digest
    def __gt__(this, that):  return this.digest  > that.digest
    def __ge__(this, that):  return this.digest >= that.digest
    def __hash__(this):      return hash(this.digest)

#------------------------------------------------------------
# Hash Functions
#------------------------------------------------------------

class HashFunction(object):
    ''' An interface for a single hash function. This is a
    factory for the specific hash function that can be injected
    into a lower level system to create new instances of the
    requested hash function.
    '''

    def create(self):
        ''' Create a new instance of the hash function

        :returns: An initialized HashFunction object
        '''
        raise NotImplemented('create')

    def hash(self, value):
        ''' Given a value, quickly compute its hash with
        the underlying hash function.

        :param value: The value to get the hash of
        :returns: The final HashCode of the object
        '''
        return create().update(value).get_hash()

class GenericHashFunction(HashFunction):
    ''' A wrapper factory for a simple hash function that
    meets the following interface::
      
        hash = function(value1, seed)
        hash = function(value2, hash)
    '''

    def __init__(self, function, seed=None):
        ''' Create a new instance of the HashFunction

        :param function: The underlying hash function
        :param seed: The initial seed value for the hash
        '''
        self.function = function
        self.seed = seed or 0

    def create(self):
        ''' Create a new instance of the hash function

        :returns: An initialized HashFunction object
        '''
        return GenericHasher(self.function, self.seed)


class PythonHashFunction(HashFunction):
    ''' This is a wrapper for a single python hashlib hash
    function. Each instance of this type represents an ongoing
    hash.
    '''

    def __init__(self, name):
        ''' Create a new instance of a PythonHashFunction

        :param name: The name of the hashing function to use
        '''
        if callable(name):
            self.name = name.__name__
            self.factory = name
        elif name in hashlib.algorithms:
            self.name = name
            self.factory = hashlib.__dict__[name] # cached lookup
        else:
            self.name = name
            self.factory = lambda: hashlib.new(name)

    def create(self):
        ''' Create a new instance of the hash function

        :returns: An initialized PythonHashFunction object
        '''
        return PythonHasher(self.factory())

class PyHashHashFunction(HashFunction):
    ''' This is a wrapper for a single hash from the pyhash
    package.Each instance of this type represents an ongoing
    hash.
    '''

    def __init__(self, name, seed=None):
        ''' Create a new instance of a PyHashFunction

        :param name: The name of the hashing function to use
        :param seed: The initial seed value for the hash
        '''
        if callable(name):
            self.name = name.__name__
            self.factory = name
        else:
            self.name = name
            self.factory = pyhash.__dict__[name] # cached lookup
        self.seed = seed or 0

    def create(self):
        ''' Create a new instance of the hash function

        :returns: An initialized PythonHashFunction object
        '''
        return PyHashHasher(self.factory(), self.seed)

#------------------------------------------------------------
# Hasher Instances
#------------------------------------------------------------

class Hasher(object):
    ''' An interface for a single instance of a
    given hash type.
    '''

    def update(self, value):
        ''' Given a new value, update the underlying hash.

        :param value: The new value to update the hash with
        :returns: The current instance to chain with
        '''
        raise NotImplemented("update")

    def get_hash(self):
        ''' Retrieve the current hash value

        :returns: A HashCode object containing the hash value
        '''
        raise NotImplemented("digest")

class GenericHasher(Hasher):
    ''' A wrapper hasher for a simple hash function that
    meets the following interface::
      
        hash = function(value1, seed)
        hash = function(value2, hash)
    '''

    __slots__ = ['function', 'digest']

    def __init__(self, function, seed=None):
        ''' Initialize a new instance of the GenericHasher

        :param function: The hash function to hash with
        :param seed: The initial hash value to start with
        '''
        self.function = function
        self.digest   = self.function(seed or 0)

    def update(self, value):
        ''' Given a new value, update the underlying hash.

        :param value: The new value to update the hash with
        :returns: The current instance to chain with
        '''
        self.digest = self.function(value, self.digest)

    def get_hash(self):
        ''' Retrieve the current hash value

        :returns: A HashCode object containing the hash value
        '''
        return HashCode(self.digest)


class PythonHasher(Hasher):
    ''' This is a simple wrapper for the exisiting python
    hashlib methods to work with this hashing abstraction.
    This can be used as follows::

        # the following two are equivalent
        hasher = PythonHashFunction('md5').create()
        hasher = PythonHasher(hashlib.md5())
    '''

    __slots__ = ['hasher']

    def __init__(self, hasher):
        ''' Create a new instance of a PythonHasher

        :param hasher: The underlying hasher to work with
        '''
        self.hasher = hasher

    def update(self, value):
        ''' Given a new value, update the underlying hash.

        :param value: The new value to update the hash with
        :returns: The current instance to chain with
        '''
        self.hasher.update(value)

    def copy(self):
        ''' Given a current hasher instance, return a copy
        that can be used to compute another hash with the same
        initial prefix.

        :returns: A copy of the underlying hasher
        '''
        return PythonHasher(self.hasher.copy())

    def get_hash(self):
        ''' Retrieve the current hash value

        :returns: A HashCode object containing the hash value
        '''
        return HashCode(self.hashser.digest())

class PyHashHasher(Hasher):
    ''' A wrapper for hashes from the pyhash library.
    '''

    __slots__ = ['function', 'digest']

    def __init__(self, function, seed=None):
        ''' Initialize a new instance of the GenericHasher

        :param function: The hash function to hash with
        :param seed: The initial hash value to start with
        '''
        self.function = function
        self.digest   = seed

    def update(self, value):
        ''' Given a new value, update the underlying hash.

        :param value: The new value to update the hash with
        :returns: The current instance to chain with
        '''
        self.digest = self.function(value, seed=self.digest)

    def get_hash(self):
        ''' Retrieve the current hash value

        :returns: A HashCode object containing the hash value
        '''
        return HashCode(self.digest)

#------------------------------------------------------------
# Hash Abstract Factory
#------------------------------------------------------------

class Hashing(object):
    ''' A simple factory for creating all the available hashing
    functions. This can be used as follows::

        hasher = Hashing.md5().create()
    '''
    md2                = staticmethod(lambda: PythonHashFunction('md2'))
    md4                = staticmethod(lambda: PythonHashFunction('md4'))
    md5                = staticmethod(lambda: PythonHashFunction('md5'))
    sha1               = staticmethod(lambda: PythonHashFunction('sha1'))
    sha224             = staticmethod(lambda: PythonHashFunction('sha224'))
    sha256             = staticmethod(lambda: PythonHashFunction('sha256'))
    sha384             = staticmethod(lambda: PythonHashFunction('sha384'))
    sha512             = staticmethod(lambda: PythonHashFunction('sha512'))
    rmd160             = staticmethod(lambda: PythonHashFunction('rmd160'))
    crc32              = staticmethod(lambda: GenericHashFunction(zlib.crc32))
    adler32            = staticmethod(lambda: GenericHashFunction(zlib.adler32))
    lookup3            = staticmethod(lambda: PyHashHashFunction('lookup3'))
    fnv1_32            = staticmethod(lambda: PyHashHashFunction('fnv1_32'))
    fnv1a_32           = staticmethod(lambda: PyHashHashFunction('fnv1a_32'))
    fnv1_64            = staticmethod(lambda: PyHashHashFunction('fnv1_64'))
    fnv1a_64           = staticmethod(lambda: PyHashHashFunction('fnv1a_64'))
    murmur1_32         = staticmethod(lambda: PyHashHashFunction('murmur1_32'))
    murmur1_aligned_32 = staticmethod(lambda: PyHashHashFunction('murmur1_aligned_32'))
    murmur2_32         = staticmethod(lambda: PyHashHashFunction('murmur2_32'))
    murmur2a_32        = staticmethod(lambda: PyHashHashFunction('murmur2a_32'))
    murmur2_aligned_32 = staticmethod(lambda: PyHashHashFunction('murmur2_aligned_32'))
    murmur2_neutral_32 = staticmethod(lambda: PyHashHashFunction('murmur2_neutral_32'))
    murmur2_x64_64a    = staticmethod(lambda: PyHashHashFunction('murmur2_x64_64a'))
    murmur2_x86_64b    = staticmethod(lambda: PyHashHashFunction('murmur2_x86_64b'))
    murmur3_32         = staticmethod(lambda: PyHashHashFunction('murmur3_32'))
    lookup3            = staticmethod(lambda: PyHashHashFunction('lookup3'))
    lookup3_little     = staticmethod(lambda: PyHashHashFunction('lookup3_little'))
    lookup3_big        = staticmethod(lambda: PyHashHashFunction('lookup3_big'))
    super_fast_hash    = staticmethod(lambda: PyHashHashFunction('super_fast_hash'))

    @classmethod
    def algorithms(klass):
        ''' Lists all the currently available algorithms
        to choose from.

        :returns: A list of all the available algorithms
        '''
        return [method for method in dir(klass)
            if not method.startswith('__') and method != "algorithms"]
