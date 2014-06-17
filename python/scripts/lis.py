'''
'''
from __future__ import division

#------------------------------------------------------------
# Common Aliases
#------------------------------------------------------------
Symbol = str
isa = isinstance

#------------------------------------------------------------
# Environment Classes
#------------------------------------------------------------
class Environment(dict):
    ''' An environment for maintainting the
    current symbols in the interpreter:
    
    * a dict of {'var':val} pairs
    * refer missing symbosl to with an parent Environment
    '''

    def __init__(self, parms=(), args=(), parent=None):
        '''
        '''
        self.update(zip(parms, args))
        self.parent = parent

    def find(self, symbol):
        ''' Find the innermost environment where the
        symbol appears.

        :param symbol: The symbol to find a binding for
        :returns: The symbol binding or None
        '''
        return self if symbol in self else self.parent.find(symbol)

    @classmethod
    def create_global(klass):
        "Add some Scheme standard procedures to an environment."
        import math, operator as op
        environment = Environment()
        environment.update(vars(math)) # sin, sqrt, ...
        environment.update({
            '+'      :op.add,
            '-'      :op.sub,
            '*'      :op.mul,
            '/'      :op.div,
            'not'    :op.not_,
            '>'      :op.gt,
            '<'      :op.lt,
            '>='     :op.ge,
            '<='     :op.le,
            '='      :op.eq, 
            'equal?' :op.eq,
            'eq?'    :op.is_,
            'length' :len,
            'cons'   :lambda x, y: [x] + y,
            'car'    :lambda x: x[0],
            'cdr'    :lambda x: x[1:],
            'append' :op.add,  
            'list'   :lambda *x: list(x),
            'list?'  :lambda x: isa(x,list), 
            'null?'  :lambda x: x==[],
            'symbol?':lambda x: isa(x, Symbol)
        })
        return environment

GLOBALS = Environment.create_global()

#------------------------------------------------------------
# Expression Evaluation
#------------------------------------------------------------

def evaluate(ast, env=GLOBALS):
    ''' Given an expression, evaluate it with the supplied
    environment.

    :param ast: The expression to evaluate
    :param env: The environment to operate with
    :returns: The result of the expression
    '''
    if isa(ast, Symbol):           # variable reference
        return env.find(ast)[ast]
    elif not isa(ast, list):       # constant literal
        return ast                
    elif ast[0] == 'quote':        # (quote exp)
        (_, exp) = ast
        return exp
    elif ast[0] == 'if':           # (if test conseq alt)
        (_, test, conseq, alt) = ast
        return evaluate((conseq if evaluate(test, env) else alt), env)
    elif ast[0] == 'set!':         # (set! var exp)
        (_, var, exp) = ast
        env.find(var)[var] = evaluate(exp, env)
    elif ast[0] == 'define':       # (define var exp)
        (_, var, exp) = ast
        env[var] = evaluate(exp, env)
    elif ast[0] == 'lambda':       # (lambda (var*) exp)
        (_, vars, exp) = ast
        return lambda *args: evaluate(exp, Environment(vars, args, env))
    elif ast[0] == 'begin':        # (begin exp*)
        for exp in ast[1:]:
            val = evaluate(exp, env)
        return val
    else:                          # (proc exp*)
        exps = [evaluate(exp, env) for exp in ast]
        proc = exps.pop(0)
        return proc(*exps)

#------------------------------------------------------------
# Interpreter Implementation
#------------------------------------------------------------

def parse(string):
    ''' Given a string, parse it into the
    abstract syntax tree.

    :param string: The string to parse
    :returns: The abstract syntax tree
    '''
    return parse_expression(tokenize(string))

def tokenize(string):
    ''' Given a string, convert it into a list
    of tokens.

    :param string: The string to parse into tokens
    :returns: The resulting tokens
    '''
    return string.replace('(',' ( ').replace(')',' ) ').split()

def parse_expression(tokens):
    ''' Given a sequence of tokens, attempt to convert
    them to a valid lisp expression.

    :param tokens: The tokens to convert
    :returns: A valid lisp expression to be evaluated
    '''
    if len(tokens) == 0:
        raise SyntaxError('unexpected EOF while reading')

    token = tokens.pop(0)
    if '(' == token:
        tree = []
        while tokens[0] != ')':
            tree.append(parse_expression(tokens))
        tokens.pop(0) # pop off ')'
        return tree
    elif ')' == token:
        raise SyntaxError('unexpected )')
    else: return atomize(token)

def atomize(token):
    ''' Convert all atoms to their value:
    * numerals are ints
    * decimals are floats
    * symbols are strings

    :param token: The token to convert into an atom
    :returns: The atomized version of the token
    '''
    try:
        return int(token)
    except ValueError:
        try:
            return float(token)
        except ValueError:
            return Symbol(token)

def to_string(expression):
    ''' Given a python object, convert it into a
    lisp readable string.

    :param expression: The expression to convert to a string
    :returns: The expression as a lisp string
    '''
    if isa(expression, list):
        return '(' + ' '.join(map(to_string, expression)) + ')'
    return str(expression)

def interpreter(prompt='> '):
    ''' A read eval print loop for testing out
    the lisp interpreter.

    :param prompt: The prompt to use for testing
    '''
    while True:
        input = raw_input(prompt)
        value = evaluate(parse(input))
        if value is not None:
            print to_string(value)
