import cv2
import cv
import numpy as np

def rotate(image, degree):
    ''' A helper method to rotate a given image by the supplied
    degrees.

    :param image: The image to rotate
    :param degree: The number of degrees to rotate the image
    :returns: The rotated image
    '''
    size   = max(image.shape)
    center = (size/2.0, size/2.0)
    rotate = cv2.getRotationMatrix(center, degree, 1.0)
    return cv2.warpAffine(image, rotate, (size, size))
