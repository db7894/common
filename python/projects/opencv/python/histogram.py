import cv2
import numpy as np
import matplotlib.pyplot as plt

def numpy_gray_histogram(path):
    '''
    '''
    image = cv2.imread(path, 0)
    length = [0, 256]
    plt.hist(image.ravel(), length[1], length)
    plt.show()

def numpy_color_histogram(path):
    '''
    '''
    image  = cv2.imread(path)
    image  = cv2.cvtColor(image, cv2.COLOR_RGB2HSV)
    #ranges = [(0, 256), (0, 256), (0, 256)]          # RGB Ranges
    ranges = [(0, 180), (0, 256), (0, 256)]          # HSV Ranges
    colors = ['b', 'g', 'r']
    for channel, color in enumerate(colors):
        histogram = cv2.calcHist([image], [channel], None, [ranges[channel][1]], ranges[channel])
        plt.plot(histogram, color=color)
        plt.xlim(ranges[channel])
    plt.show()

def opencv_color_histogram(path):
    '''
    '''
    im_base   = cv2.imread(path)
    im_base   = cv2.cvtColor(im_base, cv2.COLOR_RGB2HSV)
    histogram = np.zeros((300, 256, 3))
    bins      = np.arange(256).reshape(256, 1)
    colors    = [(255, 0, 0), (0, 255, 0), (0, 0, 255)] # RGB Colors
    #ranges    = [(0, 180), (0, 256), (0, 256)]          # HSV Ranges
    ranges    = [(0, 256), (0, 256), (0, 256)]          # RGB Ranges
    sizes     = [[value[1]] for value in ranges]

    for channel, color in enumerate(colors):
        hist_entry = cv2.calcHist([im_base], [channel], None, sizes[channel], ranges[channel])
        cv2.normalize(hist_entry, hist_entry, ranges[channel][0], ranges[channel][1], cv2.NORM_MINMAX)
        entry  = np.int32(np.around(hist_entry))
        points = np.column_stack((bins, entry))
        cv2.polylines(histogram, [points], False, color)

    histogram = np.flipud(histogram)
    cv2.imshow('colorhist', histogram)
    cv2.waitKey(0)

if __name__ == "__main__":
    import sys

    #numpy_gray_histogram(sys.argv[1])
    numpy_color_histogram(sys.argv[1])
    #opencv_color_histogram(sys.argv[1])
