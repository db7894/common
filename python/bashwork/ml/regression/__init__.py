
class Regression(object):
    ''' Interface for a regression system used to
    predict new values given previous values.
    '''

    def predict(self, x, **kwargs):
        ''' Given a new entry, predict the expected
        result of the entry.

        :param x: The entry to predict a value for
        :returns: The predicted value of the new entry
        '''
        raise NotImplemented("predict")

    def train(self, xs, ys, **kwargs):
        ''' Given a training set and the expected results,
        train a new regressor.

        :param xs: The dataset to train with
        :param ys: The expected results of the dataset
        '''
        raise NotImplemented("train")
