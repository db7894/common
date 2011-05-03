/**
 * @file manual_stopwatch.cpp
 * @brief Example of a manually using the stopwatch
 */
#include <cmath>
#include "StopWatch.hpp"

int main(void)
{
	StopWatch watch;
	for (int i = 0; i < 30; ++i) {
		watch.Start();
		std::cout << std::pow(2.0, (double)i) << "\n";
		watch.Stop("Time Required For Loop Iteration");
	}
	return 0;
}

