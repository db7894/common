import cv2, cv
import numpy as np
from random import randint

#------------------------------------------------------------
# The current state
#------------------------------------------------------------
ix, iy  = -1, -1
drawing = False
mode    = False

#------------------------------------------------------------
# helpers
#------------------------------------------------------------
def list_events():
    ''' List all the available events in the opencv
    events library.
    '''
    events = [e for e in dir(cv2) if 'EVENT' in e]
    for event in events:
        print "cv2.%s: %d" % (event, cv2.__dict__[event])

def get_random_color():
    ''' A helper method to generate random colors.

    :returns: A random color tuple
    '''
    get = lambda: randint(0, 255)
    return (get(), get(), get())

def draw_shape(x, y):
    global mode, ix, iy
    color = get_random_color()
    if mode:
        cv2.rectangle(image, (ix, iy), (x, y), color, -1)
    else: cv2.circle(image, (x, y), 100, color, -1)

def handle_events(event, x, y, flags, param):
    global drawing, ix, iy
    if event == cv2.EVENT_LBUTTONDOWN:
        ix, iy = x, y
        drawing = True
    elif event == cv2.EVENT_MOUSEMOVE:
        if drawing: draw_shape(x, y)
    elif event == cv2.EVENT_LBUTTONUP:
        draw_shape(x, y)
        drawing = False

#------------------------------------------------------------
# main
#------------------------------------------------------------
if __name__ == '__main__':
    image = np.zeros((512, 512, 3), np.uint8)
    cv2.namedWindow('image')
    cv2.setMouseCallback('image', handle_events)

    while True:
        cv2.imshow('image', image)
        key = cv2.waitKey(20) & 0xFF
        if   key == 27: break
        elif key == ord('m'): mode = not mode
    cv2.destroyAllWindows()
