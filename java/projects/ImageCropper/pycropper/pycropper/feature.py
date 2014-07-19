import sys
import cv2, cv
import numpy as np
import pandas as pd

from .utility import *

#------------------------------------------------------------
# Context Classes
#------------------------------------------------------------

class CropHistory(object):
    ''' Represents a single cropped region reading
    along with some additional data about that reading.

    .. todo:: Weight the reading based on its score.
    '''

    __slots__ = ['region', 'score', 'base_color', 'edge_color']

    def __init__(self, **kwargs):
        self.region     = kwargs.get('region')
        self.score      = kwargs.get('score', 1.0)
        self.base_color = kwargs.get('base_color')
        self.edge_color = kwargs.get('edge_color')

class FeatureContext(object):
    ''' The context of data to apply the feature
    calculations to. All data transformations can be
    supplied by properties on the context as they are
    more than likely to be used again.
    '''

    def __init__(self, **kwargs):
        ''' Initialize a new instance of the FeatureContext

        :param image: The original image in HSV format
        :param region: The region of interest in (x, y, w, h) format
        '''
        self.image      = kwargs.get('image')
        self.index      = kwargs.get('index', None)
        self.contour    = kwargs.get('contour', None)
        self.perimiter  = kwargs.get('perimiter', None)
        self.rectangle2 = kwargs.get('rectangle2', None)
        self.rectangler = kwargs.get('rectangler', None)
        self.rectangle4 = kwargs.get('rectangle4', None)
        self.edge_width = kwargs.get('edge_width', 5)
        self.history    = kwargs.get('history', [])

    @property
    def region(self):
        ''' The captured region of interest
        '''
        return self.rectangle2 or self.rectangle4

    @property
    def region_edges(self):
        ''' The pixels of the edges of the region of
        interest
        '''
        return get_image_crop_edges(self.image, self.region, self.edge_width)

    @property
    def cropped_region(self):
        ''' The pixels of the region of interest from
        the main image.
        '''
        return get_image_crop(self.image, self.region)

    @property
    def region_dimensions(self):
        ''' The dimesions (w, h) of the region of interest
        '''
        return np.array(self.region[2:])

    @property
    def image_dimensions(self):
        ''' The dimesions (w, h) of the main image
        '''
        return np.array(self.image.shape[:2])

    @property
    def image_center(self):
        ''' The center point of the main image
        '''
        size = self.image_dimensions
        return np.ceil(size).astype(int)

    @property
    def historic_average_region(self):
        ''' The average of the region of interest from
        the supplied history.
        '''
        count   = float(len(self.history))
        regions = [h.region for h in self.history]
        values  = (np.sum(regions, axis=0) / count).astype(int)
        return values

    @property
    def historic_average_region_center(self):
        ''' The average of the region of interest from
        the supplied history.
        '''
        region = self.historic_average_region
        return np.ceil(np.array(region)).astype(int)

#------------------------------------------------------------
# Image Features
#------------------------------------------------------------
def contour_white_count(context):
    ''' Feature that computes the count of blue
    pixels in the specified contour area (where
    blue is a threshold range).

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    crop = context.cropped_region
    low  = (  0,   0, 180)
    high = (255,  50, 255)
    return count_pixels_in_range(crop, low, high)

def contour_blue_count(context):
    ''' Feature that computes the count of blue
    pixels in the specified contour area (where
    blue is a threshold range).

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    crop = context.cropped_region
    low  = ( 90,  50,  50)
    high = (125, 255, 255)
    return count_pixels_in_range(crop, low, high)

def contour_area(context):
    ''' Feature that computes the area of the contour
    that we are bouding with a cropping rectangle.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    if not context.rectangler:
        return 0 # no penalty if no rotated rectangle

    return cv2.contourArea(context.contour)

def contour_area_ratio(context):
    ''' Feature that computes the area of the contour
    as a ratio of the overall image.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    if not context.rectangler:
        return 0 # no penalty if no rotated rectangle

    width, height = context.image_dimensions
    area = cv2.contourArea(context.contour)
    return area / float(width * height)

def contour_centrality(context):
    ''' Feature that computes the centrality
    of a the contour to the supplied image.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    if not context.rectangler:
        return 0 # no penalty if no rotated rectangle

    (x, y), (w, h), theta = context.rectangler
    size = context.image_dimensions
    diag = np.linalg.norm(size)
    rect = np.array((w, h)) / 2.0 + np.array((x, y))
    cent = size / 2.0
    dist = np.linalg.norm(rect - cent)
    return dist / diag

def region_area_history(context):
    ''' Feature that computes the accuracy of the
    current region reading versus a previous historical
    reading.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    if not context.history:
        return 1.0 # no penalty if we have no past data

    x1, y1, w1, h1 = context.region                           # the current reading
    x2, y2, w2, h2 = context.historic_average_region          # the past region average
    area1     = w1 * h1
    area2     = w2 * h2
    x_overlap = max(0.0, min(x1 + w1, x2 + w2) - max(x1, x2)) # x-axis overlap width
    y_overlap = max(0.0, min(y1 + h1, y2 + h2) - max(y1, y2)) # y-axis overlap width
    overlap   = (x_overlap * y_overlap * 1.0)                 # area of the overlapped region
    total     = area1 + area2 - overlap                       # remove double added overlap
    return min(1.0, overlap / total)                          # make sure we contribute max of 1.0

def region_area(context):
    ''' Feature that computes the area of the region
    of interest.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    width, height = context.region_dimensions
    return width * height

def region_area_ratio(context):
    ''' Feature that computes the ratio of the area
    of the region of interest versus the area of the
    total image.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    width1, height1 = context.region_dimensions
    width2, height2 = context.image_dimensions
    return float(width1 * height1) / float(width2 * height2)

def region_aspect_ratio(context):
    ''' Feature that computes the aspect ratio
    of the region of interest.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    width, height = context.region_dimensions
    return float(width) / float(height)

def region_skew(context):
    ''' Feature that computes the amount of skew
    in the detected region of interest.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    if not context.rectangler: return 0
    (cx, cy), (w, h), theta = context.rectangler
    return theta + 180 if w < h else theta + 90

def region_centrality(context):
    ''' Feature that computes the centrality
    of a region of interest to the supplied image.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    x, y, w, h = context.region
    size = context.image_dimensions
    diag = np.linalg.norm(size)
    rect = np.array((w, h)) / 2.0 + np.array((x, y))
    cent = size / 2.0
    dist = np.linalg.norm(rect - cent)
    return dist / diag

def edge_color_average(context):
    ''' Feature that computes the average
    edge color of the region of interest.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    edges = context.region_edges
    (mean, stds) = cv2.meanStdDev(edges)
    return np.concatenate([mean, stds]).flatten()

def region_color_average(context):
    ''' Feature that computes the average
    color of the region of interest.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    crop = context.cropped_region
    (mean, stds) = cv2.meanStdDev(crop)
    return np.concatenate([mean, stds]).flatten()

def region_pixel_intensity(context):
    ''' Feature that calculates the average
    pixel intensity of a region of interest.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    crop = context.cropped_region
    return crop.mean()

def edge_color_intensity(context):
    ''' Feature that computes the average
    pixel intensisty of the edge the region
    of interest.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    edges = context.region_edges
    return edges.mean()

#------------------------------------------------------------
# Image Feature Collection
#------------------------------------------------------------

FEATURES = [
    region_area_history,
    region_area,
    region_area_ratio,
    region_aspect_ratio,
    region_skew,
    region_centrality,
    region_color_average,
    region_pixel_intensity,
    edge_color_average,
    edge_color_intensity,
    contour_area,
    contour_area_ratio,
    contour_centrality,
    contour_blue_count,
    contour_white_count,
    # TODO HuMoments
    # TODO contour_aspect_ratio
]

#------------------------------------------------------------
# Image Feature Computing
#------------------------------------------------------------

def apply_features_to_contexts(contexts, features=FEATURES):
    ''' Given a collection of contexts, apply the features
    supplied against the contexts.

    :param contexts: A collection of contexts to apply
    :param features: The features to apply to the context
    :returns: A generator around the results (context, { name : value })
    '''
    for context in contexts:
        yield (context, { f.func_name : f(context) for f in features })

def apply_features_to_context(context, features=FEATURES):
    ''' Given a context, apply the features supplied against
    the context.

    :param context: A single context to apply
    :param features: The features to apply to the context
    :returns: The resulting features { name : value }
    '''
    return { f.func_name : f(context) for f in features }

def contour_to_context(image, contour, **kwargs):
    ''' Given a single contour for a given image, compute
    the remaining neccessary features to be used to create
    a feature context.

    :param image: The image to create the context with
    :param contour: The contour to initialize features with
    :returns: A populated FeatureContext
    '''
    perimiter = cv2.arcLength(contour, True)
    kwargs.update({
        'image'      : image,
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
    contexts = [contour_to_context(image, c) for c in contours]
    return list(apply_features_to_contexts(contexts))

def database_to_contexts(database):
    ''' A generator to convert an input image collection
    database to a FeatureContext container.

    :param database: The images database to convert
    :returns: A generator around the conversion
    '''
    for index in database['Path']:
        params = {
            'region': database['Crop'][index],
            'image' : open_if_path(database['Path'][index], hsv=True),
            'index' : index,
        }
        yield FeatureContext(**params)
 
def apply_features(database):
    ''' Given a database of images, apply the current
    set of feature descriptors to the images and store
    the results.

    :param database: The database to run the features against
    '''
    contexts = database_to_contexts(database)
    features = apply_features_to_contexts(contexts)
    return features

def apply_crop_rules(results):
    ''' Given the feature results for a collection of contours
    for a single image, apply the learned ruleset to select the
    best contour for the given image.

    :param results: A collection of [(context, { name : value })]
    :returns: The resulting selection with the given confidence
    '''
    # TODO learn these rules with a large dataset, then svm or
    # linear regression
    A = pd.DataFrame([x for _,x in results])
    B = A[A.contour_area > A.contour_area.max()               * 0.10]
    C = B[B.contour_blue_count > A.contour_blue_count.max()   * 0.10]
    D = C[C.contour_white_count > A.contour_white_count.max() * 0.01]
    E = D[D.contour_centrality < A.contour_centrality.max()   * 0.50]
    F = E[E.region_aspect_ratio < A.region_aspect_ratio.max() * 0.10]
    return A,B,C,D,E,F

#------------------------------------------------------------
# Main
#------------------------------------------------------------
if __name__ == '__main__':
    if len(sys.argv) <= 1:
        print "%s <database.json>" % sys.argv[0]
        sys.exit()

    database = json.load(open(sys.argv[1], 'r'))
    features = apply_features(database)

    with open(sys.argv[1] + '.features', 'w') as handle:
        json.dump(list(features), handle) 
