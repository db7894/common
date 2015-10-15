from bashwork.structure.crdt import CRDT

class TwoPhaseGraph(CRDT):
    '''
    '''

    __slots__ = ['pedge', 'nedge', 'pnode', 'nnode']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the graph.

        :param pedge: The added edge data
        :param nedge: The removed edge data
        :param pnode: The added node data
        :param nnode: The removed node data
        '''
        self.pedge = kwargs.get('pedge', set())
        self.nedge = kwargs.get('nedge', set())
        self.pnode = kwargs.get('pnode', set())
        self.nnode = kwargs.get('nnode', set())

    def __len__(self): return len(self.value())
    def __iter__(self): return iter(self.value())
    def __add__(self, value): self.add(value); return self
    def __sub__(self, value): self.remove(value); return self
    def __contains__(self, value): return (value in self.value())
