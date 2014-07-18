'''
This module is used to test the results of a training session
against the current implementation of the algorithm.

'''
import sys
import os
import json
import numpy as np
import cv2
import cv
from matplotlib import pyplot as plt
from optparse import OptionParser

from .utility import *

#------------------------------------------------------------
# logging
#------------------------------------------------------------

import logging
_logger = logging.getLogger(__name__)

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
    LowerThreshold = ( 90,  50,  50)
    UpperThreshold = (115, 255, 255)
    MorphKernel    = np.ones((5, 5), np.uint8)

#------------------------------------------------------------
# evaluation methods
#------------------------------------------------------------

def score_crop(rect1, rect2):
    ''' Given two rectangles of the form (x, y, w, h),
    calculate a score of how close they are to each other::

        score = inter(r1, r2) / union(r1, r2)
        score = area(inter(r1, r2)) / (area(r1) + area(r2))

    :param rect1: The first rectangle to compare
    :param rect2: The second rectangle to compare
    :returns: A score between 0.0 (fail) and 1.0 (match)
    '''
    if rect2[0] == None or rect1[0] == None:
        return 0.0 # (None,) means no rectangle was found

    x1, y1, w1, h1 = rect1
    x2, y2, w2, h2 = rect2

    x_overlap = max(0.0, min(x1 + w1, x2 + w2) - max(x1, x2)) # x-axis overlap width
    y_overlap = max(0.0, min(y1 + h1, y2 + h2) - max(y1, y2)) # y-axis overlap width
    overlap   = (x_overlap * y_overlap * 1.0)                 # area of the overlapped region

    area1 = w1 * h1
    area2 = w2 * h2
    total = area1 + area2 - overlap                         # remove double added overlap

    return min(1.0, overlap / total)                        # make sure we contribute max of 1.0

def validate_data_set(images, **kwargs):
    '''
    '''
    pathdir = kwargs.get('path', '')
    debug   = kwargs.get('debug', False)
    scores  = 0.0
    count   = len(images['Path'])
    total   = count * 1.0
    zeros   = 0

    for index, path in images['Path'].items():
        path = os.path.join(pathdir, path)
        im_base = open_if_path(path)
        im_gray = generate_pick_mask(im_base)
        rect1 = images['Crop'][index]
        rect2 = get_biggest_rectangle(im_gray, prev=rect1)
        score = score_crop(rect1, rect2)
        scores += score if score <= 0.85 else 1.0
        if score == 0.0: zeros += 1
        if debug:
            log.debug("%f\t%s\t%s" % (score, path, rect2))
            show_image_crop(path, im_base, rect2)
    print "total score: %d / %d = %f : failures[%d]" % (scores, total, scores / total, zeros)

def create_image_histogram(images, **kwargs):
    '''
    '''
    pathdir  = kwargs.get('path', '')
    channels = [0, 1]
    sizes    = [180, 256]
    ranges   = [0, 180, 0, 256]
    im_crops = []
    im_hist  = np.zeros(sizes)

    for index, path in images['Path'].items():
        rect = images['Crop'][index]
        path = os.path.join(pathdir, path)
        im_base = open_if_path(path, hsv=True)
        im_crop = get_image_crop(im_base, rect)
        im_crops.append(im_crop)
    im_hist = cv2.calcHist(im_crops, channels, None, sizes, ranges)
    cv2.normalize(im_hist, im_hist, 0, 255, cv2.NORM_MINMAX)

def create_image_backprops(images, **kwargs):
    '''
    '''
    pathdir = kwargs.get('path', '')
    kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (5, 5))

    for index, path in images['Path'].items():
        rect = images['Crop'][index]
        path = os.path.join(pathdir, path)
        im_base = open_if_path(path, hsv=True)
        im_crop = get_image_crop(im_base, rect)

        histogram = cv2.calcHist([im_crop], [0, 1], None, [180, 256], [0, 180, 0, 256])
        cv2.normalize(histogram, histogram, 0, 255, cv2.NORM_MINMAX)
        backprop = cv2.calcBackProject([im_base], [0,1], histogram, [0, 180, 0, 256], 1)
        cv2.filter2D(backprop, -1, kernel, backprop)
        _, im_thresh = cv2.threshold(backprop, 50, 255, 0)

        im_mask  = cv2.merge((im_thresh, im_thresh, im_thresh))
        im_clean = cv2.bitwise_and(im_base, im_mask)
        examine = np.vstack([im_base, im_mask, im_clean])
        cv2.imshow(path, examine)

        while cv2.waitKey(5) != ord(' '):
            pass
    cv2.destroyAllWindows()

#---------------------------------------------------------------------------# 
# initialize our program settings
#---------------------------------------------------------------------------# 

def get_options():
    ''' A helper method to parse the command line options

    :returns: The options manager
    '''
    parser = OptionParser()

    parser.add_option("-p", "--path",
        help="The path to append to the files path",
        dest="path", default="")

    parser.add_option("-d", "--debug",
        help="Enable debug tracing",
        action="store_true", dest="debug", default=False)

    parser.add_option("-v", "--validate",
        help="Enable validating a test set",
        action="store_true", dest="validate", default=True)

    parser.add_option("-i", "--input",
        help="The input database to operate with",
        dest="database", default='')

    (opt, arg) = parser.parse_args()
    return opt

#------------------------------------------------------------
# main
#------------------------------------------------------------

def main():
    option = get_options()

    if option.debug:
        logging.basicConfig(level=logging.DEBUG)

    database = json.load(open(option.database))
    params   = { 'path': option.path }

    if option.validate:
        validate_data_set(images, **params)
    #create_image_histogram(images, **params)
