import sys
import os
import json
import numpy as np
import cv2
import cv
from matplotlib import pyplot as plt

#------------------------------------------------------------
# configuration
#------------------------------------------------------------

class Color(object):
    ''' A collection of common colors to use with debugging
    or marking.
    '''
    Red   = (0, 0, 255)
    Green = (0, 255, 0)
    Blue  = (255, 0, 0)

class Config(object):
    ''' A collection of configuration parameters for the
    detection algorithm.
    '''
    LowerThreshold = (105, 100, 100)
    UpperThreshold = (145, 255, 255)
    MorphKernel    = np.ones((5, 5), np.uint8)

#------------------------------------------------------------
# image utilities
#------------------------------------------------------------

def open_if_path(path, **kwargs):
    ''' Given an object, return it if it is an image or create
    an image if it is a path to an image.

    :param path: The path to an image or an image
    :returns: A new numpy image
    '''
    is_gray = kwargs.get('gray', False)
    im_base = cv2.imread(path) if isinstance(path, unicode) else path
    if is_gray:
        return cv2.cvtColor(im_base, cv2.COLOR_BGR2GRAY)
    return im_base

def generate_pick_mask(image):
    ''' Given a path or image, generate a mask for the
    taped off space in the picking region.
    
    :param image: The image or path to generate a mask for
    :returns: The mask of the supplied image
    '''
    im_base = open_if_path(image)
    im_base = cv2.cvtColor(im_base, cv2.COLOR_BGR2HSV)
    im_blue = cv2.inRange(im_base, Config.LowerThreshold, Config.UpperThreshold)
    im_open = cv2.morphologyEx(im_blue, cv2.MORPH_OPEN, Config.MorphKernel)
    return im_open

def get_image_lines(image):
    ''' Given an image, return a collection of discovered
    lines in that image.

    This algorithm has a number of configuration parameters:

    * Canny(image, _, _, aperture-size)
    * HoughLinesP(image, 1, cv.CV_PI/180, threshold, minimum-length, maximum-gap)

    :param image: The image to get the lines from
    :returns: A collection of the lines in the image
    '''
    im_base = open_if_path(image)
    im_cann = cv2.Canny(im_base, 100, 100, apertureSize=3)
    return cv2.HoughLinesP(im_cann, 1, cv.CV_PI/180, 50, 70, 20)[0]

def add_image_lines(image, lines, **kwargs):
    ''' Given an image, add the supplied lines to 
    the image for debugging.

    :param image: The image to draw the lines on
    :param lines: The lines to draw on the image
    '''
    color = kwargs.get('color', Color.Green)
    width = kwargs.get('width', 2)

    for x1, y1, x2, y2 in lines:
        cv2.line(image, (x1, y1), (x2, y2), color, width)

def get_image_corners(image):
    ''' Given an image, retrieve all the corners of the image.

    To debug the result of this operation::

        red_color = (0, 0, 255)
        threshold = 0.01
        corners   = get_image_corners(image)
        corners   = cv2.dilate(corners, None)
        image[corners > threshold * corners.max()] = red_color
        cv2.imshow('corners', image)

    This algorithm has a number of configuration parameters:

    * cornerHarris(image, _, _, _)

    :param image: The image to retrieve the corners of
    :returns: The corners of the supplied image
    '''
    im_base = open_if_path(image)
    im_gray = np.float32(im_base)
    im_corn = cv2.cornerHarris(im_gray, 2, 3, 0.04)
    return im_corn

def get_image_contours(image):
    ''' Given an image, find all the contours that exist.

    To debug the result of this operation::

        green    = (0, 255, 0)
        contours = get_image_contours(image)
        cv2.drawContours(image, contours, -1, green, 3)       # for all the contours
        cv2.drawContours(im_copy, [contours[i]], 0, green, 3) # for a single contour
        cv2.imshow('contours', image)

    This algorithm has a number of configuration parameters:

    * Canny(image, _, _, aperture-size)
    * findContours(image, _, _)

    :param image: The image to find contours of
    :returns: A collection of the image contours
    '''
    im_base = open_if_path(image)
    im_cann = cv2.Canny(im_base, 100, 100, apertureSize=3)
    contours, hierarchy = cv2.findContours(im_cann, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    return contours

def get_biggest_rectangle(image):
    ''' Given a threshold image, retrieve all the existing
    contours and attempt to find the biggest rectangle in 
    the image.

    To debug the result of this call::

        green_color = (0, 255, 0)
        x, y, w, h = get_biggest_rectangle(image)
        cv2.rectangle(image, (x, y), (x + w, y + h), green_color, 2)
        cv2.imshow('rectangle', image)

    :param image: The threshold image to retrieve a rectangle from
    :returns: The largest rectangle in the image
    '''
    contours = get_image_contours(image)
    biggest  = (0, (None,))

    for i in range(len(contours)):
        rectangle  = cv2.approxPolyDP(contours[i], 3, True)
        x, y, w, h = cv2.boundingRect(rectangle)
        biggest = max((w * h, (x, y, w, h)), biggest)
    return biggest[1]

def get_image_crop(image, rectangle):
    ''' Given an image and a rectangle of the form
    (x, y, w, h), return the cropped rectangle portion of
    the supplied image.

    :param image: The image to crop
    :param rectangle: The portion of the image to crop
    :returns: The cropped portion of the image
    '''
    x, y, w, h = rectangle
    im_crop = image[y:y+h, x:x+w]
    return im_crop

#------------------------------------------------------------
# evaluation methods
#------------------------------------------------------------

def score_crop(rect1, rect2):
    '''
    score = inter(r1, r2) / union(r1, r2)
    score = area(inter(r1, r2)) / (area(r1) + area(r2))
    '''
    x11, y11, x12, y12 = rect1
    x21, y21, x22, y22 = rect2

    area1 = (x12 - x11) * (y12 - y11)
    area2 = (x22 - x21) * (y22 - y21)

    xover = min(x12, x22) - max(x11, x21)
    yover = min(y12, y22) - max(y11, y21)
    areao = xover * yover * 1.0

    return min(1.0, areao / (area1 + area2))

def validate_data_set(images):
    '''
    '''
    scores = 0.0
    count  = len(images['Path'])

    for index, path in images['Path'].items():
        path = os.path.join('images', path)
        im_base = open_if_path(path)
        im_gray = generate_pick_mask(im_base)
        rect1 = images['Crop'][index]
        rect2 = get_biggest_rectangle(im_gray)
        score = score_crop(rect1, rect2)
        scores += score
        print "[%f]\t%s" % (score, path)
    print "total score: %f of %f" % (scores, count * 1.0)

#------------------------------------------------------------
# main
#------------------------------------------------------------

if __name__ == "__main__":
    images = json.load(open(sys.argv[1]))
    validate_data_set(images)
