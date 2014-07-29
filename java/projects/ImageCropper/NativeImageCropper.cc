#include <iostream>
#include <boost/filesystem.hpp>
#include "boost/program_options.hpp" 
#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>

#include "Common.h"
#include "NativeImageCropper.h"

namespace po = boost::program_options; 
namespace fs = boost::filesystem;
namespace bw = bashwork::vision;

namespace bashwork {
namespace vision {

    /**
     * @summary A utility to automatically crop a pick region
     * @param image The image to get a region mask for
     * @param mask The location to store the resulting region mask
     * @return void
     */
    cv::Mat get_region_mask(const cv::Mat &image) {
        cv::Mat mask;
        cv::Mat hsv_image;
        cv::Mat dirty_mask;

        cv::cvtColor(image, hsv_image, CV_BGR2HSV);
        cv::inRange(hsv_image, constant::low_blue_threshold, constant::max_blue_threshold, dirty_mask);
        cv::morphologyEx(dirty_mask, mask, cv::MORPH_OPEN, constant::morphology_kernel);

        return std::move(mask);
    }

    /**
     * @summary Given an image, find the collection of contours.
     * @param image The image to extract the contours for.
     * @return The collection of contours.
     */
    cv::vector<cv::vector<cv::Point>> get_image_contours(const cv::Mat &image) {

        cv::Mat im_blur;
        cv::Mat im_edge;
        cv::Mat im_open;
        cv::vector<cv::vector<cv::Point>> contours;
        cv::vector<cv::Vec4i> hierarchy;
        cv::Rect rectangle;

        cv::GaussianBlur(image, im_blur, constant::kernel_size, 0, 0);
        cv::Canny(image, im_edge, constant::low_edge_threshold, constant::max_edge_threshold, constant::edge_kernel_size);
        cv::morphologyEx(im_edge, im_open, cv::MORPH_OPEN, constant::morphology_kernel);
        cv::findContours(im_open, contours, hierarchy, constant::canny_mode, constant::canny_method);

        return contours;
    }

    /**
     * @summary Given an image, find the biggest rectangle ROI
     * @param image The image to extract a rectangle ROI from
     * @return The largest rectangle ROI
     */
    cv::Rect get_largest_rectangle(const cv::Mat &image) {

        int max_area = 0;
        cv::Mat edges;
        cv::vector<cv::vector<cv::Point>> contours;
        cv::vector<cv::Vec4i> hierarchy;
        cv::Rect rectangle;

        cv::Canny(image, edges, constant::low_edge_threshold, constant::max_edge_threshold, constant::edge_kernel_size);
        cv::findContours(edges, contours, hierarchy, constant::canny_mode, constant::canny_method);

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

} // namespace </vision>
} // namespace </bashwork>


/**
 * utility methods for the console application
 */
namespace {

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
     * @summary Process the supplied file with the current algorithm
     * @param file The file to process
     */
    void process_image(const fs::path& file, bool is_debug=false) {
        cv::Mat image = cv::imread(file.string(), CV_LOAD_IMAGE_COLOR);
        if (image.data) {
            cv::Mat mask = bw::get_region_mask(image);
            cv::Rect rectangle = bw::get_largest_rectangle(mask);
            if (rectangle.area() > 0) {
                if (is_debug) {
                    display_image(file, image, rectangle);
                }
                std::cout << file << " : " << rectangle << std::endl;
            } else {
                std::cout << file << " : " << "no rectangle found" << std::endl;
            }
        } else {
            std::cout << "Invalid image supplied: " << file << std::endl;
        }
    }

} // namespace </>

/**
 * @summary Example main runner program
 * @param argc The number of arguments supplied
 * @param argv The arguments supplied
 */
int main(int argc, char** argv) {

    po::variables_map options; 
    po::options_description description("Options"); 
    description.add_options() 
        ("help,h", "Print this help message") 
        ("path,p", po::value<std::string>()->required(), "Path to the images to process") 
        ("debug,d", "Enable image debug viewing") ;

    try {
        po::store(po::parse_command_line(argc, argv, description), options);

        if (options.count("help")) {
            std::cout << description << std::endl; 
            return 0;
        }
        po::notify(options);

    } catch (po::error& ex) { 
        std::cerr << "ERROR: " << ex.what() << std::endl << std::endl; 
        std::cerr << description << std::endl; 
        return -1;
    } 

    bool is_debug = options.count("debug");
    fs::path root(options["path"].as<std::string>());

    if (fs::exists(root)) {
        if (fs::is_directory(root)) {
            fs::directory_iterator it_end;
            for (auto it = fs::directory_iterator(root); it != it_end; ++ it) {
                if (it->path().extension() == ".jpeg")
                    process_image(*it, is_debug);
            }
        } else {
            process_image(root);
        }
        if (is_debug) {
            cv::waitKey(0);
        }
    } else {
        std::cout << "File or path does not exist" << std::endl;
        return -1;
    }

    return 0;
}
