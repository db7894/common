#------------------------------------------------------------#
# classes
#------------------------------------------------------------#

class Entry(object):
    ''' Represents a single entry in a model along with
    its label and values.
    '''

    __slots__ = ['label', 'values']

    def __init__(self, label, values):
        ''' Initialize a new instance of an Entry class.

        :param label: The label associated with this entry
        :param values: The values associated with this entry
        '''
        self.label  = label
        self.values = tuple(values)

    def __str__(self):  return self.label
    def __repr__(self): return self.label
