from hamcrest import *
from collections import namedtuple
import unittest

Node = namedtuple('Node', ['value', 'tail'])

class HamcrestTester(unittest.TestCase):

    def test_equals(self):
        head = Node(12, Node(13, None))
        tail = Node(13, None)

        assert_that(head, instance_of(Node))
        assert_that(head, same_instance(head))
        assert_that(head, not_none())
        assert_that(tail.tail, none())
        assert_that(head.tail, equal_to(tail))
        assert_that(head, is_not(equal_to(tail)))

    def test_collections(self):
        xs = [1, 2, 3, 4, 5]

        assert_that(xs, contains(1, 2, 3, 4, 5))
        assert_that(xs, has_item(2))
        assert_that(xs, has_items(2, 5, 4))

if __name__ == '__main__':
    unittest.main()
