/**
 * @file ActiveObject.hpp
 * @brief Simple base active object
 * @author Galen Collins
 */
#pragma once

#ifndef ACTIVE_OBJECT_H
#define ACTIVE_OBJECT_H

//----------------------------------------------------------------------------//
// Includes
//----------------------------------------------------------------------------//
#include <boost/asio.hpp>
#include <boost/noncopyable.hpp>
#include <boost/shared_ptr.hpp>
#include <boost/thread.hpp>
#include <boost/bind.hpp>
#include <boost/function.hpp>

//----------------------------------------------------------------------------//
// Implementation
//----------------------------------------------------------------------------//

/**
 * @brief An active object implementation using boost::asio
 * @note The derived class must implement void initialize() and void destroy()
 *
 * This is made to be inherited and have a derived class implement some work
 * function.  Something like a functor in a separate thread.
 *
 * Example usage:
 * @code
 *  struct example : ActiveObject {
 *    void work(void) {
 *      boost::function<void()> function = boost::bind(&example::pimpl, this);
 *      schedule(function);
 *    }
 *
 *    void initialize() {
 *      std::cout << "Performing Initialization\n";
 *    }
 *
 *    void destroy() {
 *      std::cout << "Performing Destruction\n";
 *    }
 *
 *  private:
 *    void pimpl(void) {
 *      std::cout << "Performing Work\n";
 *    }
 *  };
 * @endcode
 */
template <class Derived>
class ActiveObject : boost::noncopyable
{
public:
	/**
	 * @brief Initializes the background thread
	 */
    ActiveObject(void)
		: scheduler_(),
		  work_(scheduler_)
    {
		executioner_.reset(new boost::thread(
			boost::bind(&boost::asio::io_service::run, &scheduler_)));
		static_cast<Derived *>(this)->initialize();
    };

protected:
	/**
	 * @brief Destroys the active object and stops the background thread
	 */
	~ActiveObject(void)
	{
		scheduler_.stop();
		executioner_->join();
		static_cast<Derived *>(this)->destroy();
    };

	/**
	 * @brief External function to trigger a unit of work
	 * @param action The unit of work to perform
	 * @return void
	 */
	//template <typename T>
	inline void schedule(boost::function<void()> action)
	{
        scheduler_.post(action);
    };

private:
	/**
	 * @brief The underlying OS handle
	 */
	boost::asio::io_service scheduler_;
	/**
	 * @brief The work spinner
	 */
	boost::asio::io_service::work work_;
	/**
	 * @brief The handle to the active object worker thread
	 */
	boost::shared_ptr<boost::thread> executioner_;
};

/**
 * @brief A generic active object implementation using boost::asio
 *
 * This class is made to be used as a tag-along helper.  Instead of 
 * creating a thread for each piece of blocking work, simply throw
 * it on the work queue and it will get done when the thread gets a
 * change.
 *
 * Example usage:
 * @code
 *  void blockingSend(const char* buffer);
 *  ...
 *  GenericActiveObject helper;
 *  char *buffer = "message";
 *  helper.schedule(boost::bind(&blockingSend, buffer));
 *  std::cout << "I can keep working in this thread\n";
 * @endcode
 */
class GenericActiveObject : boost::noncopyable
{
public:
	/**
	 * @brief Initializes the background thread
	 * @param threads The number of worker threads to use in the background
	 */
    GenericActiveObject(size_t threads=1)
		: scheduler_(),
		  work_(scheduler_)
    {
		for (size_t i = 0; i < threads; ++i) {
			threads_.create_thread(boost::bind(
				&boost::asio::io_service::run, &scheduler_));
		}
    };

	/**
	 * @brief Destroys the active object and stops the background thread
	 */
	~GenericActiveObject(void)
	{
		scheduler_.stop();
		threads_.join_all();
    };

	/**
	 * @brief External function to trigger a unit of work
	 * @param action The unit of work to perform
	 * @return void
	 */
	//template <typename T>
	inline void schedule(boost::function<void()> action)
	{
        scheduler_.post(action);
    };

private:
	/**
	 * @brief The underlying OS handle
	 */
	boost::asio::io_service scheduler_;
	/**
	 * @brief The work spinner
	 */
	boost::asio::io_service::work work_;
	/**
	 * @brief The handle to the active object worker thread
	 */
	boost::thread_group threads_;
};

#endif
