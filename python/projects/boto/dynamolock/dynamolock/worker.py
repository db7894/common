import time
from threading import Thread, Event
from datetime import timedelta

from .policy import DynamoDBLockPolicy

#--------------------------------------------------------------------------------
# logging
#--------------------------------------------------------------------------------

import logging
_logger = logging.getLogger(__name__)

#--------------------------------------------------------------------------------
# classes
#--------------------------------------------------------------------------------

class DynamoDBLockWorker(Thread):
    ''' The worker that runs to periodically update lock leases as long
    as the system is alive. This prevents long running processes from
    losing their locks by possibly fast clients.
    '''

    def __init__(self, **kwargs):
        ''' Initializes a new instance of the DynamoDBLock class

        :param client: The client to perform management with
        :param period: The length of each cycle in seconds (default 1 minutes)
        :param locks: The dictionary of locks to manage (default the client locks)
        '''
        super(DynamoDBLockWorker, self).__init__()

        self.client = kwargs.get('client')
        self.policy = kwargs.get('policy', DynamoDBLockPolicy())
        self.locks  = kwargs.get('locks', self.client.locks)
        self.period = kwargs.get('period', timedelta(minutes=1).total_seconds())
        self.is_stopped = Event()

    def stop(self, timeout=None):
        ''' Stop the underlying worker thread and join on its
        completion for the specified timeout.

        :param timeout: The amount of time to wait for the shutdown
        '''
        self.is_stopped.set()
        self.join(timeout)

    def run(self): 
        ''' The worker thread used to update the lock leases
        for the currently handled locks.
        '''
        while not self.is_stopped.is_set():
            start = self.policy.get_new_timestamp()
            for lock in self.locks.values():
                if not self.client.touch_lock(lock):
                    del self.locks[lock.name]
            elapsed = self.policy.get_new_timestamp() - start
            time.sleep(max(self.period - elapsed, 0))
