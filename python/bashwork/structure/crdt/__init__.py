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

class CRDTLaws(object):
    ''' In order for a CRDT to be valid, it must adhere to the
    following laws.
    '''

    @staticmethod
    def associative(a, b, c):
        ''' (a + (b + c)) == ((a + b) + c)

        :param a: The first CRDT instance to test
        :param b: The second CRDT instance to test
        :param c: The third CRDT instance to test
        '''
        message = "The supplied CRDT {} is not associative".format(type(a))
        assert (a.merge(b.merge(c)).value() == a.merge(b).merge(c).value()), message

    @staticmethod
    def commutative(a, b):
        ''' a + b == b + a

        :param a: The first CRDT instance to test
        :param b: The second CRDT instance to test
        '''
        message = "The supplied CRDT {} is not cummutative".format(type(a))
        assert (a.merge(b).value() == b.merge(a).value()), message

    @staticmethod
    def idempotent(a):
        ''' a + a == a

        :param a: The first CRDT instance to test
        '''
        message = "The supplied CRDT {} is not idempotent".format(type(a))
        assert (a.value() == a.merge(a).value()), message

    @classmethod
    def test(klass, a, b, c):
        klass.associative(a, b, c)
        klass.commutative(a, b)
        klass.idempotent(a)

