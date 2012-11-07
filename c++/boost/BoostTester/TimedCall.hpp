/**
 * @file TimedCall.hpp
 * @brief A generic wrapper around performing a timed call
 * @author Galen Collins
 */
#pragma once

#ifndef BASHWORK_TIMED_CALL_H
#define BASHWORK_TIMED_CALL_H

//----------------------------------------------------------------------------//
// Includes
//----------------------------------------------------------------------------//
#include <boost/asio.hpp>
#include <boost/thread.hpp>
#include <boost/noncopyable.hpp>
#include <boost/function.hpp>

//----------------------------------------------------------------------------//
// Implementation
//----------------------------------------------------------------------------//
/**
 * @brief A generic wrapper around performing a timed call
 *
 * Example usage:
 * @include timed_call.cpp
 */
class TimedCall : boost::noncopyable
{
public:
	/**
	 * @brief Initializes the timed caller
	 * @param call The function to call on timer
	 * @param wait How many milliseconds between calls (detault 1000)
	 * @param threads The number of worker threads (default 1)
	 */
	TimedCall(boost::function<void()> call, unsigned long wait=1000, size_t threads=1)
		: scheduler_(),
		  wait_(wait),
		  work_timer_(scheduler_, boost::posix_time::milliseconds(wait)),
		  call_(call)
    {
		work_timer_.async_wait(
			boost::bind(&TimedCall::runner, this));
		for (size_t i = 0; i < threads; ++i) {
			threads_.create_thread(boost::bind(
				&boost::asio::io_service::run, &scheduler_));
		}
    };

	/**
	 * @brief Stops running the timed call
	 */
	~TimedCall(void)
	{
		scheduler_.stop(); /* will stop the timer */
    };

	/**
	 * @brief Changes the delay time between calls
	 * @param wait The number of milliseconds to delay
	 * @return void
	 */
	void SetDelay(unsigned long wait)
	{
		wait_ = wait;
	}

private:
	/**
	 * @brief Timer runner
	 * @return void
	 */
	void runner(void)
	{
		call_();
		/* restart the timer */
		work_timer_.expires_at(work_timer_.expires_at()
			+ boost::posix_time::milliseconds(wait_));
		work_timer_.async_wait(
			boost::bind(&TimedCall::runner, this));
	}

private:
	/**
	 * @brief Handle to the worker thread pool
	 */
	boost::thread_group threads_;
	/**
	 * @brief Time to wait between calls
	 */
	int wait_;
	/**
	 * @brief Handle to the function to call on timer
	 */
	boost::function<void()> call_;
	/**
	 * @brief The underlying OS handle
	 */
	boost::asio::io_service scheduler_;
	/**
	 * @brief Handle to the work timer
	 */
	boost::asio::deadline_timer work_timer_;	
};

#endif
