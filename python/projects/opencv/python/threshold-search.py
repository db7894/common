import sys
import cv2, cv
import numpy as np

# open the image to test
im_base = cv2.imread(sys.argv[1])
rows, cols = im_base.shape[:2]
im_hsv  = cv2.cvtColor(im_base, cv2.COLOR_BGR2HSV)
im_big  = np.zeros((rows, cols*3, 3), np.uint8)
#cv2.namedWindow('image', cv2.CV_WINDOW_AUTOSIZE)
cv2.namedWindow('image')

# create trackbars for color changes
def nothing(_): pass
cv2.createTrackbar('H', 'image', 0, 180, nothing)
cv2.createTrackbar('S', 'image', 0, 255, nothing)
cv2.createTrackbar('V', 'image', 0, 255, nothing)
cv2.createTrackbar('R', 'image', 0, 255, nothing)

while 0xFF & cv2.waitKey(30) != 27:
    h = cv2.getTrackbarPos('H', 'image')
    s = cv2.getTrackbarPos('S', 'image')
    v = cv2.getTrackbarPos('V', 'image')
    r = cv2.getTrackbarPos('R', 'image')

    lower   = np.array((h - r,  50,  50))
    upper   = np.array((h + r, s, v))
    im_mask = cv2.inRange(im_hsv, lower, upper)
    im_item = cv2.bitwise_and(im_base, im_base, mask=im_mask)

    im_big[:,0:cols,:] = im_base
    im_big[:,cols:cols*2,0] = im_mask
    im_big[:,cols*2:,:] = im_item

    cv2.imshow('image', cv2.resize(im_big, (rows, cols / 3)))

cv2.destroyAllWindows()
