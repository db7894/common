#!/usr/bin/env python
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
    LowerThreshold = ( 90,  50,  50)
    UpperThreshold = (115, 255, 255)
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
    is_hsv  = kwargs.get('hsv', False)
    im_base = cv2.imread(path) if isinstance(path, unicode) else path
    if is_gray: return cv2.cvtColor(im_base, cv2.COLOR_BGR2GRAY)
    if is_hsv:  return cv2.cvtColor(im_base, cv2.COLOR_BGR2HSV)
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
    contours, hierarchy = cv2.findContours(im_cann, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    return contours

def get_biggest_rectangle(image, **kwargs):
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
        #epsilon    = cv2.arcLength(contours[i], True) * 0.1
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

def show_image_crop(name, image, rectangle):
    ''' Given an image and a rectangle of the form
    (x, y, w, h), crop that image and show the resulting
    crop on screen.

    :param name: The name of the image to display
    :param image: The image to crop
    :param rectangle: The portion of the image to crop
    :returns: The cropped portion of the image
    '''
    crop = get_image_crop(image, rectangle)
    cv2.imshow(name, crop)

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
