'''
http://www.acatcancode.com/find-sink-directed-graph/
'''

def is_total_sink(matrix, i):
    ''' Given an adjancency matrix, check if the
    supplied vertex index is a total sink. A total
    sink is one that:

    1. does not have any outgoing edges
    2. all other edges terminate at it

    The Matrix is such that:

    1. matrix[i][j] == True:  there is an edge from i to j
    2. matrix[i][j] == False: there is not an edge from i to j

    :param matrix: The adjacency list matrix
    :param i: The vertex index to test if is a total sink
    :returns: True if i is a total sink, False otherwise
    '''
    if not 0 < i < len(matrix): return False
    is_valid = lambda j: (i == j) or (matrix[j][i] and not matrix[i][j])
    return all(is_valid(j) for j in range(len(matrix)))

def get_total_sink(matrix):
    '''
    :param matrix: The adjacency list matrix
    :returns: The index of a sink or None if there are none
    '''
    src, dst, size = 0, 0, len(matrix)         # start at upper left and move to lower right
    sinks = [True] * size                      # all nodes are possibly sinks
    while src < size and dst < size:           # while we are still in range
        if src == dst: dst += 1                # ignore node cycles
        elif not sinks[src]: src += 1          # if we are no longer a candidate
        elif matrix[src][dst]:                 # if we have an outgoing edge
            sinks[src] = False                 # remove this node from the candidates
            src += 1                           # then skip this node
        else:                                  # if we are missing an incoming edge
            sinks[dst] = False                 # remove this node from the candidates
            dst += 1                           # then skip this node

    is_sink = src < size and is_total_sink(matrix, src)
    return src if is_sink else None

if __name__ == "__main__":
    matrix = [
        [0, 1, 0, 0, 1],
        [1, 0, 1, 0, 1],
        [0, 0, 1, 0, 1],
        [1, 0, 0, 1, 1],
        [0, 0, 0, 0, 0],
    ]
    print is_total_sink(matrix, 4)
    print get_total_sink(matrix)
