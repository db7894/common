#!/usr/bin/env python
import unittest
from bashwork.generators import *

class GeneratorTest(unittest.TestCase):

    def test_gen_function_wrapper(self):
        gen_square = gen_function_wrapper(lambda x: x * x)
        actual = list(gen_square(xrange(6)))
        expect = [0, 1, 4, 9, 16, 25]
        self.assertEqual(expect, actual)

    def test_gen_tap(self):
        expect = []
        stream = gen_tap(xrange(10), lambda x: expect.append(x))
        actual = list(stream)
        self.assertEqual(actual, expect)

    def test_gen_logger(self):
        class Logger(object):
            def __init__(self): self.expect = []
            def debug(self, x): self.expect.append(x)

        logger = Logger()
        stream = gen_tap_logger(xrange(10), logger)
        actual = list(stream)
        self.assertEqual(actual, logger.expect)

    def test_gen_with_history(self):
        stream = gen_with_history(iter(range(10)), 10)
        actual = list(reversed(list(stream)))
        self.assertEqual(actual, stream.history)

    def test_gen_concat(self):
        stream = gen_concat([xrange(0,4), xrange(4,8), xrange(8, 11)])
        actual = list(stream)
        expect = range(11)
        self.assertEqual(actual, expect)

    def test_gen_filter(self):
        stream = gen_filter(xrange(11), lambda a: a % 2 == 0)
        actual = list(stream)
        expect = [x for x in xrange(11) if x % 2 == 0]
        self.assertEqual(actual, expect)

    def test_gen_filter_regex(self):
        stream = ('a' * x for x in range(5))
        stream = gen_filter_regex(stream, "a{3}")
        actual = list(stream)
        expect = ['aaa', 'aaaa']
        self.assertEqual(expect, actual)

    def test_gen_serialized(self):
        expect = [ {'key': x} for x in range(5) ]
        stream = gen_serialized(expect)
        stream = gen_deserialized(stream)
        actual = list(stream)
        self.assertEqual(expect, actual)

    def test_gen_split_strings(self):
        stream = ['a,b,c,d', '1,2,3,4']
        stream = gen_split_strings(stream, seperator=',')
        actual = list(stream)
        expect = [['a', 'b', 'c', 'd'], ['1', '2', '3', '4']]
        self.assertEqual(expect, actual)

    def test_gen_regex_tuples(self):
        stream = ['4,5,6,7', 'a,b,c,d', '1,2,3,4']
        stream = gen_regex_tuples(stream, "(\d),(\d),(\d),(\d)")
        actual = list(stream)
        expect = [('4', '5', '6', '7'), ('1', '2', '3', '4')]
        self.assertEqual(expect, actual)

#def gen_file_paths(root, pattern='*'):
#def gen_file_handles(paths):
#def gen_file_stream(path):
#def gen_file_follower(handle):
#def gen_structs(handle, struct_format):
#def gen_lines_from_path(path, pattern="*"):
#def gen_deserialized_file(handle, deserializer=pickle.load):
#def gen_dictionaries(stream, columns, formats=None):
#def gen_named_tuples(stream, columns, formats=None):
#def gen_tcp_connections(address, connections=5):
#def gen_udp_connections(address, size=1024):
#def gen_push_via_tcp(stream, address):
#def gen_push_via_udp(stream, address):
#def gen_pull_via_tcp_file(address):
#def gen_pull_via_tcp(address):
#def gen_pull_via_udp(address):
#def push_queue_generator(stream, queue):
#def pull_queue_generator(queue):
#def threaded_consumer(stream, consumer):
#def gen_multiplex(streams):
#def consumer(method):
#def consumer_broadcast(stream, consumers):
#def consumer_network(address, serialize=pickle.dumps):
#def consumer_thread(target):
#class ThreadConsumer(threading.Thread):

#---------------------------------------------------------------------------#
# main
#---------------------------------------------------------------------------#
if __name__ == "__main__":
    unittest.main()
