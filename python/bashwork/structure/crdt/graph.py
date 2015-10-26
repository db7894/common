from bashwork.structure.crdt import CRDT
from bashwork.structure.crdt.sets import TwoPhaseSet

class TwoPhaseGraph(CRDT):
    ''' A graph represented by a collection of TwoPhaseSets.
    '''

    __slots__ = ['edge', 'node']

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the graph.

        :param edges: The edges of the graph
        :param nodes: The nodes of the graph
        '''
        self.edges = kwargs.get('edges', TwoPhaseSet())
        self.nodes = kwargs.get('nodes', TwoPhaseSet())

    def add_node(self, node):
        ''' Add the supplied node to the graph

        :param node: The node to add to the graph
        '''
        self.nodes.add(node)

    def remove_node(self, node):
        ''' Remove the supplied node to the graph

        :param node: The node to remove from the graph
        '''
        self.nodes.remove(node)

    def contains_node(self, node):
        ''' Check if the supplied node exists in the graph

        :param node: The node to check for in the graph
        :returns: True if the node exists, False otherwise
        '''
        return node in self.nodes

    def add_edge(self, src, dst):
        ''' Add the supplied edge from the graph

        :param src: The edge source to add
        :param dst: The edge destination to add
        '''
        assert src in self.nodes, "source node must exist in the graph"
        assert dst in self.nodes, "destination node must exist in the graph"
        self.edges.add((src, dst))

    def remove_edge(self, src, dst):
        ''' Remove the supplied edge from the graph

        :param src: The edge source to remove
        :param dst: The edge destination to remove
        '''
        assert src in self.nodes, "source node must exist in the graph"
        assert dst in self.nodes, "destination node must exist in the graph"
        self.edges.remove((src, dst))

    def contains_edge(self, src, dst):
        ''' Check if the supplied edge exist in the graph

        :param src: The edge source to check
        :param dst: The edge destination to check
        :returns: True if the edge exists, False otherwise
        '''
        return (src, dst) in self.edges

    def compare(self, that):
        ''' Compare two instances to see which one is larger.

        :param that: The other instance to compare
        :returns: True if that is larger than self, False otherwise
        '''
        assert that != None, "can only compare to a valid TwoPhaseGraph"
        return self.nodes.compare(that.nodes) or self.edges.compare(that.edges)

    def merge(self, that):
        ''' Given two instances, merge them into a new set instance.

        :param that: The instance to merge with
        :returns: The newly merged instance
        '''
        assert that != None, "can only merge with a valid TowPhaseGraph"
        nodes = self.nodes.merge(that.nodes)
        edges = self.edges.merge(that.edges)
        return TwoPhaseGraph(node=nodes, edges=edges)

    def serialize(self):
        ''' Serialize the current instance into a simple form
        that can be transmitted over the wire.

        :returns: The serialized form of the current instance.
        '''
        return {
            'nodes' : self.nodes.serialize(),
            'edges' : self.edges.serialize()
        }

    @classmethod
    def deserialize(klass, payload):
        ''' Given a serialized payload, convert it to an instance
        of the current type.

        :param klass: The current type to convert to
        :param payload: The payload to convert to a type instance
        :returns: An instance of the current type
        '''
        payload['nodes'] = TwoPhaseSet(**payload.get('nodes', {}))
        payload['edges'] = TwoPhaseSet(**payload.get('edges', {}))
        return klass(**payload)

    def __len__(self):
        return len(self.nodes.value())

    def __iter__(self):
        return iter(self.nodes.value())

    def __add__(self, value):
        store = self.edges if isinstance(tuple, value) else self.nodes
        store.add(value)
        return self

    def __sub__(self, value):
        store = self.edges if isinstance(tuple, value) else self.nodes
        store.remove(value)
        return self

    def __contains__(self, value):
        store = self.edges if isinstance(tuple, value) else self.nodes
        return value in store.value()

class AddOnlyMonotonicDag(CRDT):
    pass # TODO

class AddRemovePartialOrderDag(CRDT):
    pass # TODO
