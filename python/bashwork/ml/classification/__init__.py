class Classifier(object):
    ''' Interface for a simple classifier that can be
    trained to classify new entries of the same type.
    This should be generic enough to work with datasets
    that work with the numpy function abstractions.
    '''

    def train(self, dataset, labels, **kwargs):
        ''' Given a dataset, train a new classifier
        with the supplied parameters.
        
        :param dataset: The dataset to train with
        :param labels: The classified labels of the given dataset
        '''
        raise NotImplemented("train")

    def classify(self, entry, **kwargs):
        ''' Classify a new entry with the underlying
        previously trained classifier.
        
        :param entry: The entry to classify
        :returns: The label of the new entry
        '''
        raise NotImplemented("entry")

    def evaluate(self, dataset, labels, **kwargs):
        ''' Test the accurracy of the given classifier using
        the provided testing set.

        .. todo:: recall, precision, f1
       
        :param classifier: The classifier to evaluate
        :param dataset: The dataset to evaluate with the classifier
        :param labels: The labels of the supplied dataset
        :returns: (precision, recall, accuracy, f1-score)
        '''
        tp, fp, tn, fn = 0, 0, 0, 0
        classes = ((self.classify(e), l) for e, l in zip(dataset, labels))
        for guess, actual in classes:
            if actual == 1: tp, fp = tp + (guess == actual), fp + (guess != actual)
            if actual != 1: tn, fn = tn + (guess == actual), fn + (guess != actual)

        accuracy  = tp + tn
        precision = tp / float(tp + fp)
        recall    = tp / float(tp + fn)
        f1_score  = (2 * precision * recall) / (recall + precision)
        return (precision, recall, accurracy, f1_score)

