import cv2
import numpy as np

class RectangleSelector:

    def __init__(self, path):
        ''' Initialize a new RectangleSelector

        :param path: The path to the image to select from
        '''
        self.window     = "image-window"
        self.image      = cv2.imread(path)
        self.is_drawing = False
        self.rectangle  = None
        cv2.imshow(self.window, self.image)
        cv2.setMouseCallback(self.window, self.on_mouse)

    def on_mouse(self, event, x, y, flags, param):
        ''' The callback for the on_mouse movement.
        '''
        x, y = np.int16([x, y]) # cast to int

        if event == cv2.EVENT_LBUTTONDOWN:
            self.is_drawing = True
            self.rectangle  = (x, y, x, y)

        elif event == cv2.EVENT_LBUTTONUP:
            self.is_drawing = False

        if self.is_drawing:
            xc, yc, w, h = self.rectangle
            x0, y0 = np.minimum([xc, yc], [x, y])
            x1, y1 = np.maximum([xc, yc], [x, y])
            self.rectangle = (x0, y0, x1, y1)
            self.update_image()

    def update_image(self):
        ''' Given an image, draw the currently selected
        rectangle on that image.

        :param image: The image to draw the current selection on
        :returns: True if we drew an image, False otherwise
        '''
        if not self.rectangle:
            return False

        image = self.image.copy()
        x0, y0, x1, y1 = self.rectangle
        cv2.rectangle(image, (x0, y0), (x1, y1), (0, 255, 0), 2)
        cv2.imshow(self.window, image)

def select_rectangle(path):
    select = RectangleSelector(path)

    while True:
        key = cv2.waitKey(5)
        if key == ord(' '):
            break;

    cv2.destroyAllWindows()
    return select.rectangle

if __name__ == "__main__":
    import glob
    import sys

    for path in glob.glob(sys.argv[1] + "/*.jpeg"):
        rectangle = select_rectangle(path)
        print "%s\t%s" % (path, rectangle)
