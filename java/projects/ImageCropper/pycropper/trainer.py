#!/usr/bin/env python
import os
import sys
import json
import cv2
from collections import defaultdict
from optparse import OptionParser

from .graphic import select_rectangle
from .conversion import width_to_point2

#------------------------------------------------------------
# logging
#------------------------------------------------------------
import logging
logger = logging.getLogger(__name__)

#------------------------------------------------------------
# utilities
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

def view_training_data(images, **kwargs):
    ''' Given an image training set, run through the
    training set and validate that everything looks good.

    :param images: The image dataset to validate
    :param path: An optional path to append to each image's path
    :param resize: A flag indicating if the image is large
    :returns: The updated image
    '''
    resize  = kwargs.get('resize', False)
    pathdir = kwargs.get('path', '')

    for index, path in images['Path'].items():
        path = os.path.join(pathdir, path)
        rect = images['Crop'][index]
        rect = width_to_point2(rect)
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

def create_average_data_set(images, **kwargs):
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
    resize  = kwargs.get('resize', False)
    count   = kwargs.get('count', 4)
    pathdir = kwargs.get('path', '')

    for index, path in images['Path'].items():
        house  = images['Camera'][index]
        camera = images['Camera'][index]
        camera = house + "." + camera

        if images['Crop'].get(index, None):
            continue # already have cropped data
        elif camera not in average:
            path = os.path.join(pathdir, path)
            rect = select_rectangle(path, resize=resize)
            if resize: rect = map(lambda z: z * 2, rect)
            updates[camera].append(rect)
        waiting.append((index, camera))

        if len(updates[camera]) == count:
            average[camera] = average_tuples(updates[camera])
            logger.debug("average {}\t{}".format(camera, average[camera]))

    for index, camera in waiting:
        if camera not in average:
            average[camera] = average_tuples(updates[camera])
        logger.debug("average {}\t{}".format(camera, average[camera]))
        images['Crop'][index] = average[camera]

    return images

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

    parser.add_option("-v", "--view",
        help="To view a dataset that has been trained",
        action="store_true", dest="view", default=True)

    parser.add_option("-t", "--train",
        help="To train a dataset again",
        action="store_true", dest="train", default=True)

    parser.add_option("-i", "--input",
        help="The input database to operate with",
        dest="database", default='')

    (opt, arg) = parser.parse_args()
    return opt

def testingxxx(**kwargs):
    print kwargs

if __name__ == "__main__":
    option = get_options()

    if option.debug:
        logging.basicConfig(level=logging.DEBUG)

    database = json.load(open(option.database))
    output   = option.database + ".trained"

    if option.train:
        create_average_data_set(database, *option)
        json.dump(database, open(output, 'w'))
    elif option.view:
        view_training_data(database, *option)
