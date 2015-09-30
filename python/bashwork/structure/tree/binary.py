import sys
import json

class BinaryNode(object):
    ''' A simple binary node that can be used to create
    binary trees. It implements all the comparable magic methods
    based on the current value of the node.
    '''

    def __init__(self, value=None, left=None, right=None, **kwargs):
        ''' Initializes a new instance of the class.

        :param value: The value of this node
        :param left: The left child of this node (default None)
        :param right: The right child of this node (default None)
        '''
        self.value = value
        self.left  = left
        self.right = right
        self.attrs = kwargs

    def __getitem__(self, k):    return self.attrs.get(k, None)
    def __setitem__(self, k, v): self.attrs[k] = v
    def __eq__(self, other):     return isinstance(other, BinaryNode) and (self.value == other.value)
    def __ne__(self, other):     return not (self == other)
    def __lt__(self, other):     return isinstance(other, BinaryNode) and (self.value  < other.value)
    def __le__(self, other):     return isinstance(other, BinaryNode) and (self.value <= other.value)
    def __gt__(self, other):     return isinstance(other, BinaryNode) and (self.value  > other.value)
    def __ge__(self, other):     return isinstance(other, BinaryNode) and (self.value >= other.value)
    def __hash__(self):          return hash(self.value)
    def __repr__(self):          return str(self.value)
    def __str__(self):           return str(self.value)
    def __contains__(self, v):   return self.contains(v)

    def is_leaf_node(self):
        ''' Check if the current node is a leaf nodes
        or not.

        :returns: True if a leaf node, False otherwise
        '''
        return not bool(self.right or self.left)

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
        left  = self.left.height() if self.left else 0
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
        tree       = klass(value=xs[m])
        tree.left  = klass.create(xs[:m])
        tree.right = klass.create(xs[m+1:])
        return tree

    @classmethod
    def create_with_parents(klass, xs, prev=None):
        ''' Given an ordered list of values, create a binary tree
        that has parent links

        :param xs: The values to create a tree from
        :param prev: The previous head of the tree
        :returns: The balanced binary tree
        '''
        if not xs: return None
        m = len(xs) // 2
        tree       = klass(value=xs[m])
        tree.prev  = prev
        tree.left  = klass.create_with_parents(xs[:m], tree)
        tree.right = klass.create_with_parents(xs[m+1:], tree)
        return tree

def simple_is_tree_balanced(tree):
    ''' Check if the tree is balanced by just checking
    if there exists a min / max leaf distance greater
    than 1.

    :param tree: The tree to check for balance
    :returns: True if it is balanced, False otherwise
    '''
    def tree_height(node, test):
        if not node: return 0
        return 1 + test(tree_height(node.left, test),
            tree_height(node.right, test))
    return (tree_height(tree, max) - tree_height(tree, min)) <= 1

#------------------------------------------------------------
# stack/queue tree traversal
#------------------------------------------------------------
def dfs_preorder_iter(tree):
    '''

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

def dfs_inorder_iter2(tree):
    ''' This is roughly what would be needed to implement
    the inorder in java.

    :param tree: The root tree node to traverse from
    :returns: An iterator around that tree
    '''
    def has_next(stack):
        return bool(stack)

    def populate(stack, node):
        stack.append(node)
        while node.left:
            node = node.left
            stack.append(node)
        return stack

    stack = populate([], tree) 
    while has_next(stack):
        node = stack.pop()
        if node.right:
            populate(stack, node.right)
        yield node

def dfs_postorder_iter(tree):
    '''

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

    '''
    if not treea and treeb: return False
    if not treeb and treea: return False
    if not treeb and not treea: return True
    return (treea == treeb and
            is_mirror_tree_recur(treea.left, treeb.right) and
            is_mirror_tree_recur(treea.right, treeb.left))

def create_mirror_tree(tree):
    ''' Given a tree, create a mirror of the given
    tree.

    :param tree: The tree to create a mirror of
    :returns: The mirror of the supplied tree
    '''
    if not tree: return None
    left, right = create_mirror_tree(tree.right), create_mirror_tree(tree.left)
    return BinaryNode(tree.value, left, right)

def get_tree_zig_zag(root):
    ''' Given a binary tree, print it out in a
    zig zag fashion.

    :param root: The root node to return as a zig zag
    :returns: A zip zag trail through the tree
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
    ''' Determine if two trees are equal (using recursion).

    :param treea: The first tree to compare
    :param treeb: The second tree to compare
    :returns: True if the trees are equal, False otherwise
    '''
    if not treea and not treeb: return True
    if not treea  or not treeb: return False
    if treea != treeb: return False
    return (are_trees_equal_recur(treea.left,  treeb.left) and
            are_trees_equal_recur(treea.right, treeb.right))

def are_trees_equal_iter(treea, treeb):
    ''' Determine if two trees are equal (imperative solution).

    :param treea: The first tree to compare
    :param treeb: The second tree to compare
    :returns: True if the trees are equal, False otherwise
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

are_trees_equal = are_trees_equal_iter

def find_path(tree, node):
    ''' Given a tree, find a path from the root
    node to the given node element.

    :param tree: The tree to search for a path
    :param node: The node to find a path to
    :returns: A path from the root of the tree to the node
    '''
    current  = None # current node in the tree
    trail    = {}   # parent node adjacency graph
    path     = []   # the resulting path to the node
    for current in dfs_preorder_iter(tree):
        if not current or (current == node):
            break
        trail[current.right] = current
        trail[current.left]  = current

    while current:
        path.insert(0, current)
        current = trail.get(current, None)
    return path

def find_path_binary(tree, node):
    ''' Given a binary tree, find a path from the root
    node to the given node element.

    :param tree: The tree to search for a path
    :param node: The node to find a path to
    :returns: A path from the root of the tree to the node
    '''
    path = []
    while tree and (tree != node):
        path.append(tree)
        if   tree > node: tree = tree.left
        elif tree < node: tree = tree.right
    return path + [node] if (tree == node) else []

def get_all_paths(tree):
    ''' Given a tree, return all the paths from
    the supplied root node to a leaf.

    :param tree: The tree to search for a path
    :returns: A list of paths from the root of the tree to the leafs
    '''
    queue = [[tree]]
    while queue:
        path = queue.pop()
        head = path[-1]
        if not head.is_leaf_node():
            if head.left:  queue.insert(0, path + [head.left ])
            if head.right: queue.insert(0, path + [head.right])
        else: yield path

def find_common_ancestor(tree, nodea, nodeb):
    ''' Given a tree and two nodes, find the common
    ancestor of the two nodes.

    :param tree: The tree to search for a path
    :param nodea: The first node to find a path to
    :param nodeb: The second node to find a path to
    :returns: The common node or None if none exist
    '''
    patha = find_path_binary(tree, nodea)
    pathb = find_path_binary(tree, nodeb)
    match = None

    for a, b in zip(patha, pathb):
        if a != b: break # first difference in the path
        else: match = a  # current path match
    return match

def find_node_distance(tree, nodea, nodeb):
    ''' Given a tree and two nodes in the tree, find
    the minimal distance between the two nodes.

    :param tree: The tree to search for a path
    :param nodea: The first node to find a path to
    :param nodeb: The second node to find a path to
    :returns: The minimal distance between the two nodes
    '''
    patha = find_path_binary(tree, nodea)
    pathb = find_path_binary(tree, nodeb)

    if not patha or not pathb:
        return None

    while patha and pathb:
        a, b = patha.pop(0), pathb.pop(0)
        if a != b: break
    return len(patha) + len(pathb) + 2

def lca(tree, nodea, nodeb):
    ''' Given two trees, check if one is a subtree of
    the other.

    '''
    if not tree: return None               # no LCA
    if tree == nodea: return tree
    if tree == nodeb: return tree
    left  = lca(tree.left,  nodea, nodeb)  # search left side for LCA
    right = lca(tree.right, nodea, nodeb)  # search right side for LCA
    if left and right: return tree         # we are at LCA pivot
    if left: return left                   # found left, now search up
    return right                           # found right, now search up

def is_a_subtree(tree, node):
    ''' Given two trees, check if one is a subtree of
    the other.

    :param tree: The main tree to search in
    :param node: The root node to check for
    :returns: True if node is a subtree, False otherwise
    '''
    if not tree: return False
    if not node: return True
    if (tree == node and
        are_trees_equal(tree, node)): return True

    return (is_a_subtree(tree.left, node) or
            is_a_subtree(tree.right, node))

def nth_tree_value(tree, n):
    ''' Return the nth value in order of the tree.

    :param tree: The tree to retrieve a value from
    :param n: The value to retrieve from the tree
    :returns: the Nth value of the tree in order
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


def serialize_tree_sexp(tree):
    ''' Given a tree, serialize it to a sexp expression

    :param tree: The tree to serialize to a list
    :returns: The serialzied tree expression
    '''
    def serialize(node):
        if node == None: return '()'
        child = " ({}, {})".format(
            serialize(node.left), serialize(node.right))
        if len(child) == 9: child = ''
        return "({}{})".format(node.value, child)
    return serialize(tree)


def serialize_tree_list(tree):
    ''' Given a tree, serialize it to a list

    :param tree: The tree to serialize to a list
    :returns: The serialzied tree list
    '''
    def serialize(node):
        if node == None: return [None]

        xs = [node.value]
        xs.extend(serialize(node.left))
        xs.extend(serialize(node.right))
        return xs
    return serialize(tree)


def deserialize_tree(string):
    ''' Given a serialized array convert it to a tree.

    :param string: The serialized tree
    :returns: The deserialzied tree
    '''
    return array_to_tree(json.loads(string))

def deserialize_tree_list(coll):
    ''' Given a serialized tree convert it to a tree.

    :param tree: The list to deserialize to a tree
    :returns: The deserialzied tree
    '''
    def deserialize(xs):
        if not xs: return None
        node = BinaryNode(value=xs.pop(0))
        node.left  = deserialize(xs)
        node.right = deserialize(xs)
        return node
    return deserialize(coll)

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

def get_max_subtree(node):
    ''' Given a tree, return the maximum sub-tree
    of the form (value, root) where value is the sum of
    all the sub-tree's nodes.

    :param node: The tree to find the maximum sub-tree of
    :returns: The result of (value, root) 
    '''
    if node == None: return (0, None) # identity
    if node.left == None and node.right == None:
        return (node.value, node)
    l, r = get_max_subtree(node.left), get_max_subtree(node.right)
    return max(l, r, (l[0] + r[0] + node.value, node))

def get_max_balanced_subtree(tree):
    ''' Given a binary tree, return the maximum balanced
    sub-tree that exists.

    :param tree: The tree to search
    :returns: (is_balanced, height)
    '''
    if tree == None: return (False, 0)

    return max((tree.is_balanced(), tree.height()),
        get_max_balanced_subtree(tree.left),
        get_max_balanced_subtree(tree.right))

def convert_tree_to_level_lists(tree):
    ''' Given a tree, covert it to a collection of lists
    where each list represents a level of the tree.

    :param tree: The tree to convert
    :returns: A list of lists for each tree level
    '''
    lists = [[tree]]

    for level in range(0, tree.height()):
        current = []
        for node in lists[level]:
            if node.left:  current.append(node.left)
            if node.right: current.append(node.right)
        if current: lists.append(current)
    return lists

def get_next_tree_inorder_node(node):
    def left_most_node(n):
        while n.left != None:                     # until we hit a leaf
            n = n.left                            # keep moving left down the tree
        return n                                  # return the leaf node

    def right_side_parent(n):
        while n.parent != None:                   # until we have reached the parent
            n, p = n.parent, n                    # keep walking up the tree
            if n.left == p: break                 # check if parent is on the right
        return n                                  # return the parent

    if not node: return node                      # invalid node
    if node.parent == None or node.right != None: # parent node
        return left_most_node(node.right)         # or we have a right node
    return right_side_parent(node.parent)         # otherwise get on the left side

