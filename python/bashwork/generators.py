'''
To shutdown a generator, simply call `close` on it.
This will cause the generator to receive a `GeneratorExit`
exception which can be caught to perform any neccessary
cleanup. Ignoring the `GeneratorExit` will cause a runtime
error.

Generators can also not be shutdown from seperate threads
or from signal handlers. The only way to make this work is
to use a threading primitive that is checked inside the
generator (like an event flag).

TODO xml.etree.ElementTree.iterparse

Generators (yield) can also effectively mimic the
IO monad which decouples where the output goes (this
is roughly how WSGI works)::

    def do_some_operation():
        yield "BEGIN"
        for i in range(10):
            yield "COMMAND %d" %i
        yield "FINISHED"

    # can write to a file, send over network, or just print
    print ''.join(do_some_operation())
'''
import os
import re
import fnmatch
import gzip
import bz2
import time
import pickle
import threading
import Queue
import socket
import struct
from collections import namedtuple

#------------------------------------------------------------
# simple generators
#------------------------------------------------------------
def generate_wrapper(method):
    ''' Given a function, wrap it so that it 
    can be used as a source generator::

        gen_sqrt = generate_wrapper(math.sqrt)
        for entry in gen_sqrt(xrange(100-)):
            print entry

    :param method: The method to wrap as a generator
    :returns: A the method wrapped as a generator
    '''
    def wrapper(stream):
        for entry in stream:
            yield method(entry)
    return wrapper

def generate_tap(stream, method=None):
    ''' Given a generator, place a tap in
    between it and the next stage (by default
    print).

    :param stream: The stream to place a tap between
    :param method: The method to place as a tap
    :returns: A the continued generator
    '''
    def printer(e): print e
    method = method or printer
    for entry in stream:
        method(entry)
        yield entry

class generate_with_last(object):
    ''' A generator that maintains the history of
    the last item::

        stream = generate_with_last(stream)
        for entry in parse_log_stream(stream)
            print entry
            print stream.previous
    '''

    def __init__(self, stream, count=1):
        self.stream   = stream
        self.count    = count
        self.previous = []

    def next(self):
        entry = self.stream.next()
        if len(self.previous) == self.count:
            self.previous.pop()
        self.previous.insert(0, entry)
        return entry

    def __iter__(self):
        return self

#------------------------------------------------------------
# file generators
#------------------------------------------------------------

def generate_paths(root, pattern='*'):
    ''' Given a root directory, recursively walk the
    directory tree and return all files matching the
    supplied pattern (by default '*').

    :param root: The root directory to walk
    :param pattern: The pattern to filter by
    :returns: A generator around the matched paths
    '''
    for path, dirs, files in os.walk(root):
        for name in fnmatch.filter(files, pattern):
            yield os.path.join(path, name)

def generate_file_handles(paths):
    ''' Given a collection of path entries, return
    a generator of opened file handles of the various
    file types.

    :param paths: The paths to create open file handles for
    :returns: A generator of open file handles
    '''
    for path in paths:
        if path.endswith('.gz'): yield gzip.open(path)
        elif path.endswith('.bz2'): yield bz2.open(path)
        else: yield open(path)

def generate_stream(path):
    ''' Given a file path, generate a single file
    stream handle around the contents of the file.

    :param path: The path to create a file stream for
    :returns: A stream around the contents of the file
    '''
    with open(path) as stream:
        for line in stream:
            yield line

def generate_concat(streams):
    ''' Given multiple streams, concatinate them
    together serially and return a generator around
    their combined streams.

    :param streams: The streams to concatinate
    :returns: A single stream of the combined data
    '''
    for stream in streams:
        for entry in stream:
            yield entry

def generate_filter(stream, pattern):
    '''
    '''
    pattern = re.compile(pattern)
    for line in stream:
        if pattern.search(line):
            yield line

def generate_file_follower(handle):
    '''
    '''
    handle.seek(0, 2)
    while True:
        try:
            line = handle.readline()
            if not line:
                time.sleep(0.1)
                continue
            yield line
        except GeneratorExit: pass

def generate_structs(handle, struct_format):
    ''' Given a binary file handle, read a collection
    of packed structs of the supplied format from the file::

        handle = open("stockdata.bin", "rb")
        for name, shares, price in genenerate_structs(handle, "<8sif"):
            print name, share, price

    :param handle: A handle to a file object
    :param format: The struct format string to parse with
    :returns: A generator around the found struct records
    '''
    struct_size = struct.calculate(struct_format)
    while True:
        record = handle.read(struct_size)
        if not record: break
        yield struct.unpack(struct_format, record)

#------------------------------------------------------------
# format generators
#------------------------------------------------------------

def generate_serialized(stream, serialize=pickle.dumps):
    '''
    '''
    for entry in stream:
        yield serialize(entry)

def generate_deserialized(handle, deserialize=pickle.load):
    '''
    '''
    while True:
        try:
            entry = deserialize(handle)
            yield entry
        except EOFError: return

def generate_tuples(stream, seperator=','):
    '''
    '''
    for line in stream:
        yield line.split(seperator)

def generate_entries(stream, columns, formats=None):
    '''
    '''
    formats = formats or [str for i in range(len(columns))]
    Entry   = namedtuple('Entry', columns)
    for values in stream:
        values = (f(v) for f,v in zip(formats, values))
        yield Entry._make(values)

#------------------------------------------------------------
# network generators
#------------------------------------------------------------

def receive_tcp_connections(address, connections=5):
    '''
    '''
    handle = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    handle.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    handle.bind(address)
    handle.listen(connections)
    while True:
        client = handle.accept()
        yield client

def receive_udp_connections(address, size):
    '''
    '''
    handle = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    handle.bind(address)
    while True:
        message = handle.recvfrom(size)
        yield message

def push_tcp_generator(stream, address):
    '''
    '''
    handle = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    handle.connect(address)
    for entry in generate_serialized(stream):
        handle.sendall(entry)
    handle.close

def pull_tcp_generator(address):
    '''
    '''
    for client, address in receive_tcp_connections(address):
        for entry in generate_deserialized(client.makefile()):
            yield entry
        client.close()

#------------------------------------------------------------
# threaded generators
#------------------------------------------------------------

def push_queue_generator(stream, queue):
    '''
    '''
    for entry in stream:
        queue.put(entry)
    queue.put(StopIteration)

def pull_queue_generator(queue):
    '''
    '''
    while True:
        entry = queue.get()
        if entry is StopIteration:
            break
        yield entry


#------------------------------------------------------------
# combined generators
#------------------------------------------------------------

def generate_lines_from_directory(pattern, directory):
    '''
    '''
    paths = generate_paths(directory, pattern)
    hands = generate_handles(paths)
    lines = generate_streams(hands)
    return lines

def threaded_consumer(stream, consumer):
    '''
    '''
    queue  = Queue.Queue()
    worker = lambda q: consumer(pull_queue_generator(q))
    thread = threading.Thread(target=worker, args=(queue,))
    thread.start()
    push_queue_generator(stream, queue)

def generate_multiplex(streams):
    '''
    '''
    queue = Queue.Queue()
    consumers = []
    for stream in streams:
        thread = threading.Thread(target=push_queue_generator, args=(stream, queue))
        thread.start()
        consumers.append(pull_queue_generator(queue))
    return generate_concat(consumers)

#------------------------------------------------------------
# consumers
#------------------------------------------------------------
def consumer(method):
    ''' A decorator to start up a consumer coroutine
    so that the initial `next` doesn't have to be
    called.

    :param method: The consumer to decorate
    :returns: The method decorated as an initialized consumer
    '''
    def wrapper(*args, **kwargs):
        handle = method(*args, **kwargs)
        handle.next()
        return handle
    return wrapper

def consumer_broadcast(stream, consumers):
    ''' Given a collection of consumers that implement
    the consumer interface, broadcast everything in the
    stream to each consumer.

    :param stream: The stream to broadcast to consumers
    :param consumers: A collection of consumers to receive with
    '''
    for entry in stream:
        for consumer in consumers:
            consumer.send(entry)

def consumer_network(address, serialize=pickle.dumps):
    '''
    Example usage::

        handler  = generate_file_follower(open('access-log'))
        entries  = generate_log_entries(handler)
        consumer = consumer_network(("log.host", 1234))
        consumer_broadcast(entries, [consumer])
    '''
    socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    socket.connect(address)
    try:
        while True:
            entry = (yield)
            socket.sendall(serialize(entry))
    except GeneratorExit: socket.close()

def consumer_thread(target):
    '''
    Example usage::

        def print_errors(entries):
            for entry in entries:
                print entry

        handler  = generate_file_follower(open('access-log'))
        entries  = generate_log_entries(handler)
        consumer = consumer_thread(print_errors)
        consumer_broadcast(entries, [consumer])
    '''
    queue = Queue.Queue()
    thread = threading.Thread(target=lambda: target(pull_queue_generator(queue)))
    thread.start()

    try:
        while True:
            entry = (yield)
            queue.put(entry)
    except GeneratorExit: thread.shutdown()

class ThreadConsumer(threading.Thread):
    '''
    Example usage::

        def print_errors(entries):
            for entry in entries:
                print entry

        handler  = generate_file_follower(open('access-log'))
        entries  = generate_log_entries(handler)
        consumer = ThreadConsumer(print_errors)
        consumer.start()
        consumer_broadcast(entries, [consumer])
    '''

    def __init__(self, target):
        '''
        '''
        super(ThreadConsumer, self).__init__(self)
        self.setDaemon(True)
        self.queue  = Queue.Queue()
        self.target = target

    def send(self, entry):
        self.queue.put(entry)

    def run(self):
        self.target(pull_queue_generator(self.queue))

    def close(self):
        self.shutdown()


#------------------------------------------------------------
# example usage
#------------------------------------------------------------

if __name__ == "__main__":
    columns = ['address', 'request', 'response', 'size']
    formats = [str, str, int, int]
    lines   = generate_stream('stream.log')
    tuples  = generate_tuples(lines)
    entries = generate_entries(tuples, columns, formats)
    print sum(entry.size for entry in entries)

    @consumer
    def receive_count():
        try:
            while True:
                count = (yield)
                print "count is %d" % count
        except GeneratorExit: print "finished counting"

    receiver = receive_count()
    for count in range(5, 0, -1):
        receiver.send(count)
