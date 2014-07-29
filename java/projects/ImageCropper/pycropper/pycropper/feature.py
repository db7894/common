import cv2, cv
import numpy as np

from .utility import *

#------------------------------------------------------------
# logging
#------------------------------------------------------------

import logging
_logger = logging.getLogger(__name__)

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

def contour_index(context):
    ''' Feature that computes a unique index for this
    contour for the specific image.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    return context.index

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

def contour_width_ratio(context):
    ''' Feature that computes the ratio of the contour
    width that we are bouding with a cropping rectangle.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    if not context.rectangler:
        return 0 # no penalty if no rotated rectangle

    (x, y), (w, h), theta = context.rectangler
    width, height = context.image_dimensions
    if width == 0: return 0
    return float(w) / width

def contour_height_ratio(context):
    ''' Feature that computes the ratio of the contour
    height that we are bouding with a cropping rectangle.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    if not context.rectangler:
        return 0 # no penalty if no rotated rectangle

    (x, y), (w, h), theta = context.rectangler
    width, height = context.image_dimensions
    if height == 0: return 0
    return float(h) / height

def contour_aspect_ratio(context):
    ''' Feature that computes the aspect ratio of the
    contour that we are bouding with a cropping rectangle.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    if not context.rectangler:
        return 0 # no penalty if no rotated rectangle

    (x, y), (width, height), theta = context.rectangler
    if height == 0: return 0
    return float(width) / float(height)

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

def contour_moments(context):
    ''' Feature that computes the first moments
    of the supplied contour.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    # cx = int(moments['m10']/moments['m00'])
    # cy = int(moments['m01']/moments['m00'])
    # moment_area = moments['m00']
    return cv2.moments(context.contour)

def contour_hu_moments(context):
    ''' Feature that computes the seven hu moments
    of the supplied contour.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    moments = cv2.moments(context.contour)
    return cv2.HuMoments(moments)

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
    if height == 0: return 0
    return float(width) / float(height)

def region_skew(context):
    ''' Feature that computes the amount of skew
    in the detected region of interest.

    :param context: The context to calculate with
    :returns: The score for this feature
    '''
    if not context.rectangler:
        return 0

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
    contour_index,
    contour_area,
    contour_area_ratio,
    contour_aspect_ratio,
    contour_centrality,
    contour_width_ratio,
    contour_height_ratio,
    contour_blue_count,
    contour_white_count,
    contour_moments,
    contour_hu_moments,
]
