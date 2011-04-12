
def __gray_internal(current, size):
    size = size - 1
    if size <= 0: return current
    clone = [x[:] for x in reversed(current)]
    for item in current: item.insert(0,0)
    for item in clone: item.insert(0,1)
    return __gray_internal(list(current + clone), size)

def generate_gray(size):
    return __gray_internal([[0],[1]], size)
