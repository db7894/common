import cv2
import numpy as np

def image_erode(image, size=5):
    '''
    This effectively erodes away the boundaries of the
    foreground object. This works by sliding a window across
    the image setting a pixel in the original image to 1 if
    all the pixels in the window are 1, else it will be set
    to 0. It is useful for removing small white noise in
    an image (or mask).
    '''
    kernel = np.ones((size, size), np.uint8)
    return cv2.erode(image, kernel, iterations=1)

def image_dilate(image, size=5):
    '''
    This is basically the opposite of erosion. It works by
    sliding a window across the image and setting a pixel in
    the original image to 1 if at least one pixel in the window
    is 1, otherwise 0. This is usually used after an erosion to
    restore the image size after the noise is removed or to
    join broken parts of an object.
    '''
    kernel = np.ones((size, size), np.uint8)
    return cv2.dilate(image, kernel, iterations=1)

def image_opening(image, size=5):
    '''
    This is simply an erosion followed by a dilation.
    '''
    kernel = np.ones((size, size), np.uint8)
    return cv2.morphologyEx(image, cv2.MORPH_OPEN, kernel)

def image_closing(image, size=5):
    '''
    This is simply a dilation followed by an erosion. It is
    useful for closing small holes in foreground objects or
    small black points on the object.
    '''
    kernel = np.ones((size, size), np.uint8)
    return cv2.morphologyEx(image, cv2.MORPH_CLOSE, kernel)

def image_morph_gradient(image, size=5):
    '''
    This is the difference between the dilation and erosion
    of an image. The result looks like an outline of the
    object.
    '''
    kernel = np.ones((size, size), np.uint8)
    return cv2.morphologyEx(image, cv2.MORPH_GRADIENT, kernel)

def image_morph_tophat(image, size=5):
    '''
    This is the difference between the input image and the
    opening.
    '''
    kernel = np.ones((size, size), np.uint8)
    return cv2.morphologyEx(image, cv2.MORPH_TOPHAT, kernel)

def image_morph_blackhat(image, size=5):
    '''
    This is the difference between the input image and the
    closing.
    '''
    kernel = np.ones((size, size), np.uint8)
    return cv2.morphologyEx(image, cv2.MORPH_BLACKHAT, kernel)

#------------------------------------------------------------
# Structuring Kernel
#------------------------------------------------------------
# Opencv also allows for creating non-rectangular kernels and
# supplies a number of shapes:
#
# * cv2.MORPH_RECT
# * cv2.MORPH_CROSS
# * cv2.MORPH_ELLIPSE
#
# These can be used with the `getStructuringElement` function::
#
#    cv2.getStructuringElement(cv2.MORPH_RECT, (5, 5))
#    cv2.getStructuringElement(cv2.MORPH_CROSS, (5, 5))
#    cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (5, 5))
#------------------------------------------------------------


if __name__ == "__main__":
    import sys

    im_base = cv2.imread(sys.argv[1], 0)
    im_erod = image_erode(im_base)
