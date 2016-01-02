from errors import syntax_error
from lexer import Tokens

#--------------------------------------------------------------------------------
# Parser AST
#--------------------------------------------------------------------------------

class Nodes(object):
    # Name
    NAME = 'Name'

    # Document
    DOCUMENT = 'Document'
    OPERATION_DEFINITION = 'OperationDefinition'
    VARIABLE_DEFINITION = 'VariableDefinition'
    VARIABLE = 'Variable'
    SELECTION_SET = 'SelectionSet'
    FIELD = 'Field'
    ARGUMENT = 'Argument'

    # Fragments
    FRAGMENT_SPREAD = 'FragmentSpread'
    INLINE_FRAGMENT = 'InlineFragment'
    FRAGMENT_DEFINITION = 'FragmentDefinition'

    # Values
    INT = 'IntValue'
    FLOAT = 'FloatValue'
    STRING = 'StringValue'
    BOOLEAN = 'BooleanValue'
    ENUM = 'EnumValue'
    LIST = 'ListValue'
    OBJECT = 'ObjectValue'
    OBJECT_FIELD = 'ObjectField'

    # Directives
    DIRECTIVE = 'Directive'

    # Types
    NAMED_TYPE = 'NamedType'
    LIST_TYPE = 'ListType'
    NON_NULL_TYPE = 'NonNullType'

    # Type Definitions
    OBJECT_TYPE_DEFINITION = 'ObjectTypeDefinition'
    FIELD_DEFINITION = 'FieldDefinition'
    INPUT_VALUE_DEFINITION = 'InputValueDefinition'
    INTERFACE_TYPE_DEFINITION = 'InterfaceTypeDefinition'
    UNION_TYPE_DEFINITION = 'UnionTypeDefinition'
    SCALAR_TYPE_DEFINITION = 'ScalarTypeDefinition'
    ENUM_TYPE_DEFINITION = 'EnumTypeDefinition'
    ENUM_VALUE_DEFINITION = 'EnumValueDefinition'
    INPUT_OBJECT_TYPE_DEFINITION = 'InputObjectTypeDefinition'
    TYPE_EXTENSION_DEFINITION = 'TypeExtensionDefinition'

# TODO Node classes
class Node(object):pass

#--------------------------------------------------------------------------------
# Parser
#--------------------------------------------------------------------------------

class Parser(object):

    def __init__(self, **kwargs):
        self.lexer = kwargs.get('lexer')
        self.curr_token = None
        self.advance()

    def parse(self):
        return self.parse_document()

    #------------------------------------------------------------
    # Base Parsers
    #------------------------------------------------------------

    def parse_document(self):
        ''' Grammar::
            Document : Definition+
        '''
        token = self.curr_token
        definitions = []

        while not self.skip(Tokens.EOF):
            definitions.append(self.parse_definition())

        return {
            'kind': Nodes.DOCUMENT,
            'definitions': definitions,
            'location': (token.start, token.end),
        }

    def parse_definition(self):
        ''' Grammar::
            Definition : OperationDefinition FragmentDefinition TypeDefinition
        '''
        if   self.peek(Tokens.BRACE_L):
            return self.parse_operation_definition()
        elif self.peek(Tokens.NAME):
            if   self.curr_token.value in ['query', 'mutation', 'subscription']:
                return self.parse_operation_definition()
            elif self.curr_token.value in ['fragment']:
                return self.parse_fragment_definition()
            elif self.curr_token.value in ['type', 'interface', 'union', 'scalar', 'enum', 'input', 'extended']:
                return self.parse_type_definition()
        self.unexpected()

    def parse_operation_definition(self):
        token  = self.curr_token
        result = {
            'kind': Nodes.OPERATION_DEFINITION,
            'operation': 'query',
            'name': None,
            'variable_definitions': None,
            'directives': [],
            'location': (token.start, token.end),
        }

        if self.peek(Tokens.BRACE_L):
            result['selection_set'] = self.parse_selection_set()
            return result

        operation_token = self.expect(Tokens.NAME)
        if self.peek(Tokens.NAME):
            result['name'] = self.parse_name()

        result.extend({
            'operation': operation_token.value,
            'variable_definitions': self.parse_variable_definitions(),
            'directives': self.parse_directives(),
            'selection_set': self.parse_selection_set(),
        })
        return result

    def parse_variable_definitions(self):
        if self.peek(Tokens.PAREN_L):
            return self.parse_many(Tokens.PAREN_L,
                self.parse_variable_definition, Tokens.PAREN_R)
        return []

    def parse_variable_definition(self):
        token  = self.curr_token
        result = {
            'kind': Nodes.VARIABLE_DEFINITION,
            'variable': self.parse_variable(),
            'type': (self.expect(Tokens.COLON), self.parse_type())[1],
            'default_value': None,
            'location': (token.start, token.end),
        }

        if self.skip(Tokens.EQUALS):
            result['default_value'] = self.parse_value()

        return result

    def parse_variable(self):
        token  = self.curr_token
        self.expect(Tokens.DOLLAR)
        return {
            'kind': Nodes.VARIABLE,
            'name': self.parse_name(),
            'location': (token.start, token.end),
        }

    def parse_selection_set(self):
        token  = self.curr_token
        return {
            'kind': Nodes.SELECTION_SET,
            'selections': self.parse_many(Tokens.BRACE_L, self.parse_selection, Tokens.BRACE_R),
            'location': (token.start, token.end),
        }

    def parse_selection(self):
        if self.peek(Tokens.SPREAD):
            return self.parse_fragment()
        return self.parse_field()

    def parse_arguments(self):
        if self.peek(Tokens.PAREN_L):
            return self.parse_many(Tokens.PAREN_L, self.parse_argument, Tokens.PAREN_R)
        return []

    def parse_argument(self):
        token  = self.curr_token
        result = {
            'kind': Nodes.ARGUMENT,
            'name': self.parse_name(),
            'value': (self.expect(Tokens.COLON), self.parse_value())[1],
            'location': (token.start, token.end),
        }

    def parse_field(self):
        token  = self.curr_token
        name_or_alias = self.parse_name()
        name, alias = name_or_alias, name_or_alias

        if self.skip(Tokens.COLON):
            name = self.parse_name()
        else: alias = None

        result = {
            'kind': Nodes.FIELD,
            'alias': alias,
            'name': name,
            'arguments': self.parse_arguments(),
            'directives': self.parse_directives(),
            'selection_set': None,
            'location': (token.start, token.end),
        }

        if self.peek(Tokens.BRACE_L):
            result['selection_set'] = self.parse_selection_set()

        return result

    #------------------------------------------------------------
    # Fragment Parsers
    #------------------------------------------------------------

    def parse_fragment_definition(self):
        token = self.curr_token
        self.expect_name('fragment')

        return {
            'kind': Nodes.FRAGMENT_DEFINITION,
            'name': self.parse_name(),
            'type_condition': (self.expect_name('on'), self.parse_named_type())[1],
            'directives': self.parse_directives(),
            'selection_set': self.parse_selection_set(),
            'location': (token.start, token.end),
        }

    def parse_fragment_name(self):
        if self.curr_token.value == "on":
            self.unexpected()
        return self.parse_name()

    def parse_fragment(self):
        ''' Grammar::
            FragmentSpread : ... FragmentName Directives?
            InlineFragment : ... TypeCondition? Directives? SelectionSet
        '''
        token = self.curr_token
        self.expect(Tokens.SPREAD)
        type_condition = None

        if self.peek(Tokens.NAME) and (self.curr_token.value != 'on'):
            return {
                'kind': Nodes.FRAGMENT_SPREAD,
                'name': self.parse_fragment_name(),
                'directives': self.parse_directives(),
                'location': (token.start, token.end),
            }

        if self.curr_token.value == 'on':
            self.advance()
            type_conditon = self.parse_named_type()

        return {
            'kind': Nodes.INLINE_FRAGMENT,
            'type_condition': type_condition,
            'directives': self.parse_directives(),
            'selection_set': self.parse_selection_set(),
            'location': (token.start, token.end),
        }

    def parse_directives(self):
        directives = []
        while self.peek(Tokens.AT):
            directives.append(self.parse_directive())
        return directives

    def parse_directive(self):
        token = self.curr_token
        self.expect(Tokens.AT)

        return {
            'kind': Nodes.DIRECTIVE,
            'name': self.parse_name(),
            'arguments': self.parse_arguments(),
            'location': (token.start, token.end),
        }

    #------------------------------------------------------------
    # Type Parsers
    #------------------------------------------------------------

    def parse_type_definition(self):
        if not self.peek(Tokens.NAME): self.unexpected()
        if   self.curr_token.value == 'type':      return self.parse_object_type_definition()
        elif self.curr_token.value == 'interface': return self.parse_interface_type_definition()
        elif self.curr_token.value == 'union':     return self.parse_union_type_definition()
        elif self.curr_token.value == 'scalar':    return self.parse_scalar_type_definition()
        elif self.curr_token.value == 'enum':      return self.parse_enum_type_definition()
        elif self.curr_token.value == 'input':     return self.parse_input_type_definition()
        elif self.curr_token.value == 'extended':  return self.parse_extended_type_definition()
        else: self.unexpected()

    def parse_object_type_definition(self): pass
    def parse_interface_type_definition(self): pass
    def parse_union_type_definition(self): pass
    def parse_scalar_type_definition(self): pass
    def parse_enum_type_definition(self): pass
    def parse_input_type_definition(self): pass
    def parse_extended_type_definition(self): pass

    def parse_type(self):
        token  = self.curr_token
        if self.skip(Tokens.BRACKET_L):
            result = {
                'type': self.parse_type(),
                'kind': Nodes.LIST_TYPE,
                'location': (token.start, token.end),
            }
            self.expect(Tokens.BRACKET_R)
        else: result = self.parse_named_type()

        if self.skip(Tokens.BANG):
            result = {
                'type': result,
                'kind': Nodes.NON_NULL_TYPE,
                'location': (token.start, token.end),
            }
        return result

    def parse_named_type(self):
        token = self.curr_token
        return {
            'kind': Nodes.NAMED_TYPE,
            'name': self.parse_name(),
            'location': (token.start, token.end),
        }

    #------------------------------------------------------------
    # Primitive Parsers
    #------------------------------------------------------------

    def parse_name(self):
        token = self.expect(Tokens.NAME)
        return {
            'kind': Nodes.NAME,
            'value': token.value,
            'location': (token.start, token.end),
        }

    def parse_list(self, is_const=False):
        token  = self.curr_token
        parser = self.parse_constant_value if is_const else self.parse_variable_value

        return {
            'kind': Nodes.LIST,
            'values': self.parse_many(Tokens.BRACKET_L, parser, Tokens.BRACKET_R),
            'location': (token.start, token.end),
        }

    def parse_variable_value(self):
        return self.parse_value(is_const=False)

    def parse_constant_value(self):
        return self.parse_value(is_const=True)

    def parse_value(self, is_const=False):
        if   self.curr_token.kind == Tokens.BRACKET_L: return self.parse_list(is_const)
        elif self.curr_token.kind == Tokens.BRACE_L:   return self.parse_object(is_const)
        elif self.curr_token.kind == Tokens.INT:       return self.parse_int()
        elif self.curr_token.kind == Tokens.FLOAT:     return self.parse_float()
        elif self.curr_token.kind == Tokens.STRING:    return self.parse_string()
        elif self.curr_token.kind == Tokens.NAME:
            if  ((self.curr_token.value == 'true')
              or (self.curr_token.value == 'false')):  return self.parse_boolean()
            elif (self.curr_token.value != 'null'):    return self.parse_enum()
        elif   ((self.curr_token.kind == Tokens.DOLLAR)
            and (not is_const)):                       return self.parse_variable()
        self.unexpected()

    def parse_object(self, is_const=False):
        token = self.curr_token
        self.expect(Tokens.BRACE_L)
        fields = []

        while self.skip(Tokens.BRACE_R):
            fields.append(self.parse_object_field(is_const))

        return {
            'kind': Nodes.OBJECT,
            'fields': fields,
            'location': (token.start, token.end),
        }

    def parse_object_field(self, is_const=False):
        token = self.curr_token
        return {
            'kind': Nodes.OBJECT_FIELD,
            'name': self.parse_name(),
            'value': (self.expect(Tokens.COLON), self.parse_value(is_const))[1],
            'location': (token.start, token.end),
        }

    def parse_int(self):
        self.advance()
        return {
            'kind': Nodes.INT,
            'value': int(self.prev_token.value),
            'location': (self.prev_token.start, self.prev_token.end),
        }

    def parse_float(self):
        self.advance()
        return {
            'kind': Nodes.FLOAT,
            'value': float(self.prev_token.value),
            'location': (self.prev_token.start, self.prev_token.end),
        }

    def parse_string(self):
        self.advance()
        return {
            'kind': Nodes.STRING,
            'value': self.prev_token.value,
            'location': (self.prev_token.start, self.prev_token.end),
        }

    def parse_enum(self):
        ''' Parse an enum node from the token stream.

        :returns: The parsed node
        '''
        self.advance()
        return {
            'kind': Nodes.ENUM,
            'value': self.prev_token.value,
            'location': (self.prev_token.start, self.prev_token.end),
        }

    def parse_boolean(self):
        ''' Parse a boolean node from the token stream.

        :returns: The parsed node
        '''
        self.advance()
        return {
            'kind': Nodes.BOOLEAN,
            'value': self.prev_token.value == 'true',
            'location': (self.prev_token.start, self.prev_token.end),
        }

    #------------------------------------------------------------
    # Utility Parsers
    #------------------------------------------------------------

    def unexpected(self):
        ''' Called when the token stream returns an unexpected
        next token.

        :throws SyntaxError: With the unexpected token
        '''
        syntax_error(self.lexer.source, self.curr_token.start,
            "unexpected token found {}".format(self.curr_token))

    def parse_any(self, open_kind, parser, close_kind):
        ''' Given an open and close token kind, run the supplied
        parser function zero or more times.

        :param open_kind: The open token kind
        :param parser: The parsing function to operate with
        :param close_kind: The close token kind
        :returns: The resulting parsed nodes
        '''
        self.expect(open_kind)
        nodes = []

        while not self.skip(close_kind):
            nodes.append(parser())
        return nodes

    def parse_many(self, open_kind, parser, close_kind):
        ''' Given an open and close token kind, run the supplied
        parser function one or more times.

        :param open_kind: The open token kind
        :param parser: The parsing function to operate with
        :param close_kind: The close token kind
        :returns: The resulting parsed nodes
        '''
        self.expect(open_kind)
        nodes = [parser()]

        while not self.skip(close_kind):
            nodes.append(parser())
        return nodes

    def advance(self):
        ''' Advance the current state of the lexer.  '''
        self.prev_token = self.curr_token
        self.curr_token = self.lexer.next_token()

    def peek(self, kind):
        ''' Check if the next token is of the supplied kind without
        advancing the lexer.

        :param kind: The type of token to test
        :returns: True if the token was a match, False otherwise
        '''
        return self.curr_token.kind == kind

    def skip(self, kind):
        ''' Skip the next token if it is of the supplied kind.

        :param kind: The type of token to skip
        :returns: True if the token was skipped, False otherwise
        '''
        is_match = self.curr_token.kind == kind
        if is_match: self.advance()
        return is_match

    def expect(self, kind):
        ''' Checks that the current token is of the supplied kind

        :param kind: The type of token to expect
        :throws SyntaxError: If the wrong token appears
        :returns: The expected token
        '''
        token = self.curr_token

        if (token.kind == kind):
            self.advance()
            return token

        syntax_error(self.lexer.source, token.start,
            "expected {} token got {}".format(kind, token.kind))

    def expect_name(self, value):
        ''' Checks that the current token of the Tokens.NAME kind
        is of the supplied value.

        :param value: The value of the name token that is expected
        :throws SyntaxError: If the wrong token appears
        :returns: The expected token
        '''
        token = self.curr_token

        if (token.kind == Tokens.NAME) and (token.value == value):
            self.advance()
            return token

        syntax_error(self.lexer.source, token.start,
            "expected name token({}) token got {}".format(value, token.value))

if __name__ == "__main__":
    import sys
    from lexer import Lexer
    from pprint import pprint as pretty_print

    source = open(sys.argv[1]).read()
    print source
    lexer = Lexer(source=source)
    pretty_print(Parser(lexer=lexer).parse())
