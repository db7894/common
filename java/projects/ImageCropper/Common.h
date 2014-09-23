#include <opencv2/core/core.hpp>

/**
 * A collection of constants used throughout the library to control
 */
namespace bashwork {
namespace vision {
namespace constant {

    extern cv::Scalar low_white_threshold;
    extern cv::Scalar max_white_threshold;

    extern cv::Scalar low_blue_threshold;
    extern cv::Scalar max_blue_threshold;

    extern const double poly_tolerance;

    extern const int low_edge_threshold;
    extern const int max_edge_threshold;
    extern const int edge_kernel_size;

    extern const int contour_min_corner_threshold;
    extern const int contour_max_corner_threshold;
    extern const double contour_blue_pixel_threshold;
    extern const double contour_ratio_threshold;
    extern const double contour_edge_difference;
    extern const double contour_min_width_threshold;
    extern const double contour_min_height_threshold;
    extern const double contour_max_width_threshold;
    extern const double contour_max_height_threshold;

    extern const int blur_kernel_size;

    extern cv::Size kernel_size;
    extern cv::Mat  morphology_kernel;

    extern const int canny_method;
    extern const int canny_mode;

    extern const double feature_weight_area;
    extern const double feature_weight_skew;
    extern const double feature_weight_edges;
    extern const double feature_weight_ratio;
    extern const double feature_weight_perimiter;
    extern const double feature_weight_centrality;
    extern const double feature_weight_blue_count;
    extern const double feature_weight_other_count;

} // namespace </constant>
} // namespace </vision>
} // namespace </bashwork>
