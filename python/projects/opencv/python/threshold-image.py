import cv2
import numpy as np
from matplotlib import pyplot as plt
import sys

im_base = cv2.imread(sys.argv[1], 0)

#------------------------------------------------------------
# Image Global Thresholding
#------------------------------------------------------------
# The input image should be a grayscale image. If the value
# is greater than the threshold value, it is assigned the
# supplied maxValue. Also, the threshold type is specified
# as one of the following:
# * cv2.THRESH_BINARY
# * cv2.THRESH_BINARY_INV
# * cv2.THRESH_TRUNC
# * cv2.THRESH_TOZERO
# * cv2.THRESH_TOZERO_INV
#------------------------------------------------------------
ret, thresh1 = cv2.threshold(im_base, 127, 255, cv2.THRESH_BINARY)
ret, thresh2 = cv2.threshold(im_base, 127, 255, cv2.THRESH_BINARY_INV)
ret, thresh3 = cv2.threshold(im_base, 127, 255, cv2.THRESH_TRUNC)
ret, thresh4 = cv2.threshold(im_base, 127, 255, cv2.THRESH_TOZERO)
ret, thresh5 = cv2.threshold(im_base, 127, 255, cv2.THRESH_TOZERO_INV)

titles = ['Original Image','BINARY','BINARY_INV','TRUNC','TOZERO','TOZERO_INV']
images = [im_base, thresh1, thresh2, thresh3, thresh4, thresh5]

for i in xrange(6):
    plt.subplot(2, 3, i + 1)
    plt.imshow(images[i], 'gray')
    plt.title(titles[i])
    plt.xticks([])
    plt.yticks([])
plt.show()

#------------------------------------------------------------
# Image Adaptive Thresholding
#------------------------------------------------------------
# Adaptive thresholding attempts to find threshold values
# that are local to a given block. The algorithms take a
# block size to compute and a constant to subtract. The two
# available methods are:
# * ADAPTIVE_THRESH_GAUSSIAN_C
#   threshold is the weighted sum of neighborhood
#   values where weights are a gaussian window.
# * ADAPTIVE_THRESH_MEAN_C
#   threshold value is the mean of the neighboring area
#------------------------------------------------------------
im_blur = cv2.medianBlur(im_base, 5)
_, th1 = cv2.threshold(im_blur, 127, 255, cv2.THRESH_BINARY)
th2 = cv2.adaptiveThreshold(im_blur, 255, cv2.ADAPTIVE_THRESH_MEAN_C, cv2.THRESH_BINARY, 11, 2)
th3 = cv2.adaptiveThreshold(im_blue, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 11, 2)

titles = ['Original Image', 'Global Thresholding (v = 127)', 'Adaptive Mean Thresholding', 'Adaptive Gaussian Thresholding']
images = [im_base, th1, th2, th3]

for i in xrange(4):
    plt.subplot(2, 2, i+1),
    plt.imshow(images[i], 'gray')
    plt.title(titles[i])
    plt.xticks([])
    plt.yticks([])
plt.show()
