#ifndef IMAGE_FEATURES_H_
#define IMAGE_FEATURES_H_

#include <map>
#include <vector>
#include <opencv2/core/core.hpp>
#include <boost/utility.hpp>

namespace bashwork {
namespace vision {

    enum class Feature {
        ContourArea,
        ContourSkew,
        ContourCentrality,
        ContourBlueCount,
        ContourWhiteCount
    };

    class ContourContext : boost::noncopyable {
    public:

        ContourContext(const cv::Mat& image, const cv::vector<cv::Point> contour);
        std::map<Feature, double> get_features();
        double get_score();

        cv::Mat get_image() { return _image; }
        cv::RotatedRect get_rotated() { return _rotated; }
        cv::vector<cv::Point> get_contour() { return _contour; }

    private:

        double feature_contour_area();
        double feature_contour_skew();
        double feature_contour_centrality();
        double feature_contour_blue_count();
        double feature_contour_white_count();

        cv::Mat _image;
        cv::vector<cv::Point> _contour;
        cv::RotatedRect _rotated;
    };

    std::map<Feature, double> get_features(const cv::Mat &image);
};
};

#endif // IMAGE_FEATURES_H_

