import signal

class TimeoutException(Exception):
    ''' An exception raised when the timeout has
    been reached before the computation has finished.
    '''
    pass

class Timeout():
    '''
    Throw a timeout after the supplied number of seconds using the
    signal.SIGALRM signal. This can be used to interrupt tasks that
    may block in kernel for long periods of time::

        try:
            with Timeout(5):
                perform_long_io_task()
        catch TimeoutException, ex:
            handle_timeout_failure()

    Alternatively, the task can silently fail if an exception is not
    needed or wanted::

        with Timeout(5, throw=False):
            perform_long_io_task()
    '''

    def __init__(self, seconds, throw=True):
        ''' Initialize a new instance of the Timeout watcher

        :param seconds: The time in seconds to timeout at
        :param throw: True to throw, False to quick exit (default True)
        '''
        self.seconds = seconds
        self.throw   = throw

    def __enter__(self):
        ''' Called when the signal handler is installed
        '''
        self.old_handler = signal.signal(signal.SIGALRM, self.raise_timeout)
        signal.alarm(self.seconds)

    def __exit__(self, type, value, traceback):
        ''' Called when the signal is raised or the computation finishes
        '''
        signal.alarm(0)                                 # disable the alarm
        signal.signal(signal.SIGALRM, self.old_handler) # restore old handler
        return self.throw and isinstance(value, TimeoutException)

    def raise_timeout(self, *args):
        ''' The signal handler for the signal timeout
        '''
        raise TimeoutException()
