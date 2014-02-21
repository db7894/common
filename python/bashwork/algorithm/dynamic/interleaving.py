from collections import defaultdict

#--------------------------------------------------------------------------------#
#  interleaving
#--------------------------------------------------------------------------------#
def interleave_recursive(xs, ys, zs):
    ''' Given two strings, see if they interleave into the third

    :param xs: The first string
    :param ys: The second string
    :param zs: The string x and y interleave into
    '''
    lx, ly, lz = len(xs), len(ys), len(zs)
    if lz == 0: return lx == 0 and ly == 0                    # all empty
    if lz == 1 and lx == 0: return ly == 1 and ys[0] == zs[0] # x empty, y not
    if lz == 1 and ly == 0: return lx == 1 and xs[0] == zs[0] # y empty, x not
    if xs[0] == zs[0] and ys[0] == zs[1]: return interleave_recursive(xs[1:], ys[1:], zs[2:])
    if xs[0] == zs[1] and ys[0] == zs[0]: return interleave_recursive(xs[1:], ys[1:], zs[2:])
    return False

def interleave_itertive(xs, ys, zs):
    ''' Given two strings, see if they interleave into the third

    :param xs: The first string
    :param ys: The second string
    :param zs: The string x and y interleave into
    '''
    C = defaultdict(bool)
    C[0, 0] = True
    for i in range(0, len(xs)):
        for j in range(0, len(ys)):
            x, y, z = xs[i], ys[j], zs[i + j]
            if   x == z and z != y: C[i,j] = C[i-1, j]
            elif x != z and z == y: C[i,j] = C[i, j-1]
            elif x == z and z == y: C[i,j] = C[i, j-1] or C[i-1, j]
            else: C[i, j] = False
    return C[len(xs), len(ys)]

#------------------------------------------------------------
# tests
#------------------------------------------------------------
if __name__ == "__main__":
    print interleave_recursive('101', '01', '10011')
    print interleave_itertive('101', '01', '10011')
    
