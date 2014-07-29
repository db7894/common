import sys
import time
import cv2, cv
import numpy as np
import pandas as pd
from optparse import OptionParser

from .utility import *
from .feature import FeatureContext, FEATURES

#------------------------------------------------------------
# logging
#------------------------------------------------------------

import logging
_logger = logging.getLogger(__name__)

#------------------------------------------------------------
# Image Feature Utilities
#------------------------------------------------------------

def apply_features_to_context(context, features=FEATURES):
    ''' Given a context, apply the features supplied against
    the context.

    :param context: A single context to apply
    :param features: The features to apply to the context
    :returns: The resulting features { name : value }
    '''
    return { f.func_name : f(context) for f in features }

def apply_features_to_contexts(contexts, features=FEATURES):
    ''' Given a collection of contexts, apply the features
    supplied against the contexts.

    :param contexts: A collection of contexts to apply
    :param features: The features to apply to the context
    :returns: A generator around the results (context, { name : value })
    '''
    for index, context in enumerate(contexts):
        yield (index, apply_features_to_context(context))

def contour_to_context(image, index, contour, **kwargs):
    ''' Given a single contour for a given image, compute
    the remaining neccessary features to be used to create
    a feature context.

    :param image: The image to create the context with
    :param index: The index of the contour in question
    :param contour: The contour to initialize features with
    :returns: A populated FeatureContext
    '''
    perimiter = cv2.arcLength(contour, True)
    kwargs.update({
        'image'      : image,
        'index'      : index,
        'contour'    : contour,
        'perimiter'  : perimiter,
        'rectangle2' : cv2.boundingRect(contour),
        'rectangler' : cv2.minAreaRect(contour),
        'rectangle4' : cv2.approxPolyDP(contour, perimiter * 0.1, True),
    })
    return FeatureContext(**kwargs)

def image_to_features(image):
    ''' Given an image, extract the contours and for
    each of them extract a feature set.

    :param image: The image to get the features for
    :returns: [(context, { name : value })]
    '''
    contours = get_image_contours(image)
    contexts = (contour_to_context(image, i, c) for i, c in enumerate(contours))
    return apply_features_to_contexts(contexts)

def images_to_features(images):
    ''' Given a collection of images, extract the contours
    and for each of them extract a feature set.

    :param images: The images to get the features for
    :returns: A generator of [(context, { name : value })]
    '''
    for image in images:
        yield image_to_features(image)

def features_to_dataframe(index, features):
    ''' Given a collection of features for the
    specified entry in the database, return the
    features as a pandas dataframe.

    :param index: The index to associate with the database
    :param features: The resulting features for the index
    :returns: A pandas dataframe for the features
    '''
    frame = pd.DataFrame([x for _, x in features])
    frame['label'] = 0
    frame['image_index'] = index
    #frame = frame.set_index('image_index', 'contour_index')
    # TODO flatten and delete features
    return frame

def database_to_features(database, **kwargs):
    ''' Given a pandas dataframe as a database,
    for each image in the database, compute the features
    and flatten them into a final dataframe that can be
    used to train on.

    :param database: The input database to operate with
    :returns: A full database of contours to train with
    '''
    final = pd.DataFrame()
    paths = database.Path.iteritems()
    stamp = time.clock()
    count = 0

    for index, image in open_images(paths, **kwargs):
        count, stamp, check = count + 1, time.clock(), stamp
        _logger.debug("working on[%d], count[%d], time[%f]", index, count, stamp - check)
        feats = image_to_features(image)
        frame = features_to_dataframe(index, feats)
        final = final.append(frame)
    return final

def apply_crop_rules(results):
    ''' Given the feature results for a collection of contours
    for a single image, apply the learned ruleset to select the
    best contour for the given image.

    :param results: A collection of [(context, { name : value })]
    :returns: The resulting selection with the given confidence
    '''
    # TODO learn these rules with a large dataset, then svm or
    # linear regression
    #A = pd.DataFrame([x for _,x in results])
    A = results
    B = A[A.contour_area > A.contour_area.max()               * 0.10]
    C = B[B.contour_blue_count > A.contour_blue_count.max()   * 0.10]
    D = C[C.contour_white_count > A.contour_white_count.max() * 0.01]
    E = D[D.contour_centrality < A.contour_centrality.max()   * 0.50]
    F = E[E.region_aspect_ratio < A.region_aspect_ratio.max() * 0.10]
    return A,B,C,D,E,F

#---------------------------------------------------------------------------# 
# initialize our program settings
#---------------------------------------------------------------------------# 

def _get_options():
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

    parser.add_option("-i", "--input",
        help="The input database to operate with",
        dest="database", default='')

    parser.add_option("-o", "--output",
        help="The output database to store features at",
        dest="output", default=None)

    (opt, arg) = parser.parse_args()
    return opt

#------------------------------------------------------------
# main
#------------------------------------------------------------

def main():
    ''' A main driver for collection features from a given
    database of images. The database is assumed to be a
    pandas dataframe that is serialized in json.
    '''
    option = _get_options()

    # global configuration
    params   = {
        'root': option.path or '',
        'hsv': True,
    }

    if option.debug:
        logging.basicConfig(level=logging.DEBUG)

    database = pd.read_json(option.database)
    features = database_to_features(database, **params)
    storage  = option.output or (option.database + ".features")
    features.to_json(storage)
