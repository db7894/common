class Tree(object):
    ''' A simple implementation of a binary tree node
    '''

    def __init__(self, value, left=None, right=None):
        self.value = value
        self.left  = left
        self.right = right

    def __repr__(self):
        return "{}<-{}->{}".format(str(self.left), self.value, str(self.right))
    __str__ = __repr__

def is_binary_search_tree(tree):
    ''' Checks if a binary tree is a valid binary
    search tree.
    '''
    def check(node, minv, maxv):
        if node == None: return True
        if minv and minv > node.value:  return False
        if maxv and maxv <= node.value: return False
        return (check(node.left,  minv, node.value) and
                check(node.right, node.value, maxv))
    return check(tree, None, None)

def is_mirror_tree(treea, treeb):
    ''' Given two trees, check if they are mirrors
    of each other.
    '''
    if not treea and treeb: return False
    if not treeb and treea: return False
    if not treeb and not treea: return True
    return (is_mirror_tree(treea.left, treeb.right) and
            is_mirror_tree(treea.right, treeb.left))

def print_zig_zag(root):
    ''' Given a binary tree, print it out in a
    zig zag fashion.
    '''
    values = []
    stackr, stackl = [], [root]
    while stackr or stackl:
        while stackl:
            node = stackl.pop()
            values.append(node.value)
            if node.left:  stackr.append(node.left)
            if node.right: stackr.append(node.right)

        while stackr:
            node = stackr.pop()
            values.append(node.value)
            if node.right: stackl.append(node.right)
            if node.left:  stackl.append(node.left)

def is_equal_tree(treea, treeb):
    ''' Determine if two trees are equal.
    '''
    if not treea and not treeb: return True
    if not treea  or not treeb: return False
    if treea.value != treeb.value: return False
    return (is_equal_tree(treea.left,  treeb.left) and
            is_equal_tree(treea.right, treeb.right))

def is_subtree(tree, node):
    ''' Given two trees, check if one is a subtree of
    the other.
    '''
    if not tree: return False
    if not node: return True
    if (tree.value == node.value and
        is_equal_tree(tree, node)): return True
    return (is_subtree(tree.left, node) or
            is_subtree(tree.right, node))

def nth_tree_value(tree, n):
    ''' Return the nth value in order of the tree.
    '''
    stack, node = [], tree
    while node or stack:
        if node:
            stack.append(node)
            node = node.left
        else:
            n, node = n - 1, stack.pop()
            if not n: return node.value
            node = node.right
    return None

def tree_to_array(tree):
    ''' Given a binary tree, convert it into a sorted array.
    '''
    def convert(array, node):
        if node.left: convert(array, node.left)
        array.append(node.value)
        if node.right: convert(array, node.right)
        return array
    return convert([], tree)

def array_to_tree(array):
    ''' Given a sorted array, convert it into a
    minimal binary tree.
    '''
    def convert(l, h):
        if l > h: return None
        m = (l + h) / 2
        return Tree(array[m], convert(l, m - 1), convert(m + 1, h))
    return convert(0, len(array) - 1)

def invert_tree(tree):
    def invert(node, left, right):
        if not node: return []
        if node.left or node.right:
            roots  = invert(node.left, None, node)
            roots += invert(node.right, node, None)
        else: roots = [node]
        node.left, node.right = left, right
        return roots
    return invert(tree, None, None)
