import sys
import cv2
import numpy as np

class Callbacks(object):

    def __init__(self, **kwargs):
        ''' Initalize a new instance of the callbacks class

        :param im_base: The base image to display with
        :param im_blur: The blur image to operate with
        '''
        self.im_base = kwargs.get('im_base')
        self.im_blur = kwargs.get('im_blur')

    def threshold(self, threshold):
        ''' Callback for when the threshold has been updatd for
        the canny edge detection.

        :param threshold: The new threshold to operate with
        '''
        print "current threshold: %d" % theshold
        get_color = lambda: np.random.randint(0, 255, (3)).tolist()
        im_edge = cv2.Canny(self.im_blur, threshold, threshold * 2)
        im_show = np.zeros(self.im_base.shape, np.uint8)
        contours, hierarchy = cv2.findContours(im_edge, cv2.RETR_TREE, cv2.CHAIN_APPROX_SIMPLE)
        for contour in contours:
            cv2.drawContours(im_show, [contour], 0, get_color(), 2)
            cv2.imshow('output', im_show)
        cv2.imshow('input', self.im_base)

def canny_tester(**kwargs):
    ''' A helper utility to allow users to test various
    values of thresholding for the canny edge detection
    before settling on a final tuned value.
    '''
    min_threshold = kwargs.get('min_threshold', 100)
    max_threshold = kwargs.get('max_threshold', 255)
    blur_kernel   = kwargs.get('blur_kernel', (5, 5))

    im_base  = cv2.imread(kwargs.get('path'))
    im_gray  = cv2.cvtColor(im_base, cv2.COLOR_BGR2GRAY)
    im_blur  = cv2.GaussianBlur(im_gray, blur_kernel, 0)
    callback = Callbacks(im_blur=im_blur, im_base=im_base)

    cv2.namedWindow('input', cv2.WINDOW_AUTOSIZE)
    cv2.createTrackbar('canny threshold:', 'input', min_threshold, max_threshold, callback.threshold)
    callback.threshold(min_thresh)

    if cv2.waitKey(0) == 27:
        cv2.destroyAllWindows()

if __name__ == "__main__":
    main(path=sys.argv[1])
