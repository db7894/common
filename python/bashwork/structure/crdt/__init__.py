import json
import uuid
from time import time as current_time

class CRDT(object):
    ''' Base class for a CRDT. This allows us to mixin common
    behaviour to the CRDT.
    '''

    def generate_id(self):
        ''' Generate a new unique id.

        :returns: The new unique identifier.
        '''
        return str(uuid.uuid4())

    def to_json(self):
        '''
        '''
        return json.dumps(self.serialize())

    @classmethod
    def from_json(klass, payload):
        ''' Given a payload, deserialize this to a new
        CRDT instance.

        :param payload: The payload to deserialize
        :returns: A new instance of the current type
        '''
        return klass.deserialize(json.loads(payload))

    @staticmethod
    def merge(this, that):
        ''' Given two instances of a CRDT, merge them
        to produce a new CRDT instance.

        :param this: The first instance to merge
        :param that: The second instance to merge
        :returns: The new merged instance
        '''
        return this.merge(that)

    #def __cmp__(self, that): return self.compare(that)
    def __str__(self):     return str(self.value())
    def __repr__(self):    return "<{} {}>".format(self.__class__, self.value())
    def __hash__(self):    return hash(self.value())
    def __nonzero__(self): return bool(self.value())
    def __copy__(self):    return self.__class__.deserialize(self.serialize())
