class Clustering(object):
    '''
    '''

    def train(self, dataset, **kwargs):
        ''' Given a dataset, train a new classifier
        with the supplied parameters.
        
        :param dataset: The dataset to train with
        :param labels: The classified labels of the given dataset
        '''
        raise NotImplementedError("train")

    def classify(self, entry, **kwargs):
        ''' Classify a new entry with the underlying
        previously trained classifier.
        
        :param entry: The entry to classify
        :returns: The label of the new entry
        '''
        raise NotImplementedError("entry")
