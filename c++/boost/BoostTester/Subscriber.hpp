/**
 * @file Subscriber.hpp
 * @brief Base class for a generic subscriber
 */
#pragma once

#ifndef BASHWORK_SUBSCRIBER_H
#define BASHWORK_SUBSCRIBER_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <boost/bind.hpp>
#include <boost/thread.hpp>
#include <boost/shared_ptr.hpp>

//---------------------------------------------------------------------------//
// Interface
//---------------------------------------------------------------------------//
/**
 * @brief Interface a subscriber must implement
 */
template <typename Type>
class ISubscriber
{
public:
	/**
	 * @brief The method used by publisher to push new message
	 * @param[in] input The message to work on
	 * @return True to continue receiving messages, false otherwise
	 */
	virtual bool Work(boost::shared_ptr<Type> input) = 0;
};

//---------------------------------------------------------------------------//
// Implementation
//---------------------------------------------------------------------------//
/**
 * @brief A generic base subscriber
 *
 * Example:
 * @include publisher_subscriber.cpp
 */
template <typename Type>
class Subscriber : public ISubscriber<Type>
{
public:
	/**
	 * @brief Base constructor for the subscriber
	 */
	Subscriber(void);
	/**
	 * @brief Base class destructor
	 */
	virtual ~Subscriber();
	/**
	 * @brief Convenience method to stop the consumer
	 */
	void Stop(void);
	/**
	 * @brief Convenience method to restart the consumer
	 */
	void Start(void);
	/**
	 * @brief Method called by publisher on new message
	 */
	virtual bool Work(boost::shared_ptr<Type> input);

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
	 * @brief Queue of messages to work on
	 */
	ConcurrentQueue<boost::shared_ptr<Type> > queue_;
};

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//
/**
 * This automatically starts the worker thread
 */
template <typename Type>
Subscriber<Type>::Subscriber(void)
: keep_running_(false)
{
	Start();
};

/**
 * @note Can be used polymorphically
 * This stops the worker thread
 */
template <typename Type>
Subscriber<Type>::~Subscriber()
{
	Stop();
};

/**
 * @return void
 * @note This waits for the worker thread to finish its current iteration
 */
template <typename Type>
void Subscriber<Type>::Stop(void)
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
void Subscriber<Type>::Start(void)
{
	if (!keep_running_) {
		keep_running_ = true;
		thread_ = boost::thread(boost::bind(&Subscriber::dispatcher, this));
	}
}

/**
 * @param[in] input The new message to work on
 * @return true to continue working, false to unsubscribe
 */
template <typename Type>
bool Subscriber<Type>::Work(boost::shared_ptr<Type> input)
{
	/* so if we stop we don't keep building up messages */
	if (keep_running_)
		queue_.Produce(input);
	return keep_running_;
};

//---------------------------------------------------------------------------//
// Protected
//---------------------------------------------------------------------------//
/**
 * @param[in] input The new message to work on
 * @return true to continue working, false to stop
 */
template <typename Type>
bool Subscriber<Type>::callback(boost::shared_ptr<Type> input)
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
void Subscriber<Type>::dispatcher(void)
{
	while (keep_running_) {
		boost::shared_ptr<Type> output = queue_.Consume();
		keep_running_ = callback(output);
	}
};

#endif
