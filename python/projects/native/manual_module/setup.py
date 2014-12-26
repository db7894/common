from distutils.core import setup, Extension
 
 
setup(
    name = 'ExampleNativePackage',
    version = '1.0',
    description = 'An example native python package',
    ext_modules = [
        Extension('example', sources = ['example_module.c', 'example.c'])
    ])
