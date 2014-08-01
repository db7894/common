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

    extern cv::Size kernel_size;
    extern cv::Mat  morphology_kernel;

    extern const int canny_method;
    extern const int canny_mode;

    extern const double feature_weight_area;
    extern const double feature_weight_skew;
    extern const double feature_weight_ratio;
    extern const double feature_weight_convexity;
    extern const double feature_weight_centrality;
    extern const double feature_weight_blue_count;
    extern const double feature_weight_white_count;

} // namespace </constant>
} // namespace </vision>
} // namespace </bashwork>
