'''
.. todo::
   - voted, average
   - multiclass
'''
import random
import numpy as np

class Perceptron(object):

    def __init__(self, size, rate=0.2):
        ''' Create a new instance of the Perceptron

        :param size: The size of the neuron (weights)
        :param rate: The learning rate of the system
        '''
        self.weights = np.random.rand(max(1, size))
        self.learn_rate = rate

    def get_label(self, value):
        ''' A method that can be overriden to decide
        on the label for the supplied entry.

        :param value: The value to get the label for
        :returns: The decided label for the value
        '''
        return 1 if value >= 0 else 0

    def train(self, dataset, rounds):
        ''' Given a dataset, train the perceptron the
        number of rounds to update the underlying weights.

        :param dataset: The dataset to train with (values, expected)
        :param rounds: The number of rounds to train with this set
        '''
        for _ in range(rounds):
            entry, expect = random.choice(dataset)
            actual = self.predict(entry)
            errors = expect - actual
            self.weights += self.learn_rate * errors * entry

    def predict(self, entry):
        ''' Given a new entry, predict what label should
        be assigned to it.

        :param entry: The entry to predict the label for
        :returns: The predicted label for the entry
        '''
        return self.get_label(np.dot(self.weights, entry))

if __name__ == "__main__":
    # these are simple logic gates, note we cannot learn xor!
    training = [ (np.array([0,0,1]), 0), (np.array([0,1,1]), 1), (np.array([1,0,1]), 1), (np.array([1,1,1]), 1) ]
    #training = [ (np.array([0,0,1]), 0), (np.array([0,1,1]), 0), (np.array([1,0,1]), 0), (np.array([1,1,1]), 1) ]
    #training = [ (np.array([0,0,1]), 0), (np.array([0,1,1]), 1), (np.array([1,0,1]), 1), (np.array([1,1,1]), 0) ]
    neuron   = Perceptron(3, 0.2)
    neuron.train(training, rounds=100)

    for values, expect in training:
        result = neuron.predict(values)
        print "{}: {} -> {}".format(values, expect, result)
