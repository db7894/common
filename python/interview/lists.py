def merge_sorted_lists(xs, ys):
    rs = []
    while len(xs) > 0 and len(ys) > 0:
        rs.append(xs.pop(0) if xs[0] < ys[0] else ys.pop(0))
    return rs + xs + ys

class Node(object):
    def __init__(self, value, link=None):
        self.value = value
        self.link  = link

def reverse_list(xs):
    last, head = xs, None
    while last:
        temp = last.link
        last.link = head
        last, head = temp, last
    return head
