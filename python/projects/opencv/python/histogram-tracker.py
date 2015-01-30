#!/usr/bin/env python
import cv2
import numpy as np

def get_histogram(image):
    ''' Get the base normalized histogram of the
    object to track from the capture device.

    :param image: The image to get a histogram from
    :returns: The normalized histogram of the tracked object
    '''
    im_hsv  = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)
    im_mask = cv2.inRange(im_hsv, np.array((0, 60, 32)), np.array((180, 255, 255)))
    im_hist = cv2.calcHist([im_hsv], [0], im_mask, [180], [0, 180])
    cv2.normalize(im_hist, im_hist, 0, 255, cv2.NORM_MINMAX)
    return im_hist

def tracker_runner(capture, **kwargs):
    ''' The main tracker of a given object.

    :param capture: The capture device to read from
    '''
    is_valid, im_base = capture.read()
    histogram = get_histogram(im_base)
    criteria  = (cv2.TERM_CRITERIA_EPS | cv2.TERM_CRITERIA_COUNT, 10, 1)
    tracking  = (0, 0, 200, 200)
    track_method = kwargs.get('track_method', cv2.CamShift) # cv2.meanShift

    while True:
        is_valid, im_base = capture.read()
        if not is_valid:
            break # the capture device has failed

        im_hsv  = cv2.cvtColor(im_base, cv2.COLOR_BGR2HSV)
        im_back = cv2.calcBackProject([im_hsv], [0], histogram, [0, 180], 1)
        is_valid, tracking = track_method(im_back, tracking, criteria)
        x, y, w, h = tracking
        cv2.rectangle(im_base, (x, y), (x+w, y+h), 255, 2)
        cv2.imshow('tracked image', im_base)

        key = cv2.waitKey(5) & 0xff
        if key == 27: break

def capture_runner():
    ''' The main runner for setting up and tearing down
    the tracker.
    '''
    capture = cv2.VideoCapture(0)
    try:
        tracker_runner(capture)
    except Exception, ex:
        raise ex

    cv2.destroyAllWindows()
    capture.release()

if __name__ == "__main__":
    capture_runner()
