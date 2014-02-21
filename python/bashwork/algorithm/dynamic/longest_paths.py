import sys
from collections import defaultdict
from common import Graph

#------------------------------------------------------------
# longest/shortest paths
#------------------------------------------------------------
def shortest_paths(graph):
    ''' Given a DAG, return the shortest path distances
    from the head to each node in the graph.

    :param graph: a linearized DAG
    :return A dict of {node: shortest path weight}
    '''
    distance = {n:sys.maxint for n in graph.nodes()}
    queue, distance[graph.head] = [graph.head], 0
    while any(queue):
        head  = queue.pop()
        queue = list(graph.siblings(head)) + queue
        if not any(graph.siblings(head)): continue
        for sibling in graph.siblings(head):
            path = distance[head] + graph.edge(head, sibling)
            distance[sibling] = min(distance[sibling], path)
    return distance

def longest_paths(graph):
    ''' Given a DAG, return the longest path distances
    from the head to each node in the graph (without
    loops).

    :param graph: a linearized DAG
    :return A dict of {node: shortest path weight}
    '''
    distance = {n:-sys.maxint for n in graph.nodes()}
    queue, distance[graph.head] = [graph.head], 0
    while any(queue):
        head  = queue.pop()
        queue = list(graph.siblings(head)) + queue
        if not any(graph.siblings(head)): continue
        for sibling in graph.siblings(head):
            path = distance[head] + graph.edge(head, sibling)
            distance[sibling] = max(distance[sibling], path)
    return distance
   

#------------------------------------------------------------
# longest/shortest paths tests
#------------------------------------------------------------
if __name__ == "__main__":
    graph = Graph('s')
    graph.add_edge('s', 'a', 1)
    graph.add_edge('s', 'c', 2)
    graph.add_edge('c', 'd', 3)
    graph.add_edge('c', 'a', 4)
    graph.add_edge('a', 'b', 6)
    graph.add_edge('b', 'd', 1)
    graph.add_edge('b', 'e', 2)
    graph.add_edge('d', 'e', 1)

    print shortest_paths(graph)
    print longest_paths(graph)
