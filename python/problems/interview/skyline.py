import heapq
from collections import defaultdict
from collections import namedtuple

class Op(object):
    Insert = 1
    Remove = 2

class SkylineRidge(object):

    def __init__(self, buildings):
        ''' Initialize a new instance of the problem

        :param buildings: The buildings to solve with
        '''
        self.buildings = buildings

    def get_events(self):
        ''' Get a stream of events to use with solving
        the problem: `event = (x, op, y)`

        This allows the priority to the following:

        1. left events before right events
        2. insert events before remove events
        3. finally return the max height
        '''
        heap = []
        for l, h, r in self.buildings:
            heapq.heappush(heap, (l, Op.Insert, h))
            heapq.heappush(heap, (r, Op.Remove, h))
        while heap: yield heapq.heappop(heap)

    def solve(self):
        ''' Generate the solution to the problem
        using a vertical sweep line.
        '''
        results, current, tree = [], 0, []
        for x, op, h in self.get_events():
            if   op == Op.Insert: tree.append(h)
            elif op == Op.Remove: tree.remove(h)
            max_height = max(tree or [0])
            if current != max_height:
                current = max_height
                results.append((x, max_height))
        return results

class SkylineRidgeBrute(object):

    def __init__(self, buildings):
        ''' Initialize a new instance of the problem

        :param buildings: The buildings to solve with
        '''
        self.buildings = buildings

    def get_heights_map(self):
        heights = defaultdict(int)
        for l, h, r in self.buildings:
            for v in range(l, r):
                heights[v] = max(heights[v], h)
        return heights

    def solve(self):
        heights = self.get_heights_map()
        current = 0
        results = []
        ml, mr  = min(heights), max(heights)
        for x in range(ml, mr + 1):
            max_height = heights[x]
            if current != max_height:
                current = max_height
                results.append((x, max_height))
        results.append((mr + 1, 0))
        return results

def get_skyline_colors(intervals):
    '''
    :param intervals: A collection of (x1, x2, height, color)
    :returns: A collection of (x1, x2, color)
    '''
    Segment = namedtuple('Segment', ['left', 'right', 'height', 'color'])
    Point   = namedtuple('Point',   ['value', 'is_left', 'height', 'color'])

    def get_points(intervals):
        heap = []
        for x1, x2, height, color in intervals:
            heapq.heappush(heap, Point(x1, True,  height, color))
            heapq.heappush(heap, Point(x2, False, height, color))
        while heap: yield heapq.heappop(heap)

    map_height, segment = [(0, '')], None
    skyline = []

    for point in get_points(intervals):
        if segment:
            max_height, color = max(map_height)
            if segment.height != max_height or segment.color != color:
                skyline.append(segment)
                segment = Segment(segment.right, point.value, max_height, color)
            else: segment = segment._replace(right=point.value)
        else: segment = Segment(0, point.value, point.height, point.color)

        if point.is_left: map_height.append((point.height, point.color))
        else: map_height.remove((point.height, point.color))
    skyline.append(segment)
    return skyline



if __name__ == "__main__":
    solutions = [(1,11), (3,13), (9,0), (12,7), (16,3), (19,18), (22,3), (23,13), (29,0)]
    buildings = [[1,11,5],[2,6,7],[3,13,9],[12,7,16],[14,3,25],[19,18,22],[23,13,29],[24,4,28]]
    assert solutions == SkylineRidge(buildings).solve()
    assert solutions == SkylineRidgeBrute(buildings).solve()

    intervals = [(1,3,1,'r'), (2,4,2,'b'), (5,9,2,'g'), (5,7,4,'w'), (8,10,1,'y')]
    print get_skyline_colors(intervals)
