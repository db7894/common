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
logger = logging.getLogger(__name__)

#------------------------------------------------------------
# classes
#------------------------------------------------------------

class RectangleSelector:

    def __init__(self, path, **kwargs):
        ''' Initialize a new RectangleSelector

        :param path: The path to the image to select from
        '''
        self.window     = "image-window"
        self.image      = cv2.imread(path)
        if kwargs.get('resize', False):
            self.image  = cv2.resize(self.image, (0,0), fx=0.5, fy=0.5) 
        self.is_drawing = False
        self.rectangle  = kwargs.get('initial', None)
        cv2.imshow(self.window, self.image)
        cv2.setMouseCallback(self.window, self.on_mouse)
        self.update_image()

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
        if not self.rectangle:
            return False

        image = self.image.copy()
        x0, y0, x1, y1 = self.rectangle
        cv2.rectangle(image, (x0, y0), (x1, y1), (0, 255, 0), 2)
        cv2.imshow(self.window, image)

#------------------------------------------------------------
# helper methods
#------------------------------------------------------------

def crop_and_save(path, rectangle):
    ''' Given an image path and a rectangle, save the
    cropped rectangle on that image.

    :param path: The path to the image to crop
    :param rectangle: The rectangle of that image to crop
    :returns: The saved path name
    '''
    x0, y0, x1, y1 = rectangle
    name, form = path.rsplit('.', 1)
    im_base = cv2.imread(path)
    im_crop = im_base[y0:y1, x0:x1]
    path = "{}-crop.{}".format(name, form)
    cv2.imwrite(path, im_crop)
    return path

def select_rectangle(path, **kwargs):
    ''' Given an image path, run the rectangle
    selector GUI for that image.

    :param path: The image to select a rectangle for
    :returns: The selected rectangle
    '''
    select = RectangleSelector(path, **kwargs)

    while True:
        key = cv2.waitKey(5)
        if key == ord(' '):
            break;

    cv2.destroyAllWindows()
    return select.rectangle

#------------------------------------------------------------
# utilities
#------------------------------------------------------------

def point_to_width(rectangle):
    ''' Given a rectangle described by the upper
    left point and lower right point, convert it
    to a form described by the upper left point and
    a width and height.

    :param rectangle: The rectangle to convert
    :returns: The converted rectangle
    '''
    x1, y1, x2, y2 = rectangle
    return (x1, y1, x2 - x1, y2 - y1)

def width_to_point(rectangle):
    ''' Given a rectangle described by the upper
    left point and a width and height convert it
    to a form described by the upper left point
    and lower right point.

    :param rectangle: The rectangle to convert
    :returns: The converted rectangle
    '''
    x, y, w, h = rectangle
    return (x, y, x + w, y + h)

def average_tuples(tuples):
    ''' Given a list of tuples, return the average
    of the tuple elementwise.

    :param tuples: The list of equal sized tuples to average
    :returns: The average of the tuples
    '''
    count  = len(tuples) * 1.0
    totals =  map(sum, zip(*tuples))
    tuple(int(total / count) for total in totals)

#------------------------------------------------------------
# main methods
#------------------------------------------------------------

def view_training_data(images, resize=False):
    ''' Given an image training set, run through the
    training set and validate that everything looks good.

    :param images: The image dataset to validate
    :param resize: A flag indicating if the image is large
    :returns: The updated image
    '''
    for index, path in images['Path'].items():
        path = os.path.join('images', path)
        rect = images['Crop'][index]
        rect = width_to_point(rect)
        _ = select_rectangle(path, initial=rect, resize=resize)

def create_data_set(images, resize=False):
    ''' Given a training image set, generate new crop
    data for every image in the set.

    :param images: The image dataset to update
    :param resize: A flag indicating if the image is large
    :returns: The updated image
    '''
    if 'Crop' not in images:
        images['Crop'] = {}

    for index, path in images['Path'].items():
        path = os.path.join('images', path)
        rect = select_rectangle(path, resize=resize)
        if resize: rect = map(lambda z: z * 2, rect)
        images['Crop'][index] = rect
        print "{}\t{}".format(path, rect)
    return images

def create_average_data_set(images, resize=False, count=4):
    ''' Given a training image set, generate new crop
    data for every image in the set by averaging the
    values for N number of cameras.

    :param images: The image dataset to update
    :param resize: A flag indicating if the image is large
    :returns: The updated image
    '''
    updates = defaultdict(list)
    average = {} # the current average for a given camera
    waiting = [] # the current waiting list for averages

    for index, path in images['Path'].items():
        camera = images['Camera'][index]

        if index in images['Crop']:
            continue # already have cropped data
        elif camera not in average:
            path = os.path.join('images', path)
            rect = select_rectangle(path, resize=resize)
            if resize: rect = map(lambda z: z * 2, rect)
            updates[camera].append(rect)
        waiting.append((index, camera))

        if len(updates[camera]) == count:
            average[camera] = average_tuples(updates[camera])

    for index, camera in waiting:
        if camera not in average:
            average[camera] = average_tuples(updates[camera])
        logger.debug("average {}\t{}".format(camera, average[camera]))
        images['Crop'][index] = average[camera]

    return images

if __name__ == "__main__":
    path = sys.argv[2]
    data = json.load(open(sys.argv[2]))

    if sys.argv[1] == "train":
        create_average_data_set(data)
        json.dump(data, open(path + ".train", 'w'))
    elif sys.argv[1] == "view":
        view_training_data(data)
