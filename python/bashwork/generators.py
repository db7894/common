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
import Queue
import socket
import struct
import logging
from threading import Thread
from collections import namedtuple

#------------------------------------------------------------
# simple generators
#------------------------------------------------------------
def gen_function_wrapper(method):
    ''' Given a function, wrap it so that it 
    can be used as a source generator::

        gen_sqrt = gen_function_wrapper(math.sqrt)
        for entry in gen_sqrt(xrange(100-)):
            print entry

    :param method: The method to wrap as a generator
    :returns: A the method wrapped as a generator
    '''
    def wrapper(stream):
        for entry in stream:
            yield method(entry)
    return wrapper

def gen_tap(stream, method):
    ''' Given a generator, place a tap in
    between it and the next stage. 

    :param stream: The stream to place a tap between
    :param method: The method to place as a tap
    :returns: The continued generator
    '''
    for entry in stream:
        method(entry)
        yield entry

def gen_tap_printer(stream):
    ''' Given a generator, place a printing
    tap in between it and the next stage.

    :param stream: The stream to place a tap between
    :returns: The continued generator
    '''
    def printer(entry): print entry
    return gen_tap(stream, printer)

def gen_tap_logger(stream, logger=logging):
    ''' Given a generator, place a logger tap
    in between it and the next stage.

    :param stream: The stream to place a tap between
    :param logger: The logger to place in between (default root)
    :returns: The continued generator
    '''
    return gen_tap(stream, logger.debug)

class gen_with_history(object):
    ''' A generator that maintains the history of
    the last item::

        stream = generate_with_last(stream)
        for entry in parse_log_stream(stream)
            print entry
            print stream.history
    '''

    def __init__(self, stream, count=1):
        ''' Initialize a new instance of the generator

        :param stream: The stream to continue with history
        :param count: The number of previous entries to maintain
        '''
        self.stream  = stream
        self.count   = count
        self.history = []

    def next(self):
        entry = self.stream.next()
        if len(self.history) == self.count:
            self.history.pop()
        self.history.insert(0, entry)
        return entry

    def __iter__(self):
        return self

def gen_zip(*streams):
    ''' given a collection of streams
    stream by applying a regular expression
    and only passing the matches.

    :param stream: a stream of data
    :param regex: the regular expression to match with
    :returns: the entries that match the expression
    '''
    streams = map(iter, streams)
    while streams:
        yield tuple(map(next, streams))

def gen_drop_while(stream, predicate):
    ''' Given a stream of data, skip over the supplied
    data until the predicate is false.

    :param stream: A stream of entries to filter
    :param predicate: A predicate to test the entries with
    :returns: All the elements after the drop predicate
    '''
    for entry in stream:
        if not predicate(stream):
            break

    for entry in stream:
        yield entry

def gen_take_while(stream, predicate):
    ''' Given a stream of data, take all the supplied
    elements until the predicate is false.

    :param stream: A stream of entries to filter
    :param predicate: A predicate to test the entries with
    :returns: All the elements until the predicate is false
    '''
    for entry in stream:
        if not predicate(stream):
            break
        yield entry

def gen_count(start, step=1):
    ''' Generate an infinite count of numbers
    with the supplied step size starting at the
    supplied starting point.

    :param start: The point to start counting at
    :param step: The step size of the count
    :returns: A generator of increasing count
    '''
    while True:
        try:
            yield start
            start += step
        except GeneratorExit: pass

def gen_cycle(stream):
    ''' Given a stream, return all the elements of
    the stream and then when it is exhausted, repeat
    the stream indefinately.

    :param stream: The stream to repeat
    :returns: An infinite stream of the suppied elements
    '''
    saved = []
    for entry in stream:
        yield entry
        saved.append(entry)
    while True:
        try:
            for entry in saved:
                yield entry
        except GeneratorExit: pass

#------------------------------------------------------------
# file generators
#------------------------------------------------------------

def gen_file_paths(root, pattern='*'):
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

def gen_file_handles(paths):
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

def gen_file_stream(path):
    ''' Given a file path, generate a single file
    stream handle around the contents of the file.
    This can also take a file handle making this
    a helpful utility to open a file or continue
    streaming from it. Example::

        for entry in gen_file_stream('access.log'):
            print entry

    :param path: The path to create a file stream for
    :returns: A stream around the contents of the file
    '''
    if isinstance(path, file):
        for line in path:
            yield line
    else:
        with open(path) as stream:
            for line in stream:
                yield line

def gen_concat(streams):
    ''' Given multiple streams, concatinate them
    together serially and return a generator around
    their combined streams.

    :param streams: The streams to concatinate
    :returns: A single stream of the combined data
    '''
    for stream in streams:
        for entry in stream:
            yield entry

def gen_filter(stream, predicate):
    ''' Given a stream, filter the stream by
    applying a predicate and and only passing
    the entries that the predicate is true for.

    :param stream: A stream of data
    :param predicate: The predicate to test with
    :returns: The entries that pass the predicate
    '''
    for entry in stream:
        if predicate(entry):
            yield entry

def gen_filter_regex(stream, pattern):
    ''' given a stream strings, filter the
    stream by applying a regular expression
    and only passing the matches.

    :param stream: a stream of data
    :param regex: the regular expression to match with
    :returns: the entries that match the expression
    '''
    pattern = re.compile(pattern)
    return gen_filter(stream, lambda e: pattern.search(e))

def gen_file_follower(handle):
    ''' Given a file handle, follow it and generate
    new lines as the file changes.

    :param handle: A handle to the supplied file
    :returns: A generator to the changing file data
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

def gen_structs(handle, struct_format):
    ''' Given a binary file handle, read a collection
    of packed structs of the supplied format from the file.
    Example::

        handle = open("stockdata.bin", "rb")
        for name, shares, price in gen_structs(handle, "<8sif"):
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

def gen_lines_from_path(path, pattern="*"):
    ''' Given a directory path, create a generator
    for all the lines recursively in all the paths
    matching the supplied file pattern.

    :param path: The root path to start reading from
    :param pattern: The pattern to filter files with (default *)
    :returns: A generator of all the lines in the directory path
    '''
    paths = gen_file_paths(path, pattern)
    hands = gen_file_handles(paths)
    lines = gen_concat(hands)
    return lines

#------------------------------------------------------------
# format generators
#------------------------------------------------------------

def gen_map(stream, mapper):
    ''' Given a stream of entries, map each entry through
    the supplied transform step and return a generater of
    the results.

    :param stream: A generator to transform the entries of
    :param mapper: The transformation function to apply
    :returns: A generator of the transformed functions
    '''
    for entry in stream:
        yield mapper(entry)

def gen_serialized(stream, serializer=pickle.dumps):
    ''' Given a stream of entries, generate a new stream
    with each entry serialized with the supplied serialization
    scheme. Example::

        stream  = gen_file_stream('/tmp/log')
        entries = parse_stream(stream)
        for entry in gen_serialized(entries, json.dumps):
            socket.sendall(entry)

    :param stream: The stream of entries to serialize
    :param serializer: The utility to serialize with
    :returns: A stream of serialized entities
    '''
    return gen_map(stream, serializer)

def gen_deserialized(stream, deserializer=pickle.loads):
    ''' Given a stream of entries that are serialized, generate
    a new stream with each entry deserialized with the supplied
    deserialization scheme.

    :param stream: The stream of entries to deserialize
    :param deserializer: The utility to deserialize with
    :returns: A stream of deserialized entities
    '''
    return gen_map(stream, deserializer)

def gen_deserialized_file(handle, deserializer=pickle.load):
    ''' Given a file handle of entries that are serialized, generate
    a new stream with each entry deserialized with the supplied
    deserialization scheme.

    :param stream: The stream of entries to deserialize
    :param deserializer: The utility to deserialize with
    :returns: A stream of deserialized entities
    '''
    while True:
        try:
            entry = deserializer(handle)
            yield entry
        except GeneratorExit: pass

def gen_split_strings(stream, seperator=','):
    ''' Given a stream of strings, split them into
    tuples using the supplied seperator.

    :param stream: The stream of lines to convert to tuples
    :param seperator: The seperator to split at (default ",")
    :returns: A stream of split tuples
    '''
    for line in stream:
        yield line.split(seperator)

def gen_regex_tuples(stream, pattern):
    ''' Given a stream of strings, split them into
    tuples using the supplied seperator.

    :param stream: The stream of lines to convert to tuples
    :param seperator: The seperator to split at (default ",")
    :returns: A stream of split tuples
    '''
    pattern = re.compile(pattern)
    for line in stream:
        match = pattern.search(line)
        if match:
            yield match.groups()

def gen_dictionaries(stream, columns, formats=None):
    ''' Given a stream of tuples, convert them into dict
    values with the supplied columns and optional format
    conversions. Example::

        columns = ['address', 'request', 'response', 'size']
        formats = [str, str, int, int]
        lines   = gen_stream('stream.log')
        tuples  = gen_tuples(lines)
        entries = gen_dictionaries(tuples, columns, formats)
        print sum(entry['size'] for entry in entries)

    :param stream: The stream of tuples to convert to entities
    :param columns: The column names of the tuples
    :param formats: Format conversion for each column (default str)
    :returns: A stream of converted dict entries
    '''
    formats = formats or [str for i in range(len(columns))]
    mapper  = lambda vs: { c:f(v) for f,v,c in zip(formats, vs, columns) }
    return gen_map(stream, mapper)

def gen_named_tuples(stream, columns, formats=None):
    ''' Given a stream of tuples, convert them into named
    tuples with the supplied columns and optional format
    conversions. Example::

        columns = ['address', 'request', 'response', 'size']
        formats = [str, str, int, int]
        lines   = gen_stream('stream.log')
        tuples  = gen_tuples(lines)
        entries = gen_named_tuples(tuples, columns, formats)
        print sum(entry.size for entry in entries)

    :param stream: The stream of tuples to convert to entities
    :param columns: The column names of the tuples
    :param formats: Format conversion for each column (default str)
    :returns: A stream of converted entities
    '''
    formats = formats or [str for i in range(len(columns))]
    entry   = namedtuple('Entry', columns)
    maapper = lambda vs: entry._make(f(v) for f,v in zip(formats, vs))
    return gen_map(stream, mapper)

#------------------------------------------------------------
# network generators
#------------------------------------------------------------

def gen_tcp_connections(address, connections=5):
    ''' Given an address to bind to, create a TCP server that 
    yields the current cilent handles up to the specified
    number of connections.

    :param address: (host, port) tuple to bind to
    :param connections: The number of queued connections (default 5)
    :returns: A generator of (connection, address) entries
    '''
    handle = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    handle.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    handle.bind(address)
    handle.listen(connections)

    while True:
        try:
            client = handle.accept()
            yield client
        except GeneratorExit: handle.close()

def gen_udp_connections(address, size=1024):
    ''' Given an address to bind to, create a UDP server that 
    yields the incoming UDP messages received.

    :param address: (host, port) tuple to bind to
    :param size: The maximum message size to listen for
    :returns: A generator of (message, address) entries
    '''
    handle = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    handle.bind(address)

    while True:
        try:
            message = handle.recvfrom(size)
            yield message
        except GeneratorExit: handle.close()

def gen_push_via_tcp(stream, address):
    ''' Given a stream of entries, send them as they
    arrive to the supplied address over a TCP socket.

    :param stream: The stream to send over the wire
    :param address: The (host, port) to send the messages to
    '''
    handle = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    handle.connect(address)
    for entry in stream:
        handle.sendall(entry)
    handle.close()

def gen_push_via_udp(stream, address):
    ''' Given a stream of entries, send them as they
    arrive to the supplied address over a UDP socket.

    :param stream: The stream to send over the wire
    :param address: The (host, port) to send the messages to
    '''
    handle = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    for entry in stream:
        handle.sendto(entry, address)
    handle.close()

def gen_pull_via_tcp_file(address):
    ''' Given an address to bind to, listen for client
    connections over TCP and then read and stream entries
    that are send over that connection. Example::

        stream = gen_pull_via_tcp(("localhost", 8080))
        for entry in gen_deserialize(stream):
            print entry

    :param address: The (host, port) to listen on
    :returns: A generator of socket files objects to read from
    '''
    for client, address in gen_tcp_connections(address):
        yield client.makefile()
        client.close()

def gen_pull_via_tcp(address):
    ''' Given an address to bind to, listen for client
    connections over TCP and then read and stream entries
    that are send over that connection. Example::

        for entry in gen_pull_via_tcp(("localhost", 8080)):
            print entry

    :param address: The (host, port) to listen on
    :returns: A generator of socket files objects to read from
    '''
    for client, address in gen_tcp_connections(address):
        for entry in gen_deserialized_file(client.makefile()):
            yield entry
        client.close()

def gen_pull_via_udp(address):
    ''' Given an address to bind to, listen for client
    connections over UDP and then read and stream entries
    that are send over that connection. Example::

        stream = gen_pull_via_udp(("localhost", 8080))
        for entry in gen_deserialize(stream):
            print entry

    :param address: The (host, port) to listen on
    :returns: A generator of (file, address) to read from
    '''
    for message, address in gen_udp_connections(address):
        yield message

#------------------------------------------------------------
# threaded generators
#------------------------------------------------------------

def gen_push_via_queue(stream, queue):
    ''' Given a stream and a thread safe queue,
    push all the stream entries into the queue to be
    consumed by another thread or process.

    :param stream: The stream to push to a queue
    :param queue: The queue to push entries to
    '''
    for entry in stream:
        queue.put(entry)
    queue.put(StopIteration)

def gen_pull_via_queue(queue):
    ''' Given a queue, stream the entries from
    the queue in a blocking fashion. The pulling
    can be stopped by dropping in the poison pill
    of `StopIteration` as a queue message.

    :param queue: The queue to stream entries from
    :returns: A generator around the queue contents
    '''
    while True:
        try:
            entry = queue.get()
            if entry is StopIteration:
                break
            yield entry
        except GeneratorExit: pass

def gen_multiplex_streams(streams):
    ''' Given a collection of streams, concatinate
    them in a multiplexed fashion such that each
    stream will contribute entries in a latest to
    push fashion.

    :param streams: The collection of streams to multiplex
    :returns: A generator around the multiplexed streams
    '''
    queue = Queue.Queue()
    consumers = []
    for stream in streams:
        thread = Thread(target=gen_push_via_queue, args=(stream, queue))
        thread.start()
        consumers.append(gen_pull_via_queue(queue))
    return generate_concat(consumers)

def consume_via_thread(stream, consumer):
    ''' Given a stream and a consuming function,
    run the consumer in a seperate thread and push
    into that consumer from the current thread.

    :param stream: The stream to push into the consumer
    :param consumer: The consuming function
    '''
    queue  = Queue.Queue()
    worker = lambda q: consumer(gen_pull_via_queue(q))
    thread = Thread(target=worker, args=(queue,))
    thread.start()
    gen_push_via_queue(stream, queue)

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
    queue  = Queue.Queue()
    thread = Thread(target=lambda: target(gen_pull_via_queue(queue)))
    thread.start()

    try:
        while True:
            entry = (yield)
            queue.put(entry)
    except GeneratorExit: thread.shutdown()

class ThreadConsumer(Thread):
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
    #columns = ['address', 'request', 'response', 'size']
    #formats = [str, str, int, int]
    #lines   = gen_file_stream('stream.log')
    #tuples  = gen_tuples(lines)
    #entries = gen_entries(tuples, columns, formats)
    #print sum(entry.size for entry in entries)

    #@consumer
    #def receive_count():
    #    try:
    #        while True:
    #            count = (yield)
    #            print "count is %d" % count
    #    except GeneratorExit: print "finished counting"

    #receiver = receive_count()
    #for count in range(5, 0, -1):
    #    receiver.send(count)
    def listener():
        stream = gen_pull_via_udp_file(("localhost", 8081))
        for entry in gen_deserialized(stream):
            print entry
    lthread = Thread(target=listener)
    lthread.start()

    def producer():
        stream  = gen_file_stream('/var/log/system.log')
        groups  = gen_regex_tuples(stream, "(...\s*\d+.\d+:\d+:\d+)\s*([^:]+):\s*(.*)")
        entries = gen_dictionaries(groups, ['date', 'name', 'message'])
        serial  = gen_serialized(entries)
        gen_push_via_udp(serial, ("localhost", 8081))
    producer()
    lthread.join()

