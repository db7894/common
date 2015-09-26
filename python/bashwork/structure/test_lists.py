#!/usr/bin/env python
import unittest
from bashwork.structure.lists import *

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class ListsTest(unittest.TestCase):

    def test_linked_list(self):
        xs = Node.create([1, 2, 3, 4, 5])
        xs.append(6)
        xs.link.link.insert(0)
        self.assertEqual(xs.to_list(), [1, 2, 3, 0, 4, 5, 6])
        self.assertEqual(xs.find(6).value, 6)
        self.assertEqual(xs.last().value, 6)
        self.assertEqual(xs.next().value, 2)
        self.assertEqual(xs, Node(1))
        self.assertNotEqual(xs, Node(2))
        self.assertEqual(str(xs), '1')
        self.assertEqual(repr(xs), '1')
        self.assertEqual(hash(xs), 1)

    def test_sum_integer_nodes(self):
        xs = Node.create(3, 1, 5)
        ys = Node.create(5, 9, 2)
        rs = sum_integer_nodes(xs, ys)
        self.assertEqual(rs.to_list(), [9, 0, 7])

        xs = Node.create(9, 1, 5)
        ys = Node.create(5, 9, 2)
        rs = sum_integer_nodes(xs, ys)
        self.assertEqual(rs.to_list(), [1, 5, 0, 7])

    def test_sum_integer_nodes_little_endian(self):
        xs = Node.create(3, 1, 5)
        ys = Node.create(5, 9, 2)
        rs = sum_integer_nodes_little_endian(xs, ys)
        self.assertEqual(rs.to_list(), [8, 0, 8])

        xs = Node.create(9, 1, 5)
        ys = Node.create(5, 9)
        rs = sum_integer_nodes_little_endian(xs, ys)
        self.assertEqual(rs.to_list(), [4, 1, 6])

        xs = Node.create(9, 1, 5)
        ys = Node.create(5, 9, 8)
        rs = sum_integer_nodes_little_endian(xs, ys)
        self.assertEqual(rs.to_list(), [4, 1, 4, 1])

    def test_delete_middle_node(self):
        xs = Node.create(1, 2, 3, 4, 5, 6)
        rs = delete_middle_node(xs.link.link)
        self.assertEqual(xs.to_list(), [1, 2, 4, 5, 6])

    def test_find_missing_number(self):
        numbers = Node.create(1, 2, 3, 4, 5, 6, 8, 9, 10)
        number  = find_missing_number(numbers)
        self.assertEqual(number, 7)

    def test_make_unique_list(self):
        xs = Node.create(1, 2, 1, 3, 4, 5, 3)
        rs = make_unique_list(xs)[0]
        self.assertEqual(rs.to_list(), [1, 2, 3, 4, 5])

        xs = Node.create(1, 2, 1, 3, 4, 5, 3)
        rs = make_unique_list_no_storage(xs)
        self.assertEqual(rs.to_list(), [1, 2, 3, 4, 5])

    def test_reverse_list(self):
        xs = Node.create(1, 2, 3, 4, 5)
        rs = reverse_list(xs)
        self.assertEqual(rs.to_list(), [5, 4, 3, 2, 1])

        xs = Node.create(1, 2, 3, 4, 5)
        rs = reverse_list_recursive(xs)
        self.assertEqual(rs.to_list(), [5, 4, 3, 2, 1])

    def test_find_list_middle(self):
        xs = Node(1, Node(2, Node(3, Node(4, Node(5)))))
        rs = find_list_middle(xs)
        self.assertEqual(rs.value, 3)

    def test_find_from_list_end(self):
        xs = Node.create([1, 2, 3, 4, 5, 6])
        rs = find_from_list_end(xs, n=3)
        self.assertEqual(rs.value, 4)


#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
