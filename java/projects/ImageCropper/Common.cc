#include <opencv2/imgproc/imgproc.hpp>
#include "Common.h"

/**
 * A collection of constants used throughout the library to control
 */
namespace bashwork {
namespace vision {
namespace constant {

    /**
     * Controls the threshold of the white color range for when the
     * white pixels are counted in a bounding contour.
     */
    cv::Scalar low_white_threshold = cv::Scalar(  0,   0, 180);
    cv::Scalar max_white_threshold = cv::Scalar(255,  50, 255);

    /**
     * Controls the threshold of the white color range for when the
     * blue pixels are counted in a bounding contour.
     */
    cv::Scalar low_blue_threshold  = cv::Scalar( 90,  50,  50);
    cv::Scalar max_blue_threshold  = cv::Scalar(155, 255, 255);

    /**
     * Control the amount of noise removed from the image before performing
     * edge detection. The larger the kernel, the less the noise. However,
     * if it is too large, the image one needs to detect may be affected.
     */
    cv::Size kernel_size           = cv::Size(5, 5);
    cv::Mat  morphology_kernel     = cv::getStructuringElement(cv::MORPH_RECT, kernel_size);

    /**
     * Control the operation of the canny edge detection. These options are a
     * tradeoff between precision and processing complexity. For method, no 
     * approximation will get more bounding points (which is useful for contours
     * without strong corners say because of occlusion), but will take more
     * memory and longer to process.
     *
     * For the mode, unless one needs to make use of internal contours, it makes
     * sense to simply use the most external contours. However, one may miss a
     * contour if it is surrounded by another object (which may get interpreted
     * as the parent contour).
     *
     * The remaining parameters control the threshold of the resulting lines
     * that the canny edge detection will return. These are sensitive to tuning,
     * so it makes sense to leave them as open as possible so that positive results
     * are not filtered.
     */
    const int canny_method         = CV_CHAIN_APPROX_NONE;
    const int canny_mode           = CV_RETR_LIST;
    const int low_edge_threshold   = 100;
    const int max_edge_threshold   = 100;
    const int edge_kernel_size     = 3;

} // namespace </constant>
} // namespace </vision>
} // namespace </bashwork>
