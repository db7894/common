#ifndef IMAGE_FEATURES_H_
#define IMAGE_FEATURES_H_

#include <map>
#include <vector>
#include <opencv2/core/core.hpp>
#include <boost/utility.hpp>

namespace bashwork {
namespace vision {

    /**
     * @summary Represents the feature associated with a value
     */
    enum class Feature {
        ContourArea,
        ContourEdges,
        ContourPerimiter,
        ContourSkew,
        ContourRatio,
        ContourCorners,
        ContourCentrality,
        ContourBlueCount,
        ContourOtherCount
    };

    /**
     * @summary Given a Feature enumeration, print out its string value
     * @param stream The output stream to write to
     * @param feature The enum feature to wrtie to stream
     * @return The stream we wrote to
     */
    inline std::ostream &operator<<(std::ostream& stream, const Feature feature) {
        switch (feature) {
            case Feature::ContourArea:       stream << "area   "; break;
            case Feature::ContourSkew:       stream << "skew   "; break;
            case Feature::ContourEdges:      stream << "edges  "; break;
            case Feature::ContourPerimiter:  stream << "perim  "; break;
            case Feature::ContourRatio:      stream << "ratio  "; break;
            case Feature::ContourCorners:    stream << "corners"; break;
            case Feature::ContourCentrality: stream << "central"; break;
            case Feature::ContourBlueCount:  stream << "blues  "; break;
            case Feature::ContourOtherCount: stream << "other  "; break;
        }
        return stream;
    }

    /**
     * @summary Context manager for performing contour feature extration
     */
    class ContourContext : boost::noncopyable {
    public:

        ContourContext(const cv::vector<cv::Point>& contour, const cv::Size& image_size);
        void initialize(const cv::Mat& blue_mask, const cv::Mat& white_mask);
        std::map<Feature, double> get_features();
        double get_score();
        bool is_valid();

        cv::vector<cv::Point> get_contour() const { return _contour_hull; }
        cv::Rect get_rectangle() const { return _rectangle; }
        cv::RotatedRect get_rotated_rectangle() const { return _rotated_rectangle; }
        cv::Size get_rectangle_size() const { return _rotated_rectangle.size; }
        cv::Point get_rectangle_center() const { return _rotated_rectangle.center; }
        cv::Mat get_contour_mask() const { return _contour_mask; }

    private:

        double _contour_perimiter;
        double _contour_area;
        double _contour_skew;
        double _contour_ratio;
        double _contour_centrality;
        double _contour_blue_pixel_count;
        double _contour_other_pixel_count;
        double _contour_edge_difference;

        cv::vector<cv::Point> _contour;
        cv::vector<cv::Point> _contour_hull;
        cv::Size _image_size;
        cv::Mat _contour_mask;
        cv::Rect _rectangle;
        cv::RotatedRect _rotated_rectangle;
    };

};
};

#endif // IMAGE_FEATURES_H_
