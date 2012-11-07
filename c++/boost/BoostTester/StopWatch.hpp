/**
 * @file StopWatch.hpp
 * @brief RAII Timer for testing speed of some activity
 * @author Galen Collins
 */
#pragma once

#ifndef BASHWORK_STOPWATCH_H
#define BASHWORK_STOPWATCH_H
//----------------------------------------------------------------------------//
// Includes
//----------------------------------------------------------------------------//
/**
 * @brief Uncomment to use the high resolution timer which is
 * a great deal more accurate (...sometimes...) than clock() 
 */
//#define CLOCK_IMPLEMENTATION
#ifdef CLOCK_IMPLEMENTATION
#	include <ctime>
#else
#	include <windows.h>
#endif
#include <iostream>
#include <string>

//---------------------------------------------------------------------------//
// Implementation
//---------------------------------------------------------------------------//
/**
 * @brief RAII timer for testing the speed of some activity
 *
 * Simple Examles:
 * @include manual_stopwatch.cpp
 * @include raii_stopwatch.cpp
 */
class StopWatch
{
public:
	/**
	 * @brief Initializes the start time
	 */
	StopWatch(void)
	{
#ifdef CLOCK_IMPLEMENTATION
		start_(std::clock())
#else
		if (QueryPerformanceFrequency((LARGE_INTEGER *)&frequency_) == false)
			throw new std::exception("High Resolution Timer Not Supported");
		Start();
#endif
	}

	/**
	 * @brief Calls stop to print the final time
	 */
	~StopWatch(void)
	{
		Stop("Total Running Time");
	}

	/**
	 * @brief Used to manually update the start time
	 * @return void
	 */
	void Start(void)
	{
#ifdef CLOCK_IMPLEMENTATION
		start_ = std::clock();
#else
		QueryPerformanceCounter((LARGE_INTEGER *)&start_);
#endif
	}

	/**
	 * @brief Used to manually call a stop checkpoint
	 * @param message The message to append to the time stamp
	 * @return void
	 */
	void Stop(std::string message)
	{
#ifdef CLOCK_IMPLEMENTATION
		clock_t total = clock() - start_;
		std::cout << message << ":\t" << total;
		std::cout << "\t" << (double)total/CLOCKS_PER_SEC << " seconds\n";
#else
		__int64 total;
		QueryPerformanceCounter((LARGE_INTEGER *)&total);
		total = total - start_;
		std::cout << message << ":\n" << total << " ticks\t";
		std::cout << (total * 1.0)/frequency_ << " secs\n";
#endif
	}

private:
#ifdef CLOCK_IMPLEMENTATION
	std::clock_t start_;
#else
	__int64 frequency_;
	__int64 start_;
#endif
};

#endif
