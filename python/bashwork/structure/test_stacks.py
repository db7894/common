#!/usr/bin/env python
import unittest
from bashwork.structure.stacks import *

#---------------------------------------------------------------------------#
# fixture
#---------------------------------------------------------------------------#
class StacksTest(unittest.TestCase):

    def test_min_stack(self):
        s = MinStack()
        assert(s.push(3) == None); assert(s.minimum() == 3)
        assert(s.push(4) == None); assert(s.minimum() == 3)
        assert(s.push(2) == None); assert(s.minimum() == 2)
        assert(s.push(1) == None); assert(s.minimum() == 1)
        assert(s.pop()   == 1);    assert(s.minimum() == 2)
        assert(s.pop()   == 2);    assert(s.minimum() == 3)
        assert(s.push(0) == None); assert(s.minimum() == 0)

    def test_stack_set(self):
        s = StackSet(threshold=2)
        self.assertEqual(s.push(3), None)
        self.assertEqual(s.push(4), None)
        self.assertEqual(s.push(2), None)
        self.assertEqual(len(s.stacks), 2)
        self.assertEqual(s.pop(), 2)     
        self.assertEqual(len(s.stacks), 1)
        self.assertEqual(s.pop(), 4)     
        self.assertEqual(s.pop(), 3)     
        self.assertEqual(len(s.stacks), 0)
        self.assertRaises(IndexError, lambda: s.pop())

    def test_stack_queue(self):
        q = StackQueue()
        self.assertEqual(q.enqueue(3), None)
        self.assertEqual(q.enqueue(4), None)
        self.assertEqual(q.enqueue(2), None)
        self.assertEqual(q.dequeue(), 3)     
        self.assertEqual(q.dequeue(), 4)     
        self.assertEqual(q.enqueue(1), None)
        self.assertEqual(q.dequeue(), 2)     
        self.assertEqual(q.dequeue(), 1)     

    def test_stack_sort(self):
        stack  = [1, 4, 2, 3, 5, 6, 7]
        expect = [7, 6, 5, 4, 3, 2, 1]
        actual = stack_sort(stack)
        self.assertEqual(actual, expect)


#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
