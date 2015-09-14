import cv2
import cv
import numpy as np

def variance_of_laplacian(image):
    return cv2.Laplacian(image, cv2.CV_64F).var()

if __name__ == "__main__":
    import sys
    path    = sys.argv[1]
    im_full = cv2.imread(path)
    im_gray = cv2.cvtColor(im_full, cv2.COLOR_RGB2HSV)
    score   = variance_of_laplacian(im_gray)
    text    = "blurry" if score <= 750 else "good"
    cv2.putText(im_full, "{}: {:.2f}".format(text, score),
        (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 0, 255), 3)
    cv2.imshow("Image", im_full)
    cv2.waitKey(0)
