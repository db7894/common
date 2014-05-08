import glob
import numpy as np
import cv2
import cv
from matplotlib import pyplot as plt

def get_direction(line):
    x1, y1, x2, y2 = line
    return max((abs(x1 - x2), 0), (abs(y1 - y2), 1))[1]

def same_direction(l1, l2):
    return get_direction(l1) == get_direction(l2)

def get_intersection(l1, l2):
    if same_direction(l1, l2):
        return (-1, -1)

    xdiff = (l1[0] - l1[2], l2[0] - l2[2])
    ydiff = (l1[1] - l2[3], l2[1] - l2[3])

    def det(a, b):
        return a[0] * b[1] - a[1] * b[0]

    div = det(xdiff, ydiff)
    if div == 0: return (-1, -1)

    d = (det((l1[0], l1[1]), (l1[2], l1[3])), det((l2[0], l2[1]), (l2[2], l2[3])))
    x = det(d, xdiff) / div
    y = det(d, ydiff) / div
    return x, y

#------------------------------------------------------------
# Image Utilities
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
    LowerThreshold = (105,  50,  50)
    UpperThreshold = (115, 255, 255)
    #LowerThreshold = (100,  0,  0)
    #UpperThreshold = (255, 150, 75)
    MorphKernel    = np.ones((5, 5), np.uint8)

def open_if_path(path, **kwargs):
    ''' Given an object, return it if it is an image or create
    an image if it is a path to an image.

    :param path: The path to an image or an image
    :returns: A new numpy image
    '''
    is_gray = kwargs.get('gray', False)
    im_base = cv2.imread(path) if isinstance(path, str) else path
    if is_gray:
        return cv2.cvtColor(im_base, cv2.COLOR_BGR2GRAY)
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
    contours, hierarchy = cv2.findContours(im_cann, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
    return contours

def get_biggest_rectangle(image):
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

def stress_test():
    for path in glob.glob('*.jpeg'):
        print "processing: " + path
        im_base = open_if_path(path)
        im_gray = generate_pick_mask(im_base)
        #im_corn = get_image_corners(im_gray)
        #lines   = get_image_lines(im_gray)
        rectangle = get_biggest_rectangle(im_gray)
        im_crop = get_image_crop(im_base, rectangle)

        #x, y, w, h = rectangle
        #cv2.rectangle(im_base, (x, y), (x + w, y + h), (0, 255, 0), 2)
        #im_base[im_gray > 1] = (0, 255, 0)
        #im_base[im_corn > 0.01 * im_corn.max()] = [0, 0, 255]
        #add_image_lines(im_base, lines)
        cv2.imshow(path, im_crop)

#------------------------------------------------------------
# Image Cleanup
#------------------------------------------------------------
#lower_blue = (100,  0,  0)
#upper_blue = (255, 150, 75)
#kernel = np.ones((5, 5), np.uint8)
#
#im_root = cv2.imread('ABE2_Interior.Inbound.West_Receive.Station_F10_1397818231461_full.jpeg')
##im_root = cv2.imread('ABE2_Interior.Inbound.West_Receive.Station_F10_1397818870800_full.jpeg')
#im_copy = im_root.copy()
#im_gray = cv2.cvtColor(im_root, cv2.COLOR_BGR2GRAY)
#im_blue = cv2.inRange(im_root, lower_blue, upper_blue)
#im_blur = cv2.blur(im_blue, (5, 5)) 
##im_erod = cv2.erode(im_blue, kernel, iterations = 1)
#im_open = cv2.morphologyEx(im_blue, cv2.MORPH_OPEN, kernel)
#(thresh, im_mask) = cv2.threshold(im_gray, 128, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)
#
#cv2.imshow('threshold', im_blue)
##cv2.imshow('gaussian', im_blur)
#cv2.imshow('opening', im_open)
##cv2.imshow('erosion', im_erod)

#------------------------------------------------------------
# Hough Prob Line Detection
#------------------------------------------------------------
#im_cann = cv2.Canny(im_blur, 100, 100, apertureSize=3)
#lines = cv2.HoughLinesP(im_cann, 1, cv.CV_PI/180, 50, 70, 20)[0] # thresh, minlen, maxgap
#
#for x1, y1, x2, y2 in lines:
#    if (x1, y1) <= im_copy.size:
#        print im_copy[x1, y1]#, im_copy[x2, y2]
#    cv2.line(im_copy, (x1, y1), (x2, y2), (0, 255, 0), 2)
#
#rects = []
#for line1 in lines:
#    for idx, line2 in enumerate(lines):
#        point = get_intersection(line1, line2)
#        if point[0] > 0 and point[1] > 0:
#            cv2.circle(im_copy, point, 3, (0, 0, 255), 2)
#            rects.append(point)

#------------------------------------------------------------
# Hough Line Detection
#------------------------------------------------------------
#im_cann = cv2.Canny(im_blur, 50, 150, apertureSize=3)
#lines   = cv2.HoughLines(im_cann, 1, cv.CV_PI/180, 200)
#
#for rho, theta in lines[0]:
#    a  = np.cos(theta)
#    b  = np.sin(theta)
#    x0 = a * rho
#    y0 = b * rho
#    x1 = int(x0 + 1000 * (-b))
#    y1 = int(y0 + 1000 * a)
#    x2 = int(x0 - 1000 * (-b))
#    y2 = int(y0 - 1000 * a)
#
#    cv2.line(im_copy, (x1, y1), (x2, y2), (0, 255, 0), 2)
#
#------------------------------------------------------------
# Corner Detection
#------------------------------------------------------------
#im_gray = np.float32(im_gray)
#im_corn = cv2.cornerHarris(im_gray, 2, 3, 0.04)
#im_copy[im_corn > 0.01 * im_corn.max()] = [0, 0, 255]

#------------------------------------------------------------
# Crop Image
#------------------------------------------------------------
#x1,y1,x2,y2 = rectangle
#im_crop = im_root[x1:x2, y1:y2]
#cv2.imshow('cropped', im_crop)

#------------------------------------------------------------
# Visualization
#------------------------------------------------------------
if __name__ == "__main__":
    stress_test()
    cv2.waitKey(0)
    cv2.destroyAllWindows()

#plt.subplot(231), plt.imshow(im_erod, 'gray'), plt.title('Erosion')
#plt.subplot(232), plt.imshow(im_blur, 'gray'), plt.title('Gaussian')
#plt.subplot(233), plt.imshow(im_cann, 'gray'), plt.title('Canny')
#plt.subplot(234), plt.imshow(im_copy, 'gray'), plt.title('Final')
#plt.show()
