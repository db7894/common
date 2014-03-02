from Queue import Queue
from collections import defaultdict
from datetime import datetime
import select
import socket
import logging
import types

logger = logging.getLogger(__file__)

#------------------------------------------------------------
# Interfaces
#------------------------------------------------------------

class Task(object):
    ''' A simple task wrapper around a coroutine.
    Example::

        def print_task():
            print "starting..."
            yield
            print "stopping"

        task = Task(print_task())
        task.run()
        task.run()
    '''
    __task_id = 0

    def __init__(self, target):
        self.tid, Task.__task_id = Task.__task_id, Task.__task_id + 1
        self.target  = target
        self.message = None
        self.stack   = []

    def run(self):
        while True:
            try:
                result = self.target.send(self.message)
                if isinstance(result, SystemCall):
                    return result
                if isinstance(result, types.GeneratorType):
                    self.stack.append(self.target)
                    self.message = None
                    self.target  = result
                else:
                    if not self.stack: return
                    self.message = result
                    self.target  = self.stack.pop()
            except StopIteration:
                if not self.stack: raise
                self.message = None
                self.target  = self.stack.pop()

class SystemCall(object):
    ''' Represents a call to the underlying
    system for some resource.
    '''
    def register(self, task, scheduler):
        self.task = task
        self.scheduler = scheduler
        return self

    def execute(self):
        pass

#------------------------------------------------------------
# A collection of system calls
#------------------------------------------------------------

class GetTaskId(SystemCall):

    def execute(self):
        self.task.message = self.task.tid
        self.scheduler.schedule(self.task)

class CreateTask(SystemCall):
    ''' Example::

        def time_printer():
            while True:
                print datetime.now()
                yield

        def task_creater():
            yield CreateTask(time_printer())
    '''

    def __init__(self, target):
        self.target = target

    def execute(self):
        tid = self.scheduler.create_task(self.target)
        self.task.message = tid
        self.scheduler.schedule(self.task)

class KillTask(SystemCall):
    ''' Example::

        def task_creater():
            tid = yield CreateTask(some_task())
            was_killed = yield KillTask(tid)
    '''

    def __init__(self, tid):
        self.tid = tid

    def execute(self):
        task = self.scheduler.tasks.get(self.tid)
        if task: task.target.close()
        self.task.message = bool(task)
        self.scheduler.schedule(self.task)

class WaitForTask(SystemCall):
    ''' Example::

        def task_creater():
            tid = yield CreateTask(some_task())
            print "child created %d" % tid
            yield WaitForTask(tid)
            print "child killed %d" % tid
    '''

    def __init__(self, tid):
        self.tid = tid

    def execute(self):
        task_exists = self.scheduler.wait_for_task(self.task, self.tid)
        self.task.message = task_exists
        if not task_exists: # task already dead
            self.scheduler.schedule(self.task)

class ReadTask(SystemCall):

    def __init__(self, handle):
        self.handle = handle

    def execute(self):
        self.scheduler.wait_for_read(self.task, self.handle.fileno())

class WriteTask(SystemCall):

    def __init__(self, handle):
        self.handle = handle

    def execute(self):
        self.scheduler.wait_for_write(self.task, self.handle.fileno())

#------------------------------------------------------------
# Coroutine Utilities
#------------------------------------------------------------
class CoSocket(object):

    def __init__(self, sock):
        self.sock   = sock
        self.fileno = sock.fileno

    def accept(self):
        yield ReadTask(self.sock)
        client, address = self.sock.accept()
        yield CoSocket(client), address

    def send(self, buffer):
        while buffer:
            yield WriteTask(self.sock)
            len = self.sock.send(buffer)
            buffer = buffer[len:]

    def recv(self, maxbytes):
        yield ReadTask(self.sock)
        yield self.sock.recv(maxbytes)

    def close(self):
        yield self.sock.close()

#------------------------------------------------------------
# The main scheduler
#------------------------------------------------------------

class Scheduler(object):
    ''' A simple scheduler for managing the execution of 
    coroutine tasks. Example usage::

        def ping():
            tid = yield GetTaskId()
            for count in range(5):
                print "[%d] ping %d" % (tid, count)
                yield

        def pong():
            tid = yield GetTaskId()
            for count in range(5):
                print "[%d] pong %d" % (tid, count)
                yield

        scheduler = Scheduler()
        scheduler.create_task(ping())
        scheduler.create_task(pong())
        scheduler.main_loop()
    '''

    def __init__(self):
        self.ready         = Queue()
        self.waiting       = defaultdict(list)
        self.tasks         = {}
        self.read_waiting  = {}
        self.write_waiting = {}

    def wait_for_read(self, task, handle):
        ''' Add a new file handle to be read
        from for the supplied task.

        :param task: The task to perform a read
        :param handle: The handle to read from
        '''
        self.read_waiting[handle] = task

    def wait_for_write(self, task, handle):
        ''' Add a new file handle to be written
        to for the supplied task.

        :param task: The task to perform a write
        :param handle: The handle to write to
        '''
        self.write_waiting[handle] = task

    def poll_io_events(self, timeout):
        ''' Perform an io poll on the current file
        handles to determine if any are available to
        be operated on.

        :param timeout: The amount of time to poll for
        '''
        if self.read_waiting or self.write_waiting:
            rs, ws, es = select.select(
                self.read_waiting, self.write_waiting, [], timeout)
            for r in rs: self.schedule(self.read_waiting.pop(r)) 
            for w in ws: self.schedule(self.write_waiting.pop(w)) 

    def kill_task(self, task):
        ''' Called when a task has finished
        running to remove it from being
        re-scheduled.

        :param task: The task to de-schedule
        '''
        logger.debug("task %d shutting down", task.tid)
        del self.tasks[task.tid]
        for task in self.waiting.pop(task.tid, []):
            self.schedule(task)

    def wait_for_task(self, task, waitid):
        ''' Schedule the supplied task such that
        it will only run after the supplied waitid
        has terminate.

        :param task: The task to schedule
        :param waitid: The task to wait for exit
        '''
        if waitid in self.tasks:
            self.waiting[waitid].append(task)
        return waitid in self.tasks

    def create_task(self, target):
        ''' Given a new coroutine, convert it
        into a new task and scheduled it.

        :param target: The coroutine to creat a task for
        :returns: The identifier for the new task
        '''
        task = Task(target)
        self.tasks[task.tid] = task
        self.schedule(task)
        return task.tid

    def schedule(self, task):
        ''' Given a new task, add it to the 
        ready run queue.

        :param task: The task to schedule for execution
        '''
        self.ready.put(task)

    def main_loop(self):
        ''' Called to start the main event loop
        after all the initial running tasks have
        been created and scheduled.
        '''
        self.create_task(self.io_event_handler())

        while self.tasks:
            task = self.ready.get()
            try:
                result = task.run()
                if isinstance(result, SystemCall):
                    result.register(task, scheduler).execute()
                    continue
            except StopIteration:
                self.kill_task(task)
                continue
            self.schedule(task)

    def io_event_handler(self):
        ''' The event handler task for all IO events '''
        while True:
            timeout = None if self.ready.empty() else 0
            self.poll_io_events(timeout)
            yield

if __name__ == "__main__":
    logger.setLevel(logging.DEBUG)
    logging.basicConfig()

    def handle_client(client, address):
        logger.debug("client connected %s", address)
        while True:
            data = yield client.recv(65536)
            if not data: break
            yield client.send(data)
        client.close()
        logger.debug("client disconnected %s", address)

    def echo_server(address):
        logger.debug("starting server on %s", address)
        sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        sock.bind(address)
        sock.listen(5)
        sock = CoSocket(sock)

        while True:
            client, address = yield sock.accept()
            yield CreateTask(handle_client(client, address))
        logger.debug("server shutting down")

    def server_heartbeat():
        while True:
            logger.info("server running: %s", datetime.now())
            yield

    scheduler = Scheduler()
    scheduler.create_task(echo_server(("", 45000)))
    #scheduler.create_task(server_heartbeat())
    scheduler.main_loop()
