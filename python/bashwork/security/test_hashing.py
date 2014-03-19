#!/usr/bin/env python
import unittest
from bashwork.security.hashing import *

class HashingTest(unittest.TestCase):

    def test_hashing_factory(self):
        ''' test that all the hashing factory methods work '''
        self.assertIsInstance(Hashing.md2()                , PythonHashFunction)
        self.assertIsInstance(Hashing.md4()                , PythonHashFunction)
        self.assertIsInstance(Hashing.md5()                , PythonHashFunction)
        self.assertIsInstance(Hashing.sha1()               , PythonHashFunction)
        self.assertIsInstance(Hashing.sha224()             , PythonHashFunction)
        self.assertIsInstance(Hashing.sha256()             , PythonHashFunction)
        self.assertIsInstance(Hashing.sha384()             , PythonHashFunction)
        self.assertIsInstance(Hashing.sha512()             , PythonHashFunction)
        self.assertIsInstance(Hashing.rmd160()             , PythonHashFunction)
        self.assertIsInstance(Hashing.crc32()              , GenericHashFunction)
        self.assertIsInstance(Hashing.lookup3()            , PyHashHashFunction)
        self.assertIsInstance(Hashing.fnv1_32()            , PyHashHashFunction)
        self.assertIsInstance(Hashing.fnv1a_32()           , PyHashHashFunction)
        self.assertIsInstance(Hashing.fnv1_64()            , PyHashHashFunction)
        self.assertIsInstance(Hashing.fnv1a_64()           , PyHashHashFunction)
        self.assertIsInstance(Hashing.murmur1_32()         , PyHashHashFunction)
        self.assertIsInstance(Hashing.murmur1_aligned_32() , PyHashHashFunction)
        self.assertIsInstance(Hashing.murmur2_32()         , PyHashHashFunction)
        self.assertIsInstance(Hashing.murmur2a_32()        , PyHashHashFunction)
        self.assertIsInstance(Hashing.murmur2_aligned_32() , PyHashHashFunction)
        self.assertIsInstance(Hashing.murmur2_neutral_32() , PyHashHashFunction)
        self.assertIsInstance(Hashing.murmur2_x64_64a()    , PyHashHashFunction)
        self.assertIsInstance(Hashing.murmur2_x86_64b()    , PyHashHashFunction)
        self.assertIsInstance(Hashing.murmur3_32()         , PyHashHashFunction)
        self.assertIsInstance(Hashing.lookup3()            , PyHashHashFunction)
        self.assertIsInstance(Hashing.lookup3_little()     , PyHashHashFunction)
        self.assertIsInstance(Hashing.lookup3_big()        , PyHashHashFunction)
        self.assertIsInstance(Hashing.super_fast_hash()    , PyHashHashFunction)

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
