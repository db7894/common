#--------------------------------------------------------------------------------
# Parser AST
#--------------------------------------------------------------------------------

DocumentKeys = {
    'Name'                      : [],
    'Document'                  : [ 'definitions' ],
    'OperationDefinition'       : [ 'name', 'variable_definitions', 'directives', 'selection_set' ],
    'VariableDefinition'        : [ 'variable', 'type', 'default_value' ],
    'Variable'                  : [ 'name' ],
    'SelectionSet'              : [ 'selections' ],
    'Field'                     : [ 'alias', 'name', 'arguments', 'directives', 'selection_set' ],
    'Argument'                  : [ 'name', 'value' ],
    'FragmentSpread'            : [ 'name', 'directives' ],
    'InlineFragment'            : [ 'type_condition', 'directives', 'selection_set' ],
    'FragmentDefinition'        : [ 'name', 'type_condition', 'directives', 'selection_set' ],
    'IntValue'                  : [],
    'FloatValue'                : [],
    'StringValue'               : [],
    'BooleanValue'              : [],
    'EnumValue'                 : [],
    'ListValue'                 : [ 'values' ],
    'ObjectValue'               : [ 'fields' ],
    'ObjectField'               : [ 'name', 'value' ],
    'Directive'                 : [ 'name', 'arguments' ],
    'NamedType'                 : [ 'name' ],
    'ListType'                  : [ 'type' ],
    'NonNullType'               : [ 'type' ],
    'ObjectTypeDefinition'      : [ 'name', 'interfaces', 'fields' ],
    'FieldDefinition'           : [ 'name', 'arguments', 'type' ],
    'InputValueDefinition'      : [ 'name', 'type', 'default_value' ],
    'InterfaceTypeDefinition'   : [ 'name', 'fields' ],
    'UnionTypeDefinition'       : [ 'name', 'types' ],
    'ScalarTypeDefinition'      : [ 'name' ],
    'EnumTypeDefinition'        : [ 'name', 'values' ],
    'EnumValueDefinition'       : [ 'name' ],
    'InputObjectTypeDefinition' : [ 'name', 'fields' ],
    'TypeExtensionDefinition'   : [ 'definition' ],
}

class Visitor(object):
    ''' Base class for a visitor for the graphQL
    language. This will call default methods for
    visitor methods that are not implemented.
    '''

    def __getattr__(self, name):
        def missing(*args, **kwargs):
            pass
        return missing

class PrinterVisitor(Visitor):
    def visit_Name(self, **kwargs): pass
    def visit_Document(self, **kwargs): pass
    def visit_OperationDefinition(self, **kwargs): pass
    def visit_VariableDefinition(self, **kwargs): pass
    def visit_Variable(self, **kwargs): pass
    def visit_SelectionSet(self, **kwargs): pass
    def visit_Field(self, **kwargs): pass
    def visit_Argument(self, **kwargs): pass
    def visit_FragmentSpread(self, **kwargs): pass
    def visit_InlineFragment(self, **kwargs): pass
    def visit_FragmentDefinition(self, **kwargs): pass
    def visit_IntValue(self, **kwargs): pass
    def visit_FloatValue(self, **kwargs): pass
    def visit_StringValue(self, **kwargs): pass
    def visit_BooleanValue(self, **kwargs): pass
    def visit_EnumValue(self, **kwargs): pass
    def visit_ListValue(self, **kwargs): pass
    def visit_ObjectValue(self, **kwargs): pass
    def visit_ObjectField(self, **kwargs): pass
    def visit_Directive(self, **kwargs): pass
    def visit_NamedType(self, **kwargs): pass
    def visit_ListType(self, **kwargs): pass
    def visit_NonNullType(self, **kwargs): pass
    def visit_ObjectTypeDefinition(self, **kwargs): pass
    def visit_FieldDefinition(self, **kwargs): pass
    def visit_InputValueDefinition(self, **kwargs): pass
    def visit_InterfaceTypeDefinition(self, **kwargs): pass
    def visit_UnionTypeDefinition(self, **kwargs): pass
    def visit_ScalarTypeDefinition(self, **kwargs): pass
    def visit_EnumTypeDefinition(self, **kwargs): pass
    def visit_EnumValueDefinition(self, **kwargs): pass
    def visit_InputObjectTypeDefinition(self, **kwargs): pass
    def visit_TypeExtensionDefinition(self, **kwargs): pass

def visit_syntax_tree(syntax_tree, visitor):
    ''' Depth first visitor framework for an abstract syntax
    tree.

    :param visitor: The visitor to visit with
    :param ast: The abstract syntax tree
    '''
    stack = [ syntax_tree ]
    while stack:
        node = stack.pop()
        kind = node.pop('kind')
        locs = node.pop('location')
        keys = DocumentKeys[kind]
        vals = { key: node[key] for key in keys }
        getattr(visitor, "visit_" + kind)(**node)
        stack.extend(vals.values())

class DefaultVisitor(object):
    ''' Base class for a visitor for the graphQL
    language. This will call default methods for
    visitor methods that are not implemented.
    '''

    def __getattr__(self, name):
        def missing(*args, **kwargs):
            print(kwargs.keys())
        return missing

def print_syntax_tree(syntax_tree):
    visit_syntax_tree(syntax_tree, PVisitor())

if __name__ == "__main__":
    import sys
    from lexer import Lexer
    from parser import Parser
    from pprint import pprint as pretty_print

    source = open(sys.argv[1]).read()
    lexer  = Lexer(source=source)
    syntax = Parser(lexer=lexer).parse()
    print_syntax_tree(syntax)
