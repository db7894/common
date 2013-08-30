import heapq
from collections import defaultdict

class Op(object):
    Insert = 1
    Remove = 2

class Skyline(object):

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

class SkylineBrute(object):

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

               
solutions = [(1,11), (3,13), (9,0), (12,7), (16,3), (19,18), (22,3), (23,13), (29,0)]
buildings = [[1,11,5],[2,6,7],[3,13,9],[12,7,16],[14,3,25],[19,18,22],[23,13,29],[24,4,28]]
assert solutions == Skyline(buildings).solve()
assert solutions == SkylineBrute(buildings).solve()
