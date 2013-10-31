'''
https://github.com/networkx/networkx/tree/master/networkx/algorithms
'''
class Graph(object):
    ''' A basic graph
    '''

    def __init__(self, **attrs):
        ''' Initializes a new instance of the graph. All
        keywords specified will be added as properties on
        the graph.
        '''
        self.nodes = {}
        self.edges = {}
        self.attrs = {}
        self.attrs.update(attrs)

    def add_node(self, node, **attrs):
        '''
        '''
        if node not in self.nodes:
            self.nodes[node] = {}
            self.edges[node] = {}
        self.nodes[node].update(attrs)

    def add_nodes(self, nodes, **attrs):
        '''
        '''
        for node in nodes:
            self.add_node(node, **attrs)

    def remove_node(self, node):
        '''
        '''
        pass

    def remove_nodes(self, nodes):
        '''
        '''
        for node in nodes:
            self.remove_node(node)

    def __key(self):           return (self.nodes, self.edges, self.attrs)
    def __len__(self):         return len(self.nodes)
    def __eq__(self, other):   return other and (other.__key() == self.__key())
    def __ne__(self, other):   return other and (other.__key() != self.__key())
    def __hash__(self):        return hash(self.__key())
    def __repr__(self):        return self.attr.get('name', '')
    def __str__(self):         return self.attr.get('name', '')
    def __contains__(self, v): return (v in self.nodes)

#------------------------------------------------------------
# test runner
#------------------------------------------------------------
if __name__ == "__main__":
    import doctest
    doctest.testmod()
