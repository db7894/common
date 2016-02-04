#!/usr/bin/env python
'''
Given a 'filesystem' as a graph and a share of some
'files' as a graph, determine if the given file is
in the filesystem and then in the share.
'''
from bashwork.structure.graph import Graph

def is_shared(shared_root, find_node, root, graph):
    shares = set(graph.get_edges(shared_root))
    stack  = [ [root] ]

    while stack:
        path = stack.pop()
        if path[-1] == find_node:
            return any(p in shares for p in path)

        for node in graph.get_edges(path[-1]):
            stack.append(path + [node])
    return False

if __name__ == "__main__":
    graph = Graph(nodes=list('abcdefghijk'))
    graph.add_edge('a', 'b')
    graph.add_edge('a', 'c')
    graph.add_edge('a', 'd')
    graph.add_edge('b', 'e')
    graph.add_edge('d', 'f')
    graph.add_edge('d', 'g')
    graph.add_edge('g', 'h')
    graph.add_edge('g', 'i')
    graph.add_edge('j', 'g')
    graph.add_edge('j', 'k')
    graph.add_edge('k', 'b')

    print("is node 'h' shared in 'j': %s" % is_shared('j', 'h', 'a', graph)) 
