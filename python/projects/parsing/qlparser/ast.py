#--------------------------------------------------------------------------------
# Abstract Syntax Nodes
#--------------------------------------------------------------------------------

class Node(object):
    ''' A base class for all the node instances that uses reflection
    to create all the magic methods.
    '''
    __slots__ = ()

    def __init__(self, **kwargs):
        for slot in self.__slots__:
            setattr(self, slot, kwargs.get(slot))

    def as_dict(self):
        ''' Return the current node as a dictionary

        :returns: The current node as a dictionary
        '''
        return { slot : getattr(self, slot) for slot in self.__slots__ }

    def kind(self):
        return self.__class__.__name__[:-4]

    def __copy__(self):
        return type(self)(**self.as_dict())

    def __eq__(self, other):
        return ((self is other)
            or  (isinstance(other, type(self)))
           and  (self.as_dict() == other.as_dict()))

    def __unicode__(self):
        return "{}{}".format(self.kind(), self.as_dict())

    __str__  = __unicode__
    __repr__ = __unicode__

    def __hash__(self):
        return id(self)

class NameNode(Node):
    __slots__ = ['value', 'location']

class DocumentNode(Node):
    __slots__ = ['definitions', 'location']

class OperationDefinitionNode(Node):
    __slots__ = ['operation', 'name', 'variable_definitions', 'directives', 'location']

class VariableDefinitionNode(Node):
    __slots__ = ['variable', 'type', 'default_value', 'location']

class VariableNode(Node):
    __slots__ = ['name', 'location']

class SelectionSetNode(Node):
    __slots__ = ['selections', 'location']

class FieldNode(Node):
    __slots__ = ['alias', 'name', 'arguments', 'directives', 'selection_set', 'location']

class ArgumentNode(Node):
    __slots__ = ['name', 'value', 'location']

class FragmentSpreadNode(Node):
    __slots__ = ['type_condition', 'directives', 'selection_set', 'location']

class InlineFragmentNode(Node):
    __slots__ = ['type_condition', 'directives', 'selection_set', 'location']

class FragmentDefinitionNode(Node):
    __slots__ = ['name', 'type_condition', 'directives', 'selection_set', 'location']

class IntNode(Node):
    __slots__ = ['value', 'location']

class FloatNode(Node):
    __slots__ = ['value', 'location']

class StringNode(Node):
    __slots__ = ['value', 'location']

class BooleanNode(Node):
    __slots__ = ['value', 'location']

class EnumNode(Node):
    __slots__ = ['value', 'location']

class ListNode(Node):
    __slots__ = ['values', 'location']

class ObjectNode(Node):
    __slots__ = ['fields', 'location']

class ObjectFieldNode(Node):
    __slots__ = ['name', 'value', 'location']

class DirectiveNode(Node):
    __slots__ = ['name', 'arguments', 'location']

class NamedTypeNode(Node):
    __slots__ = ['name', 'location']

class ListTypeNode(Node):
    __slots__ = ['type', 'location']

class NonNullTypeNode(Node):
    __slots__ = ['type', 'location']

# Type Definitions
class ObjectTypeDefinitionNode(Node):
    __slots__ = ()

class FieldDefinitionNode(Node):
    __slots__ = ()

class InputValueDefinitionNode(Node):
    __slots__ = ()

class InterfaceTypeDefinitionNode(Node):
    __slots__ = ()

class UnionTypeDefinitionNode(Node):
    __slots__ = ()

class ScalarTypeDefinitionNode(Node):
    __slots__ = ()

class EnumTypeDefinitionNode(Node):
    __slots__ = ()

class EnumValueDefinitionNode(Node):
    __slots__ = ()

class InputObjectTypeDefinitionNode(Node):
    __slots__ = ()

class TypeExtensionDefinitionNode(Node):
    __slots__ = ()
