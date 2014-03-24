'''A Kernel function maps two vectors onto a scalar.

Kernels must be decomposable into the product of some
function mapped to each of the input vectors.

http://crsouza.blogspot.com/2010/03/kernel-functions-for-machine-learning.html
'''
import numpy as np

def dot_product():
    return lambda a, b: np.dot(a, b)

def linear(c=0.0):
    return lambda a, b: np.dot(a, b) + c

def gaussian(sigma):
    return lambda a, b: np.exp(-np.linalg.norm(b - a) ** 2 / (2 * sigma ** 2))

def gaussian_gamma(gamma):
    return lambda a, b: np.exp(-gamma * np.linalg.norm(b - a) ** 2)

def rational_quadradic(c=0.0):
    def kernel(a, b):
        normal = np.linalg.norm(b - a) ** 2
        return 1.0 - (normal / (normal + c))
    return kernel

def multiquadradic(c=0.0):
    return lambda a, b:np.sqrt((b - a).sum() ** 2 + c ** 2)

def inverse_multiquadradic(c=0.0):
    return lambda a, b: 1.0 / np.sqrt((b - a).sum() ** 2 + c ** 2)

def exponential(sigma):
    return lambda a, b: np.exp((b - a).sum() / (-2 * sigma * sigma))

def laplacian(sigma):
    return lambda a, b: np.exp((b - a).sum() / -sigma)

def sigmoid(alpha=1.0, c=0.0):
    return lambda a, b: np.tanh(alpha * np.dot(a, b) + c)

def polynomial(d, alpha=1.0, c=0):
    return lambda a, b: (alpha * np.dot(a, b) + c) ** d
