import pylab as pl
from PIL import Image
from scipy.ndimage import filters
import sys

def choose_points(path, count=4):
    ''' Given a path to an image, open it and allow the user
    to pick N points on the image.

    :param path: The path to the image
    :param count: The number of points to select
    :returns: The selected points rounded to ints
    '''
    image = pl.array(Image.open(path)) # Image.fromArray(image)
    pl.imshow(image)
    points = pl.ginput(count)
    points = [(int(x), int(y)) for x, y in points]
    pl.close()
    return points

def equalize(image, bins=256):
    ''' Given an image, equalize it based on the generated
    histogram so all the intensities are balanced.

    :param image: The image to equalize
    :param bins: The number of bins for the histogram
    :returns: The equalized image
    '''
    im_hist, bins = pl.histogram(image.flatten(), bins, normed=True)
    cdf = im_hist.cumsum()                               # cumulative distribution function
    cdf = 255 * cdf / cdf[-1]                            # normalize
    im_norm = pl.interp(image.flatten(), bins[:-1], cdf) # linear interpolate new pixel values
    return im_norm.reshape(image.shape)                  # restore to the correct shape

def gradient(image):
    ''' Given an image, this computes its gradient using
    sobel filters::

             [-1 0 1]        [ -1 -2 -1 ]
        Dx = [-2 0 2]   Dy = [  0  0  0 ]
             [-1 0 1]        [  1  2  1 ]

    The prewitt filters can also be used::

             [-1 0 1]        [ -1 -1 -1 ]
        Dx = [-1 0 1]   Dy = [  0  0  0 ]
             [-1 0 1]        [  1  1  1 ]
    '''
    im_x = pl.zeros(image.shape)
    im_y = pl.zeros(image.shape)

    filters.sobel(image, 1, im_x)
    filters.sobel(image, 0, im_y)
    magnitude = pl.sqrt(im_x**2 + im_y**2)
    angle = pl.arctan2(im_y, im_x)
    return magnitude, angle
