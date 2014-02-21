from binary import BinaryNode
#TODO
class KDNode(BinaryNode):

    def __init__(self, value, axis=0, left=None, right=None):
        ''' Initializes a new instance of the kdtree

        :param value: The n-dimensional point value of this node
        :param axis: The axis this node uses
        :param left: The left child of this node (default None)
        :param right: The right child of this node (default None)
        '''
        super(KDNode, self).__init__(value, left, right)
        self.k = len(value)
        self.x = axis

    @classmethod
    def create(klass, ys, depth=0):
        ''' Create a new kd tree given a collection of points

        :param ys: The points to create a tree from
        :param depth: The current depth of the tree (default 0)
        :returns: The initialized kd-tree
        '''
        if not ys: return None      # recursion base case
        k = len(ys[0])              # find the number of axis
        x = depth % k               # choose the correct axis
        ys.sort(key=lambda y: y[x]) # sort by current axis
        m = len(ys) // 2            # select mid point
        node = klass(ys[m], axis=x) # create mid point node
        node.left  = klass.create(ys[:m], depth + 1)
        node.right = klass.create(ys[m+1:], depth + 1)
        return node

    def add(self, point):
        ''' Insert a new n-dimensional point into the tree

        :param point: The point to insert in the tree
        '''
        curr = self
        while curr:
            print curr.value, point
            xn = (curr.x + 1) % curr.k
            if curr.value == point: break
            if curr.value[curr.x] > point[curr.x]:
                if not curr.left:
                    curr.left = type(self)(point, axis=xn)
                    break
                else: curr = curr.left
            elif curr.value[curr.x] < point[curr.x]:
                if not curr.right:
                    curr.right = type(self)(point, axis=xn)
                    break
                else: curr = curr.right

    def neighbors(self, point, k):
        ''' Given a point and a number k, return
        the k nearest neighbors of the given point.

        :param point: The point to find the neighbors of
        :param k: The number of neighbors to return
        :returns: The k-nearest neighbors
        '''
        pass

    def __in_region(self, region, bound):
        '''
        '''
        k = self.k // 2
        return (all(r <= b for r, b in zip(region[:k], bound[:k])) 
            and all(r >= b for r, b in zip(region[k+1:], bound[k+1:])))

    def search(self, region):
        ''' Given a point and a number k, return
        the k nearest neighbors of the given point.

        :param point: The point to find the neighbors of
        :param k: The number of neighbors to return
        :returns: The k-nearest neighbors
        '''
        pass


#------------------------------------------------------------
# Implement points in leaf only tree
#------------------------------------------------------------
