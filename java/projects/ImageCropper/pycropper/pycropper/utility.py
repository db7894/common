import os
import cv2, cv
import numpy as np
import urllib2

#------------------------------------------------------------
# logging
#------------------------------------------------------------

import logging
_logger = logging.getLogger(__name__)

#------------------------------------------------------------
# utilities 
#------------------------------------------------------------

def random_color(size=3):
    ''' Return a random color from the supplied range.

    :param size: The depth of the color
    :returns: A new random color
    '''
    return np.random.randint(0, 255, size).tolist()

def resize_image(image, ratio):
    ''' Given an image, resize it by the supplied rate.

    :param image: The image to resize
    :param ratio: The ratio to resize the image by
    :returns: The resized image
    '''
    if not image: return None # assuming PIL
    return image.resize([int(s * ratio) for s in image.size])

def open_if_path(path, **kwargs):
    ''' Given an object, return it if it is an image or create
    an image if it is a path to an image.

    :param path: The path to an image or an image
    :returns: A new numpy image
    '''
    if not isinstance(path, unicode):
        return path # already opened

    _logger.debug("opening image at: %s", path)
    is_gray = kwargs.get('gray', False)
    is_hsv  = kwargs.get('hsv', False)
    im_base = cv2.imread(path)
    if is_gray: return cv2.cvtColor(im_base, cv2.COLOR_BGR2GRAY)
    if is_hsv:  return cv2.cvtColor(im_base, cv2.COLOR_BGR2HSV)
    return im_base

def open_images(paths, **kwargs):
    ''' Given a collection of paths, open all the paths
    and return an image handle to that image.

    :param paths: The collection of paths to open [(index, path)]
    :param root: An optional path to append to each image's path
    :returns: A generator of (index, image)
    '''
    pathdir = kwargs.get('root', '')

    for index, path in paths:
        path  = os.path.join(pathdir, path)
        image = open_if_path(path, **kwargs)
        yield (index, image)

def generate_pick_mask(image, **kwargs):
    ''' Given a path or image, generate a mask for the
    taped off space in the picking region.
    
    :param image: The image or path to generate a mask for
    :returns: The mask of the supplied image
    '''
    lower_threshold = kwargs.get('color_low_threshold', ( 90,  50,  50))
    upper_threshold = kwargs.get('color_top_threshold', (125, 255, 255))
    opening_kernel  = kwargs.get('opening_kernel', np.ones((5, 5), np.uint8))
    should_blur     = kwargs.get('should_blur', True)

    im_base = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)
    im_near = cv2.inRange(im_base, lower_threshold, upper_threshold)
    if should_blur:
        return cv2.morphologyEx(im_near, cv2.MORPH_OPEN, opening_kernel)
    return im_near

def get_image_lines(image, **kwargs):
    ''' Given an image, return a collection of discovered
    lines in that image.

    This algorithm has a number of configuration parameters:

    * Canny(image, _, _, aperture-size)
    * HoughLinesP(image, 1, cv.CV_PI/180, threshold, minimum-length, maximum-gap)

    :param image: The image to get the lines from
    :returns: A collection of the lines in the image
    '''
    blur_kernel   = kwargs.get('blur-kernel', np.ones((3, 3), np.uint8))
    cann_low_thr  = kwargs.get('canny-low-threshold', 100)
    cann_high_thr = kwargs.get('canny-high-threshold', 100)
    cann_size     = kwargs.get('canny-aperture-size', 3)
    hough_rho     = kwargs.get('hough-rho', 1)
    hough_theta   = kwargs.get('hough-theta', cv.CV_PI / 180)
    hough_thr     = kwargs.get('hough-max-threshold', 70)
    hough_len     = kwargs.get('hough-max-length', 30)
    hough_gap     = kwargs.get('hough-min-gap', 10)

    im_blur = cv2.blur(image, blur_kernel)
    im_cann = cv2.Canny(im_blur, cann_low_thr, cann_high_thr, apertureSize=cann_size)
    lines   = cv2.HoughLinesP(im_cann, hough_rho, hough_theta, hough_thr, hough_len, hough_gap)[0]
    return lines

def add_image_lines(image, lines, **kwargs):
    ''' Given an image, add the supplied lines to 
    the image for debugging.

    :param image: The image to draw the lines on
    :param lines: The lines to draw on the image
    '''
    color = kwargs.get('color', (0, 255, 0))
    width = kwargs.get('width', 2)

    for x1, y1, x2, y2 in lines:
        cv2.line(image, (x1, y1), (x2, y2), color, width)
    return image

def add_image_points(image, points, **kwargs):
    ''' Given an image, add the supplied lines to 
    the image for debugging.

    :param image: The image to draw the lines on
    :param lines: The lines to draw on the image
    '''
    color  = kwargs.get('color', (0, 0, 255))
    width  = kwargs.get('width', 2)
    radius = kwargs.get('radius', 3)

    for point in points:
        cv2.circle(image, tuple(point), radius, color, width)
    return image

def get_image_corners(image, **kwargs):
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
    im_gray = np.float32(image)
    im_corn = cv2.cornerHarris(im_gray, 2, 3, 0.04)
    return im_corn

def get_image_contours(image, **kwargs):
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
    cann_low_thr  = kwargs.get('canny-low-threshold', 100)
    cann_high_thr = kwargs.get('canny-high-threshold', 100)
    cann_size     = kwargs.get('canny-aperture-size', 3)
    blur_kernel   = kwargs.get('blur-kernel', (3, 3))

    im_gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    im_blur = cv2.blur(im_gray, blur_kernel)
    im_cann = cv2.Canny(im_blur, cann_low_thr, cann_high_thr, apertureSize=cann_size)
    contours, hierarchy = cv2.findContours(im_cann, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    return contours

def get_largest_contours(image, **kwargs):
    ''' Given an image, find the largest contour(s)
    that exist in the image.

    :param image: The image to find contours of
    :param count: The number of contours to retrieve
    :returns: A collection of the image contours
    '''
    count    = kwargs.get('count', 1)
    sortby   = kwargs.get('sortby', cv2.contourArea)
    contours = get_image_contours(image)
    contours = sorted(contours, key=sortby, reverse=True)[:count]  
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
        epsilon    = cv2.arcLength(contours[i], True) * 0.1
        rectangle  = cv2.approxPolyDP(contours[i], epsilon, True)
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

def get_image_crop_edges(image, rectangle, width=5):
    ''' Given an image and a rectangle of the form
    (x, y, w, h), return the edges of the cropped
    rectangle portion of the supplied image.

    :param image: The image to crop
    :param rectangle: The portion of the image to crop
    :param width: The width of the edges to include
    :returns: The combined edges of the resulting image
    '''
    x, y, w, h = rectangle
    edges = [
        image[y:y+h, x:x+width],
        image[y:y+h, x+w-width:x+w],
        image[y:y+width, x:x+w],
        image[y+h-width:y+h, x:x+w]
    ]
    return np.concatenate([e.flatten() for e in edges])

def show_image_crop(image, rectangle, name="cropped"):
    ''' Given an image and a rectangle of the form
    (x, y, w, h), crop that image and show the resulting
    crop on screen.

    :param image: The image to crop
    :param rectangle: The portion of the image to crop
    :param name: The name of the image to display
    :returns: The cropped portion of the image
    '''
    crop = get_image_crop(image, rectangle)
    cv2.imshow(name, crop)

def find_intersection(line1, line2):
    ''' Given two lines of the form (x1, y1, x2, y2),
    return the point of intersection or (None, None)

    :param line1: The first line to find an intersect
    :param line2: The second line to find an intersect
    :returns: The (x, y) intersect or (None, None) if none
    '''
    x1, y1, x2, y2 = line1
    x3, y3, x4, y4 = line2
    d = ((x1 - x2) * (y3 - y4)) - ((y1 - y2) * (x3 - x4))
    if d == 0: return None
    x = ((x1 * y2 - y1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * y4 - y3 * x4)) / d
    y = ((x1 * y2 - y1 * x2) * (y3 - y4) - (y1 - y2) * (x3 * y4 - y3 * x4)) / d
    return (x, y)

def find_all_intersections(lines):
    ''' Given a collection of lines, find all the
    intersecting points.

    :param lines: All the lines to find intersect points
    :returns: The intersecting points
    '''
    points = (find_intersection(l1, l2) for l1 in lines for l2 in lines)
    points = [point for point in points if point != None]
    return points

def get_polygon(points, **kwargs):
    ''' Given a collection of points, attempt to fit a polygon
    to the points and return those points.

    :param points: The points to fit a polygon around
    :returns: The bounding polygon
    '''
    curve     = np.array(points)
    perimiter = cv2.arcLength(curve, True) * 0.02
    polygon   = cv2.approxPolyDP(curve, perimiter, True)
    return polygon

def get_center_point(points):
    ''' Given a collection of points, find the center
    of mass for the collection.

    :param points: The points to find the center for
    :returns: The center of all the points
    '''
    center = np.array(points).sum(axis=0) * (1.0 / len(points))
    return tuple(center.astype(np.int))

def get_image_rectangle_tight(image, **kwargs):
    ''' Given an image, find the largest complete
    rectangle in the image and return its points as tight
    as possible to the resulting polygon.

    :param image: The image to get the rectangle for
    :returns: The points of the discovered rectangle
    '''
    im_gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    im_blur = cv2.blur(im_gray, (3, 3)) 
    im_cann = cv2.Canny(im_blur, 100, 250)
    #cv2.imshow('canny', im_cann)
    contours, hierarchy = cv2.findContours(im_cann, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)
    contour = contours[0]
    epsilon = cv2.arcLength(contour, True) * 0.1 
    points  = cv2.approxPolyDP(contour, epsilon, True)
    return [tuple(point[0]) for point in points]

def get_image_rectangle_fuzzy(image, **kwargs):
    ''' Given an image, find the largest complete
    rectangle in the image and return points around
    the rough area that includes it (may be larger
    than the rectangle).

    :param image: The image to get the rectangle for
    :returns: The points of the discovered rectangle
    '''
    best = (0, (None,))
    contours = get_image_contours(image, **kwargs)

    for contour in contours:
        area = cv2.contourArea(contour)
        rect = cv2.boundingRect(contour)
        best = max((area, rect), best)
    return best[1]

def get_warped_rectangle(image, rect, size):
    ''' Given an image and a discovered rectangle of
    (bl, tl, tr, br), extract the rectangle and project
    it to a normalized image of the supplied size.

    :param image: The image to extract the rectangle from
    :param rect: The rectangle to extract from the image
    :param size: The size to project the rectangle to
    :returns: The extracted and projected rectangle
    '''
    w, h  = size
    rect  = np.array(rect, np.float32)
    dest  = np.array([(0, h), (w, h), (w, 0), (0, 0)], np.float32)
    transform = cv2.getPerspectiveTransform(rect, dest)
    im_warp = cv2.warpPerspective(image, transform, (w, h)) 
    return im_warp

def count_pixels_in_range(image, low, high):
    ''' Given an image and a high and low color
    range, count all the pixels that fall between
    these ranges.

    :param image: The input image to count pixels for
    :param low: The low color threshold to count
    :param high: The high color threshold to count
    :returns: The number of pixels that were in range
    '''
    params = {
        'color_low_threshold': low,
        'color_top_threshold': high,
        'should_blur'        : False,
    }
    im_mask = generate_pick_mask(image, **params)
    return cv2.countNonZero(im_mask)
