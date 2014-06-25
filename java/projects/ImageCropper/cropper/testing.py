#!/usr/bin/env python
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

from .utility import *

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

def validate_data_set(images):
    '''
    '''
    scores = 0.0
    count  = len(images['Path'])
    total  = count * 1.0
    zeros  = 0

    for index, path in images['Path'].items():
        path = os.path.join('images', path)
        im_base = open_if_path(path)
        im_gray = generate_pick_mask(im_base)
        rect1 = images['Crop'][index]
        rect2 = get_biggest_rectangle(im_gray, prev=rect1)
        score = score_crop(rect1, rect2)
        scores += score if score <= 0.85 else 1.0
        if score == 0.0: zeros += 1
        #print "%f\t%s\t%s" % (score, path, rect2)
        #show_image_crop(path, im_base, rect2)
    print "total score: %d / %d = %f : failures[%d]" % (scores, total, scores / total, zeros)

def create_image_histogram(images):
    '''
    '''
    channels = [0, 1]
    sizes    = [180, 256]
    ranges   = [0, 180, 0, 256]
    im_crops = []
    im_hist  = np.zeros(sizes)

    for index, path in images['Path'].items():
        rect = images['Crop'][index]
        path = os.path.join('images', path)
        im_base = open_if_path(path, hsv=True)
        im_crop = get_image_crop(im_base, rect)
        im_crops.append(im_crop)
    im_hist = cv2.calcHist(im_crops, channels, None, sizes, ranges)
    import ipdb;ipdb.set_trace()
    cv2.normalize(im_hist, im_hist, 0, 255, cv2.NORM_MINMAX)

def create_image_backprops(images):
    '''
    '''
    kernel = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (5, 5))
    for index, path in images['Path'].items():
        rect = images['Crop'][index]
        path = os.path.join('images', path)
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

#------------------------------------------------------------
# main
#------------------------------------------------------------

if __name__ == "__main__":
    images = json.load(open(sys.argv[1]))
    #create_image_histogram(images)
    validate_data_set(images)
