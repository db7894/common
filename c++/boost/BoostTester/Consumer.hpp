/**
 * @file Consumer.hpp
 * @brief Base class for a generic consumer
 */
#pragma once

#ifndef BASHWORK_CONSUMER_H
#define BASHWORK_CONSUMER_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <boost/bind.hpp>
#include <boost/thread.hpp>
#include <boost/shared_ptr.hpp>
#include "Producer.hpp"

//---------------------------------------------------------------------------//
// Implementation
//---------------------------------------------------------------------------//
/**
 * @brief A generic base consumer
 *
 * Example:
 * @include producer_consumer.cpp
 */
template <typename Type>
class Consumer
{
public:
	/**
	 * @brief Base constructor for the consumer
	 */
	Consumer(IProducer<Type>* producer);
	/**
	 * @brief Base class destructor
	 */
	virtual ~Consumer(void);
	/**
	 * @brief Convenience method to stop the consumer
	 */
	void Stop(void);
	/**
	 * @brief Convenience method to restart the consumer
	 */
	void Start(void);

protected:
	/**
	 * @brief Callback for derived class to implement on new work
	 */
	virtual bool callback(boost::shared_ptr<Type> input);

private:
	/**
	 * @brief The main running thread to handle gathering new messages
	 */
	void dispatcher(void);

private:
	/**
	 * @brief Sentinal to control thread state
	 */
	bool keep_running_;
	/**
	 * @brief Handle to underlying thread
	 */
	boost::thread thread_;
	/**
	 * @brief Handle to message producer
	 */
	IProducer<Type>* producer_;
};

//---------------------------------------------------------------------------//
// Global Helpers
//---------------------------------------------------------------------------//
/**
 * @brief Helper function to alleviate template madness
 * @param producer The producer to construct the consumer with
 * @return A new initialized consumer
 */
template <typename Type>
static Consumer<Type>* make_consumer(IProducer<Type>& producer)
{
	return new Consumer<Type>(producer);
}

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//
/**
 * @param[in] producer The producer to pull data from
 * The derived class is expected to initialize the base class.
 * This eliminates the need to check if the producer is attached
 * and removes the need for attach/detach junk.
 * After initialization, the Consumer will be started via RAII.
 */
template <typename Type>
Consumer<Type>::Consumer(IProducer<Type>* producer)
: producer_(producer), keep_running_(false)
{
	Start();
};

/**
 * @note Can be used polymorphically
 */
template <typename Type>
Consumer<Type>::~Consumer(void)
{
	Stop();
};

/**
 * @return void
 */
template <typename Type>
void Consumer<Type>::Stop(void)
{
	keep_running_ = false;
	thread_.interrupt();
	thread_.join(); /* so we don't accidently start multiple threads */
}

/**
 * @return void
 */
template <typename Type>
void Consumer<Type>::Start(void)
{
	if (!keep_running_) {
		keep_running_ = true;
		thread_ = boost::thread(boost::bind(&Consumer::dispatcher, this));
	}
}

//---------------------------------------------------------------------------//
// Protected
//---------------------------------------------------------------------------//
/**
 * @param[in] input The new message to work on
 * @return true to continue working, false to stop
 */
template <typename Type>
bool Consumer<Type>::callback(boost::shared_ptr<Type>/*unused*/)
{
	return true;
};

//---------------------------------------------------------------------------//
// Private
//---------------------------------------------------------------------------//
/**
 * @return void
 */
template <typename Type>
void Consumer<Type>::dispatcher(void)
{
	while (keep_running_) {
		boost::shared_ptr<Type> output = producer_->Consume();
		keep_running_ = callback(output);
	}
};

#endif
