#--------------------------------------------------------------------------------
# logging
#--------------------------------------------------------------------------------

import logging
_logger = logging.getLogger(__name__)

#--------------------------------------------------------------------------------
# classes
#--------------------------------------------------------------------------------

class DynamoDBLockContext(object):

    def __init__(self, **kwargs):
        '''
        '''
        self.client = kwargs.get('client')
        self.name   = kwargs.get('name')

    def __enter__(self):
        self.lock = self.client.acquire_lock(name)
        return self

    def __exit__(self, ex_type, value, traceback):
        self.client.release_lock(self.lock)
