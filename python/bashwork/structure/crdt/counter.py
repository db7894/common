from bashwork.structure.crdt import CRDT

class GCounter(CRDT):
    ''' A grow only counter that computes the overall sum
    of all the vectors by taking the max value of all the
    values it has seen.
    '''

    __slots__ = ['name', 'data']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the counter.

        :param name: The unique identifier for this user.
        :param data: The current dataset to operate with.
        '''
        self.name = kwargs.get('name', self.generate_id())
        self.data = kwargs.get('data', { self.name : 0 })

    def value(self):
        ''' Retrieve the current value of the instance.

        :returns: The current value of the instance
        '''
        return sum(self.data.values())

    def increment(self, value=1, name=None):
        ''' Increment the value for the supplied name.

        :param value: The amount to increment with (default 1)
        :param name: The name to increment for (default self.name)
        '''
        assert value >= 0, "can only increment a GCounter"
        name = name or self.name
        self.data[name] += value

    def compare(self, that):
        ''' Compare two instances to see which one is larger.

        :param that: The other instance to compare
        :returns: True if that is larger than self, False otherwise
        '''
        assert that != None, "can only compare to a valid GCounter"
        keys = set(that.data) | set(self.data)
        return all(self.data.get(key, 0) <= that.data.get(key, 0) for key in keys)

    def merge(self, that):
        ''' Given two instances, merge them into a new counter instance.

        :param that: The instance to merge with
        :returns: The newly merged instance
        '''
        assert that != None, "can only merge with a valid GCounter"
        keys = set(that.data) | set(self.data)
        data = { key : max(self.data.get(key, 0), that.data.get(key, 0)) for key in keys }
        return GCounter(name=self.name, data=data)

    def serialize(self):
        ''' Serialize the current instance into a simple form
        that can be transmitted over the wire.

        :returns: The serialized form of the current instance.
        '''
        return { 'name' : self.name, 'data' : self.data }

    @classmethod
    def deserialize(klass, payload):
        ''' Given a serialized payload, convert it to an instance
        of the current type.

        :param klass: The current type to convert to
        :param payload: The payload to convert to a type instance
        :returns: An instance of the current type
        '''
        return klass(**payload)

    def __add__(self, value): self.increment(value); return self

class PNCounter(CRDT):
    ''' A grow only counter that computes the overall sum
    of all the vectors by taking the max value of all the
    values it has seen.
    '''

    __slots__ = ['name', 'pdata', 'ndata']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the counter.

        :param name: The unique identifier for this user.
        :param pdata: The current increments dataset
        :param ndata: The current decrements dataset
        '''
        self.name  = kwargs.get('name', self.generate_id())
        self.pdata = kwargs.get('pdata', GCounter(name=self.name))
        self.ndata = kwargs.get('ndata', GCounter(name=self.name))

    def value(self):
        ''' Retrieve the current value of the instance.

        :returns: The current value of the instance
        '''
        return self.pdata.value() - self.ndata.value()

    def increment(self, value=1, name=None):
        ''' Increment the value for the supplied name.

        :param value: The amount to increment with (default 1)
        :param name: The name to increment for (default self.name)
        '''
        assert value >= 0, "can only increment with a positive value"
        self.pdata.increment(value, name)

    def decrement(self, value=1, name=None):
        ''' Decrement the value for the supplied name.

        :param value: The amount to decrement with (default 1)
        :param name: The name to decrement for (default self.name)
        '''
        assert value >= 0, "can only decrement with a positive value"
        self.ndata.increment(value, name)

    def compare(self, that):
        ''' Compare two instances to see which one is larger.

        :param that: The other instance to compare
        :returns: True if that is larger than self, False otherwise
        '''
        assert that != None, "can only compare to a valid PNCounter"
        return self.pdata.compare(that.pdata) and self.ndata.compare(that.ndata)

    def merge(self, that):
        ''' Given two instances, merge them into a new counter instance.

        :param that: The instance to merge with
        :returns: The newly merged instance
        '''
        assert that != None, "can only merge with a valid PNCounter"
        pdata = self.pdata.merge(that.pdata)
        ndata = self.ndata.merge(that.ndata)
        return PNCounter(name=self.name, pdata=pdata, ndata=ndata)

    def serialize(self):
        ''' Serialize the current instance into a simple form
        that can be transmitted over the wire.

        :returns: The serialized form of the current instance.
        '''
        return {
            'name'  : self.name,
            'pdata' : self.pdata.serialize(),
            'ndata': self.ndata.serialize()
        }

    @classmethod
    def deserialize(klass, payload):
        ''' Given a serialized payload, convert it to an instance
        of the current type.

        :param klass: The current type to convert to
        :param payload: The payload to convert to a type instance
        :returns: An instance of the current type
        '''
        payload['pdata'] = GCounter(**payload.get('pdata', {}))
        payload['ndata'] = GCounter(**payload.get('ndata', {}))
        return klass(**payload)

    def __add__(self, value): self.increment(value); return self
    def __sub__(self, value): self.decrement(value); return self
