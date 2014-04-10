from collections import defaultdict
from bashwork.ml.cluster import Clustering
from bashwork.ml.utility import distance
from bashwork.ml.utility import sample


class KMeans(Clustering):
    '''
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of KMeans class.

        :param k: The number of centroids to create
        :param distance: The distance function to compare with
        :param initialize: The initialization function to use
        :param centroids: The initial centroids to use
        '''
        self.k          = kwargs.get('k', 5)
        self.distance   = kwargs.get('distance', distance.euclidean_distance)
        self.initialize = kwargs.get('initialize', sample.random)
        self.centroids  = kwargs.get('centroids', None)
        self.terminate  = kwargs.get('terminate', None)

    def train(self, dataset):
        '''
        '''
        groups = defaultdict(list)
        centroids = self.centroids or self.initialize(dataset, self.k)
        while not self.terminate(self):
            for entry in dataset:
                group = sorted((distance(entry, c), c) for c in centroids)[0][1]
                groups[group].append(entry)
            centroids = 
        self.centroids = centroids

    def classify(self, entry):
        '''
        '''
