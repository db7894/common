
class Environment(object):
    ''' A simple hierarchical symbol table
    '''

    def __init__(self, parent=None):
        ''' Initialize a new instance of the Environment

        :param parent: An optional parent to descend from
        '''
        self.table  = {}
        self.parent = parent

    def get(self, symbol):
        ''' Attempt to get the supplied symbol from the
        environment. This will walk the hierarcy
        attempting to find the symbol and return None if
        it cannot be found.

        :param symbol: The symbol to get from the env
        :returns: The value of the symbol in the env
        '''
        current = self
        while current != None:
            if symbol in current.table:
                return current.table[symbol]
            current = current.parent
        return None

    def put(self, symbol, value):
        ''' Add the supplied symbol and value to the
        environment.

        :param symbol: The symbol to add to the env
        :param value: The value of the symbol to add
        '''
        self.table[symbol] = value

# ------------------------------------------------------------
# exported symbols
# ------------------------------------------------------------
__all__ = [ "Environment" ]
