import os
from distutils.core import setup, Extension

os.environ["CC"]  = "g++-4.9"
os.environ["CXX"] = "g++-4.9"
 
setup(
    name = 'ExampleSwigPackage',
    version = '1.0',
    description = 'An example native python package',
    ext_modules = [
        Extension('_example', sources = ['example_module.cc', 'example.c'])
    ])
