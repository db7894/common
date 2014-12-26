import numpy as np
import cv2

capture  = cv2.VideoCapture(0)
width    = capture.get(cv2.cv.CV_CAP_PROP_FRAME_WIDTH)
height   = capture.get(cv2.cv.CV_CAP_PROP_FRAME_HEIGHT)
subtract = cv2.BackgroundSubtractorMOG2()

while True:
    result, frame = capture.read()
    mask = subtract.apply(frame)

    cv2.imshow('foreground mask', mask)
    cv2.imshow('foreground item', cv2.bitwise_and(frame, frame, mask=mask))
    if cv2.waitKey(1) & 0xFF == 27:
        break

capture.release()
cv2.destroyAllWindows()
