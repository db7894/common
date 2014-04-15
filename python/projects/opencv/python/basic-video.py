import numpy as np
import cv2, cv
import time

#
# This is the underlying video capture, properties can be
# retrieved and set by using the property identifier constants:
#
# - CV_CAP_PROP_POS_MSEC Current position of the video file in milliseconds or video capture timestamp.
# - CV_CAP_PROP_POS_FRAMES 0-based index of the frame to be decoded/captured next.
# - CV_CAP_PROP_POS_AVI_RATIO Relative position of the video file: 0 - start of the film, 1 - end of the film.
# - CV_CAP_PROP_FRAME_WIDTH Width of the frames in the video stream.
# - CV_CAP_PROP_FRAME_HEIGHT Height of the frames in the video stream.
# - CV_CAP_PROP_FPS Frame rate.
# - CV_CAP_PROP_FOURCC 4-character code of codec.
# - CV_CAP_PROP_FRAME_COUNT Number of frames in the video file.
# - CV_CAP_PROP_FORMAT Format of the Mat objects returned by retrieve() .
# - CV_CAP_PROP_MODE Backend-specific value indicating the current capture mode.
# - CV_CAP_PROP_BRIGHTNESS Brightness of the image (only for cameras).
# - CV_CAP_PROP_CONTRAST Contrast of the image (only for cameras).
# - CV_CAP_PROP_SATURATION Saturation of the image (only for cameras).
# - CV_CAP_PROP_HUE Hue of the image (only for cameras).
# - CV_CAP_PROP_GAIN Gain of the image (only for cameras).
# - CV_CAP_PROP_EXPOSURE Exposure (only for cameras).
# - CV_CAP_PROP_CONVERT_RGB Boolean flags indicating whether images should be converted to RGB.
# - CV_CAP_PROP_WHITE_BALANCE Currently not supported
# - CV_CAP_PROP_RECTIFICATION Rectification flag for stereo cameras (note: only supported by DC1394 v 2.x backend currently)
#

def display_webcam():
    capture = cv2.VideoCapture(0)
    width   = capture.get(cv2.cv.CV_CAP_PROP_FRAME_WIDTH)
    height  = capture.get(cv2.cv.CV_CAP_PROP_FRAME_HEIGHT)

    while True:
        result, frame = capture.read()
        grayed = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

        cv2.imshow('frame', grayed)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    capture.release()
    cv2.destroyAllWindows()

def play_file(path):
    capture = cv2.VideoCapture(path)

    while cap.isOpened():
        result, frame = capture.read()
        grayed = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

        cv2.imshow('frame', grayed)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    capture.release()
    cv2.destroyAllWindows()

def save_video(path, form='DIVX'):
    ''' There are a number of file formats that can be saved
    as and they are based on the codes at http://fourcc.org:

    * DIVX, XVID, MJPG, X264, WMV1, WMV2

    :param path: The path to save the stream to
    :param form: The format to save the files as
    '''
    capture = cv2.VideoCapture(0)
    fourcc  = cv.CV_FOURCC(*form)
    capture.set(cv.CV_CAP_PROP_FRAME_WIDTH,  640)
    capture.set(cv.CV_CAP_PROP_FRAME_HEIGHT, 480)
    output  = cv2.VideoWriter(path, fourcc, 25, (640, 480), True)
    start   = time.time()

    while capture.isOpened():
        result, frame = capture.read()
        if result:
            frame = cv2.flip(frame, 0)  # flip the frame verticaly
            output.write(frame)
            cv2.imshow('frame', frame)
            if time.time() - start > 5:
                break
        else: break

    capture.release()
    cv2.destroyAllWindows()

save_video('output.mpeg')
