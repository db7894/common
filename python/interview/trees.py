class TreeNode(object):
    ''' A simple implementation of a binary tree node
    '''

    def __init__(self, value, left=None, right=None):
        self.left  = left
        self.right = right
        self.value = value

def is_binary_search_tree(tree):
    ''' Checks if a binary tree is a valid binary
    search tree.
    '''
    def check(node, minv, maxv):
        if node == None: return True
        if minv and minv > node.value:  return False
        if maxv and maxv <= node.value: return False
        return check(node.left,  minv, node.value) and
               check(node.right, node.value, maxv)
    return check(tree, None, None)
