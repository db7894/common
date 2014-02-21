from functools import wraps
from threading import Timer

class timed_updater(object):
    ''' Given a callback function, call it periodically
    at the specified interval until the function returns
    false.
    '''

    def __init__(self, interval):
        ''' Initialize a new instance of the timed_updater
        decorator.

        :param interval: The interval to call the function at
        '''
        self.interval = interval

    def __call__(self, callback):
        ''' The retryable wrapper for the method call.

        :param callback: The decorated method
        '''
        @wraps(callback)
        def wrapper(): # *args
            try:
                if not callback(): # *args
                    return # do not reschedule if we return False
            except Exception: pass

            timer = Timer(self.interval, wrapper)
            timer.daemon = True
            timer.start()
        return wrapper

if __name__ == "__main__":
    from time import sleep
    from datetime import datetime

    @timed_updater(1)
    def update_printer():
        print "this is the new update: %s" % datetime.now()
        return True

    update_printer()
    sleep(5)
