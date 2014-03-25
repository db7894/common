import numpy as np
from bashwork.ml.classification import kernel

class Regression(object):
    pass

class LinearRegression(Regression):

    def __init__(self, **kwargs):
        '''
        '''
        self.size    = kwargs.get('size')
        self.rate    = kwargs.get('rate', 0.01)
        self.weights = kwargs.get('weights', np.zeros(size, 'd'))
        self.kernel  = kwargs.get('kernel', kernel.dot_product())

    def get_cost(self, xs, ys):
        '''
        '''
        hs = self.kernel(xs, self.weights) - ys
        return hs.T.dot(hs) / (2 * ys.size)


    def train(self, xs, ys, iterations=1):
        '''
        '''
        history = np.zeros(shape=(iterations, 1))
        for i in range(iterations):
            prediction = self.kernel(xs, self.weights)
            errors = (prediction - y) * xs
            self.weights = (self.weights - self.rate * errors) / ys.size
            history[i, 0] = self.get_cost(xs, ys)
        return history

class NormalLinearRegression(LinearRegression):

    def train(self, xs, ys, iterations=1):
        '''
        '''
        X = np.ones((ys.size, xs.shape[1] + 1), 'd')
        X[:, 1:] = xs
        self.weights = np.linalg.pinv(X.T.dot(X)).dot(X.T.dot(ys))
