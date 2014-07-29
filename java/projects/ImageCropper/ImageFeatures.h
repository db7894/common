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
        ContourConvex,
        ContourCentrality,
        ContourBlueCount,
        ContourWhiteCount
    };

    inline std::ostream &operator<<(std::ostream& stream, const Feature feature) {
        switch (feature) {
            case Feature::ContourArea:       stream << "area   "; break;
            case Feature::ContourSkew:       stream << "skew   "; break;
            case Feature::ContourConvex:     stream << "convex "; break;
            case Feature::ContourCentrality: stream << "central"; break;
            case Feature::ContourBlueCount:  stream << "blues  "; break;
            case Feature::ContourWhiteCount: stream << "whites "; break;
        }
        return stream;
    }

    class ContourContext : boost::noncopyable {
    public:

        ContourContext(const std::pair<cv::Mat, cv::Mat>& parts, const cv::vector<cv::Point>& contour);
        void initialize();
        std::map<Feature, double> get_features();
        double get_score();

        cv::vector<cv::Point> get_polygon() { return _polygon; }
        cv::Rect get_rectangle() { return _rectangle; }
        cv::RotatedRect get_rotated_rectangle() { return _rotated; }

    private:

        double feature_contour_area();
        double feature_contour_convex();
        double feature_contour_skew();
        double feature_contour_centrality();
        double feature_contour_pixel_count(const cv::Mat& mask);

        cv::vector<cv::Point> _contour;
        cv::vector<cv::Point> _polygon;
        cv::Mat _blue_mask;
        cv::Mat _white_mask;
        cv::Mat _contour_mask;
        cv::Size _size;
        cv::RotatedRect _rotated;
        double _perimiter;
        cv::Rect _rectangle;
    };

};
};

#endif // IMAGE_FEATURES_H_
