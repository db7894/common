#!/usr/bin/env python

def allow_anything(func):
    ''' A decorator that will allow just about anything
        to be accepted as an argument.

    @param func The function to execute
    '''
    def wrap(*args):
        result = []
        for arg in args:
            if hasattr(arg, '__iter__'):
                result.extend(wrap(*arg))
            else: result.append(func(arg))
        return result[0] if result.count == 1 else result
    return wrap

#---------------------------------------------------------------------------# 
# Testing
#---------------------------------------------------------------------------# 
if __name__ == '__main__':
    @allow_anything
    def increment(a):
        return a + 1

    def gen_number(max, start=0):
        while start < max:
            yield start
            start += 1

    def pull_result(input):
        if hasattr(input, '__iter__'):
            for v in input: print v,
            print
        else: print input

    pull_result(increment(1))
    pull_result(increment([[[1,2,(5,6,gen_number(20,1),7),3],[4,5,6]],7,8,9]))
