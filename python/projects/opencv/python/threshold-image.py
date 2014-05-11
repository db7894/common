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
th3 = cv2.adaptiveThreshold(im_blur, 255, cv2.ADAPTIVE_THRESH_GAUSSIAN_C, cv2.THRESH_BINARY, 11, 2)

titles = ['Original Image', 'Global Thresholding (v = 127)', 'Adaptive Mean Thresholding', 'Adaptive Gaussian Thresholding']
images = [im_base, th1, th2, th3]

for i in xrange(4):
    plt.subplot(2, 2, i + 1),
    plt.imshow(images[i], 'gray')
    plt.title(titles[i])
    plt.xticks([])
    plt.yticks([])
plt.show()

#------------------------------------------------------------
# Otsu's Method
#------------------------------------------------------------
# This is useful when we have a bimodal image whose histogram
# has two peaks. This will find a value in the middle of the
# two that will be useful in thresholding.
#------------------------------------------------------------
ret1, th1 = cv2.threshold(im_base, 127, 255, cv2.THRESH_BINARY)                  # global thresholding
ret2, th2 = cv2.threshold(im_base, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)  # Otsu's thresholding
im_blur = cv2.GaussianBlur(im_base, (5, 5), 0)                                   # Gaussian filtering (remove noise)
ret3, th3 = cv2.threshold(im_blur, 0, 255, cv2.THRESH_BINARY + cv2.THRESH_OTSU)  # Otsu's thresholding

images = [im_base, 0, th1, im_base, 0, th2, im_blur, 0, th3]
titles = [
    'Original Noisy Image', 'Histogram','Global Thresholding (v=127)',
    'Original Noisy Image', 'Histogram', "Otsu's Thresholding",
    'Gaussian filtered Image', 'Histogram', "Otsu's Thresholding"
]

for i in xrange(3):
    plt.subplot(3, 3, i * 3 + 1), plt.imshow(images[i * 3], 'gray')
    plt.title(titles[i * 3]), plt.xticks([]), plt.yticks([])

    plt.subplot(3, 3, i * 3 + 2), plt.hist(images[i * 3].ravel(), 256)
    plt.title(titles[i * 3 + 1]), plt.xticks([]), plt.yticks([])

    plt.subplot(3, 3, i * 3 + 3), plt.imshow(images[i * 3 + 2], 'gray')
    plt.title(titles[i * 3 + 2]), plt.xticks([]), plt.yticks([])
plt.show()
