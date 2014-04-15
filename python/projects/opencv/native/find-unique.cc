/**
 * Compile with:
 * g++ `pkg-config --cflags --libs opencv` -o find-unique find-unique.cc
 */
#include "opencv2/opencv.hpp"

using namespace cv;

int main(int argc, char **argv)
{
    Mat image = imread(argv[1]);
    Mat edges;
    namedWindow("edges",1);

    cvtColor(image, edges, CV_BGR2GRAY);
    GaussianBlur(edges, edges, Size(7, 7), 2.5, 2.5);
    Canny(edges, edges, 0, 30, 3);
    imshow("edges", edges);

    while (true) {
        if (waitKey(30) >= 0) break;
    }
    return 0;
}
