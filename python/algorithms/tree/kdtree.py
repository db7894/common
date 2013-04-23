from tree import BinaryNode

class KDNode(BinaryNode):

    def __init__(self, value, axis=0, left=None, right=None):
        super(KDTree, self).__init__(value, left, right)
        self.k = len(value)
        self.x = axis

    @classmethod
    def create(klass, ys, depth=0):
        ''' Create a new kd tree given a collection of points

        :param ys: The points to create a tree from
        :param depth: The current depth of the tree (default 0)
        :returns: The initialized kd-tree
        '''
        if not xs: return None      # recursion base case
        k = len(ys[0])              # find the number of axis
        x = depth % k               # choose the correct axis
        ys.sort(key=lambda y: y[x]  # sort by current axis
        m = len(xs) // 2            # select mid point
        node = klass(xs[m], axis=x) # create mid point node
        node.left  = klass.create(ys[:m], depth + 1)
        node.right = klass.create(ys[m+1:], depth + 1)
        return node

    def add(self, point):
        ''' Insert a new n-dimensional point into the tree

        :param point: The point to insert in the tree
        '''
        curr, node = self, type(self)(point)
        while curr:
            x = (curr.x + 1) % curr.k
            if curr.value[curr.x] > value[curr.x]:
                if not curr.left:
                    curr.left = type(self)(point, axis=x)
                    break
                else: curr = curr.left
            elif curr.value[curr.x] < value[curr.x]:
                if not curr.right:
                    curr.right = type(self)(point, axis=x)
                    break
                else: curr = curr.right

#------------------------------------------------------------
# Implement points in leaf only tree
#------------------------------------------------------------
