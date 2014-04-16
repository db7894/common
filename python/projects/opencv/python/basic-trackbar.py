import cv2, cv
import numpy as np

# create a blank image
image = np.zeros((128, 512, 3), np.uint8)
cv2.namedWindow('image')

# create trackbars for color changes
def nothing(_): pass
switch = '0 : OFF \n1 : ON'
cv2.createTrackbar('R', 'image', 0, 255, nothing)
cv2.createTrackbar('G', 'image', 0, 255, nothing)
cv2.createTrackbar('B', 'image', 0, 255, nothing)
cv2.createTrackbar(switch, 'image', 0, 1, nothing)

while 0xFF & cv2.waitKey(30) != 27:
    cv2.imshow('image', image)

    r = cv2.getTrackbarPos('R', 'image')
    g = cv2.getTrackbarPos('G', 'image')
    b = cv2.getTrackbarPos('B', 'image')
    s = cv2.getTrackbarPos(switch, 'image')
    if s == 0: image[:] = 0
    else: image[:] = [b, g, r]
cv2.destroyAllWindows()
