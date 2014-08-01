from collections import namedtuple

#--------------------------------------------------------------------------------
# logging
#--------------------------------------------------------------------------------

import logging
_logger = logging.getLogger(__name__)

#--------------------------------------------------------------------------------
# classes
#--------------------------------------------------------------------------------

''' Represents a single instance of a lock along with the
relevant information needed track its state.
'''
DynamoDBLock = namedtuple('DynamoDBLock',
    ['name', 'version', 'owner', 'duration', 'timestamp', 'is_locked', 'payload'])
