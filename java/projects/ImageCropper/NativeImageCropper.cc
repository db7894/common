#include <iostream>
#include <boost/filesystem.hpp>
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>

#include "NativeImageCropper.h"

namespace fs = boost::filesystem;
namespace bw = bashwork::vision;

/**
 * Constants to control the algorithms
 */
namespace {
    static const int low_edge_threshold   = 100;
    static const int max_edge_threshold   = 100;
    static const int edge_kernel_size     = 3;
    static cv::Scalar low_color_threshold = cv::Scalar(105, 100, 100);
    static cv::Scalar max_color_threshold = cv::Scalar(115, 255, 255);
    static cv::Size kernel_size           = cv::Size(5, 5);
    static cv::Mat  morphology_kernel     = cv::getStructuringElement(cv::MORPH_RECT, kernel_size);
}

/**
 * @summary Quickly display an image for debugging
 * @param image The image to display
 * @param name The name of the window to display the image in
 */
void display_image(const fs::path& file, const cv::Mat& image, const cv::Rect& rectangle) {
    std::string name = file.string();
    cv::Mat crop = image(rectangle);
    cv::namedWindow(name, cv::WINDOW_AUTOSIZE);
    cv::imshow(name, crop);
}

/**
 * @summary A utility to automatically crop a pick region
 * @param image The image to get a region mask for
 * @param mask The location to store the resulting region mask
 * @return void
 */
cv::Mat bashwork::vision::get_region_mask(const cv::Mat &image) {
    cv::Mat mask;
    cv::Mat hsv_image;
    cv::Mat dirty_mask;

    cv::cvtColor(image, hsv_image, CV_BGR2HSV);
    cv::inRange(hsv_image, low_color_threshold, max_color_threshold, dirty_mask);
    cv::morphologyEx(dirty_mask, mask, cv::MORPH_OPEN, morphology_kernel);

    return std::move(mask);
}

/**
 * @summary Given an image, find the biggest rectangle ROI
 * @param image The image to extract a rectangle ROI from
 * @return The largest rectangle ROI
 */
cv::Rect bashwork::vision::get_largest_rectangle(const cv::Mat &image) {

    int max_area = 0;
    cv::Mat edges;
    cv::vector<cv::vector<cv::Point>> contours;
    cv::vector<cv::Vec4i> hierarchy;
    cv::Rect rectangle;

    cv::Canny(image, edges, low_edge_threshold, max_edge_threshold, edge_kernel_size);
    cv::findContours(edges, contours, hierarchy, CV_RETR_TREE, CV_CHAIN_APPROX_SIMPLE);

    for (auto contour : contours) {
        cv::vector<cv::Point> approximate;
        cv::approxPolyDP(contour, approximate, 3, true);
        cv::Rect possible = cv::boundingRect(approximate);
        if (possible.area() > max_area) {
            rectangle = possible;
            max_area = possible.area();
        }
    }

    return rectangle;
}

/**
 * @summary Process the supplied file with the current algorithm
 * @param file The file to process
 */
void process_image(const fs::path& file) {
    cv::Mat image = cv::imread(file.string(), CV_LOAD_IMAGE_COLOR);
    if (image.data) {
        cv::Mat mask = bw::get_region_mask(image);
        cv::Rect rectangle = bw::get_largest_rectangle(mask);
        display_image(file, image, rectangle);
        std::cout << file << " : " << rectangle << std::endl;
    } else {
        std::cout << "Invalid image supplied: " << file << std::endl;
    }
}

/**
 * @summary Example main runner program
 * @param argc The number of arguments supplied
 * @param argv The arguments supplied
 */
int main(int argc, char** argv) {
    if (argc != 2) {
        std::cout << argv[0] << " <path to image>" << std::endl;
        return -1;
    }

    fs::path root(argv[1]);
    if (fs::exists(root)) {
        if (fs::is_directory(root)) {
            fs::directory_iterator it_end;
            for (auto it = fs::directory_iterator(root); it != it_end; ++ it) {
                if (it->path().extension() == ".jpeg")
                    process_image(*it);
            }
        } else {
            process_image(root);
        }
        cv::waitKey(0);
    } else {
        std::cout << "File or path does not exist" << std::endl;
        return -1;
    }

    return 0;
}
