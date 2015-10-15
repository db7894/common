import uuid
from bashwork.structure.crdt import CRDT, current_time

class LWWRegister(CRDT):
    ''' A register where the last write wins as the current
    value of the register.
    '''

    __slots__ = ['time', 'data']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the LWWRegister

        :param time: The updated time of the data (default 0)
        :param data: The current data in the register (default None)
        '''
        self.time = kwargs.get('time', 0)
        self.data = kwargs.get('data', None)

    def value(self):
        ''' Retrieve the current value of the instance.

        :returns: The current value of the instance
        '''
        return self.data

    def set(self, value, time=None):
        ''' Given a new value, attempt to set it in the register

        :param value: The new value to set
        :param time: The new time to compare with
        '''
        self.time = time or current_time()
        self.data = value

    def compare(self, that):
        ''' Compare two instances to see which one is
        latest.

        :param that: The other instance to compare with
        :returns: True if that is later, False otherwise
        '''
        assert that != None, "can only compare to a valid LWWRegister"
        return self.time <= that.time

    def merge(self, that):
        ''' Given two instances, merge them into a new instance.

        :param that: The other instance to merge with this one
        :returns: A new instance with the merged data
        '''
        assert that != None, "can only merge with a valid LWWRegister"
        time = max(self.time, that.time)
        data = self.data if self.time >= that.time else that.data
        return LWWRegister(time=time, data=data)

    def serialize(self):
        ''' Serialize the current instance into a simple form
        that can be transmitted over the wire.

        :returns: The serialized form of the current instance.
        '''
        return { 'time' : self.time, 'data' : self.data }

    @classmethod
    def deserialize(klass, payload):
        ''' Given a serialized payload, convert it to an instance
        of the current type.

        :param klass: The current type to convert to
        :param payload: The payload to convert to a type instance
        :returns: An instance of the current type
        '''
        return klass(**payload)

class MVRegister(CRDT):
    ''' A register where the last write wins as the current
    value of the register.

    .. TODO:: finish this type as the spec/impls are weird
    '''

    __slots__ = ['name', 'data']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the MVRegister

        :param name: The unique identifier for this user.
        :param data: The current data in the register (default None)
        '''
        self.name = kwargs.get('name', self.generate_id())
        self.data = kwargs.get('data', {})

    def value(self):
        ''' Retrieve the current value of the instance.

        :returns: The current value of the instance
        '''
        return self.data.keys()

    def increment_version(self, name=None):
        ''' Increment the version value for the supplied name.

        :param name: The name to update the version for (default self.name)
        '''
        name = name or self.name
        time = max(value for key, value in self.data.items() if key != name)
        self.data[name] = time + 1

    def set(self, value, time=None):
        ''' Given a new value, attempt to set it in the register

        :param value: The new value to set
        :param time: The new time to compare with
        '''
        self.time = time or current_time()
        self.data = value

    def compare(self, that):
        ''' Compare two instances to see which one is
        latest.

        :param that: The other instance to compare with
        :returns: True if that is later, False otherwise
        '''
        assert that != None, "can only compare to a valid MVRegister"
        keys = set(that.data) | set(self.data)
        return all(self.data.get(key, 0) <= that.data.get(key, 0) for key in keys)

    def merge(self, that):
        ''' Given two instances, merge them into a new instance.

        :param that: The other instance to merge with this one
        :returns: A new instance with the merged data
        '''
        assert that != None, "can only merge with a valid MVRegister"
        time = max(self.time, that.time)
        data = self.data if self.time >= that.time else that.data
        return LWWRegister(time=time, data=data)

    def serialize(self):
        ''' Serialize the current instance into a simple form
        that can be transmitted over the wire.

        :returns: The serialized form of the current instance.
        '''
        return { 'time' : self.time, 'data' : self.data }

    @classmethod
    def deserialize(klass, payload):
        ''' Given a serialized payload, convert it to an instance
        of the current type.

        :param klass: The current type to convert to
        :param payload: The payload to convert to a type instance
        :returns: An instance of the current type
        '''
        return klass(**payload)
