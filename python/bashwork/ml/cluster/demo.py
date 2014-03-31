from bashwork.ml.cluster import Clustering
from bashwork.ml.utility import distance
from bashwork.ml.utility import sample


class KMeans(Clustering):
    '''
    '''

    def __init__(self, **kwargs):
        '''
        '''
        self.k          = kwargs.get('k', 5)
        self.distance   = kwargs.get('distance', distance.euclidean_distance)
        self.initialize = kwargs.get('initialize', sample.random)
        self.centroids  = kwargs.get('centroids', None)

    def train(self, dataset):
        '''
        '''
        self.centroids = self.centroids or self.initialize(dataset, self.k)

    def classify(self, entry):
        '''
        '''
