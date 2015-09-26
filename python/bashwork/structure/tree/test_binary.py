import unittest
from bashwork.structure.tree.binary import *

#------------------------------------------------------------
# aliases
#------------------------------------------------------------
Node = BinaryNode

#------------------------------------------------------------
# tests
#------------------------------------------------------------

class BinaryTreeTest(unittest.TestCase):

#------------------------------------------------------------
# arbitrary tree traversals
#------------------------------------------------------------

    def test_dfs_preorder_iter(self):
        tree   = BinaryNode.create(range(10))
        actual = [node.value for node in dfs_preorder_iter(tree)]
        expect = [5, 2, 1, 0, 4, 3, 8, 7, 6, 9]
        self.assertEqual(actual, expect)

    def test_dfs_inorder_iter(self):
        tree = BinaryNode.create(range(10))
        actual = [n.value for n in dfs_inorder_iter(tree)]
        expect = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
        self.assertEqual(actual, expect)

    def test_dfs_inorder_iter2(self):
        tree = BinaryNode.create(range(10))
        actual = [n.value for n in dfs_inorder_iter2(tree)]
        expect = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
        self.assertEqual(actual, expect)

    def test_dfs_postorder_iter(self):
        tree = BinaryNode.create(range(10))
        actual = [n.value for n in dfs_postorder_iter(tree)]
        expect = [0, 1, 3, 4, 2, 6, 7, 9, 8, 5]
        self.assertEqual(actual, expect)

    def test_bfs_ltor_iter(self):
        tree = BinaryNode.create(range(10))
        actual = [n.value for n in bfs_ltor_iter(tree)]
        expect = [5, 2, 8, 1, 4, 7, 9, 0, 3, 6]
        self.assertEqual(actual, expect)

    def test_bfs_rtol_iter(self):
        tree = BinaryNode.create(range(10))
        actual = [n.value for n in bfs_rtol_iter(tree)]
        expect = [5, 8, 2, 9, 7, 4, 1, 6, 3, 0]
        self.assertEqual(actual, expect)

    def test_get_tree_zig_zag(self):
        pass

#------------------------------------------------------------
# recursive tree traversal
#------------------------------------------------------------

    def test_dfs_preorder_recur(self):
        tree   = BinaryNode.create(range(10))
        actual = [n.value for n in dfs_preorder_recur(tree)]
        expect = [5, 2, 1, 0, 4, 3, 8, 7, 6, 9]
        self.assertEquals(actual, expect)

    def test_dfs_inorder_recur(self):
        tree   = BinaryNode.create(range(10))
        actual = [n.value for n in dfs_inorder_recur(tree)]
        expect = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
        self.assertEquals(actual, expect)

    def test_dfs_postorder_recur(self):
        tree   = BinaryNode.create(range(10))
        actual = [n.value for n in dfs_postorder_recur(tree)]
        expect = [0, 1, 3, 4, 2, 6, 7, 9, 8, 5]
        self.assertEquals(actual, expect)

#------------------------------------------------------------
# tree traversal with parent links
#------------------------------------------------------------

    def test_dfs_preorder_prev(self):
        tree = BinaryNode.create_with_parents(range(10))
        actual = [n.value for n in dfs_preorder_prev(tree)]
        expect = [5, 2, 1, 0, 4, 3, 8, 7, 6, 9]
        self.assertEqual(actual, expect)

    def test_dfs_inorder_prev(self):
        tree = BinaryNode.create_with_parents(range(10))
        actual = [n.value for n in dfs_inorder_prev(tree)]
        expect = [0, 1, 2, 3, 4, 5, 6, 7, 8, 9]
        self.assertEqual(actual, expect)

    def test_dfs_postorder_prev(self):
        tree = BinaryNode.create_with_parents(range(10))
        actual = [n.value for n in dfs_postorder_prev(tree)]
        expect = [0, 1, 3, 4, 2, 6, 7, 9, 8, 5]
        self.assertEqual(actual, expect)

#------------------------------------------------------------
# tree traversal operations
#------------------------------------------------------------

    def test_tree_contains_recur(self):
        tree = BinaryNode.create(range(10))
        self.assertTrue(tree_contains_recur(tree, 5))
        self.assertFalse(tree_contains_recur(tree, 10))

    def test_tree_insert_recur(self):
        tree = BinaryNode.create(range(5))
        tree_insert_recur(tree, 5)
        actual = [n.value for n in dfs_inorder_iter(tree)]
        expect = [0, 1, 2, 3, 4, 5]
        self.assertEqual(actual, expect)

    def test_tree_insert_immut(self):
        tree   = BinaryNode.create(range(5))
        tree   = tree_insert_immut(tree, 5)
        actual = [n.value for n in dfs_inorder_iter(tree)]
        expect = [0, 1, 2, 3, 4, 5]
        self.assertEqual(actual, expect)

    def test_is_binary_search_tree_recur(self):
        tree = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        self.assertTrue(is_binary_search_tree_recur(tree))
        
        tree = Node(5, Node(2, Node(1), Node(3, right=Node(9))), Node(6))
        self.assertFalse(is_binary_search_tree_recur(tree))

    def test_is_mirror_tree_recur(self):
        treea = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        treeb = Node(5, Node(6), Node(2, Node(3, Node(4)), Node(1)))
        self.assertTrue(is_mirror_tree_recur(treea, treeb))
    
        treea = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        treeb = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        self.assertFalse(is_mirror_tree_recur(treea, treeb))

    def test_create_mirror_tree(self):
        treea = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        treeb = create_mirror_tree(treea)
        self.assertTrue(is_mirror_tree_recur(treea, treeb))

    def test_are_trees_equal_recur(self):
        treea = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        treeb = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        self.assertTrue(are_trees_equal_recur(treea, treeb))
    
        treea = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        treeb = Node(5, Node(2, Node(1), Node(3, right=Node(9))), Node(6))
        self.assertFalse(are_trees_equal_recur(treea, treeb))

    def test_are_trees_equal_iter(self):
        treea = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        treeb = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        self.assertTrue(are_trees_equal_iter(treea, treeb))
    
        treea = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        treeb = Node(5, Node(2, Node(1), Node(3, right=Node(9))), Node(6))
        self.assertFalse(are_trees_equal_iter(treea, treeb))

    def test_find_path(self):
        tree   = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        node   = Node(4)
        actual = find_path(tree, node).pop().value
        expect = 4
        self.assertEqual(actual, expect)

    def test_find_path_binary(self):
        tree   = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        node   = Node(4)
        actual = find_path_binary(tree, node).pop().value
        expect = 4
        self.assertEqual(actual, expect)

    def test_get_all_paths(self):
        tree   = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        actual = [[n.value for n in path] for path in get_all_paths(tree)]
        expect = [[5, 6], [5, 2, 1], [5, 2, 3, 4]]
        self.assertEquals(actual, expect)

    def test_find_common_ancestor(self):
        tree   = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        nodea  = Node(4)
        nodeb  = Node(1)
        actual = find_common_ancestor(tree, nodea, nodeb).value
        expect = 2
        self.assertEqual(actual, expect)

    def test_find_node_distance(self):
        tree   = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        nodea  = Node(4)
        nodeb  = Node(1)
        actual = find_node_distance(tree, nodea, nodeb)
        expect = 3
        self.assertEqual(actual, expect)

    def test_lca(self):
        tree   = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        nodea  = Node(4)
        nodeb  = Node(1)
        actual = lca(tree, nodea, nodeb).value
        expect = 2
        self.assertEqual(actual, expect)

    def test_is_a_subtree(self):
        treea = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        treeb = Node(2, Node(1), Node(3, right=Node(4)))
        self.assertTrue(is_a_subtree(treea, treeb))
    
        treea = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        treeb = Node(6, Node(5, Node(7)))
        self.assertFalse(is_a_subtree(treea, treeb))

    def test_nth_tree_value(self):
        tree   = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        actual = nth_tree_value(tree, 4)
        expect = 4
        self.assertEquals(actual, expect)

    def test_tree_to_array(self):
        tree   = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        actual = tree_to_array(tree)
        expect = [1, 2, 3, 4, 5, 6]
        self.assertEquals(actual, expect)

    def test_array_to_tree(self):
        array  = [1, 2, 3, 4, 5, 6]
        tree   = array_to_tree(array)
        actual = tree_to_array(tree)
        expect = [1, 2, 3, 4, 5, 6]
        self.assertEquals(actual, expect)

    def test_tree_serialize_list(self):
        tree = BinaryNode.create(range(10))
        actual = serialize_tree_list(tree)
        expect = [5, 2, 1, 0,
            None, None, None, 4, 3,
            None, None, None, 8, 7, 6,
            None, None, None, 9,
            None, None
        ]
        self.assertEquals(actual, expect)

    def test_tree_deserialize_list(self):
        expect = BinaryNode.create(range(10))
        actual = deserialize_tree_list(serialize_tree_list(expect))
        self.assertEquals(actual, expect)

    def test_simple_is_tree_balanced(self):
        tree = Node(5, Node(2, Node(1), Node(3, right=Node(4))), Node(6))
        self.assertFalse(simple_is_tree_balanced(tree))

        tree = array_to_tree([1, 2, 3, 4, 5, 6])
        self.assertTrue(simple_is_tree_balanced(tree))

#------------------------------------------------------------
# test runner
#------------------------------------------------------------
if __name__ == "__main__":
    unittest.main()
