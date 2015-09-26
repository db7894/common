def print_matrix(matrix):
    ''' A helper method to print out a formatted
    matrix to the screen.

    :param matrix: The matrix to print to the screen
    '''
    for row in matrix:
        print(row)
    print('')

def zero_fill_matching_entries(matrix):
    ''' Given a matrix of values, if an entry
    is 0, fill its entire row and column with
    zeros as well.

    :param matrix: The matrix to zero fill
    :returns: The zero filled resulting matrix
    '''
    rows, cols = set(), set()

    for x, xs in enumerate(matrix):
        for y, ys in enumerate(xs):
            if matrix[x][y] == 0:
                rows.add(x)
                cols.add(y)

    for x, xs in enumerate(matrix):
        for y, ys in enumerate(xs):
            if x in rows or y in cols:
                matrix[x][y] = 0

    return matrix


def count_groups(matrix):
    ''' Given a matrix of water(0) and land(1),
    count the number of island groups.

    :param matrix: The matrix to count island groups in
    :returns: The number of island groups found
    '''
    count = 0
    def zero_group(x, y):
        if ((x < 0) or (x >= len(matrix)) or 
            (y < 0) or (y >= len(matrix[x])) or
            not matrix[x][y]): return 0
        matrix[x][y] = 0
        zero_group(x - 1, y)
        zero_group(x + 1, y)
        zero_group(x, y - 1)
        zero_group(x, y + 1)
        return 1

    for x, xs in enumerate(matrix):
        for y, ys in enumerate(xs):
            if ys: count += zero_group(x, y)
    return count


def is_matrix_path(matrix):
    ''' Given a matrix with obsticals marked by 1,
    is there a path down and to the right only to
    reach the corner un-obstructed.

    :param matrix: The matrix to test
    :returns: True if there is a path, False otherwise
    '''
    def is_path(x, y):
        if (x == len(matrix) or y == len(matrix[x])
            or matrix[x][y]): return False
        if (x == len(matrix) - 1 and y == len(matrix[x]) -1
            and not matrix[x][y]): return True
        return is_path(x + 1, y) or is_path(x, y + 1)
    return is_path(0, 0)


def rotate_matrix_ninety_degress(matrix):
    ''' Given a matrix of values, rotate the
    entire matrix by 90 degress clockwise.

    :param matrix: The matrix to rotate.
    :returns: The rotated matrix.
    '''
    N = len(matrix)                                 # the matrix must be square
    for layer in range(0, N / 2):                   # for each onion layer
        vmin = layer                                # the minimum x,y
        vmax = N - 1 - layer                        # the maximum x,y
        for vcur in range(vmin, vmax):              # for each element in that layer
            voff = vmax + vmin - vcur               # the current matrix offset
            top_left = matrix[vmin][vcur]           # copy top_left  -> temporary
            matrix[vmin][vcur] = matrix[voff][vmin] # copy low_left  -> top_left
            matrix[voff][vmin] = matrix[vmax][voff] # copy low_right -> low_left
            matrix[vmax][voff] = matrix[vcur][vmax] # copy top_right -> low_left
            matrix[vcur][vmax] = top_left           # copy top_left  -> top_right
    return matrix
