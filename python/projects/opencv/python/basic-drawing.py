import numpy as np
import cv2, cv

class Color(object):
    ''' A collection of commonly used colors '''

    White  = (255, 255, 255)
    Blue   = (255, 0, 0)
    Green  = (0, 255, 0)
    Red    = (0, 0, 255)
    Yellow = (0, 255, 255)

# create a blank image
image = np.zeros((512, 512, 3), np.uint8)

# draw arbitrary shapes
cv2.line(image, (0, 0), (511, 511), Color.Blue, 5)
cv2.rectangle(image, (384, 0), (510, 128), Color.Green, 3)
cv2.circle(image, (447, 63), 63, Color.Red, -1)
cv2.ellipse(image, (256, 256), (100, 50), 0, 0, 180, 255, -1)

# draw an arbitray polygon
points = np.array([[10, 5],[ 20, 30], [70, 20], [50, 10]], np.int32)
points = points.reshape((-1,1,2))
cv2.polylines(image, [points], True, Color.Yellow)

# draw arbitray text on the image
font = cv2.FONT_HERSHEY_SIMPLEX
cv2.putText(image, 'OpenCV', (10, 500), font, 4, Color.White, 2, cv2.CV_AA)
cv2.imshow('image', image)

while 0xFF & cv2.waitKey(30) != 27:
    pass
cv2.destroyAllWindows()
