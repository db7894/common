#include <iostream>
#include <opencv2/imgproc/imgproc.hpp>

#include "Common.h"
#include "ImageFeatures.h"
#include "NativeImageCropper.h"

namespace constant = bashwork::vision::constant;

/**
 * The following are the simple features that are computed for each
 * contour found for the supplied image. All of these will be combined
 * in a linear fashion to give the single contour an overall score which
 * can be compared among its peers to determine the best contour from the
 * group. They are defined here to make sure they are inlined to increase
 * performance and to abstract their underlying implementation.
 */
namespace {

    inline double get_contour_perimiter(const cv::vector<cv::Point>& contour) {
        return cv::arcLength(contour, true) * constant::poly_tolerance;
    }

    inline double get_contour_area(const cv::vector<cv::Point>& contour) {
        return cv::contourArea(contour);
    }
   
    inline double get_contour_skew(const cv::RotatedRect& rectangle) {
        double angle = std::abs(rectangle.angle);
        return std::min(angle, 90.0 - angle);
    }

    inline double get_contour_ratio(const cv::RotatedRect& rectangle) {
        if ((rectangle.size.height == 0)
         || (rectangle.size.width  == 0)) return 0.0;

        return (rectangle.size.height > rectangle.size.width)
            ?  (rectangle.size.height / rectangle.size.width)
            :  (rectangle.size.width  / rectangle.size.height);
    }
    
    inline double get_contour_centrality(const cv::RotatedRect& rectangle, const cv::Size& size) {
        return cv::norm((cv::Point2f(size) * 0.5) - rectangle.center);
    }

    inline double get_contour_pixel_count(const cv::Mat& mask, const cv::Mat& contour) {
        return cv::countNonZero(mask & contour);
    }

    inline double get_contour_other_pixel_count(const cv::Mat& mask, const cv::Mat& contour) {
        return cv::countNonZero(contour) - cv::countNonZero(mask & contour);
    }

    inline double get_edge_difference(const cv::RotatedRect& rectangle, const cv::vector<cv::Point> contour) {
        double difference = 0.0;
        cv::Point2f points[4];
        rectangle.points(points);

        for (auto point : points) {
            difference += cv::pointPolygonTest(contour, point, true);
        }
        return difference;
    }
}

namespace bashwork {
namespace vision {

    /**
     * @summary Initialize a new instance of the ContourContext class
     * @param image The image that this contour is associated with
     * @param contour The contour to build a context for
     */
    ContourContext::ContourContext(const cv::vector<cv::Point>& contour, const cv::Size& image_size) :
      _contour(contour),
      _image_size(image_size),
      _contour_mask(cv::Mat::zeros(_image_size, CV_8UC1))
    {}

    /**
     * @summary Perform any complex initialization of the context
     * @return void
     */
    void ContourContext::initialize(const cv::Mat& blue_mask, const cv::Mat& white_mask) {
        // initialize the structures we will calculate features with
        cv::convexHull(cv::Mat(_contour), _contour_hull, false);
        cv::drawContours(_contour_mask, cv::vector<cv::vector<cv::Point>>(1, _contour_hull), -1, cv::Scalar(255), CV_FILLED);
        _rotated_rectangle = cv::minAreaRect(_contour_hull);
        //cv::Point2f points[4];
        //_rotated_rectangle.points(points);
        //_contour_edge_difference = 0.0;
        //for (auto point : points) {
        //    _contour_edge_difference += cv::pointPolygonTest(_contour_hull, point, true);
        //}
        _rectangle = cv::boundingRect(_contour_hull);

        // calculate the features so we can use them in validation
        _contour_edge_difference   = get_edge_difference(_rotated_rectangle, _contour_hull);
        _contour_perimiter         = get_contour_perimiter(_contour_hull);
        _contour_area              = get_contour_area(_contour_hull);
        _contour_skew              = get_contour_skew(_rotated_rectangle);
        _contour_ratio             = get_contour_ratio(_rotated_rectangle);
        _contour_centrality        = get_contour_centrality(_rotated_rectangle, _image_size);
        _contour_blue_pixel_count  = get_contour_pixel_count(blue_mask, _contour_mask);
        _contour_other_pixel_count = get_contour_other_pixel_count(white_mask, _contour_mask)
                                   - _contour_blue_pixel_count;

        //cv::vector<cv::Point> _polygon;
        //cv::approxPolyDP(_contour, _polygon, _perimiter, true);
        //_rectangle = cv::boundingRect(_polygon);
        //_rectangle = _rotated.boundingRect();
    }

    /**
     * @summary Perform a quick validation of this contour
     * @return true if this is valid, false otherwise
     */
    bool ContourContext::is_valid() {
        return cv::isContourConvex(_contour_hull)
            //&& (_contour_edge_difference       > constant::contour_edge_difference)
            && (_contour_hull.size()           > constant::contour_min_corner_threshold)
            //&& (_contour_hull.size()           < constant::contour_max_corner_threshold)
            && (_contour_ratio                 < constant::contour_ratio_threshold)
            //&& (_contour_blue_pixel_count      > constant::contour_blue_pixel_threshold)
            && (_rotated_rectangle.size.width  > constant::contour_min_width_threshold)
            && (_rotated_rectangle.size.height > constant::contour_min_height_threshold)
            && (_rotated_rectangle.size.width  < constant::contour_max_width_threshold)
            && (_rotated_rectangle.size.height < constant::contour_max_height_threshold);
    }

    /**
     * @summary Given an image contour, extract a number of features for it
     * @return A collection of features for this contour
     */
    std::map<Feature, double> ContourContext::get_features() {
        std::map<Feature, double> features;

        features.insert(std::make_pair(Feature::ContourArea, _contour_area));
        features.insert(std::make_pair(Feature::ContourSkew, _contour_skew));
        features.insert(std::make_pair(Feature::ContourEdges, _contour_edge_difference));
        features.insert(std::make_pair(Feature::ContourRatio, _contour_ratio));
        features.insert(std::make_pair(Feature::ContourCorners, _contour_hull.size()));
        features.insert(std::make_pair(Feature::ContourPerimiter, _contour_perimiter));
        features.insert(std::make_pair(Feature::ContourCentrality, _contour_centrality));
        features.insert(std::make_pair(Feature::ContourBlueCount, _contour_blue_pixel_count));
        features.insert(std::make_pair(Feature::ContourOtherCount, _contour_other_pixel_count));

        return features;
    }

    /**
     * @summary Given a number of features, calculate the score for this contour
     * @return The resulting score for this contour.
     */
    double ContourContext::get_score() {

        double result = 0.0;

        result += _contour_area              * constant::feature_weight_area;
        result += _contour_skew              * constant::feature_weight_skew;
        result += _contour_ratio             * constant::feature_weight_ratio;
        result += _contour_perimiter         * constant::feature_weight_perimiter;
        result += _contour_centrality        * constant::feature_weight_centrality;
        result += _contour_blue_pixel_count  * constant::feature_weight_blue_count;
        result += _contour_other_pixel_count * constant::feature_weight_other_count;

        return result;
    }

}; // namespace </vision>
}; // namespace </bashwork>
