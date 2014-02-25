#!/usr/bin/env python
import unittest
from kdtree import KDNode
from binary import dfs_inorder_iter

#---------------------------------------------------------------------------#
# helper methods
#---------------------------------------------------------------------------#
def is_valid(tree):
    ''' Given a kdtree, validate that it is valid

    :param tree: The tree to validate for correctness
    :returns: True if valid, False otherwise
    '''
    if not tree: return True                            # if we are at a leaf, we are valid
    l,c,r = tree.left, tree.value[tree.x], tree.right   # unpack values
    if l and l.value[tree.x] > c: return False          # left child should not be greater than current 
    if r and r.value[tree.x] < c: return False          # right child should not be greater than current 
    return is_valid(tree.left) and is_valid(tree.right) # all children should be valid


#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class KDNodeTest(unittest.TestCase):
    ''' Code to validate that the kdtree implementation is correct.
    '''

    def setUp(self):
        self.size   = 5
        self.points = [(x, y) for x in range(self.size) for y in range(self.size)]
        self.tree   = KDNode.create(self.points)

    def test_create(self):
        ''' Test that the kdtree is created correctly '''
        self.assertTrue(is_valid(self.tree))

    def test_add_to_tree(self):
        ''' Test that we can correctly add points to the kdtree '''
        for point in range(self.size + 1):
            self.tree.add((self.size + 1, point))
            self.tree.add((point, self.size + 1))
        self.assertTrue(is_valid(self.tree))


#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
raise unittest.SkipTest("broken tests")
if __name__ == "__main__":
    unittest.main()
