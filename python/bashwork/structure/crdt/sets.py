from bashwork.structure.crdt import CRDT, current_time

class GSet(CRDT):
    ''' A grow only set where the merging of two instances
    is simply the set union.
    '''

    __slots__ = ['data']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the set.

        :param data: The current dataset to operate with.
        '''
        self.data = set(kwargs.get('data', []))

    def value(self):
        ''' Retrieve the current value of the instance.

        :returns: The current value of the instance
        '''
        return self.data

    def add(self, value):
        ''' Add the supplied value to the set.

        :param value: The value to add to the set.
        '''
        self.data.add(value)

    def compare(self, that):
        ''' Compare two instances to see which one is larger.

        :param that: The other instance to compare
        :returns: True if that is larger than self, False otherwise
        '''
        assert that != None, "can only compare to a valid GSet"
        return self.data.issubset(that.data)

    def merge(self, that):
        ''' Given two instances, merge them into a new counter instance.

        :param that: The instance to merge with
        :returns: The newly merged instance
        '''
        assert that != None, "can only merge with a valid GSet"
        data = that.data | self.data
        return GSet(data=data)

    def serialize(self):
        ''' Serialize the current instance into a simple form
        that can be transmitted over the wire.

        :returns: The serialized form of the current instance.
        '''
        return { 'data' : list(self.data) }

    @classmethod
    def deserialize(klass, payload):
        ''' Given a serialized payload, convert it to an instance
        of the current type.

        :param klass: The current type to convert to
        :param payload: The payload to convert to a type instance
        :returns: An instance of the current type
        '''
        return klass(**payload)

    def __len__(self): return len(self.value())
    def __iter__(self): return iter(self.value())
    def __add__(self, value): self.add(value); return self
    def __contains__(self, value): return (value in self.value())

class USet(CRDT):
    ''' A set that allows additions and removals under the assumption
    that values are unique. In this case, a removed value can never
    be added again so tracking the remove set is redundant. It is up
    to the user to assure that values are unique.
    '''

    def remove(self, value):
        ''' Remove the value from the set.

        :param value: The value to remove from the set
        '''
        assert value in self.data, "cannot remove a value that is not present"
        self.data.remove(value)

    def __sub__(self, value): self.remove(value); return self

class TwoPhaseSet(CRDT):
    ''' A grow only set that allows additions and removals
    once. The final value is computed by taking the set union
    of all added values and disjunction of all removals.
    '''

    __slots__ = ['pdata', 'ndata']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the set.

        :param pdata: The current additions dataset
        :param ndata: The current removals dataset
        '''
        self.pdata = kwargs.get('pdata', GSet())
        self.ndata = kwargs.get('ndata', GSet())

    def value(self):
        ''' Retrieve the current value of the instance.

        :returns: The current value of the instance
        '''
        return self.pdata.value() - self.ndata.value()

    def add(self, value):
        ''' Add the value to the set.

        :param value: The value to add to the set
        '''
        self.pdata.add(value)

    def remove(self, value):
        ''' Remove the value from the set.

        :param value: The value to remove from the set
        '''
        assert value in self.pdata, "cannot remove a value that is not present"
        self.ndata.add(value)

    def compare(self, that):
        ''' Compare two instances to see which one is larger.

        :param that: The other instance to compare
        :returns: True if that is larger than self, False otherwise
        '''
        assert that != None, "can only compare to a valid PNSet"
        return self.pdata.compare(that.pdata) or self.ndata.compare(that.ndata)

    def merge(self, that):
        ''' Given two instances, merge them into a new set instance.

        :param that: The instance to merge with
        :returns: The newly merged instance
        '''
        assert that != None, "can only merge with a valid PNSet"
        pdata = self.pdata.merge(that.pdata)
        ndata = self.ndata.merge(that.ndata)
        return TwoPhaseSet(pdata=pdata, ndata=ndata)

    def serialize(self):
        ''' Serialize the current instance into a simple form
        that can be transmitted over the wire.

        :returns: The serialized form of the current instance.
        '''
        return {
            'pdata' : self.pdata.serialize(),
            'ndata' : self.ndata.serialize()
        }

    @classmethod
    def deserialize(klass, payload):
        ''' Given a serialized payload, convert it to an instance
        of the current type.

        :param klass: The current type to convert to
        :param payload: The payload to convert to a type instance
        :returns: An instance of the current type
        '''
        payload['pdata'] = GSet(**payload.get('pdata', {}))
        payload['ndata'] = GSet(**payload.get('ndata', {}))
        return klass(**payload)

    def __len__(self): return len(self.value())
    def __iter__(self): return iter(self.value())
    def __add__(self, value): self.add(value); return self
    def __sub__(self, value): self.remove(value); return self
    def __contains__(self, value): return (value in self.value())

class LWWSet(CRDT):
    ''' A set that records a timestamp with each operation such
    that the latest operation wins.
    '''

    __slots__ = ['pdata', 'ndata']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the set.

        :param pdata: The current additions dataset
        :param ndata: The current removals dataset
        '''
        self.pdata = kwargs.get('pdata', {})
        self.ndata = kwargs.get('ndata', {})

    def value(self):
        ''' Retrieve the current value of the instance.

        :returns: The current value of the instance
        '''
        keys = set(self.pdata) | set(self.ndata)
        return { key for key in keys if self.pdata.get(key, 0) > self.ndata.get(key, 0) }

    def add(self, value, time=None):
        ''' Add the value to the set.

        :param value: The value to add to the set
        :param time: The time to add this value at
        '''
        time = time or current_time()
        self.pdata[value] = time

    def remove(self, value, time=None):
        ''' Remove the value from the set.

        :param value: The value to remove from the set
        :param time: The time to remove this value at
        '''
        assert value in self.pdata, "cannot remove a value that is not present"
        time = time or current_time()
        self.ndata[value] = time

    def compare(self, that):
        ''' Compare two instances to see which one is larger.

        :param that: The other instance to compare
        :returns: True if that is larger than self, False otherwise
        '''
        assert that != None, "can only compare to a valid LWWSet"
        return (all(self.pdata.get(key, 0) <= that.pdata.get(key, 0) for key in set(self.pdata) | set(that.pdata))
           and  all(self.ndata.get(key, 0) <= that.ndata.get(key, 0) for key in set(self.ndata) | set(that.ndata)))

    def merge(self, that):
        ''' Given two instances, merge them into a new set instance.

        :param that: The instance to merge with
        :returns: The newly merged instance
        '''
        assert that != None, "can only merge with a valid LWWSet"
        pdata = { k : max(that.pdata.get(k, 0), self.pdata.get(k, 0)) for k in set(that.pdata) | set(self.pdata) }
        ndata = { k : max(that.ndata.get(k, 0), self.ndata.get(k, 0)) for k in set(that.ndata) | set(self.ndata) }
        return LWWSet(pdata=pdata, ndata=ndata)

    def serialize(self):
        ''' Serialize the current instance into a simple form
        that can be transmitted over the wire.

        :returns: The serialized form of the current instance.
        '''
        return {
            'pdata' : self.pdata,
            'ndata' : self.ndata,
        }

    @classmethod
    def deserialize(klass, payload):
        ''' Given a serialized payload, convert it to an instance
        of the current type.

        :param klass: The current type to convert to
        :param payload: The payload to convert to a type instance
        :returns: An instance of the current type
        '''
        return klass(**payload)

    def __len__(self): return len(self.value())
    def __iter__(self): return iter(self.value())
    def __add__(self, value): self.add(value); return self
    def __sub__(self, value): self.remove(value); return self
    def __contains__(self, value): return (value in self.value())

class PNSet(CRDT):
    ''' A set that records a counter with each operation such
    that add increments the counter and remove decrements the
    counter. If the counter value is positive, it is in the set.

    Extensions to add and remove are to prohibit adding or removing
    if the current count is negative or positive respectively.
    '''

    __slots__ = ['data']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the set.

        :param data: The current additions dataset
        '''
        self.data = kwargs.get('data', {})

    def value(self):
        ''' Retrieve the current value of the instance.

        :returns: The current value of the instance
        '''
        return { key for key in self.data if self.data[key] > 0 }

    def add(self, value):
        ''' Add the value to the set.

        :param value: The value to add to the set
        '''
        self.data[value] = self.data.get(value, 0) + 1

    def remove(self, value):
        ''' Remove the value from the set.

        :param value: The value to remove from the set
        '''
        self.data[value] = self.data.get(value, 0) - 1

    def compare(self, that):
        ''' Compare two instances to see which one is larger.

        :param that: The other instance to compare
        :returns: True if that is larger than self, False otherwise
        '''
        assert that != None, "can only compare to a valid PNSet"
        keys = set(self.data) | set(that.data)
        return all(self.data.get(key, 0) <= that.data.get(key, 0) for key in keys )

    def merge(self, that):
        ''' Given two instances, merge them into a new set instance.

        :param that: The instance to merge with
        :returns: The newly merged instance
        '''
        assert that != None, "can only merge with a valid PNSet"
        keys = set(self.data) | set(that.data)
        data = { key : self.data.get(key, 0) + that.data.get(key, 0) for key in keys } 
        return PNSet(data=data)

    def serialize(self):
        ''' Serialize the current instance into a simple form
        that can be transmitted over the wire.

        :returns: The serialized form of the current instance.
        '''
        return { 'data' : self.data }

    @classmethod
    def deserialize(klass, payload):
        ''' Given a serialized payload, convert it to an instance
        of the current type.

        :param klass: The current type to convert to
        :param payload: The payload to convert to a type instance
        :returns: An instance of the current type
        '''
        return klass(**payload)

    def __len__(self): return len(self.value())
    def __iter__(self): return iter(self.value())
    def __add__(self, value): self.add(value); return self
    def __sub__(self, value): self.remove(value); return self
    def __contains__(self, value): return (value in self.value())

class ORSet(CRDT):
    ''' A set that records a counter with each operation such
    that add increments the counter and remove decrements the
    counter. If the counter value is positive, it is in the set.

    Extensions to add and remove are to prohibit adding or removing
    if the current count is negative or positive respectively.
    '''

    __slots__ = ['data']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the set.

        :param data: The current additions dataset
        '''
        self.data = kwargs.get('data', {})

    def value(self):
        ''' Retrieve the current value of the instance.

        :returns: The current value of the instance
        '''
        return set(self.data)

    def add(self, value, name=None):
        ''' Add the value to the set.

        :param value: The value to add to the set
        '''
        name = name or self.generate_id()
        self.data.setdefault(value, set()).add(name)

    def remove(self, value):
        ''' Remove the value from the set.

        :param value: The value to remove from the set
        '''
        assert value in self.data, "cannot remove a value that is not present"
        del self.data[value]

    def compare(self, that):
        ''' Compare two instances to see which one is larger.

        :param that: The other instance to compare
        :returns: True if that is larger than self, False otherwise
        '''
        assert that != None, "can only compare to a valid ORSet"
        keys = set(self.data) | set(that.data)
        return all(self.data.get(key, 0) <= that.data.get(key, 0) for key in keys )

    def merge(self, that):
        ''' Given two instances, merge them into a new set instance.

        :param that: The instance to merge with
        :returns: The newly merged instance
        '''
        assert that != None, "can only merge with a valid ORSet"
        keys = set(self.data) | set(that.data)
        data = { key : self.data.get(key, set()) | that.data.get(key, set()) for key in keys } 
        return ORSet(data=data)

    def serialize(self):
        ''' Serialize the current instance into a simple form
        that can be transmitted over the wire.

        :returns: The serialized form of the current instance.
        '''
        return { 'data' : { key : list(ids) for key, ids in self.data.items() } }

    @classmethod
    def deserialize(klass, payload):
        ''' Given a serialized payload, convert it to an instance
        of the current type.

        :param klass: The current type to convert to
        :param payload: The payload to convert to a type instance
        :returns: An instance of the current type
        '''
        payload['data'] = { key : set(ids) for key, ids in payload['data'].items() }
        return klass(**payload)

    def __len__(self): return len(self.value())
    def __iter__(self): return iter(self.value())
    def __add__(self, value): self.add(value); return self
    def __sub__(self, value): self.remove(value); return self
    def __contains__(self, value): return (value in self.value())
