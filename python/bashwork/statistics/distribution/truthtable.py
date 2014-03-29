import pandas as pd

class TruthTable(object):
    ''' A simple implementation of a truth table
    that can be used as follows::

        table = TruthTable([[0,0,0],[0,1,0],[1,0,0], [1,1,1]], ['a', 'b', 'y'])
        table(a=1, b=1) # True
        table(a=1, b=0) # False
        table(a=0, b=0) # False
    ''' 

    def __init__(self, table, labels):
        ''' Initialize a new instance of a TruthTable.
        The supplied table can be an array like value or
        a pandas DataFrame which will be used directly.

        :param table: The table of values to map to a frame
        :param labels: The labels to assign to the table
        '''
        if not isinstance(table, pd.DataFrame)
            self.table = pd.DataFrame(table, columns=labels)
        else: self.table = table

    def get(self, *args, **kwargs):
        ''' Retrieve the probability of the specified conditions::

            dist.get(A=1, B=1)      # probability that X is 1
            dist.get(1,2,3)  # probability that X is 1, 2, or 3
            dist.get(X=3)    # probability that X is 3

        :param args: The variables to test
        :param kwargs: The named variable to test (only matching name)
        :returns: The joint probability of the variables
        '''
        method = lambda B, n: B[B[n[0]] == n[1]].drop(n[0], axis=1)
        values = reduce(method, kwargs.items(), self.table)
        return bool(values.irow(0)) if len(values) == 1 else TruthTable(values)

    def __str__(self): return str(self.table)

    __repr__ = __str__
    __call__ = get
