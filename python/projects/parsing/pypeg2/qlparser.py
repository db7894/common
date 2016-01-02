from pypeg2 import *
from pprint import pprint
from collections import namedtuple

# http://fdik.org/pyPEG2/
#http://facebook.github.io/graphql/#sec-Appendix-Grammar-Summary.Query-Document
 
#SelectionSet = None
#FragmentDefinition = None
#
#class Argument(object):
#    grammar = name(), ":", attr('value', word)
#
#class Arguments(Namespace):
#    grammar = optional("(", csl(Argument), ")")
#
#class Directive(object):
#    grammar = "@", name(), Arguments
#
#class Directives(List):
#    grammar = maybe_some(Directive)
#
#class TypeCondition(object):
#    grammar = optional("on", name())
#
#class FragmentSpread(object):
#    grammar = "...", name(), Directives
#
#class InlineFragment(object):
#    grammar = "...", TypeCondition, Directives, SelectionSet
#
#class Field(object):
#    grammar = name(), Arguments, Directives, optional(SelectionSet)
#
#class Selection(object):
#    grammar = [Field, FragmentSpread, InlineFragment]
#
#class SelectionSet(Namespace):
#    grammar = '{', some(Selection), '}'
#
#class OperationType(Keyword):
#    grammar = Enum(K("query"), K("mutation"))
#
#class OperationDefinition(object):
#    grammar = [SelectionSet, OperationType]
#
#class Definition(object):
#    grammar = [OperationDefinition, FragmentDefinition]

#class Document(List):
#    grammar = some(Definition)

#--------------------------------------------------------------------------------
# Forward Declarations
#--------------------------------------------------------------------------------

class SelectionSet(List):
    pass

#--------------------------------------------------------------------------------
# GraphQL PEG Grammar V1
#--------------------------------------------------------------------------------

class Argument(object):
    grammar = name(), ":", attr('value', word)

    def __repr__(self):
        return "{} : {}".format(self.name, self.value)

class Arguments(List):
    grammar = optional("(", csl(Argument), ")")

class Selection(List):
    grammar = name(), attr('arguments', Arguments), SelectionSet

SelectionSet.grammar = optional('{', some(Selection), '}')

class Document(List):
    grammar = SelectionSet

#--------------------------------------------------------------------------------
# Visitors
#--------------------------------------------------------------------------------

def transform_to_dict(ast):

    def transform_arguments(arguments):
        return { str(a.name) : a.value for a in arguments }

    def transform_sets(sets):
        return { str(s.name) : {
            'children'  : transform_sets(s),
            'arguments' : transform_arguments(s.arguments)
        } for ss in sets for s in ss }

    return transform_sets(ast)

#--------------------------------------------------------------------------------
# Utility Methods
#--------------------------------------------------------------------------------

def parse_graphql(query):
    ''' Given a graphQL query, parse it into the AST
    which can be converted into an underlying query.

    :param query: The query to parse
    :returns: The parsed AST
    '''
    return parse(query, Document)

query = '''{
  name 
  email 
  user(id: 1) {
    date
  }
}
'''

print(parse_graphql(query))
