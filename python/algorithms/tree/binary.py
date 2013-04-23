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

#------------------------------------------------------------
# test runner
#------------------------------------------------------------
if __name__ == "__main__":
    import doctest
    doctest.testmod()
