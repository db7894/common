'''
.. todo::

  http://datasciencelab.wordpress.com/2013/12/27/finding-the-k-in-k-means-clustering/

'''
from collections import defaultdict
from bashwork.ml.cluster import Clustering
from bashwork.ml.utility import distance
from bashwork.ml.utility import sample

#------------------------------------------------------------#
# terminate strategies
# - no/min re-assignment of points
# - no/min re-assignment of clusters
# - min decrease of SSE
#------------------------------------------------------------#
def count_terminate(rounds):
    count = rounds
    def implementation():
        count -= 1
        return count == 0
    return implementation

class KMeans(Clustering):
    '''
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of KMeans class.

        :param count: The number of centroids to create
        :param distance: The distance function to compare with
        :param initialize: The initialization function to use
        :param centroids: The initial centroids to use
        '''
        self.count      = kwargs.get('count', 5)
        self.distance   = kwargs.get('distance', distance.euclidean_distance)
        self.initialize = kwargs.get('initialize', sample.random)
        self.centroids  = kwargs.get('centroids', None)
        self.terminate  = kwargs.get('terminate', None)

    def train(self, dataset):
        '''
        '''
        centroids = self.centroids or self.initialize(dataset, self.count)

        while not self.terminate(self):
            groups = defaultdict(list)
            for entry in dataset:
                scores = ((distance(entry, centroid), group) for group, centroid in enumerate(centroids))
                group  = sorted(scores)[0][1] 
                groups[group].append(entry)
            centroids = [np.mean(values, axis=1) for values in groups.values()]
        return centroids
