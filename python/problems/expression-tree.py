#!/usr/bin/env python
# -*- coding: latin-1 -*-
'''
'''

#------------------------------------------------------------
# Node Hierarchy
#------------------------------------------------------------

class Node(object):
    ''' A base node in the expression tree
    '''

    def __init__(self, name, left, right):
        ''' Initialize a new expression tree Node

        :param name: The identifier for this node
        :param left: The left branch of this node
        :param right: The right branch of this node
        '''
        self.name  = name
        self.left  = left
        self.right = right

    def visit(self, visitor):
        ''' Given the supplied visitor, let it visit this node

        :param visitor: The visitor to apply to this node
        '''
        visitor(self)

class DivNode(Node):

    def __init__(self, left, right):
        super(DivNode, self).__init__(" / ", left, right)

class MulNode(Node):

    def __init__(self, left, right):
        super(MulNode, self).__init__(" * ", left, right)

class SubNode(Node):

    def __init__(self, left, right):
        super(SubNode, self).__init__(" - ", left, right)

class AddNode(Node):

    def __init__(self, left, right):
        super(AddNode, self).__init__(" + ", left, right)

class VarNode(Node):

    def __init__(self, value):
        super(VarNode, self).__init__("var", None, None)
        self.value = value

class ValNode(Node):

    def __init__(self, value):
        super(ValNode, self).__init__("val", None, None)
        self.value = value

#------------------------------------------------------------
# Visitor Hierarchy
#------------------------------------------------------------

class Visitor(object):

    def visit(self, tree):
        ''' Given a tree node, dispatch to the
        correct visitor method based on the operation.

        :param tree: The tree to dispatch on
        :returns: The result of the visit
        '''
        if tree.name == " + ":
            return self.visit_add(tree)
        elif tree.name == " - ":
            return self.visit_sub(tree)
        elif tree.name == " * ":
            return self.visit_mul(tree)
        elif tree.name == " / ":
            return self.visit_div(tree)
        elif tree.name == "var":
            return self.visit_var(tree)
        elif tree.name == "val":
            return self.visit_val(tree)

class StringVisitor(Visitor):
    ''' A visitor that will return the expression
    formatted as a string.
    '''

    def __init__(self, **kwargs):
        self.env = kwargs.get("env", {})

    def visit_div(self, node):
        return self.visit(node.left) + " / " + self.visit(node.right)

    def visit_mul(self, node):
        return self.visit(node.left) + " * " + self.visit(node.right)

    def visit_add(self, node):
        return self.visit(node.left) + " + " + self.visit(node.right)

    def visit_sub(self, node):
        return self.visit(node.left) + " - " + self.visit(node.right)

    def visit_var(self, node):
        return str(self.env[node.value])

    def visit_val(self, node):
        return str(node.value)

class ComputeVisitor(Visitor):
    ''' A visitor that will evaluate the expression
    and return the result.
    '''

    def __init__(self, **kwargs):
        self.env = kwargs.get("env", {})

    def visit_div(self, node):
        return self.visit(node.left) / self.visit(node.right)

    def visit_mul(self, node):
        return self.visit(node.left) * self.visit(node.right)

    def visit_add(self, node):
        return self.visit(node.left) + self.visit(node.right)

    def visit_sub(self, node):
        return self.visit(node.left) - self.visit(node.right)

    def visit_var(self, node):
        return self.env[node.value]

    def visit_val(self, node):
        return node.value

#------------------------------------------------------------
# Functional Approach
#------------------------------------------------------------

def visit_compute(node):
    ''' Compute the result of an expression tree. Each node is a
    tuple of the operation and its parameters::

        add -> ('+', 1, 1)
        sub -> ('-', ('+', 2, 4), 3)
        mul -> ('*', 2, 3)
        div -> ('/', 6, 3)

    :param node: The node to compute
    :returns: The result of the computation
    ''' 
    if not isinstance(node, tuple): return node
    elif node[0] == '+': return visit_compute(node[1]) + visit_compute(node[2])
    elif node[0] == '-': return visit_compute(node[1]) - visit_compute(node[2])
    elif node[0] == '/': return visit_compute(node[1]) / visit_compute(node[2])
    elif node[0] == '*': return visit_compute(node[1]) * visit_compute(node[2])
    else: raise Exception("unsupported operation")

def visit_string(node):
    ''' Create a string for the supplied expression tree.

        add -> ('+', 1, 1)
        sub -> ('-', ('+', 2, 4), 3)
        mul -> ('*', 2, 3)
        div -> ('/', 6, 3)

    :param node: The node to build a string for
    :returns: The resulting string for the computation
    ''' 
    if not isinstance(node, tuple):
        return str(node)

    op, le, ri = node
    return "{} {} {}".format(visit_string(le), op, visit_string(ri))

#------------------------------------------------------------
# main driver
#------------------------------------------------------------

if __name__ == "__main__":
    env  = { 'x' : 2 }
    tree = MulNode(AddNode(ValNode(4), ValNode(5)), SubNode(ValNode(5), VarNode('x')))
    print ComputeVisitor(env=env).visit(tree)
    print StringVisitor(env=env).visit(tree)
    print visit_string(('*', ('+', 4, 5), ('-', 5, 2)))
    print visit_compute(('*', ('+', 4, 5), ('-', 5, 2)))
