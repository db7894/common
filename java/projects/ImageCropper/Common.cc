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
     * Control the window to be examined to control the median blur
     * to remove noise from the edge detection.
     */
    const int blur_kernel_size     = 3;

    /**
     * Control the amount of noise removed from the image before performing
     * edge detection. The larger the kernel, the less the noise. However,
     * if it is too large, the image one needs to detect may be affected.
     */
    cv::Size kernel_size           = cv::Size(9, 9);
    cv::Mat  morphology_kernel     = cv::getStructuringElement(cv::MORPH_RECT, kernel_size);

    /**
     * Control the approximation of the polygon given a contour.
     */
    const double poly_tolerance    = 0.1;

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
    const int canny_mode           = CV_RETR_EXTERNAL;
    const int low_edge_threshold   = 100;
    const int max_edge_threshold   = 250;
    const int edge_kernel_size     = 3;

    /**
     * Control the various thresholding of the features
     * based on extreme values that are indicative of invalid
     * regions. Tune these carefully and keep in mind that occlusions,
     * rotations, external entities, and various lighting conditions
     * will affect the idealized values!
     */
    const int    contour_min_corner_threshold  =    3;
    const int    contour_max_corner_threshold  =   15;
    const double contour_ratio_threshold       =    2.0;
    const double contour_edge_difference       =  -50.0;
    const double contour_blue_pixel_threshold  =  100.0;
    const double contour_min_width_threshold   =   50.0;
    const double contour_min_height_threshold  =   50.0;
    const double contour_max_width_threshold   = 1000.0;
    const double contour_max_height_threshold  = 1000.0;


    /**
     * Control the value that each feature adds to the overall score of
     * the contour it is applicable to.
     */
    const double feature_weight_area        =    0.1;
    const double feature_weight_skew        =  -10.0;
    const double feature_weight_edges       =   10.0;
    const double feature_weight_ratio       = -100.0;
    const double feature_weight_perimiter   =    0.0;
    const double feature_weight_centrality  =   -0.1;
    const double feature_weight_blue_count  =    0.1;
    const double feature_weight_other_count =   -0.1;

} // namespace </constant>
} // namespace </vision>
} // namespace </bashwork>
