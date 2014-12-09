import bsddb3
import msgpack

class BTree(object):
    ''' A simple wrapper around the bsddb database
    to provide for easily storing / loading complex
    data in some serialized format (json, msgpack).
    '''

    def __init__(self, path, serializer=msgpack):
        ''' Initialize a new instance of the BTree class.

        :param path: The path to the database on disk
        :param serializer: The serializer to use for advanced types
        '''
        self.database = bsddb3.btopen(path, 'n')
        self.serializer = serializer

    def store(self, key, value):
        ''' Store the value at the specified key

        :param key: The key to store data at
        :param value: The value to store at the given key
        '''
        self.database[key] = self.serializer.dumps(value)

    def load(self, key):
        ''' Load the data at the specified key

        :param key: The key to load data at
        :returns: The value at that key
        '''
        return self.serializer.loads(self.database[key])
