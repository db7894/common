from bashwork.structure.crdt import CRDT

class UMap(CRDT):
    ''' If the keys for the map are always unique, then the
    map will trivialy converge.
    '''

    __slots__ = ['data']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the map.

        :param data: The current dataset to operate with.
        '''
        self.data = kwargs.get('data', {})

    def value(self):
        ''' Retrieve the current value of the instance.

        :returns: The current value of the instance
        '''
        return self.data

    def add(self, key, value):
        ''' Add the supplied key/value to the map.

        :param key: The key to add the value at in the map.
        :param value: The value to add to the map.
        '''
        assert key not in self.data, "A non unique value has been added"
        self.data[key] = value

    def remove(self, key):
        ''' Remove the supplied key from the map.

        :param key: The key to remove from the map
        '''
        assert key in self.data, "The supplied key must exist in the map"
        del self.data[key]

    def compare(self, that):
        ''' Compare two instances to see which one is larger.

        :param that: The other instance to compare
        :returns: True if that is larger than self, False otherwise
        '''
        assert that != None, "can only compare to a valid UMap"
        return set(self.data).issubset(set(that.data))

    def merge(self, that):
        ''' Given two instances, merge them into a new instance.

        :param that: The instance to merge with
        :returns: The newly merged instance
        '''
        assert that != None, "can only merge with a valid UMap"
        data = dict(self.data)
        data.update(that.data)
        return UMap(data=data)

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
    def __contains__(self, key): return (key in self.value())
