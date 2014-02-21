from random import Random

class Airport(object):

    def __init__(self, **kwargs):
        self.name = kwargs.get('name')

    @classmethod
    def initialize(klass):
        r = Random()
        klass.airport_names = ['sfo', 'sea', 'jfk', 'stl', 'stp', 'ohr', 'bhm']
        klass.airport_map   = { n:klass(name=n) for n in klass.airport_names }
        klass.airports      = klass.airport_map.values()
        klass.connections   = { a:r.sample(klass.airports, r.randint(0, len(klass.airports))) for a in klass.airports }

    @classmethod
    def getAirport(klass, name):
        return klass.airport_map.get(name, None)

    @classmethod
    def getConnections(klass, port):
        return klass.connections.get(port, [])

    def __eq__(this, that): return this.name == that.name
    def __ne__(this, that): return this.name != that.name
    def __str__(this):      return this.name
    def __repr__(this):     return this.name

class BFS(object): # queue
    def __init__(self): self.q = []
    def push(self, v):  self.q.insert(0, v)
    def take(self):     return self.q.pop()
    def __len__(self):  return len(self.q)

class DFS(object): # stack
    def __init__(self): self.q = []
    def push(self, v):  self.q.append(v)
    def take(self):     return self.q.pop()
    def __len__(self):  return len(self.q)

def airport_routes(src, dst, method=BFS):
    ''' Given two airports source and destination,
    return a collection of all the paths between
    the two.

    :param src: The starting point of the route
    :param dst: The destination point of the route
    :returns: A list of all the paths
    '''
    frontier, results = method(), []
    frontier.push([src])
    while frontier:
        path = frontier.take()
        if path[-1] != dst:
            for conn in Airport.getConnections(path[-1]):
                if conn not in path:
                    frontier.push(path + [conn])
        else: results.append(path)
    return results

def airport_routes_recursive(src, dst):
    ''' Given two airports source and destination,
    return a collection of all the paths between
    the two.

    :param src: The starting point of the route
    :param dst: The destination point of the route
    :returns: A list of all the paths
    '''
    def loop(path, end, results):
        if path[-1] != end:
            for conn in Airport.getConnections(path[-1]):
                if conn not in path:
                    loop(path + [conn], end, results)
        else: results.append(path)
    r = []
    loop([src], dst, r)
    return r

#------------------------------------------------------------
# main
#------------------------------------------------------------

if __name__ == "__main__":
    Airport.initialize()
    #print Airport.connections.items()
    src = Airport.getAirport('sea')
    dst = Airport.getAirport('jfk')
    print airport_routes(src, dst)
