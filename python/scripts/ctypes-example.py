#!/usr/bin/env python
import ctypes
import time
from ctypes.util import find_library

#--------------------------------------------------------------------------------
# load the library with ctypes
#--------------------------------------------------------------------------------
lib_c_name = find_library("c")      # search for the library on any OS
#lib_c_name = "libc.dylib"          # load directly on osx
#lib_c_name = "libc.so.6"           # load directly on linux
ctypes.cdll.LoadLibrary(lib_c_name) # load the library
lib_c = ctypes.CDLL(lib_c_name)     # get a reference to the library

#--------------------------------------------------------------------------------
# define our structure mappings
#--------------------------------------------------------------------------------

class CTime(ctypes.Structure):
    _fields_ = [
        ("tm_sec", ctypes.c_int),
        ("tm_min", ctypes.c_int),
        ("tm_hour", ctypes.c_int),
        ("tm_mday", ctypes.c_int),
        ("tm_mon", ctypes.c_int),
        ("tm_year", ctypes.c_int),
        ("tm_wday", ctypes.c_int),
        ("tm_yday", ctypes.c_int),
        ("tm_isdst", ctypes.c_int),
        ("tm_gmtoff", ctypes.c_long),
        ("tm_zone", ctypes.c_char_p)
    ]

def parse_time_string(string):
    reference = CTime()
    lib_c.strptime("2014-05-02T10:29:09", "%Y-%m-%dT%H:%M:%S", ctypes.byref(reference));
    return reference

#--------------------------------------------------------------------------------
# quick test of performance
#--------------------------------------------------------------------------------

string = "2014-05-02T10:29:09"
start  = time.time()

for i in xrange(1000000):
    parse_time_string(string)

end = time.time()

print(end - start)
