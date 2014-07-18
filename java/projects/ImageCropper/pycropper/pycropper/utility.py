import os
import cv2, cv
import numpy as np
import urllib2

def download_image(path, post=None, root="downloads"):
    ''' Given a URL path to an image, download
    it to the supplied destination directory.

    :param path: The path to the image to download
    :param post: An operation to perform before saving (resize)
    :param root: The base output directory to save to
    :returns: The path to the downloaded image
    '''
    post = post or (lambda x: x)
    name = path.rsplit('/', 1)[-1].split('?')[0]
    name = os.path.join(root, name)
    http = urllib2.urlopen(path)
    with open(name, 'wb') as handle:
        handle.write(post(http.read()))
    return name

def download_images(paths, post=None, root="downloads"):
    ''' Given a collection of URL paths to images,
    download them all to the supplied destination
    directory.

    :param paths: The paths to the images to download
    :param post: An operation to perform before saving (resize)
    :param root: The base output directory to save to
    :returns: The paths to the downloaded images
    '''
    return [download_image(path, post, root) for path in paths]

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
    is_gray = kwargs.get('gray', False)
    is_hsv  = kwargs.get('hsv', False)
    im_base = cv2.imread(path) if isinstance(path, unicode) else path
    if is_gray: return cv2.cvtColor(im_base, cv2.COLOR_BGR2GRAY)
    if is_hsv:  return cv2.cvtColor(im_base, cv2.COLOR_BGR2HSV)
    return im_base

def generate_pick_mask(image, config):
    ''' Given a path or image, generate a mask for the
    taped off space in the picking region.
    
    :param image: The image or path to generate a mask for
    :param config: The configuration for the tuning parameters
    :returns: The mask of the supplied image
    '''
    im_base = open_if_path(image)
    im_base = cv2.cvtColor(im_base, cv2.COLOR_BGR2HSV)
    im_blue = cv2.inRange(im_base, config.LowerThreshold, config.UpperThreshold)
    im_open = cv2.morphologyEx(im_blue, cv2.MORPH_OPEN, config.MorphKernel)
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
    blur_kernel   = kwargs.get('blur-kernel', (3, 3))
    cann_low_thr  = kwargs.get('canny-low-threshold', 100)
    cann_high_thr = kwargs.get('canny-high-threshold', 100)
    cann_size     = kwargs.get('canny-aperture-size', 3)
    hough_rho     = kwargs.get('hough-rho', 1)
    hough_theta   = kwargs.get('hough-theta', cv.CV_PI / 180)
    hough_thr     = kwargs.get('hough-max-threshold', 70)
    hough_len     = kwargs.get('hough-max-length', 30)
    hough_gap     = kwargs.get('hough-min-gap', 10)

    im_base = open_if_path(image)
    im_blur = cv2.blur(im, blur_kernel)
    im_cann = cv2.Canny(im_blur, cann_low_thr, cann_high_thr, apertureSize=cann_size)
    lines   = cv2.HoughLinesP(im_cann, hough_rho, hough_theta, hough_thr, hough_len, hough_gap)[0]
    return lines

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
    return image

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

def get_image_rectangle(image):
    ''' Given an image, find the largest complete
    rectangle in the image and return its points.

    :param image: The image to get the rectangle for
    :returns: The points of the discovered rectangle
    '''
    im_gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    im_blur = cv2.blur(im_gray, (3, 3)) 
    im_cann = cv2.Canny(im_blur, 100, 250)
    #cv2.imshow('canny', im_cann)
    contours, hierarchy = cv2.findContours(im_cann, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_NONE)
    epsilon = cv2.arcLength(contours[0], True) * 0.1 
    points  = cv2.approxPolyDP(contours[0], epsilon, True)
    return [tuple(point[0]) for point in points]

def get_warped_rectangle(image, rect, size):
    w, h  = size
    rect  = np.array(rect, np.float32)
    dest  = np.array([(0, h), (w, h), (w, 0), (0, 0)], np.float32)
    transform = cv2.getPerspectiveTransform(rect, dest)
    warp = cv2.warpPerspective(image, transform, (w, h)) 
    return warp
