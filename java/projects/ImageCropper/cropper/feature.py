import cv2
import numpy as np

from .utility import *

#--------------------------------------------------------------------------------
# Context Classes
#--------------------------------------------------------------------------------

class CropHistory(object):
    ''' Represents a single cropped region reading
    along with some additional data about that reading.
    '''

    __slots__ = ['region', 'base_color', 'edge_color']

    def __init__(self, **kwargs):
        self.region     = kwargs.get('region')
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
        self.index      = kwargs.get('index')
        self.image      = kwargs.get('image')
        self.region     = kwargs.get('region')
        self.edge_width = kwargs.get('edge_width', 5)
        self.history    = kwargs.get('history', [])

    @property
    def region_edges(self):
        return get_image_crop_edges(self.image, self.region, self.edge_width)

    @property
    def cropped_region(self):
        return get_image_crop(self.image, self.region)

    @property
    def cropped_region(self):
        return get_image_crop(self.image, self.region)

    @property
    def region_dimensions(self):
        return self.region[2:]

    @property
    def image_dimensions(self):
        return self.image.shape[:2]

    @property
    def historic_average_region(self):
        count   = float(len(self.history))
        regions = [h.region for h in self.history]
        values  = [int(sum(group) / count + 0.5) for group in zip(*regions)]
        return values

#--------------------------------------------------------------------------------
# Image Feature Collection
#--------------------------------------------------------------------------------

FEATURES = [
    region_area_history,
    region_area_ratio,
    region_aspect_ratio,
    #region_skew,
    region_centrality,
    edge_color_average,
    region_color_average,
    region_pixel_intensity,
    edge_color_intensity,
]

#--------------------------------------------------------------------------------
# Image Features
#--------------------------------------------------------------------------------
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
    #tl, tr, bl, br = kwargs.get('region')
    return 0

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

#--------------------------------------------------------------------------------
# Image Feature Computing
#--------------------------------------------------------------------------------

def apply_features_to_contexts(contexts, features=FEATURES):
    ''' Given a collection of contexts, apply the features
    supplied against the contexts.

    :param contexts: A collection of contexts to apply
    :param features: The features to apply to the context
    :returns: A generator around the results
    '''
    for context in contexts:
        yield [(context.index, feature(context)) for features in features]

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

    with open('feature.results.json', 'w') as handle:
        #for index, values in features:
        json.dump(list(features), handle) 

