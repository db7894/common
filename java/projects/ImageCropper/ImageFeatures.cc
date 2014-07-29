#include <opencv2/imgproc/imgproc.hpp>

#include "Common.h"
#include "ImageFeatures.h"
#include "NativeImageCropper.h"

namespace bashwork {
namespace vision {

    /**
     * @summary Initialize a new instance of the ContourContext class
     * @param image The image that this contour is associated with
     * @param contour The contour to build a context for
     */
    ContourContext::ContourContext(const cv::Mat& image, const cv::vector<cv::Point> contour) :
      _image(image),
      _contour(contour),
      _rotated(cv::minAreaRect(contour))
    { }

    /**
     * @summary Given an image contour, extract a number of features for it
     * @return A collection of features for this contour
     */
    std::map<Feature, double> ContourContext::get_features() {
        std::map<Feature, double> features;

        features.insert(std::make_pair(Feature::ContourArea, feature_contour_area()));
        features.insert(std::make_pair(Feature::ContourSkew, feature_contour_skew()));
        features.insert(std::make_pair(Feature::ContourCentrality, feature_contour_centrality()));
        features.insert(std::make_pair(Feature::ContourBlueCount, feature_contour_blue_count()));
        features.insert(std::make_pair(Feature::ContourWhiteCount, feature_contour_white_count()));

        return features;
    }

    /**
     * @summary Given a number of features, calculate the score for this contour
     * @return The resulting score for this contour.
     */
    double ContourContext::get_score() {

        double result = 0.0;

        result += feature_contour_area();
        result += feature_contour_skew();
        result += feature_contour_centrality();
        result += feature_contour_blue_count();
        result += feature_contour_white_count();

        return result;
    }

    /**
     * The following are the simple features that are computed for each
     * contour found for the supplied image. All of these will be combined
     * in a linear fashion to give the single contour an overall score which
     * can be compared among its peers to determine the best contour from the
     * group.
     */

    double ContourContext::feature_contour_area() {
        return cv::contourArea(_contour);
    }
    
    double ContourContext::feature_contour_skew() {
        return _rotated.angle;
    }
    
    double ContourContext::feature_contour_centrality() {
        cv::Point2f size(_image.size());
        return cv::norm((size * 0.5) - _rotated.center);
    }
    
    double ContourContext::feature_contour_blue_count() {
        cv::Mat mask;
        cv::inRange(_image, constant::low_blue_threshold, constant::max_blue_threshold, mask);
        return cv::countNonZero(mask);
    }
    
    double ContourContext::feature_contour_white_count() {
        cv::Mat mask;
        cv::inRange(_image, constant::low_white_threshold, constant::max_white_threshold, mask);
        return cv::countNonZero(mask);
    }

}; // namespace </vision>
}; // namespace </bashwork>
