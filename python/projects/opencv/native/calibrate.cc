#include "opencv2/highgui/highgui.hpp"
#include "opencv2/imgproc/imgproc.hpp"
#include "opencv2/calib3d/calib3d.hpp"

#include <iostream>

/**
 *
 */
static void print_help()
{
    std::cout << "\nThis program demonstrates calibrating a camera.\n"
            "Usage:\n"
            "./calibrate <image_name>, Default is image.png\n" << std::endl;
}

int main(int argc, char** argv)
{
    const char* filename = (argc >= 2) ? argv[1] : "image.png";

    cv::Mat source = cv::imread(filename, 0);
    if (source.empty()) {
        print_help();
        std::cout << "can not open " << filename << std::endl;
        return -1;
    }

    cv::Size grid_size(4, 11);
    cv::Size size(8, 9);
    cv::vector<cv::Point2f> points;

    unsigned int flags = cv::CALIB_CB_ASYMMETRIC_GRID
                       | cv::CALIB_CB_CLUSTERING;
    bool was_found = cv::findCirclesGrid(source, grid_size, points, flags);
    if (was_found) {
        std::cout << "found calibration matrix" << std::endl;
        cv::drawChessboardCorners(source, size, cv::Mat(points), was_found);
        cv::imshow("source", source);
        cv::waitKey();
    } else {
        std::cout << "no calibration grid was found" << std::endl;
    }

    return 0;
}
