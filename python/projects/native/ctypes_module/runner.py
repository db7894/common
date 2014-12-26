from os import path
from ctypes import *

cur_dir = path.abspath(path.dirname(__file__))
lib_dir = path.join(cur_dir, "..", "example.so")
example = cdll.LoadLibrary(lib_dir)

func = example.get_data_of_size
func.restype  = POINTER(c_char)
func.argtypes = [POINTER(c_int)]

size = c_int()
data = func(byref(size))

print data, size, data.contents

example.print_data(data, size)
