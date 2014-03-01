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
def push_filter(target):

@coroutine
def push_printer():
    ''' Example::

        handle = open("/var/log/system.log", 'r')
        push_file_follower(handle, push_printer())
    '''
    try:
        while True:
            line = (yield)
            print line
    except GeneratorExit: pass
