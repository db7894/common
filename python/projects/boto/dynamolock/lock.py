#--------------------------------------------------------------------------------
# logging
#--------------------------------------------------------------------------------

import logging
_logger = logging.getLogger(__name__)

#--------------------------------------------------------------------------------
# classes
#--------------------------------------------------------------------------------

class DynamoDBLock(object):
    ''' Represents a single instance of a lock along with the
    relevant information needed track its state.
    '''
    #TODO named tuple for immutability, reduce copies
    #TODO how to inject policy

    def __init__(self, **kwargs):
        ''' Initializes a new instance of the DynamoDBLock class

        :param duration: The relative duration of the held lock
        '''
        self.name      = kwargs.get('name')
        self.version   = kwargs.get('version')
        self.owner     = kwargs.get('owner')
        self.duration  = kwargs.get('duration')
        self.timestamp = kwargs.get('timestamp', self.policy.get_new_timestamp())
        self.is_locked = kwargs.get('is_locked', False)
        self.payload   = kwargs.get('payload', None)

    @property
    def is_expired(self):
        ''' Check if the current lock is expired and needs to be
        re-aquired.

        :returns: True if expired, False otherwise
        '''
        return (self.timestamp + self.duration) < self.policy.get_new_timestamp()

    def __hash__(self):
        return hash(self.name + self.owner)

    def __str__(self):
        return json.dumps(self.__dict__)

    __repr__ = __str__
