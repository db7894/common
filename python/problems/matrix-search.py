#!/usr/bin/env python

# ------------------------------------------------------------
# utilities
# ------------------------------------------------------------

def is_valid(matrix, x, y):
    X, Y = len(matrix[0]), len(matrix)
    return (0 <= x < X) and (0 <= y < Y)

# ------------------------------------------------------------
# version 1 (diagonal reduction)
# ------------------------------------------------------------

def search_version_1(matrix, value):
    X, Y = len(matrix[0]), len(matrix)
    def next_middle(x, xm, y, ym):
        return (xm + x) / 2, (ym + y) / 2

    def recurse(x, xm, y, ym):
        xn, yn = next_middle(x, xm, y, ym)
        pivot  = matrix[yn][xn]

        if pivot == value:
            return [(xn, yn)]
        elif x >= xm and y >= ym:
            return []
        elif pivot > value:
            return recurse(x, xn - 1, yn, ym) + recurse(x, xm, y, ym - 1)
        elif pivot < value:
            return recurse(xn + 1, xm, y, ym) + recurse(x, xn, yn + 1, ym)
    return recurse(0, X - 1, 0, Y - 1)[0]

# ------------------------------------------------------------
# version 2 (right to left reduction)
# ------------------------------------------------------------

def search_version_2(matrix, value):
    def recurse(x, y):
        if not is_valid(matrix, x, y): return None
        elif matrix[y][x] == value: return (x, y)
        elif matrix[y][x]  > value: return recurse(x - 1, y)
        else: return recurse(x, y + 1)
    return recurse(len(matrix[0]) - 1, 0)

# ------------------------------------------------------------
# version 3 (dfs)
# ------------------------------------------------------------

def get_next(matrix, x, y):
    if is_valid(matrix, x + 1, y): yield (x + 1, y)
    if is_valid(matrix, x, y + 1): yield (x, y + 1)

def search_version_3(matrix, value):
    def recurse(x, y):
        if matrix[y][x] == value:
            return (x, y)

        for xn, yn in get_next(matrix, x, y):
            result = recurse(xn, yn)
            if result: return result
        return None

    return recurse(0, 0)

# ------------------------------------------------------------
# constants
# ------------------------------------------------------------

MATRIX = [
    [ 1,  4,  6,  8, 11, 24, 27],
    [ 3,  8,  9, 11, 12, 25, 30],
    [ 7,  9, 11, 23, 26, 27, 31],
    [ 8, 22, 24, 27, 33, 35, 42],
    [24, 30, 35, 40, 45, 50, 55],
]

# ------------------------------------------------------------
# main
# ------------------------------------------------------------

if __name__ == "__main__":
    assert (search_version_1(MATRIX, 23) == (3, 2))
    assert (search_version_2(MATRIX, 23) == (3, 2))
    assert (search_version_3(MATRIX, 23) == (3, 2))
