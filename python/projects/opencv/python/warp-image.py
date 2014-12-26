import cv2
import numpy as np
import sys
import os

def order_points(points):
    ''' Given a collection of unsorted points, sort them
    in order of upper left, upper right, lower right, lower
    left of the rectangle. It is assumed there are four points
    passed in.

    :param points: The points to sort correctly
    :returns: The sorted points
    ''' 
    rect    = np.zeros((4, 2), np.float32)
    sums    = points.sum(axis=1)      # take the sum of the points
    rect[0] = points[np.argmin(sums)] # top left has the largest sum
    rect[2] = points[np.argmax(sums)] # bottom right has the smallest sum
    diff    = np.diff(points, axis=1) # take the different of the points
    rect[1] = points[np.argmin(diff)] # upper right will have lowest diff
    rect[3] = points[np.argmax(diff)] # bottom left will have largest diff
    return rect

def find_rectangle_points_quick(image):
    ''' Given an image, attempt to find the rectangular
    region of the image that can be later be transformed.

    :param image: The image with a rectangular region to transform
    :retuns: The points of the rectangular region
    '''
    im_base = image
    im_gray = cv2.cvtColor(im_base, cv2.COLOR_BGR2GRAY)
    im_blur = cv2.GaussianBlur(im_gray, (5, 5), 0)
    im_edge = cv2.Canny(im_gray, 75, 200)
    corners = cv2.goodFeaturesToTrack(im_edge, 4, 0.01, 10)
    return corners.reshape((-1, 2))

def find_rectangle_points(image):
    ''' Given an image, attempt to find the rectangular
    region of the image that can be later be transformed.

    :param image: The image with a rectangular region to transform
    :retuns: The points of the rectangular region
    '''
    im_base = image
    im_gray = cv2.cvtColor(im_base, cv2.COLOR_BGR2GRAY)
    im_blur = cv2.GaussianBlur(im_gray, (5, 5), 0)
    im_edge = cv2.Canny(im_gray, 75, 200)
    contours, _ = cv2.findContours(im_edge, cv2.RETR_LIST, cv2.CHAIN_APPROX_SIMPLE)
    contours = sorted(contours, key = cv2.contourArea, reverse = True)[:5]
     
    for contour in contours:
        perimiter   = cv2.arcLength(contour, True)
        approximate = cv2.approxPolyDP(contour, 0.02 * perimiter, True)
        if len(approximate) == 4:
            return approximate.reshape(4, 2)
    raise Exception("Could not find a rectangle in the image")

def transform_rectangle(image, points):
    ''' Given an image and a collection of points representing
    the rectangle to transform, perform an affine transformation
    on the given region specified by the supplied points.

    :param image: The image to transform the region from
    :param points: The four points marking the region to transform
    :returns: The transformed region of the image
    '''
    corners = order_points(points)
    tl, tr, br, bl = points

    width_t = np.sqrt(((tr[0] - tl[0]) ** 2) + ((tr[0] - tl[0]) ** 2)) 
    width_b = np.sqrt(((br[0] - bl[0]) ** 2) + ((br[0] - bl[0]) ** 2)) 
    width_m = int(max(width_t, width_b))

    height_t = np.sqrt(((tr[1] - br[1]) ** 2) + ((tr[1] - br[1]) ** 2)) 
    height_b = np.sqrt(((tl[1] - bl[1]) ** 2) + ((tl[1] - bl[1]) ** 2)) 
    height_m = int(max(height_t, height_b))

    warped_corners = np.array([
        [0, 0],
        [width_m - 1, 0],
        [width_m - 1, height_m - 1],
        [0, height_m - 1],
    ], np.float32)

    affine = cv2.getPerspectiveTransform(corners, warped_corners)
    return cv2.warpPerspective(image, affine, (width_m, height_m))

def black_and_white(image):
    ''' Convert the image to black and white like a
    newspaper.

    :param image: The image to convert to black and white
    :returns: The converted image
    '''
    #method  = cv2.ADAPTIVE_THRESH_MEAN_C
    method  = cv2.ADAPTIVE_THRESH_GAUSSIAN_C
    thresh  = cv2.THRESH_BINARY
    im_blck = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    im_blck = cv2.adaptiveThreshold(im_blck, 255, method, thresh, 11, 2)
    return im_blck

if __name__ == "__main__":
    path   = os.path.abspath(sys.argv[1])
    source = cv2.imread(path)
    points = find_rectangle_points(source)
    warped = transform_rectangle(source, points)

    cv2.imshow('source', source)
    cv2.imshow('warped', black_and_white(warped))
    cv2.waitKey(0)
    cv2.destroyAllWindows()
