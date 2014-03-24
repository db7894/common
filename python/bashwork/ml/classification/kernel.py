'''A Kernel function maps two vectors onto a scalar.

Kernels must be decomposable into the product of some
function mapped to each of the input vectors.

http://crsouza.blogspot.com/2010/03/kernel-functions-for-machine-learning.html
'''
import numpy as np


def dot_product():
    def kernel(a, b):
        return np.dot(a, b)
    return kernel

def linear(c=0.0):
    def kernel(a, b):
        return np.dot(a, b) + c
    return kernel

def gaussian(gamma):
    def kernel(a, b):
        delta = b - a
        return np.exp(-gamma * (delta * delta).sum())
    return kernel

def rational_quadradic(c=0.0):
    def kernel(a, b):
        delta  = b - a
        normal = (delta * delta).sum()
        return 1.0 - (normal / (normal + c))
    return kernel

def multiquadradic(c=0.0):
    def kernel(a, b):
        delta  = b - a
        return np.sqrt(delta.sum() ** 2 + c ** 2)
    return kernel

def inverse_multiquadradic(c=0.0):
    def kernel(a, b):
        delta  = b - a
        return 1.0 / np.sqrt(delta.sum() ** 2 + c ** 2)
    return kernel

def exponential(sigma):
    def kernel(a, b):
        return np.exp((b - a).sum() / (-2 * sigma * sigma))
    return kernel

def laplacian(sigma):
    def kernel(a, b):
        return np.exp((b - a).sum() / -sigma)
    return kernel

def sigmoid(alpha=1.0, c=0.0):
    def kernel(a, b):
        return np.tanh(alpha * np.dot(a, b) + c)
    return kernel

def polynomial(d, alpha=1.0, c=0):
    def kernel(a, b):
        return (alpha * np.dot(a, b) + c) ** d
