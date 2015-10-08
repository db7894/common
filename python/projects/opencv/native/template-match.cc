#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <iostream>

/**
 * Function to perform fast template matching with image pyramid
 *
 * @param srca The reference image
 * @param srcb The template image
 * @param max_level The maximum size of the pyramid
 * @return The matching image
 */
cv::Mat fast_match(cv::Mat& srca, cv::Mat& srcb, int max_level) {
    std::vector<cv::Mat> refs, tpls, results;

    // Build Gaussian pyramid
    cv::buildPyramid(srca, refs, max_level);
    cv::buildPyramid(srcb, tpls, max_level);

    cv::Mat ref, tpl, res;

    // Process each level
    for (int level = max_level; level >= 0; level--) {
        ref = refs[level];
        tpl = tpls[level];
        res = cv::Mat::zeros(ref.size() + cv::Size(1,1) - tpl.size(), CV_32FC1);

        if (level == max_level) {
            // On the smallest level, just perform regular template matching
            cv::matchTemplate(ref, tpl, res, CV_TM_CCORR_NORMED);
        } else {
            // On the next layers, template matching is performed on pre-defined 
            // ROI areas.  We define the ROI using the template matching result 
            // from the previous layer.

            cv::Mat mask;
            cv::pyrUp(results.back(), mask);

            cv::Mat mask8u;
            mask.convertTo(mask8u, CV_8U);

            // Find matches from previous layer
            std::vector<std::vector<cv::Point> > contours;
            cv::findContours(mask8u, contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_NONE);

            // Use the contours to define region of interest and 
            // perform template matching on the areas
            for (int i = 0; i < contours.size(); i++) {
                cv::Rect r = cv::boundingRect(contours[i]);
                cv::matchTemplate(
                    ref(r + (tpl.size() - cv::Size(1,1))), 
                    tpl, 
                    res(r), 
                    CV_TM_CCORR_NORMED
                );
            }
        }

        // Only keep good matches
        cv::threshold(res, res, 0.94, 1., CV_THRESH_TOZERO);
        results.push_back(res);
    }

    return res;
}

int main(int argc, char** argv) {

    if (argc < 3) {
        std::cout << argv[0] << " <image> <template>" << std::endl;
        return -1;
    }

    cv::Mat ref = cv::imread(argv[1]);
    cv::Mat tpl = cv::imread(argv[2]);

    if (ref.empty() || tpl.empty()) {
        return -1;
    }


    bool is_finished = false;
    cv::Mat ref_gray, tpl_gray;
    cv::cvtColor(ref, ref_gray, CV_BGR2GRAY);
    cv::cvtColor(tpl, tpl_gray, CV_BGR2GRAY);
    cv::Mat dst = fast_match(ref_gray, tpl_gray, 2);

    while (!is_finished) {
        double minval, maxval;
        cv::Point minloc, maxloc;
        cv::minMaxLoc(dst, &minval, &maxval, &minloc, &maxloc);

        if (maxval >= 0.9) {
            cv::rectangle(ref, maxloc, 
                cv::Point(maxloc.x + tpl.cols, maxloc.y + tpl.rows), 
                CV_RGB(0,255,0), 2);

            cv::floodFill(dst, maxloc, 
                cv::Scalar(0), 0, cv::Scalar(.1), cv::Scalar(1.));
        } else {
            is_finished = true;
        }
    }

    cv::imshow("result", ref);
    cv::waitKey();
    return 0;
}
