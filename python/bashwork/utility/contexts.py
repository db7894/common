import time
import tempfile, shutil
from contextlib import contextmanager

@contextmanager
def timer(label):
    '''
    example::
        
        with timer('counting'):
            n = 1000000
            while n > 0:
                n -= 1
    '''
    start = time.time()
    try:
        yield
    finally:
        end = time.time()
        print("%s: %0.3f" % (label, end - start))

@contextmanager
def temp_directory(label):
    '''
    example::
        
        with temp_directory() as directory:
            ...
    '''
    directory = tempfile.mkdtemp()
    try:
        yield directory
    finally:
        shutil.rmtree(directory)
