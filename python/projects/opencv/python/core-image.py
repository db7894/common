import cv2
import numpy as np
from matplotlib import pyplot as plt

im_1 = cv2.imread(sys.argv[1])
im_2 = cv2.imread(sys.argv[2])

#------------------------------------------------------------
# Addition
#------------------------------------------------------------
im_3 = cv2.add(im_1, im_2) # saturation operation (260 + 10 = 260 = 255)
im_4 = im_1 + im_2         # modulo operation (250 + 10 = 260 % 256 = 4)
im_5 = cv2.addWeighted(im_1, 0.7, im_2, 0.3, 0) # dst = a * im1 + b * im2 + c

cv2.imshow('saturated', im3)
cv2.imshow('modulated', im4)
cv2.imshow('add-weighted', im5)

#------------------------------------------------------------
# Bitwise Overlay
#------------------------------------------------------------
im_logo = cv2.imread(sys.argv[3])    # acquire our overlay
rows, cols, channels = im_logo.shape # get shape of the overlay
im_roi = im_1[0:rows, 0:cols]        # extract region of interest

# create a mask for the overlay and its inverse
im_gray = cv2.cvtColor(im_logo, cv2.COLOR_BGR2GRAY)
ret, im_mask = cv2.threshold(im_gray, 200, 255, cv2.THRESH_BINARY)
im_imask = cv2.bitwise_not(im_mask)

# extract main background and logo foreground
im_fg = cv2.bitwise_and(im_roi, im_roi, mask=im_mask)
im_bg = cv2.bitwise_and(im_2, im_2, mask=im_imask)

# add the logo to the main image
im_edit = cv2.add(im_bg, im_fg)
im_1[0:rows, 0:cols] = im_edit
cv2.imshow('overlayed', im_1)

#------------------------------------------------------------
# How To Instrument Code
#------------------------------------------------------------
# One can also use the %timeit command in ipython
#------------------------------------------------------------
if not cv2.useOptimized(): # should be on by default
    cv2.setUseOptimized(True)

t1 = cv2.getTickCount()

# ... your code execution ...

t2 = cv2.getTickCount()
time = (t2 - t1) / cv2.getTickFrequency()

#------------------------------------------------------------
# Color Conversion
#------------------------------------------------------------
im_gray = cv2.cvtColor(im_1, cv2.COLOR_BGR2GRAY)
im_hsv  = cv2.cvtColor(im_1, cv2.COLOR_BGR2HSV)

green = np.uint8([[[0, 255, 0]]])
hsv_green = cv2.cvtColor(green, cv2.COLOR_BGR2HSV)

#------------------------------------------------------------
# Multi Channel Color inRange
#------------------------------------------------------------
b, g, r = cv2.split(im_1)
im_b =  (b > 64) & (b < 128)
im_g =  (g > 72) & (g < 155)
im_r =  (r > 64) & (r < 155)
im_mask = im_b & im_g & im_r

#------------------------------------------------------------
# Main Close
#------------------------------------------------------------
cv2.waitKey(0)
cv2.destroyAllWindows()
