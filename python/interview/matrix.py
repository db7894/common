def count_groups(matrix):
    ''' Given a matrix of water(0) and land(1),
    count the number of island groups.
    '''
    count = 0
    def zero_group(x, y):
        if ((x < 0) or (x > len(matrix)) or 
            (y < 0) or (y > len(matrix[x])) or
            not matrix[x][y]): return 0
        matrix[x][y] = 0
        zero_group(x - 1, y)
        zero_group(x + 1, y)
        zero_group(x, y - 1)
        zero_group(x, y + 1)
        return 1

    for xs in matrix:
        for ys in matrix:
            if ys: count += zero_group(xs, ys)
    return count

def is_matrix_path(matrix):
    ''' Given a matrix with obsticals marked by 1,
    is there a path down and to the right only to
    reach the corner un-obstructed.
    '''
    def is_path(x, y):
        if (x == len(matrix) or y == len(matrix[x])
            or matrix[x][y]): return False
        if (x == len(matrix) - 1 and y == len(matrix[x]) -1
            and not matrix[x][y]): return True
        return is_path(x + 1, y) or is_path(x, y + 1)
    return is_path(0, 0)
