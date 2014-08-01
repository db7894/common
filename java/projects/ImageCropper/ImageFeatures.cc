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
    ContourContext::ContourContext(const std::pair<cv::Mat, cv::Mat>& parts, const cv::vector<cv::Point>& contour) :
      _contour(contour),
      _blue_mask(std::get<0>(parts)),
      _white_mask(std::get<1>(parts)),
      _size(_blue_mask.size())
    {}

    /**
     * @summary Perform any complex initialization of the context
     * @return void
     */
    void ContourContext::initialize() {
        _rotated = cv::minAreaRect(_contour);
        _perimiter = cv::arcLength(_contour, true) * constant::poly_tolerance;
        cv::approxPolyDP(_contour, _polygon, _perimiter, true);
        _rectangle = cv::boundingRect(_polygon);
        _contour_mask = cv::Mat::zeros(_size, CV_8UC1);
        cv::drawContours(_contour_mask, cv::vector<cv::vector<cv::Point>>(1, _contour), -1, cv::Scalar(255), CV_FILLED);
    }

    /**
     * @summary Given an image contour, extract a number of features for it
     * @return A collection of features for this contour
     */
    std::map<Feature, double> ContourContext::get_features() {
        std::map<Feature, double> features;

        features.insert(std::make_pair(Feature::ContourArea, feature_contour_area()));
        features.insert(std::make_pair(Feature::ContourSkew, feature_contour_skew()));
        features.insert(std::make_pair(Feature::ContourRatio, feature_contour_ratio()));
        features.insert(std::make_pair(Feature::ContourConvex, feature_contour_convex()));
        features.insert(std::make_pair(Feature::ContourCentrality, feature_contour_centrality()));
        features.insert(std::make_pair(Feature::ContourBlueCount, feature_contour_pixel_count(_blue_mask)));
        features.insert(std::make_pair(Feature::ContourWhiteCount, feature_contour_pixel_count(_white_mask)));

        return features;
    }

    /**
     * @summary Given a number of features, calculate the score for this contour
     * @return The resulting score for this contour.
     */
    double ContourContext::get_score() {

        double result = 0.0;

        result += feature_contour_area()                   * constant::feature_weight_area;
        result += feature_contour_skew()                   * constant::feature_weight_skew;
        result += feature_contour_ratio()                  * constant::feature_weight_ratio;
        result += feature_contour_convex()                 * constant::feature_weight_convexity;
        result += feature_contour_centrality()             * constant::feature_weight_centrality;
        result += feature_contour_pixel_count(_blue_mask)  * constant::feature_weight_blue_count;
        result += feature_contour_pixel_count(_white_mask) * constant::feature_weight_white_count;

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
        double angle = std::abs(_rotated.angle);
        return std::min(angle, 90.0 - angle);
    }

    double ContourContext::feature_contour_ratio() {
        if ((_rotated.size.height == 0)
         || (_rotated.size.width  == 0)) return 0.0;

        return (_rotated.size.height > _rotated.size.width)
            ?  (_rotated.size.height / _rotated.size.width)
            :  (_rotated.size.width  / _rotated.size.height);
    }

    double ContourContext::feature_contour_convex() {
        return (double)cv::isContourConvex(_polygon);
    }
    
    double ContourContext::feature_contour_centrality() {
        cv::Point2f size(_size);
        return cv::norm((size * 0.5) - _rotated.center);
    }
    
    double ContourContext::feature_contour_pixel_count(const cv::Mat& mask) {
        return cv::countNonZero(mask & _contour_mask);
    }

}; // namespace </vision>
}; // namespace </bashwork>
