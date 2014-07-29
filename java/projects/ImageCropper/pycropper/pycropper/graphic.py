#!/usr/bin/env python
import os
import sys
import json
import cv2
import numpy as np
import pandas as pd
from collections import defaultdict

#------------------------------------------------------------
# logging
#------------------------------------------------------------
import logging
_logger = logging.getLogger(__name__)

#------------------------------------------------------------
# Rectangle Selectors
#------------------------------------------------------------

class PointSelector(object):
    ''' A rectangle selector that works by selecting the
    four corner points of the rectangle. This allows for
    warped rectangles (skew, affine, etc).
    '''

    def __init__(self, path, **kwargs):
        ''' Initialize a new PointSelector

        :param path: The path to the image to select from
        :param resize: True to resize the image, False for no resize
        :param initial: An initial selection to display
        '''
        self.window    = "image-window"
        self.image     = cv2.imread(path)
        if kwargs.get('resize', False):
            self.image = cv2.resize(self.image, (0,0), fx=0.5, fy=0.5) 
        self.points    = kwargs.get('initial', [])

        self.update_image()
        cv2.setMouseCallback(self.window, self.on_mouse)

    def get_result(self):
        ''' Gets the selected result

        :returns: The selected region
        '''
        return self.points

    def on_mouse(self, event, x, y, flags, param):
        ''' The callback for the on_mouse movement.
        '''
        x, y = np.int16([x, y]) # cast to int

        if event == cv2.EVENT_LBUTTONDOWN:
            self.points.append((x, y))
            self.update_image()

        if event == cv2.EVENT_RBUTTONDOWN:
            self.points = []
            self.update_image()

    def update_image(self):
        ''' Given an image, draw the currently selected
        rectangle on that image.

        :param image: The image to draw the current selection on
        :returns: True if we drew an image, False otherwise
        '''
        image = self.image.copy()
        green = (0, 255, 0)

        for idx in range(len(self.points)):
            cv2.circle(image, self.points[idx], 3, green, thickness=-1)
            if idx > 0:
                cv2.line(image, self.points[idx - 1], self.points[idx], green, 1)

        if len(self.points) > 2:
            cv2.line(image, self.points[0], self.points[-1], green, 1)

        cv2.imshow(self.window, image)

class RectangleSelector(object):

    def __init__(self, path, **kwargs):
        ''' Initialize a new RectangleSelector

        :param path: The path to the image to select from
        :param resize: True to resize the image, False for no resize
        :param initial: An initial selection to display
        '''
        self.window     = "image-window"
        self.image      = cv2.imread(path)
        if kwargs.get('resize', False):
            self.image  = cv2.resize(self.image, (0,0), fx=0.5, fy=0.5) 
        self.is_drawing = False
        self.rectangle  = kwargs.get('initial', None)

        self.update_image()
        cv2.setMouseCallback(self.window, self.on_mouse)

    def get_result(self):
        ''' Gets the selected result

        :returns: The selected region
        '''
        return self.rectangle

    def on_mouse(self, event, x, y, flags, param):
        ''' The callback for the on_mouse movement.
        '''
        x, y = np.int16([x, y]) # cast to int

        if event == cv2.EVENT_LBUTTONDOWN:
            self.is_drawing = True
            self.rectangle  = (x, y, x, y)

        elif event == cv2.EVENT_LBUTTONUP:
            self.is_drawing = False

        if self.is_drawing:
            xc, yc, w, h = self.rectangle
            x0, y0 = np.minimum([xc, yc], [x, y])
            x1, y1 = np.maximum([xc, yc], [x, y])
            self.rectangle = (x0, y0, x1, y1)
            self.update_image()

    def update_image(self):
        ''' Given an image, draw the currently selected
        rectangle on that image.

        :param image: The image to draw the current selection on
        :returns: True if we drew an image, False otherwise
        '''
        image = self.image.copy()
        green = (0, 255, 0)

        if self.rectangle:
            x0, y0, x1, y1 = self.rectangle
            cv2.rectangle(image, (x0, y0), (x1, y1), green, 2)
        cv2.imshow(self.window, image)

def select_rectangle(path, **kwargs):
    ''' Given an image path, run the rectangle
    selector GUI for that image.

    :param path: The image to select a rectangle for
    :returns: The selected rectangle
    '''
    method = kwargs.get('method', RectangleSelector)
    select = method(path, **kwargs)
    _logger.debug("selecting rectangle for %s", path)

    while True:
        key = cv2.waitKey(5)
        if key == ord(' '):
            break;

    cv2.destroyAllWindows()
    return select.get_result()
