from errors import syntax_error
from collections import namedtuple

#--------------------------------------------------------------------------------
# GraphQL Tokens
#--------------------------------------------------------------------------------
class LexerException(Exception):
    pass

class Tokens(object):
    EOF       = 1
    BANG      = 2
    DOLLAR    = 3
    PAREN_L   = 4
    PAREN_R   = 5
    SPREAD    = 6
    COLON     = 7
    EQUALS    = 8
    AT        = 9
    BRACKET_L = 10
    BRACKET_R = 11
    BRACE_L   = 12
    PIPE      = 13
    BRACE_R   = 14
    NAME      = 15
    VARIABLE  = 16
    INT       = 17
    FLOAT     = 18
    STRING    = 19

    __descriptions = {
         1  : 'EOF',
         2  : '!',
         3  : '$',
         4  : '(',
         5  : ')',
         6  : '...',
         7  : ':',
         8  : '=',
         9  : '@',
         10 : '[',
         11 : ']',
         12 : '{',
         13 : '|',
         14 : '}',
         15 : 'Name',
         16 : 'Variable',
         17 : 'Int',
         18 : 'Float',
         19 : 'String',
    }

    @classmethod
    def describe(klass, token):
        return klass.__descriptions.get(token)

Token = namedtuple('Token', ['kind', 'start', 'end', 'value'])

#--------------------------------------------------------------------------------
# GraphQL Lexer
#--------------------------------------------------------------------------------

class Lexer(object):

    def __init__(self, **kwargs):
        '''
        '''
        self.source = kwargs.get('source')
        self.position = kwargs.get('position', 0)
        self.eof = kwargs.get('eof', len(self.source))

    def next_token(self, position=None):
        ''' Return the next token being parsed '''
        if not position:
            position = self.position

        token = self.read_token(position)
        self.position = token.end

        return token

    def next(self):
        token = self.next_token()
        if token.kind == Tokens.EOF:
            raise StopIteration
        return token

    def make_token(self, kind, start, end=None, value=None):
        ''' Generate a token with the supplied data '''
        if not end: end = start + 1
        if not value: value = self.source[start:end]
        return Token(kind, start, end, value)

    def read_token(self, start):
        ''' Read the next symbol out of the text '''
        _, position = self.read_whitespace(start)

        if position >= self.eof:
            return self.make_token(Tokens.EOF, position, 0)

        char = self.source[position]
        if (    (char < ' ')
            and (char != '\t' or char != '\n' or char != '\r')):
            syntax_error(source, position,
                "invalid character code: {}".format(self.source[position]))

        if   char == '!': return self.make_token(Tokens.BANG, position)
        elif char == '$': return self.make_token(Tokens.DOLLAR, position)
        elif char == '(': return self.make_token(Tokens.PAREN_L, position)
        elif char == ')': return self.make_token(Tokens.PAREN_R, position)
        elif char == '.':
            if (self.source[position:position + 3] == '...'):
               return self.make_token(Tokens.SPREAD, position, position + 3)
        elif char == ':': return self.make_token(Tokens.COLON, position)
        elif char == '=': return self.make_token(Tokens.EQUALS, position)
        elif char == '@': return self.make_token(Tokens.AT, position)
        elif char == '[': return self.make_token(Tokens.BRACKET_L, position)
        elif char == ']': return self.make_token(Tokens.BRACKET_R, position)
        elif char == '{': return self.make_token(Tokens.BRACE_L, position)
        elif char == '|': return self.make_token(Tokens.PIPE, position)
        elif char == '}': return self.make_token(Tokens.BRACE_R, position)
        elif ('a' <= char <= 'z') or ('A' <= char <= 'Z') or (char == '_'):
            return self.read_name(position)
        elif '0' <= char <= '9': return self.read_number(position)
        elif char == '"': return self.read_string(position)
        else: syntax_error(source, position, "unexpected token: {}".format(char))

    def read_number(self, start):
        ''' Read the next number out of the text '''
        is_float = False
        position = start

        if self.source[position] == '-':
            position += 1

        _, position = self.read_digits(position)

        if self.source[position] == '.':
            is_float = True
            _, position = self.read_digits(position + 1)

        token = Tokens.FLOAT if is_float else Tokens.INT
        return self.make_token(token, start, position)

    def read_digits(self, start):
        ''' Read the next digits out of the text '''
        position = start

        if '0' > self.source[position] > '9':
            syntax_error(source, position, "expected digit: {}".format(self.source[position]))

        while (  (position < self.eof)
            and  ('0' <= self.source[position] <= '9')):
            position += 1

        return start, position

    def read_name(self, start):
        ''' Read the next name out of the text '''
        position = start

        while ( (position < self.eof)
            and ((self.source[position] == '_')
             or  ('0' <= self.source[position] <= '9')
             or  ('a' <= self.source[position] <= 'z')
             or  ('A' <= self.source[position] <= 'Z'))):
            position += 1

        return self.make_token(Tokens.NAME, start, position)

    def read_string(self, start):
        ''' Read the next string out of the text '''
        position = start + 1 # pass start quote mark

        # TODO handle escaped characters in the quote
        while ( (position < self.eof)
            and (self.source[position] != '"')):
            position += 1

        if self.source[position] != '"':
            syntax_error(source, position, "unterminated string: {}".format(self.source[position]))

        return self.make_token(Tokens.STRING, start, position + 1,
            self.source[start + 1:position - 1])

    def read_whitespace(self, start):
        ''' Read past the next available whitespace in the text '''
        position = start

        while (position < self.eof):
            if (   (self.source[position] == ' ')
                or (self.source[position] == '\t')
                or (self.source[position] == '\r')
                or (self.source[position] == '\n')
                or (self.source[position] == ',')):
                position += 1
            elif (self.source[position] == '#'):
                position += 1
                while ( (position < self.eof)
                    and (self.source[position] >= ' ')
                    and (self.source[position] != '\n')
                    and (self.source[position] != '\r')):
                    position += 1
            else: break
        
        return start, position

    def __iter__(self): return self
    def __next__(self): return self.next()

#--------------------------------------------------------------------------------
# main test method
#--------------------------------------------------------------------------------
if __name__ == "__main__":
    import sys
    from pprint import pprint as pretty_print

    source = open(sys.argv[1]).read()
    print source
    pretty_print(list(Lexer(source=source)))
