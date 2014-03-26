from bashwork.structure.tree.binary import BinaryNode as Node

def get_color_skyline(intervals):
    '''
    :param intervals: A collection of (x1, x2, height, color)
    :returns: A collection of (x1, x2, color)
    '''
    intervals = sorted(intervals, key=lambda x: x[1])
    skyline   = []

    for interval in intervals:

