def merge_sorted_lists(xs, ys):
    rs = []
    while len(xs) > 0 and len(ys) > 0:
        rs.append(xs.pop(0) if xs[0] < ys[0] else ys.pop(0))
    return rs + xs + ys
