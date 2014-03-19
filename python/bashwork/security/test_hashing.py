#!/usr/bin/env python
import unittest
from bashwork.security.hashing import *

class HashingTest(unittest.TestCase):
    ''' Code to test the hashing abstraction library '''

    def test_hashing_factory_algorithms(self):
        ''' test that all the hashing factory algorithms work '''
        for algorithm in Hashing.algorithms():
            function = Hashing.__dict__[algorithm].__get__(None, Hashing)
            self.assertTrue(callable(function))
            self.assertIsInstance(function(), HashFunction)

    def test_python_hash_loop(self):
        ''' test that the python hashing loop works correctly '''
        method  = Hashing.md5()
        hasher  = method.create()
        hasher2 = hasher.update("something")
        hashed  = hasher.get_hash()
        quick   = method.hash("something")

        self.assertEquals(hasher, hasher2)
        self.assertEquals(hashed.digest, 'C{\x93\r\xb8K\x80y\xc2\xdd\x80Jq\x93k_')
        self.assertEquals(hashed.digest, quick.digest)

        self.assertIsInstance(method, HashFunction)
        self.assertIsInstance(hasher, Hasher)
        self.assertIsInstance(hashed, HashCode)

    def test_pyhash_hash_loop(self):
        ''' test that the pyhash loop works correctly '''
        method  = Hashing.murmur3_32()
        hasher  = method.create()
        hasher2 = hasher.update("something")
        hashed  = hasher.get_hash()
        quick   = method.hash("something")

        self.assertEquals(hasher, hasher2)
        self.assertEquals(hashed.digest, 'U\x7f\xdf\xf6\x00\x00\x00\x00')
        self.assertEquals(hashed.digest, quick.digest)

        self.assertIsInstance(method, HashFunction)
        self.assertIsInstance(hasher, Hasher)
        self.assertIsInstance(hashed, HashCode)

    def test_generic_hash_loop(self):
        ''' test that the generic hash algorithm loop works correctly '''
        method  = Hashing.crc32()
        hasher  = method.create()
        hasher2 = hasher.update("something")
        hashed  = hasher.get_hash()
        quick   = method.hash("something")

        self.assertEquals(hasher, hasher2)
        self.assertEquals(hashed.digest, '\xfb1\xda\t\x00\x00\x00\x00')
        self.assertEquals(hashed.digest, quick.digest)

        self.assertIsInstance(method, HashFunction)
        self.assertIsInstance(hasher, Hasher)
        self.assertIsInstance(hashed, HashCode)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
