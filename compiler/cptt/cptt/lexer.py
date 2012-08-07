import re


class LexerException(Exception):
    ''' Exception thrown when unrecoverable
    errors are encountered in the lexing stage.
    '''
    pass


class Token(object):
    ''' Represents a single lexed token from the
    input stream.
    '''
    def __init__(self, type, value, lineno, lexpos):
        '''

        :param type: The type of token this represents
        :param value: The value of this token
        :param lineno: The line number this was found on
        :poaram lexpos: The lexer position this was found at
        '''
        self.type  = type
        self.value  = value
        self.lineno = lineno
        self.lexpos = lexpos

    def __str__(self):
        params = (self.type, self.value, self.lineno, self.lexpos)
        return "Token(%s,%r,%d,%d)" % params

    def __repr__(self):
        return str(self)

class Tokens(object):
    ''' A collection of predefined tokens
    that can quickly be added to a project.
    '''
    WhiteSpace = { "WS"     : r' \t'
                 }
    NewLine    = { "NewLine": r'\n'
                 }
    Operators  = { "Plus"   : r'\+',
                   "Minus"  : r'-',
                   "Times"  : r'*',
                   "Divide" : r'/',
                   "Assign" : r'=',
                 }
    Bitwise    = { "Xor"    : r'^',
                   "Not"    : r'!',
                   "BitAnd" : r'&',
                   "Mod"    : r'%',
                   "BitOr"  : r'|',
                   "LShift" : r'<<',
                   "RShift" : r'>>',
                 }
    Logical    = { "And"    : r'&&',
                   "Or"     : r'||',
                 }
    Parens     = { "LParen" : r'\(',
                   "RParen" : r'\)',
                 }
    Brackets   = { "LBrack" : r'{',
                   "RBrack" : r'}',
                 }
    Indexes    = { "LIndex" : r'\[',
                   "RIndex" : r'\]',
                 }
    Comparison = { "EQ"     : r'==',
                   "NEQ"    : r'!=',
                   "GT"     : r'>',
                   "LT"     : r'<',
                   "GTE"    : r'>=',
                   "LTE"    : r'<=',
                 }
    Letter     = { "Letter" : r'[a-zA-Z]'
                 }
    Digit      = { "Digit"  : r'[0-9]'
                 }
    Name       = { "Name"   : r'[a-zA-Z_][a-zA-Z0-9_]*'
                 }
    Number     = { "Number" : r'[0-9]+(\.[0-9]+)?(e[+-]?[0-9]+)?'
                 }
