/**
 * Compile with:
 * g++ `pkg-config --cflags --libs opencv` -o test test.cc
 */
#include "opencv2/opencv.hpp"

using namespace cv;

/**
 * Given an image, add random noise to it
 *
 * @param image The image to apply noise to
 * @param n The number of pixels to add noise to
 */
void salt(cv::Mat &image, int n) {
    for (int k = 0; k < n; k++) {
        int i = rand() % image.cols;
        int j = rand() % image.rows;

        if (image.channels() == 1) {        // gray image
            image.at<uchar>(j,i) = 255;
        } else if (image.channels() == 3) { // color image
            image.at<cv::Vec3b>(j,i)[0] = 255;
            image.at<cv::Vec3b>(j,i)[1] = 255;
            image.at<cv::Vec3b>(j,i)[2] = 255;
        }
    }
}

int main(int, char**)
{
    getBuildInformation();
    VideoCapture capture(0); // open the default camera
    if (!capture.isOpened()) // check if we succeeded
        return -1;

    int width  = 640;
    int height = 480;
    int format = CV_FOURCC('D', 'I', 'V', 'X'); // -1

    capture.set(CV_CAP_PROP_FRAME_WIDTH, width);
    capture.set(CV_CAP_PROP_FRAME_HEIGHT, height);

    VideoWriter writer("output.mpeg", format, 25.0, cvSize(width, height), 1);
    if (!writer.isOpened()) // check if we succeeded
        return -1;

    namedWindow("camera", 1);
    while (true) {
        Mat frame;
        capture >> frame;
        writer  << frame;
        imshow("camera", frame);
        if (waitKey(30) >= 0) break;
    }
    return 0;
}
