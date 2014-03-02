'''
Exceptions can be thrown via coroutines::

    co = coroutine()
    co.send("next value")
    co.throw(RuntimeError, "failure")

Coroutines need a source to drive the pushing stream;
this is usually not a coroutine in its own right::

    def source(coroutine):
        while not done:
            item = produce_next_item()
            coroutine.send(item)
        coroutine.close()

The coroutine must also have a sink to pull the data
through the stream::

    @coroutine
    def sink():
        try:
            while True:
                item = (yield)
        except GeneratorExit: pass
'''
import xml.sax
import pickle
from queue import Queue
from threading import Thread

def coroutine(method):
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

def push_file_follower(handle, target):
    ''' Given a file handle
    '''
    handle.seek(0, 2)
    while True:
        line = handle.readline()
        if not line:
            time.sleep(0.1)
            continue
        target.send(line)

@coroutine
def push_filter(target, predicate):
    ''' Given a continuation target, continue
    sending all inputs that evaluate to True with
    the supplied predicate.

    :param target: The continuation to pass to
    :param predicate: The test to apply to input
    '''
    while True:
        entry = (yield)
        if predicate(entry)
            target.send(entry)

@coroutine
def push_filter_string(target, pattern):
    ''' Given a continuation target, continue
    sending all inputs that have the supplied
    string in their content.

    :param target: The continuation to pass to
    :param pattern: The pattern to check input for
    '''
    while True:
        line = (yield)
        if pattern in line:
            target.send(line)

@coroutine
def push_filter_regex(target, pattern):
    ''' Given a continuation target, continue
    sending all inputs that match the supplied
    regex pattern.

    :param target: The continuation to pass to
    :param pattern: The pattern to check input against
    '''
    pattern = re.compile(pattern)
    while True:
        line = (yield)
        if pattern.search(line):
            target.send(line)

@coroutine
def push_printer():
    ''' Example::

        handle = open("/var/log/system.log", 'r')
        push_file_follower(handle, push_printer())
    '''
    while True:
        line = (yield)
        print line


@coroutine
def push_broadcast(targets):
    ''' Given a collection of targets, push
    the new input value to each of them. Example::
        
        printer = push_printer()
        push_follow(open('/var/log/system.log'
          push_broadcast([
            push_filter_string("python", printer),
            push_filter_string("scala",printer)])))

        push_broadcast([push_printer, push_printer])

    :param targets: The targets to broadcast to
    :returns: A new coroutine
    '''
    while True:
        entry = (yield)
        for target in targets:
            target.send(entry)

@coroutine
def push_threaded(target):
    ''' Given a target, push messages to
    it on a separate thread.

    :param target: The target to send message to
    :returns: A new threaded coroutine
    '''
    messages = Queue()
    def target_worker():
        while True
            message = messages.get()
            if message is GeneratorExit:
                target.close()
                return
            else: target.send(message)
    Thread(target=target_worker).start()

    try:
        while True:
            message = (yield)
            messages.put(message)
        except GeneratorExit:
            messages.put(GeneratorExit)

@coroutine
def push_via_handle(handle, serializer=pickle.dump):
    ''' Given a file descriptor handle (say a pipe
    or a socket), push all messages over the handle
    to be read by another process.

    :param handle: The file handle to write to
    :param serializer: The serializer to convert messages with
    :returns: A new coroutine
    '''
    try:
        while True:
            message = (yield)
            pickle.dump(message, handle)
            handle.flush()
    except StopIteration:
        handle.close()

@coroutine
def pull_via_handle(handle, target, deserializer=pickle.load):
    ''' Given a file descriptor handle (say a pipe
    or a socket), pull all arriving messages and send them
    to the supplied target.

    :param handle: The file handle to read from
    :param target: The target to send new messages to
    :param deserializer: The deserializer to convert messages with
    :returns: A new coroutine
    '''
    try:
        while True:
            message = deserializer(handle)
            target.send(message)
    except EOFError:
        target.close()


class CoroutineXmlProcessor(xml.sax.ContentHandler):
    ''' A simple coroutine sax XML processor. It simply
    emits events based on what is sees in the document:

    * (START, (name, attributes))
    * (ENTRY, text)
    * (CLOSE, name)

    This allows one to convert parsing of XML documents
    to easy to construct state machines.
    '''
    START = 0x1
    ENTRY = 0x2
    CLOSE = 0x4

    def __init__(self, target):
        self.target = target

    def startElement(self, name, attrs):
        self.target.send((self.START, (name, attrs._attrs)))

    def characters(self, text):
        self.target.send((self.ENTRY, text))

    def endElement(self, name):
        self.target.send((self.CLOSE, name))
