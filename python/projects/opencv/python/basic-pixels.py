import cv2
import numpy as np
from matplotlib import pyplot as plt

image = cv2.imread(sys.argv[1])   # load our image in to memory
print image.size                   # The total number of pixels in the image
print image.shape                  # A tuple of (width, height, colors)
print image.dtype                  # The underlying pixel data type

# The slice syntax is better used for working with pixel ranges
# as numpy is more efficient for operating over vectors than single
# values.

pixel = image[100, 100]           # read a single pixel
blue  = image[100, 100, 0]        # access just the blue pixel
image[100, 100] = [255, 255, 255] # set the pixel value
image.item(100, 100, 0)           # access using item getter
image.itemset((100, 100, 0), 100) # access using item setter

region = image[150:200, 150:200]  # extract a region of interest
image[200:250, 150:200] = region  # clone that region into the image

b, g, r = cv2.split(image)        # split an image into color channels
image = cv2.merge((b, g, r))      # merge channels back into a single image

b = image[:, :, 0]                # faster color channel extraction
image[:, :, 2] = 0                # remove the red channel

# Examples for adding borders to existing images in a
# number of different ways.

replicate  = cv2.copyMakeBorder(image, 10,10,10,10, cv2.BORDER_REPLICATE)
reflect    = cv2.copyMakeBorder(image, 10,10,10,10, cv2.BORDER_REFLECT)
reflect101 = cv2.copyMakeBorder(image, 10,10,10,10, cv2.BORDER_REFLECT_101)
wrap       = cv2.copyMakeBorder(image, 10,10,10,10, cv2.BORDER_WRAP)
constant   = cv2.copyMakeBorder(image, 10,10,10,10, cv2.BORDER_CONSTANT,value=[255, 0, 0])

plt.subplot(231), plt.imshow(image,'gray'),      plt.title('ORIGINAL')
plt.subplot(232), plt.imshow(replicate,'gray'),  plt.title('REPLICATE')
plt.subplot(233), plt.imshow(reflect,'gray'),    plt.title('REFLECT')
plt.subplot(234), plt.imshow(reflect101,'gray'), plt.title('REFLECT_101')
plt.subplot(235), plt.imshow(wrap,'gray'),       plt.title('WRAP')
plt.subplot(236), plt.imshow(constant,'gray'),   plt.title('CONSTANT')
plt.show()
