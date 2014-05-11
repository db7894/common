import cv2
import numpy as np

#------------------------------------------------------------
# Color Spaces
#------------------------------------------------------------
# Opencv uses the following ranges for HSV:
# Hue = [0..179]
# Sat = [0..255]
# Val = [0..255]
#------------------------------------------------------------
def print_all_color_flags():
    ''' This will print all the available color conversion
    flags that can be used with::
    
        cv2.cvtColor(image, flag)
    '''
    flags = [f for f in dir(cv2) if f.startswith('COLOR')]
    print flags

def convert_color(color):
    ''' Given a BGR color, print the correct HSV value
    to use for that color. For tracking, simply take that
    value and use the following ranges:

    * (H-10, 100, 100) # lower value
    * (H+10, 255, 255) # upper value
    '''
    hsv = cv2.cvtColor(color, cv2.COLOR_BGR2HSV)
    print "{} -> {}".format(color, hsv)

#------------------------------------------------------------
# Image Tracker
#------------------------------------------------------------
capture = cv2.VideoCapture(0)
while True:

    _, im_base = capture.read()
    im_hsv  = cv2.cvtColor(im_base, cv2.COLOR_BGR2HSV)

    # define range of blue color in HSV
    lower_blue = np.array((110,  50,  50))
    upper_blue = np.array((130, 255, 255))

    # extract mask and use it to extract tracked image
    im_mask = cv2.inRange(im_hsv, lower_blue, upper_blue)
    im_item = cv2.bitwise_and(im_base, im_base, mask=im_mask)

    cv2.imshow('original frame', im_base)
    cv2.imshow('item mask', im_mask)
    cv2.imshow('tracked item', im_item)

    key = cv2.waitKey(5) & 0xff
    if key == 27: break

cv2.destroyAllWindows()
