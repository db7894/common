#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
This is a simple reverse polish calculator
'''
import operator

def evaluate(expression, operations):
    stack = []
    for element in expression:
        if element in operations:
            operation = operations[element]
            stack.append(operation(stack.pop(), stack.pop()))
        else: stack.append(int(element))
    return stack.pop()

#------------------------------------------------------------
# constants
#------------------------------------------------------------

OPERATIONS = {
    '*': operator.mul,
    '-': operator.sub,
    '/': operator.div,
    '+': operator.add,
}

#------------------------------------------------------------
# main
#------------------------------------------------------------

if __name__ == "__main__":
    expression = '3,4,*,1,2,+,+'.split(',')
    print evaluate(expression, OPERATIONS)
