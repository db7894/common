from collections import Counter
from bashwork.ml.distance import manhattan_distance

class KNearestNeighbors(object):
    ''' A naive implementation of K Nearest Neighbors that
    recomputes the distance to each entry in the dataset
    every time a request is made.
    '''

    def __init__(self, dataset, distance):
        ''' Initialize a new instance of KNearestNeighbors class
        
        :param dataset: The training dataset
        :param distance: The distance function to use
        '''
        self.dataset  = dataset
        self.distance = distance or manhattan_distance

    def get_neighbors(self, entry, k=1):
        ''' Retrieve the K nearest neighbors of the supplied
        entry.
        
        :param entry: The entry to label
        :param k: The number of neighbors to test
        :returns: The K nearest neighbors to the supplied entry
        '''
        values = entry.values
        neighbors = sorted((self.distance(d.values, values), d) for d in self.dataset)
        return neighbors[0:k]

    def get_label(self, entry, k=1):
        ''' Retrieve the associated label of the given
        entry based on the K nearest neighbors.
        
        :param entry: The entry to label
        :param k: The number of neighbors to test
        :returns: The label of the resulting entry
        '''
        entries = self.get_neighbors(entry, k)
        counter = Counter(entries)
        return counter.most_common(1)[0][0].label
