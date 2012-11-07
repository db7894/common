/**
 * @file Publisher.hpp
 * @brief Base class for a generic producer
 * @code
 * todo
 * @endcode
 */
#pragma once

#ifndef BASHWORK_PUBLISHER_H
#define BASHWORK_PUBLISHER_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <boost/bind.hpp>
#include <boost/shared_ptr.hpp>
#include <boost/thread.hpp>
#include <boost/thread/mutex.hpp>
#include "ConcurrentQueue.hpp"
#include "Subscriber.hpp"

//---------------------------------------------------------------------------//
// Implementation
//---------------------------------------------------------------------------//

/**
 * @brief A generic one message one user producer
 *
 * Example:
 * @include publisher_subscriber.cpp
 */
template <typename Type>
class Publisher
{
public:
	/**
	 * @brief Default constructor
	 */
	Publisher(void);
	/**
	 * @brief Default destructor
	 */
	virtual ~Publisher(void);
	/**
	 * @brief Handle method to add more data to the queue
	 */
	void Publish(boost::shared_ptr<Type> input);
	/**
	 * @brief Called by interested parties
	 */
	void Subscribe(boost::shared_ptr<ISubscriber<Type> > subscriber);
	/**
	 * @brief Convenience method to restart the publisher
	 */
	void Start(void);
	/**
	 * @brief Convenience method to stop the publisher
	 */
	void Stop(void);
	/**
	 * @brief Used to query the number of waiting messages
	 */
	size_t Size(void);

private:
	/**
	 * @brief Background worker method
	 */
	void dispatcher(void);

private:
	/**
	 * @brief Subscriber handler
	 */
	std::vector<boost::shared_ptr<ISubscriber<Type> > > subscribers_;
	/**
	 * @brief Synchronized queue
	 */
	ConcurrentQueue<boost::shared_ptr<Type> > queue_;
	/**
	 * @brief Mutex to handle subscriber management
	 */
	boost::mutex mutex_;
	/**
	 * @brief Sentinal to control thread state
	 */
	bool keep_running_;
	/**
	 * @brief Handle to underlying thread
	 */
	boost::thread thread_;
};

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//

/**
 * This automatically starts the push thread
 */
template <typename Type>
Publisher<Type>::Publisher(void)
: keep_running_(false)
{
	Start();
};

/**
 * @note This class can be used polymorphically
 */
template <typename Type>
Publisher<Type>::~Publisher(void)
{
	Stop();
	subscribers_.clear();
	queue_.Clear();
};

/**
 * @return void
 * @note This waits on the current message push to finish
 */
template <typename Type>
void Publisher<Type>::Stop(void)
{
	keep_running_ = false;
	thread_.interrupt();
	thread_.join(); /* so we don't accidently start multiple threads */
}

/**
 * @return void
 * @note This guards against starting multiple threads
 */
template <typename Type>
void Publisher<Type>::Start(void)
{
	if (!keep_running_) {
		keep_running_ = true;
		thread_ = boost::thread(boost::bind(&Publisher::dispatcher, this));
	}
}

/**
 * @param input Data to append to the consumer queue
 * @return void
 * @note This may quickly block if a message is being consumed
 */
template <typename Type>
void Publisher<Type>::Publish(boost::shared_ptr<Type> input)
{
	queue_.Produce(input);
};

/**
 * @return The number of waiting messages
 * @note This may quickly block if a message is being consumed
 */
template <typename Type>
size_t Publisher<Type>::Size(void)
{
	return queue_.Size();
};

/**
 * @param subscriber A new interested party
 * @return void
 * @note This may block if a message push is in progress
 */
template <typename Type>
void Publisher<Type>::Subscribe(boost::shared_ptr<ISubscriber<Type> > subscriber)
{
	boost::mutex::scoped_lock lock(mutex_);
	subscribers_.push_back(subscriber);
};

//---------------------------------------------------------------------------//
// Private
//---------------------------------------------------------------------------//
/**
 * @return void
 * This is the main runner thread that will handle pushing requests
 */
template <typename Type>
void Publisher<Type>::dispatcher(void)
{
	while (keep_running_) {
		boost::shared_ptr<Type> output = queue_.Consume();
		{ /* forced scope */
			boost::mutex::scoped_lock lock(mutex_);
			std::for_each(subscribers_.begin(), subscribers_.end(),
				boost::bind(&ISubscriber<Type>::Work, _1, output));
		}
	}
};

#endif
