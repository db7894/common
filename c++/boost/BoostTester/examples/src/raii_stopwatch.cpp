/**
 * @file raii_stopwatch.cpp
 * @brief Example of a raii stopwatch
 */
#include <iostream>
#include <string>
#include "StopWatch.hpp"

int main(void)
{
	{ /* forced scope */
		StopWatch watch;
		for (int i = 0; i < 1000; ++i) {
			int j = i * i;
		}
	}
	return 0;
}

