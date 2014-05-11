import cv2
import numpy as np
import sys

im_base = cv2.imread(sys.argv[1], 0)

#------------------------------------------------------------
# Scaling
#------------------------------------------------------------
# Opencv allows one to scale by supplying a scaling factor
# or by supplying the requested size to scale to. It also
# has a number of methods for interpolation, however, good
# methods are as follows:
#
# * cv2.INTER_AREA   - shrinking
# * cv2.INTER_CUBIC  - zooming (slow)
# * cv2.INTER_Linear - zooming
#------------------------------------------------------------
def example_resize(image):
    im_res = cv2.resize(image, None, fx=2, fy=2, interpolation=cv2.INTER_CUBIC)
    height, width = image.shape[:2] # can also resize this way
    im_res = cv2.resize(image, (2 * width, 2 * height), interpolation=cv2.INTER_CUBIC)
    return im_res

#------------------------------------------------------------
# Translation
#------------------------------------------------------------
# This is the shifting of an object's location which is done
# using the transformation matrix::
#
#     [ 1 0 tx ]
#     [ 0 1 ty ]
#
#------------------------------------------------------------
def translate_image(image, tx=0, ty=0):
    ''' Given an image and the translation values on the x
    and y axis, return the translated image.

    :param image: The image to translate
    :param tx: The translation on the x axis
    :param ty: The translation on the y axis
    :returns: The resulting translated image
    '''
    rows, cols = image.shape[:2]
    matrix = np.float32([[1, 0, tx],[0, 1, ty]])
    # the third argument is the size of the output image in the
    # form of (width=cols, height=rows)
    return cv2.warpAffine(image, matrix, (cols, rows))

#------------------------------------------------------------
# Rotation
#------------------------------------------------------------
# This is the rotating of an object's location which is done
# using the transformation matrix::
#
#     [ cos x  -sin x ]
#     [ sin x   cos x ]
#
# Opencv provides a scaled rotation with an adjustable center
# of rotation so one can rotate at any location they prefer::
#
#    [  a  B  (1 - a) * center.x - B * center.y]
#    [ -B  a  B * center.x + (1 - a) * center.y]
#
#    a = scale * cos x
#    B = scale * sin x
#------------------------------------------------------------
def rotate_image(image, degree):
    ''' Rotates the supplied image around its center by the
    supplied number of degrees.

    :param image: The image to rotate
    :param degree: The degree of rotation
    :returns: The resulting rotated image
    '''
    rows, cols = img.shape[:2]
    matrix = cv2.getRotationMatrix2D((cols / 2, rows / 2), degree, 1)
    return cv2.warpAffine(image, matrix, (cols, rows))

#------------------------------------------------------------
# Affine Transformation
#------------------------------------------------------------
# After an affine transformation, all the existing parallel
# lines in an image will be parallel after the transformation.
# To get the transformation matrix, opencv needs three points
# in the image and where they will be after the transformation.
#------------------------------------------------------------
def affine_image(image, before, after):
    ''' Given an image, perform the affine transformation where
    the specified points will move from the before location to
    the after location.

    :param image: The image to perform an affine transform on
    :param before: The original points to move
    :param after: Where the points should be after the transform
    :returns: The resulting transformed image
    '''
    rows, cols = image.shape[:2]
    points1 = before # np.float32([[50,  50], [200, 50], [ 50, 200]])
    points2 = after  # np.float32([[10, 100], [200, 50], [100, 250]])
    matrix = cv2.getAffineTransform(points1, points2)
    return cv2.warpAffine(image, matrix, (cols, rows))

#------------------------------------------------------------
# Perspective Transformation
#------------------------------------------------------------
# After a perspective transformation, straight lines will
# remain straight. To find this transformation, we need four
# points (before and after). Of these four, 3 should not be
# co-linear.
#------------------------------------------------------------
def affine_image(image, before, after):
    ''' Given an image, perform the affine transformation where
    the specified points will move from the before location to
    the after location.

    :param image: The image to perform an affine transform on
    :param before: The original points to move
    :param after: Where the points should be after the transform
    :returns: The resulting transformed image
    '''
    rows, cols = image.shape[:2]
    points1 = before # np.float32([[56,65],[368,52],[28,387],[389,390]])
    points2 = after  # np.float32([[0,0],[300,0],[0,300],[300,300]])
    matrix = cv2.getPerspectiveTransform(points1, points2)
    return cv2.warpPerspective(image, matrix, (cols, rows))

#------------------------------------------------------------
# Smoothing Images
#------------------------------------------------------------
# 2D Convolution (image filtering) allows various filters to
# be applied just like in signal processing:
#
# * low-pass filter: helps in removing noise
# * high-pass filter: helps in finding edges
#------------------------------------------------------------
def image_average(image):
    ''' Given an image perform a window average to the
    image by:

    1. For each pixel, a 5x5 window is centered on that pixel
    2. All the pixels falling within that window are summed up
    3. The resuls is divided by 25

    :param image: The image to perform an average transform on
    :returns: The resulting averaged image
    '''
    kernel = np.ones((5, 5), np.float32) / 25
    return cv2.filter2D(image, -1, kernel)

def image_blur(image):
    ''' Given an image perform blur on the image by:

    1. For each pixel, a 3x3 window is centered on that pixel
    2. Replace the central pixel by the window average

    :param image: The image to perform a blur on
    :returns: The resulting blurred image
    '''
    # cv2.boxFilter(image, normalize=False)
    return cv2.blur(image, (5, 5))

def image_gaussian_blur(image):
    ''' Given an image perform blur on the image. The kernel
    is described by:

    1. A kernel of size x, y where x and y are positive and odd
    2. The standard deviation of x, y with sigmaX, sigmaY
    3. If both of these are 0, they are calculated from the kernel size

    :param image: The image to perform a blur on
    :returns: The resulting blurred image
    '''
    # cv2.getGaussianKernal((5, 5), 5)
    return cv2.GaussianBlur(image, (5, 5), 0)

def image_median_blur(image):
    ''' Given an image perform blur on the image by:

    1. compute the median of all pixels under a window
    2. replace the central pixel with that median value

    :param image: The image to perform a blur on
    :returns: The resulting blurred image
    '''
    return cv2.medianBlur(image, 5)

def image_edge_blur(image):
    ''' Given an image, perform a blur but preserve the
    edges.

    :param image: The image to perform a blur on
    :returns: The resulting blurred image
    '''
    return cv2.bilateralFilter(image, 9, 75, 75)
