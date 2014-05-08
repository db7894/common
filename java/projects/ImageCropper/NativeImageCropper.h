#ifndef IMAGE_CROPPER_H_
#define IMAGE_CROPPER_H_

#include <opencv2/core/core.hpp>

namespace bashwork {
namespace vision {
    cv::Mat  get_region_mask(const cv::Mat &image);
    cv::Rect get_largest_rectangle(const cv::Mat &image);
};
};

#endif // IMAGE_CROPPER_H_
