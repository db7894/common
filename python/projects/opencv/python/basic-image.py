import cv2
import numpy as np
from matplotlib import pyplot as plt
import sys

img = cv2.imread(sys.argv[1], 0)
cv2.imshow('image', img)
key = cv2.waitKey(0) & 0xff
if key != 27: # ESC
    cv2.imwrite('output.png', img)
cv2.destroyAllWindows()
