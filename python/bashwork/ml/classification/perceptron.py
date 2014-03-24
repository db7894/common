'''
.. todo::
   - voted, average
   - multiclass
'''
import random
import numpy as np
from bashwork.ml.classification import kernel

class Perceptron(object):

    def __init__(self, **kwargs):
        ''' Create a new instance of the Perceptron.

        :param size: The size of the neuron (weights)
        :param rate: The learning rate of the system
        :param kernel: The underlying kernel to use for comparison
        '''
        self.size    = kwargs.get('size')
        self.weights = kwargs.get('weights', np.zeros(self.size, 'd'))
        self.rate    = kwargs.get('rate', 1.0)
        self.bias    = kwargs.get('bias', 0.0)
        self.kernel  = kwargs.get('kernel', kernel.dot_product())

    def get_label(self, value):
        ''' A method that can be overriden to decide
        on the label for the supplied entry.

        :param value: The value to get the label for
        :returns: The decided label for the value
        '''
        return np.sign(value)

    def train(self, dataset, rounds=1):
        ''' Given a dataset, train the perceptron the
        number of rounds to update the underlying weights.

        :param dataset: The dataset to train with (values, expected)
        :param rounds: The number of rounds to train with this set
        '''
        for _ in range(rounds):
            for entry, expect in dataset:
                actual = self.predict(entry)
                errors = expect - actual # 0 if correct, no update
                self.weights += self.rate * errors * entry
                self.bias    += errors
            np.random.shuffle(dataset)

    def predict(self, entry):
        ''' Given a new entry, predict what label should
        be assigned to it.

        :param entry: The entry to predict the label for
        :returns: The predicted label for the entry
        '''
        prediction = self.kernel(self.weights, entry) + self.bias
        return self.get_label(prediction)

class RandomPerceptron(Perceptron):
    ''' Just like the regular perceptron, except we
    use a random sampling of the dataset to train with
    instead of the entire dataset.
    '''

    def __init__(self, **kwargs):
        ''' Create a new instance of the Perceptron.

        :param size: The size of the neuron (weights)
        :param rate: The learning rate of the system
        '''
        kwargs['weights'] = np.random.rand(kwargs['size'])
        kwargs['bias']    = random.random()
        super(RandomPerceptron, self).__init__(**kwargs)

    def train(self, dataset, rounds):
        ''' Given a dataset, train the perceptron the
        number of rounds to update the underlying weights.

        :param dataset: The dataset to train with (values, expected)
        :param rounds: The number of rounds to train with this set
        '''
        for _ in range(rounds):
            entry, expect = random.choice(dataset)
            actual = self.predict(entry)
            errors = expect - actual # 0 if correct, no update
            self.weights += self.rate * errors * entry
            self.bias    += errors

class AveragedPerceptron(Perceptron):

    def __init__(self, **kwargs):
        ''' Initailize a new instance of the AveragedPerceptron class
        '''
        super(AveragedPerceptron, self).__init__(**kwargs)
        self.survivals   = 0
        self.iterations  = 0
        self.weights_acc = np.zeros(self.weights.shape, 'd')
        self.bias_acc    = 0.0

    def train(self, dataset, rounds=1):
        ''' Given a dataset, train the perceptron the
        number of rounds to update the underlying weights.

        :param dataset: The dataset to train with (values, expected)
        :param rounds: The number of rounds to train with this set
        '''
        for _ in range(rounds):
            for entry, expect in dataset:
                self.iterations += 1
                actual = self.predict(entry)
                errors = expect - actual # 0 if correct, no update
                if actual != expect:
                    self.weights_acc += self.weights * self.survivals
                    self.weights     += self.rate * errors * entry
                    self.bias_acc    += errors * self.survivals
                    self.bias        += errors
                    self.survivals    = 0
                else: self.survivals += 1
            np.random.shuffle(dataset)
        self.bias    = self.bias - self.bias_acc / self.iterations
        self.weights = self.weights - self.weights_acc / self.iterations
        #self.bias    = self.bias_acc / self.iterations
        #self.weights = self.weights_acc / self.iterations

class KernelPerceptron(Perceptron):

    def train(self, dataset, rounds):
        ''' Given a dataset, train the perceptron the
        number of rounds to update the underlying weights.

        :param dataset: The dataset to train with (values, expected)
        :param rounds: The number of rounds to train with this set
        '''
        for _ in range(rounds):
            entry, expect = random.choice(dataset)
            actual = self.predict(entry)
            if actual != expect:
                classify = lambda x: classify(x) + expect * self.kernel(entry, x) + expect
            errors = expect - actual # 0 if correct, no update
            self.weights += self.learn_rate * errors * self.kernel(entry)
            self.bias += errors

    def predict(self, entry):
        ''' Given a new entry, predict what label should
        be assigned to it.

        :param entry: The entry to predict the label for
        :returns: The predicted label for the entry
        '''
        return self.get_label(self.classify(entry))

if __name__ == "__main__":
    # these are simple logic gates, note we cannot learn xor!
    training = [ (np.array([0,0]), -1), (np.array([0,1]), 1), (np.array([1,0]), 1), (np.array([1,1]), 1) ]
    #training = [ (np.array([0,0]), -1), (np.array([0,1]), -1), (np.array([1,0]), -1), (np.array([1,1]), 1) ]
    #training = [ (np.array([0,0]), -1), (np.array([0,1]), 1), (np.array([1,0]), 1), (np.array([1,1]), -1) ]
    #neuron = Perceptron(size=2)
    neuron = AveragedPerceptron(size=2)
    #neuron   = KernelPerceptron(kernel=lambda x: x*x, size=2, rate=0.2)
    neuron.train(training, rounds=100)

    label = lambda x: 0 if x < 0 else 1
    for values, expect in training:
        result = neuron.predict(values)
        print "{}: {} -> {}".format(values, label(expect), label(result))
