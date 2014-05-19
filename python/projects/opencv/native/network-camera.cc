#include "cv.h"
#include "highgui.h"
#include <iostream>

int main(int, char**) {
    cv::VideoCapture capture;
    cv::Mat image;

    const std::string address = "rtsp://10.59.142.36:554/live/av0?user=admin&passwd=admin"; 
    //const std::string address = "rtsp://10.59.136.204:554/live/av0?user=admin&passwd=admin"; 

    //open the video stream and make sure it's opened
    if(!capture.open(address)) {
        std::cout << "Error opening video stream or file" << std::endl;
        return -1;
    }

    cv::namedWindow("Output Window");

    while (true) {
        if (!capture.read(image)) {
            std::cout << "No frame" << std::endl;
            cv::waitKey();
        }
        cv::imshow("Output Window", image);
        if (cv::waitKey(1) >= 0) break;
    }   
}
