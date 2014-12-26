import cv2
import numpy as np

#------------------------------------------------------------
# classes
#------------------------------------------------------------

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

#------------------------------------------------------------
# helper methods
#------------------------------------------------------------

def crop_and_save(path, rectangle):
    ''' Given an image path and a rectangle, save the
    cropped rectangle on that image.

    :param path: The path to the image to crop
    :param rectangle: The rectangle of that image to crop
    :returns: The saved path name
    '''
    x0, y0, x1, y1 = rectangle
    name, form = path.rsplit('.', 1)
    im_base = cv2.imread(path)
    im_crop = im_base[y0:y1, x0:x1]
    path = "{}-crop.{}".format(name, form)
    cv2.imwrite(path, im_crop)
    return path

def select_rectangle(path):
    ''' Given an image path, run the rectangle
    selector GUI for that image.

    :param path: The image to select a rectangle for
    :returns: The selected rectangle
    '''
    select = RectangleSelector(path)

    while True:
        key = cv2.waitKey(5)
        if key == ord(' '):
            break;

    cv2.destroyAllWindows()
    return select.rectangle

#------------------------------------------------------------
# main
#------------------------------------------------------------

if __name__ == "__main__":
    import glob
    import sys

    for path in glob.glob(sys.argv[1] + "/*.jpeg"):
        rectangle = select_rectangle(path)
        crop_path = crop_and_save(path, rectangle)
        print "{}\t{}\t{}".format(path, rectangle, crop_path)
