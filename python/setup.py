#!/usr/bin/env python
from setuptools import setup, find_packages

setup(name  = 'bashwork',
    version = '1.0.0',
    description = 'A collection of common code to work with',
    long_description='A collection of common code to work with',
    classifiers = [
        'Development Status :: 4 - Beta',
        'Environment :: Console',
        'Intended Audience :: Developers',
        'License :: OSI Approved :: BSD License',
        'Operating System :: POSIX :: Linux',
        'Operating System :: Unix',
        'Programming Language :: Python',
        'Topic :: System :: Networking',
        'Topic :: Utilities'
    ],
    keywords = 'python',
    license = 'BSD',
    packages = find_packages(),
    platforms = ['Linux', 'Mac OS X', 'Win'],
    include_package_data = True,
    zip_safe = True,
    install_requires = [],
    extras_require = {
        'quality'   : [ 'coverage >= 3.5.3', 'nose >= 1.2.1', 'mock >= 1.0.0', 'pep8 >= 1.3.3' ],
        'documents' : [ 'sphinx >= 1.1.3' ],
    },
    test_suite = 'nose.collector'
)
