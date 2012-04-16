import time
import random

# ------------------------------------------------------------ 
# utilities
# ------------------------------------------------------------ 
def cross(left, right):
    return [l + r for l in left for r in right]

def some(sequence):
    for value in sequence:
        if value: return value
    return False

def from_file(filename, sep='\n'):
    return file(filename).read().strip().split(sep)

def shuffled(sequence):
    sequence = list(sequence)
    random.shuffle(sequence)
    return sequence

# ------------------------------------------------------------ 
# common globals
# ------------------------------------------------------------ 
digits = '123456789'
rows   = 'ABCDEFGHI'
cols   = digits

squares  = cross(rows, cols)
unitlist = (
    [cross(rows, c) for c in cols] +
    [cross(r, cols) for r in rows] +
    [cross(rs, cs)  for rs in ('ABC', 'DEF', 'GHI') for cs in ('123', '456', '789')])
units = dict((s, [u for u in unitlist if s in u]) for s in squares)
peers = dict((s, set(sum(units[s], [])) - set([s])) for s in squares)

# ------------------------------------------------------------ 
# parsing
# ------------------------------------------------------------ 
def parse_grid(grid):
    values = dict((s, digits) for s in squares)
    for s, d in grid_values(grid).items():
        if d in digits and not assign(values, s ,d):
            return False
    return values

def grid_values(grid):
    chars = [c for c in grid if c in digits or c in '0.']
    assert len(chars) == 81
    return dict(zip(squares, chars))

# ------------------------------------------------------------ 
# constraint propagation
# ------------------------------------------------------------ 
def assign(values, s, d):
    other = values[s].replace(d, '')
    if all(eliminate(values, s, dn) for dn in other):
        return values
    else: return False

def eliminate(values, s, d):
    if d not in values[s]: return values
    values[s] = values[s].replace(d, '')
    if len(values[s]) == 0: return False
    elif len(values[s]) == 1:
        dn = values[s]
        if not all(eliminate(values, sn, dn) for sn in peers[s]):
            return False

    for unit in units[s]:
        dplaces = [s for s in unit if d in values[s]]
        if len(dplaces) == 0: return False
        elif len(dplaces) == 1:
            if not assign(values, dplaces[0], d):
                return False
    return values

# ------------------------------------------------------------ 
# display
# ------------------------------------------------------------ 
def display(values):
    width = 1 + max(len(values[s]) for s in squares)
    line  = '+'.join(['-' * (width * 3)] * 3)
    for row in rows:
        print ''.join(values[row+c].center(width) + ('|' if c in '36' else '')
                for c in cols)
        if row in 'CF': print line
    print

# ------------------------------------------------------------ 
# search
# ------------------------------------------------------------ 
def is_solved(values):
    def unit_solved(unit):
        return set(values[s] for s in unit) == set(digits)
    is_valid = values is not False
    return is_valid and all(unit_solved(unit) for unit in unitlist)

def solve(grid):
    return search(parse_grid(grid))

def search(values):
    if values is False: return False
    if all(len(values[s]) == 1 for s in squares):
        return values
    n,s = min((len(values[s]), s) for s in squares if len(values[s]) > 1)
    return some(search(assign(values.copy(), s, d)) for d in values[s])

# ------------------------------------------------------------ 
# runners
# ------------------------------------------------------------ 
def random_puzzle(N=17):
    values = dict((s, digits) for s in squares)
    for s in shuffled(squares):
        if not assign(values, s, random.choice(values[s])):
            break
        ds = [values[s] for s in squares if len(values[s]) == 1]
        if len(ds) >= N and len(set(ds)) >= 8:
            return ''.join(values[s] if len(values[s]) == 1 else '.' for s in squares)
    return random_puzzle(N)

def constant_test():
    "A set of tests that must pass."
    assert len(squares) == 81
    assert len(unitlist) == 27
    assert all(len(units[s]) == 3 for s in squares)
    assert all(len(peers[s]) == 20 for s in squares)
    assert units['C2'] == [['A2', 'B2', 'C2', 'D2', 'E2', 'F2', 'G2', 'H2', 'I2'],
                           ['C1', 'C2', 'C3', 'C4', 'C5', 'C6', 'C7', 'C8', 'C9'],
                           ['A1', 'A2', 'A3', 'B1', 'B2', 'B3', 'C1', 'C2', 'C3']]
    assert peers['C2'] == set(['A2', 'B2', 'D2', 'E2', 'F2', 'G2', 'H2', 'I2',
                               'C1', 'C3', 'C4', 'C5', 'C6', 'C7', 'C8', 'C9',
                               'A1', 'A3', 'B1', 'B3'])
    print 'All tests pass.'

def time_solve(grid):
    values = grid_values(grid)
    display(values)
    start = time.clock()
    solution = solve(grid)
    done = time.clock() - start
    display(solution)
    valid = is_solved(solution)
    print "valid solution(%s) in %.2f seconds\n" % (valid, done)

if __name__ == '__main__':
    constant_test()
    grid1  = '003020600900305001001806400008102900700000008006708200002609500800203009005010300'
    grid2  = '4.....8.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......'
    hard1  = '.....6....59.....82....8....45........3........6..3.54...325..6..................'
    time_solve(hard1)
