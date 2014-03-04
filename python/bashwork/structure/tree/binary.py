import json

class BinaryNode(object):
    ''' A simple binary node that can be used to create
    binary trees. It implements all the comparable magic methods
    based on the current value of the node.
    '''

    def __init__(self, value, left=None, right=None):
        ''' Initializes a new instance of the class.

        :param value: The value of this node
        :param left: The left child of this node (default None)
        :param right: The right child of this node (default None)
        '''
        self.value = value
        self.left  = left
        self.right = right

    def __eq__(self, other):   return other and (other.value == self.value)
    def __ne__(self, other):   return other and (other.value != self.value)
    def __lt__(self, other):   return other and (other.value  < self.value)
    def __le__(self, other):   return other and (other.value <= self.value)
    def __gt__(self, other):   return other and (other.value  > self.value)
    def __ge__(self, other):   return other and (other.value >= self.value)
    def __hash__(self):        return hash(self.value)
    def __repr__(self):        return str(self.value)
    def __str__(self):         return str(self.value)
    def __contains__(self, v): return self.contains(v)

    def height(self):
        ''' Given a tree, return its current max height

        :returns: The max height of the tree
        '''
        left  = 1 + self.left.height()  if self.left  else 0
        right = 1 + self.right.height() if self.right else 0
        return max(left, right)

    def is_balanced(self):
        ''' Given a tree, check if it is balanced.  A tree
        is balanced if the heights of each of its subtrees
        differ by at most one degree.

        :returns: True if balanced, False otherwise
        '''
        left  = self.left.height()  if self.left  else 0
        right = self.right.height() if self.right else 0
        if abs(left - right) > 1: return False
        return ((self.left.is_balanced() if self.left else True)
            and (self.right.is_balanced() if self.right else True))

    def min(self):
        ''' Given a binary tree, find the minimum value

        :returns: The smallest value in a tree
        '''
        curr = self
        while curr.left: curr = curr.left
        return curr

    def max(self):
        ''' Given a binary tree, find the largest value

        :returns: The largest value in the tree
        '''
        curr = self
        while curr.right: curr = curr.right
        return curr

    def contains(self, value):
        ''' Check if the supplied value is in the tree

        :param value: The value to check existance for
        '''
        curr = self
        while curr:
            if curr.value == value: return True
            if curr.value  > value: curr = curr.left
            if curr.value  < value: curr = curr.right
        return False

    def add(self, value):
        ''' Insert the supplied point into the tree

        :param value: The value to insert into the tree
        '''
        curr, node = self, type(self)(value)
        while curr:
            if curr.value == value: break
            if curr.value > value:
                if curr.left: curr = curr.left
                else: curr.left = node
            if curr.value < value:
                if curr.right: curr = curr.right
                else: curr.right = node

    def remove(self, value):
        ''' Remove the supplied point from the tree

        :param value: The value to remove into the tree
        '''
        def insert(p, c, n):
            if p.left  == c: p.left  = n
            if p.right == c: p.right = n

        prev, curr = None, self
        while curr:
            if curr.value == value:                              # found who to remove
                if curr.left and curr.right:                     # node has left and right children
                    node = curr.left.max()                       # find max predecessor
                    curr.remove(node)                            # remove hoisted node
                    node.left  = curr.left                       # take over left child
                    node.right = curr.right                      # take over right child
                    insert(prev, curr, node)                     # insert to the parent
                elif curr.left:  insert(prev, curr, curr.left)   # only left children
                elif curr.right: insert(prev, curr, curr.right)  # only right children
                else: insert(prev, curr, None)                   # no children
            if curr.value > value: prev, curr = curr, curr.left  # search left
            if curr.value < value: prev, curr = curr, curr.right # search right

    @classmethod
    def create(klass, xs):
        ''' Given an ordered list of values, create a binary tree

        :param xs: The values to create a tree from
        :returns: The balanced binary tree
        '''
        if not xs: return None
        m = len(xs) // 2
        tree       = klass(xs[m])
        tree.left  = klass.create(xs[:m])
        tree.right = klass.create(xs[m+1:])
        return tree

    @classmethod
    def create_prev(klass, xs, prev=None):
        ''' Given an ordered list of values, create a binary tree
        that has parent links

        :param xs: The values to create a tree from
        :param prev: The previous head of the tree
        :returns: The balanced binary tree
        '''
        if not xs: return None
        m = len(xs) // 2
        tree       = klass(xs[m])
        tree.prev  = prev
        tree.left  = klass.create_prev(xs[:m], tree)
        tree.right = klass.create_prev(xs[m+1:], tree)
        return tree

#------------------------------------------------------------
# stack/queue tree traversal
#------------------------------------------------------------
def dfs_preorder_iter(tree):
    '''
    >>> tree = BinaryNode.create(range(10))
    >>> node = dfs_preorder_iter(tree)
    >>> list(node)
    [5, 2, 1, 0, 4, 3, 8, 7, 6, 9]

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    if not tree: return
    stack = [tree]
    while stack:
        current = stack.pop()
        yield current
        if current.right: stack.append(current.right)
        if current.left:  stack.append(current.left)

def dfs_inorder_iter(tree):
    '''
    >>> tree = BinaryNode.create(range(10))
    >>> node = dfs_inorder_iter(tree)
    >>> list(node)
    [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    node, stack = tree, []
    while stack or node:
        if node:
            stack.append(node)
            node = node.left
        else:
            node = stack.pop()
            yield node
            node = node.right

def dfs_postorder_iter(tree):
    '''
    >>> tree = BinaryNode.create(range(10))
    >>> node = dfs_postorder_iter(tree)
    >>> list(node)
    [0, 1, 3, 4, 2, 6, 7, 9, 8, 5]

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    prev, stack = None, [tree]
    while stack:
        curr = stack[-1]
        # going down first time, left, or right
        if not prev or (prev.left == curr) or (prev.right == curr):
            if curr.left: stack.append(curr.left)
            elif curr.right: stack.append(curr.right)
        # coming back up from the left
        elif curr.left == prev:
            if curr.right: stack.append(curr.right)
        # coming back up from the right
        else:
            yield curr
            stack.pop()
        prev = curr

def bfs_ltor_iter(tree):
    '''
    >>> tree = BinaryNode.create(range(10))
    >>> node = bfs_ltor_iter(tree)
    >>> list(node)
    [5, 2, 8, 1, 4, 7, 9, 0, 3, 6]

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    if not tree: return
    queue = [tree]
    while queue:
        current = queue.pop()
        yield current
        if current.left:  queue.insert(0, current.left)
        if current.right: queue.insert(0, current.right)

def bfs_rtol_iter(tree):
    '''
    >>> tree = BinaryNode.create(range(10))
    >>> node = bfs_rtol_iter(tree)
    >>> list(node)
    [5, 8, 2, 9, 7, 4, 1, 6, 3, 0]

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    if not tree: return
    queue = [tree]
    while queue:
        current = queue.pop()
        yield current
        if current.right: queue.insert(0, current.right)
        if current.left:  queue.insert(0, current.left)

#------------------------------------------------------------
# recursive tree traversal
#------------------------------------------------------------
def dfs_preorder_recur(tree):
    '''
    >>> tree = BinaryNode.create(range(10))
    >>> node = dfs_preorder_recur(tree)
    >>> list(node)
    [5, 2, 1, 0, 4, 3, 8, 7, 6, 9]

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    if not tree: return
    yield tree
    if tree.left:
        for node in dfs_preorder_recur(tree.left):
            yield node
    if tree.right:
        for node in dfs_preorder_recur(tree.right):
            yield node

def dfs_inorder_recur(tree):
    '''
    >>> tree = BinaryNode.create(range(10))
    >>> node = dfs_inorder_recur(tree)
    >>> list(node)
    [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    if not tree: return
    if tree.left:
        for node in dfs_inorder_recur(tree.left):
            yield node
    yield tree
    if tree.right:
        for node in dfs_inorder_recur(tree.right):
            yield node

def dfs_postorder_recur(tree):
    '''
    >>> tree = BinaryNode.create(range(10))
    >>> node = dfs_postorder_recur(tree)
    >>> list(node)
    [0, 1, 3, 4, 2, 6, 7, 9, 8, 5]

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    if not tree: return
    if tree.left:
        for node in dfs_postorder_recur(tree.left):
            yield node
    if tree.right:
        for node in dfs_postorder_recur(tree.right):
            yield node
    yield tree

#------------------------------------------------------------
# linked tree traversal
#------------------------------------------------------------
def dfs_preorder_prev(tree):
    '''
    >>> tree = BinaryNode.create_prev(range(10))
    >>> node = dfs_preorder_prev(tree)
    >>> list(node)
    [5, 2, 1, 0, 4, 3, 8, 7, 6, 9]

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    curr, prev = tree, None
    while curr: 
        if prev == curr.prev:       # going down
            yield curr
            prev, curr = curr, curr.left or curr.right or curr.prev
        elif prev == curr.left:     # up from the left
            prev, curr = curr, curr.right or curr.prev
        elif prev == curr.right:    # up from the right
            prev, curr = curr, curr.prev

def dfs_inorder_prev(tree):
    '''
    >>> tree = BinaryNode.create_prev(range(10))
    >>> node = dfs_inorder_prev(tree)
    >>> list(node)
    [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    curr, prev = tree, None
    while curr: 
        if prev == curr.prev:       # going down
            if not curr.left: yield curr
            prev, curr = curr, curr.left or curr.right or curr.prev
        elif prev == curr.left:     # up from the left
            yield curr
            prev, curr = curr, curr.right or curr.prev
        elif prev == curr.right:    # up from the right
            prev, curr = curr, curr.prev

def dfs_postorder_prev(tree):
    '''

    >>> tree = BinaryNode.create_prev(range(10))
    >>> node = dfs_postorder_prev(tree)
    >>> list(node)
    [0, 1, 3, 4, 2, 6, 7, 9, 8, 5]

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    curr, prev = tree, None
    while curr: 
        if prev == curr.prev:       # going down
            if not curr.left: yield curr
            prev, curr = curr, curr.left or curr.right or curr.prev
        elif prev == curr.left:     # up from the left
            if not curr.right: yield curr
            prev, curr = curr, curr.right or curr.prev
        elif prev == curr.right:    # up from the right
            yield curr
            prev, curr = curr, curr.prev

#------------------------------------------------------------
# recursive tree operations
#------------------------------------------------------------
def tree_contains_recur(tree, value):
    ''' A recursive implemplememtation to check if a value is
    in a tree.
    >>> tree = BinaryNode.create(range(10))
    >>> tree_contains_recur(tree, 5)
    True
    >>> tree_contains_recur(tree, 10)
    False

    :param tree: The tree to check if a value is in
    :param value: The value to check if in the tree
    :returns: True if in the tree, False otherwise
    '''
    if tree == None: return False
    return ((tree.value == value)
        or tree_contains_recur(tree.left, value)
        or tree_contains_recur(tree.right, value))

def tree_insert_recur(tree, value):
    ''' A recursive implemplememtation to check if a value is
    in a tree.
    >>> tree = BinaryNode.create(range(5))
    >>> tree_insert_recur(tree, 5)
    >>> node = dfs_inorder_iter(tree)
    >>> list(node)
    [0, 1, 2, 3, 4, 5]

    :param tree: The tree to check if a value is in
    :param value: The value to check if in the tree
    :returns: True if in the tree, False otherwise
    '''
    if tree.value > value:
        if tree.left: tree_insert_recur(tree.left, value)
        else: tree.left = type(tree)(value)
    elif tree.value < value:
        if tree.right: tree_insert_recur(tree.right, value)
        else: tree.right = type(tree)(value)
    else: pass # value already in the tree

def tree_insert_immut(tree, value):
    ''' A recursive implemplememtation to check if a value is
    in a tree.
    >>> tree = BinaryNode.create(range(5))
    >>> tree = tree_insert_immut(tree, 5)
    >>> node = dfs_inorder_iter(tree)
    >>> list(node)
    [0, 1, 2, 3, 4, 5]

    :param tree: The tree to check if a value is in
    :param value: The value to check if in the tree
    :returns: True if in the tree, False otherwise
    '''
    node = type(tree)
    if not tree: return BinaryNode(value)
    if tree.value == value: return tree
    if tree.value  < value:
        return node(tree.value, tree.left, tree_insert_immut(tree.right, value))
    if tree.value  > value:
        return node(tree.value, tree_insert_immut(tree.left, value), tree.right)

def is_binary_search_tree_recur(tree):
    ''' Checks if a binary tree is a valid binary
    search tree.
    >>> tree = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> is_binary_search_tree_recur(tree)
    True

    >>> tree = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(9))), Tree(6))
    >>> is_binary_search_tree_recur(tree)
    False
    '''
    def check(node, minv, maxv):
        if node == None: return True
        if minv and minv > node.value:  return False
        if maxv and maxv <= node.value: return False
        return (check(node.left,  minv, node.value) and
                check(node.right, node.value, maxv))
    return check(tree, None, None)


def is_mirror_tree_recur(treea, treeb):
    ''' Given two trees, check if they are mirrors
    of each other.

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(5, Tree(6), Tree(2, Tree(3, Tree(4)), Tree(1)))
    >>> is_mirror_tree_recur(treea, treeb)
    True

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> is_mirror_tree_recur(treea, treeb)
    False
    '''
    if not treea and treeb: return False
    if not treeb and treea: return False
    if not treeb and not treea: return True
    return (treea.value == treeb.value and
            is_mirror_tree_recur(treea.left, treeb.right) and
            is_mirror_tree_recur(treea.right, treeb.left))

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
    return values

def are_trees_equal_recur(treea, treeb):
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
    return (are_trees_equal_recur(treea.left,  treeb.left) and
            are_trees_equal_recur(treea.right, treeb.right))


def are_trees_equal_iter(treea, treeb):
    ''' Determine if two trees are equal.

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> are_trees_equal_iter(treea, treeb)
    True

    >>> treea = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(4))), Tree(6))
    >>> treeb = Tree(5, Tree(2, Tree(1), Tree(3, right=Tree(9))), Tree(6))
    >>> are_trees_equal_iter(treea, treeb)
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

    :param tree: The tree to convert to a sorted array
    :returns: The sorted array representing the tree
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

    :param array: The array to convert to a tree
    :returns: The converted tree
    '''
    if not array: return BinaryNode(None)
    def convert(l, h):
        if l > h: return None
        m = (l + h) / 2
        return BinaryNode(array[m], convert(l, m - 1), convert(m + 1, h))
    return convert(0, len(array) - 1)


def serialize_tree(tree):
    ''' Given a serialized array convert it to a tree.

    :param tree: The tree to serialize to json
    :returns: The serialzied tree
    '''
    return json.dumps(tree_to_array(tree))


def deserialize_tree(string):
    ''' Given a serialized array convert it to a tree.

    :param string: The serialized tree
    :returns: The deserialzied tree
    '''
    return json.loads(array_to_tree(tree))

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


#------------------------------------------------------------
# test runner
#------------------------------------------------------------

if __name__ == "__main__":
    import doctest
    doctest.testmod()