import sys

def print_matrix(matrix): # pragma: no cover
    ''' A helper method to print out a formatted
    matrix to the screen.

    :param matrix: The matrix to print to the screen
    '''
    nump = max(max(row) for row in matrix)
    numn = min(min(row) for row in matrix)
    size  = max(len(str(nump)), len(str(numn)))
    form  = '{:%d}' % size
    string = []

    for row in matrix:
        entry = ' '.join(form.format(v) for v in row)
        string.append('[' + entry + ']')
        
    print('\n'.join(string))


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


def find_value_in_sorted_matrix(matrix, value):
    ''' Given a matrix that is sorted left to right
    (cols) and top to bottom (rows), find the given
    value in the matrix.

    :param matrix: The matrix to search for values in
    :param value: The value to find in the matrix
    :returns: The coordinates (row, col) for the value
    '''
    N = len(matrix[0]) - 1             # get the max size of the matrix
    row, col = 0, N                    # start at the upper right

    while row <= N and col >= 0:       # until we are at the bottom left
        if matrix[row][col] == value:  # if we find the value
            return (row, col)          # we are done
        elif matrix[row][col] > value: # otherwise if we are too big
            col -= 1                   # move left
        else: row += 1                 # if too small, move down
    return None                        # our value does not exist


def find_largest_submatrix_sum(matrix):
    ''' Given a matrix, find the maximum sum of any sub matrix
    in that matrix.

    :param matrix: The matrix to find the sub-matrix in
    :returns: The maximum sub array sum
    '''
    N = len(matrix)

    def get_integral_matrix(matrix):
        ''' This precomputes the integral image such that any sum
        operation can be reduced to four operations regardless of
        the size.
        '''
        ix = [[matrix[0][0]] * N for _ in range(N)]
        for r in range(1, N): ix[r][0] = ix[r - 1][0] + matrix[r][0]
        for c in range(1, N): ix[0][c] = ix[0][c - 1] + matrix[0][c]
        for r in range(1, N):
            for c in range(1, N):
                ix[r][c] = ix[r - 1][c] + ix[r][c - 1] - ix[r - 1][c - 1] + matrix[r][c]
        return ix

    def sum_matrix_at(ix, top_left, bot_right):
        ''' Perform a sum on the integral image given the top
        left coordinates and the bottom right coordinates.
        '''
        rl, cl = top_left
        rh, ch = bot_right
        if rl == 0 and cl == 0: return ix[rh][ch]
        elif rl == 0: return ix[rh][ch] - ix[rh][cl - 1]
        elif cl == 0: return ix[rh][ch] - ix[rl - 1][ch]
        return ix[rh][ch] - ix[rh][cl] - ix[rl][ch] + ix[rl][cl] 

    def get_matrix_iterator():
        ''' This simply abstracts generating all the pairs
        of (rt, ct) and (rb, cb)
        '''
        for r1 in range(0, N):
            for r2 in range(r1, N):
                for c1 in range(0, N):
                    for c2 in range(c1, N):
                        yield (r1, c1), (r2, c2)

    integral = get_integral_matrix(matrix)
    max_sum  = (-sys.maxint, None, None)
    for top, bot in get_matrix_iterator():
        cur_sum = (sum_matrix_at(integral, top, bot), top, bot)
        max_sum = max(cur_sum, max_sum)
    return max_sum


def get_largest_string_matrix(dictionary):
    ''' Given a dictionary of possible words,
    construct a matrix such that every row and
    every column is a valid word in the dictionary.

    :param dictionary: The dictionary to get words from
    :returns: The largest solution found
    '''
    pass # TODO
