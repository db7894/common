class Tree(object):
    ''' A simple implementation of a binary tree node
    '''

    def __init__(self, value, left=None, right=None):
        self.value = value
        self.left  = left
        self.right = right

    def __repr__(self):
        return str(tree_to_array(self))
    __str__ = __repr__

def tree_dfs(tree):
    ''' Given a tree, walk it in dfs left-to-right

    >>> tree = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> [n.value for n in tree_dfs(tree)]
    [5, 2, 1, 3, 4, 6]
    '''
    stack = [tree]
    while stack:
        current = stack.pop()
        yield current
        if current.right: stack.append(current.right)
        if current.left: stack.append(current.left)

def tree_dfs_reverse(tree):
    ''' Given a tree, walk it in dfs right-to-left

    >>> tree = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> [n.value for n in tree_dfs_reverse(tree)]
    [5, 6, 2, 3, 4, 1]
    '''
    stack = [tree]
    while stack:
        current = stack.pop()
        yield current
        if current.left:  stack.append(current.left)
        if current.right: stack.append(current.right)

def tree_bfs(tree):
    ''' Given a tree, walk it in bfs left-to-right

    >>> tree = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> [n.value for n in tree_bfs(tree)]
    [5, 2, 6, 1, 3, 4]
    '''
    queue = [tree]
    while queue:
        current = queue.pop()
        yield current
        if current.left:  queue.insert(0, current.left)
        if current.right: queue.insert(0, current.right)

def tree_bfs_reverse(tree):
    ''' Given a tree, walk it in bfs right-to-left

    >>> tree = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> [n.value for n in tree_bfs_reverse(tree)]
    [5, 6, 2, 3, 1, 4]
    '''
    queue = [tree]
    while queue:
        current = queue.pop()
        yield current
        if current.right: queue.insert(0, current.right)
        if current.left:  queue.insert(0, current.left)

def is_binary_search_tree(tree):
    ''' Checks if a binary tree is a valid binary
    search tree.
    >>> tree = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> is_binary_search_tree(tree)
    True

    >>> tree = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(9))), Tree(6))
    >>> is_binary_search_tree(tree)
    False
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

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(5, Tree(6), Tree(2, Tree(3, Tree(4)), Tree(1)))
    >>> is_mirror_tree(treea, treeb)
    True

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> is_mirror_tree(treea, treeb)
    False
    '''
    if not treea and treeb: return False
    if not treeb and treea: return False
    if not treeb and not treea: return True
    return (treea.value == treeb.value and
            is_mirror_tree(treea.left, treeb.right) and
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


def are_trees_equal(treea, treeb):
    ''' Determine if two trees are equal.

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> are_trees_equal(treea, treeb)
    True

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(9))), Tree(6))
    >>> are_trees_equal(treea, treeb)
    False
    '''
    if not treea and not treeb: return True
    if not treea  or not treeb: return False
    if treea.value != treeb.value: return False
    return (are_trees_equal(treea.left,  treeb.left) and
            are_trees_equal(treea.right, treeb.right))


def are_trees_equal_II(treea, treeb):
    ''' Determine if two trees are equal.

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> are_trees_equal_II(treea, treeb)
    True

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(9))), Tree(6))
    >>> are_trees_equal_II(treea, treeb)
    False
    '''
    stack = [(treea, treeb)]
    while stack:
        l, r = stack.pop()
        if not l and not r: continue     # both are null leafs
        if not l or  not r: return False # only one null leaf
        if l.value != r.value: return False
        stack.insert(0, (l.left, r.left))
        stack.insert(0, (l.right, r.right))
    return True


def find_path(tree, node):
    ''' Given a tree, find a path from the root
    node to the given node element.

    >>> tree = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> node = Tree(4)
    >>> find_path(tree, node).pop().value
    4
    '''
    current  = None # current node in the tree
    trail    = {}   # parent node adjacency graph
    path     = []   # the resulting path to the node
    for current in tree_dfs(tree):
        if not current or (current.value == node.value):
            break
        trail[current.right] = current
        trail[current.left]  = current

    while current:
        path.insert(0, current)
        current = trail.get(current, None)
    return path

def find_common_ancestor(tree, nodea, nodeb):
    ''' Given two trees, check if one is a subtree of
    the other.

    >>> tree  = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> nodea = Tree(4)
    >>> nodeb = Tree(1)
    >>> find_common_ancestor(tree, nodea, nodeb).value
    2
    '''
    patha = find_path(tree, nodea)
    pathb = find_path(tree, nodeb)
    match = None
    for a, b in zip(patha, pathb):
        if a.value != b.value: break
        match = a
    return match

def lca(tree, nodea, nodeb):
    ''' Given two trees, check if one is a subtree of
    the other.

    >>> tree  = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> nodea = Tree(4)
    >>> nodeb = Tree(1)
    >>> lca(tree, nodea, nodeb).value
    2
    '''
    if not tree: return None               # no LCA
    if tree.value == nodea.value: return tree
    if tree.value == nodeb.value: return tree
    left  = lca(tree.left,  nodea, nodeb)  # search left side for LCA
    right = lca(tree.right, nodea, nodeb)  # search right side for LCA
    if left and right: return tree         # we are at LCA pivot
    if left: return left                   # found left, now search up
    return right                           # found right, now search up

def is_a_subtree(tree, node):
    ''' Given two trees, check if one is a subtree of
    the other.

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(2, Tree(1), Tree(3, right=Tree(4)))
    >>> is_a_subtree(treea, treeb)
    True

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(6, Tree(5, Tree(7)))
    >>> is_a_subtree(treea, treeb)
    False
    '''
    if not tree: return False
    if not node: return True
    if (tree.value == node.value and
        are_trees_equal(tree, node)): return True
    return (is_a_subtree(tree.left, node) or
            is_a_subtree(tree.right, node))

def nth_tree_value(tree, n):
    ''' Return the nth value in order of the tree.

    >>> tree = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> nth_tree_value(tree, 4)
    4
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
    >>> tree = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> tree_to_array(tree)
    [1, 2, 3, 4, 5, 6]
    '''
    if not tree: return []
    def convert(array, node):
        if node.left: convert(array, node.left)
        array.append(node.value)
        if node.right: convert(array, node.right)
        return array
    return convert([], tree)


def array_to_tree(array):
    ''' Given a sorted array, convert it into a
    minimal binary tree.
    >>> array = [1, 2, 3, 4, 5, 6]
    >>> array_to_tree(array)
    [1, 2, 3, 4, 5, 6]
    '''
    if not array: return Tree(None)
    def convert(l, h):
        if l > h: return None
        m = (l + h) / 2
        return Tree(array[m], convert(l, m - 1), convert(m + 1, h))
    return convert(0, len(array) - 1)


def serialize_tree(tree):
    ''' Given a serialized array convert it to a tree.
    '''
    from json import dumps
    return dumps(tree_to_array(tree))


def deserialize_tree(string):
    ''' Given a serialized array convert it to a tree.
    '''
    from json import loads
    return loads(array_to_tree(tree))

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

if __name__ == "__main__":
    import doctest
    doctest.testmod()
