/**
 * @file ConcurrentQueue.hpp
 * @brief Simple dual access queue
 * @todo Find a way to have users of Consume to exit
 * without using boost::thread::interrupt
 */
#pragma once

#ifndef SCOTTRADE_CONCURRENT_QUEUE_H
#define SCOTTRADE_CONCURRENT_QUEUE_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <deque>
#include <boost/shared_ptr.hpp>
#include <boost/thread/mutex.hpp>
#include <boost/thread/condition.hpp>

//---------------------------------------------------------------------------//
// Implementation
//---------------------------------------------------------------------------//
/**
 * @brief Simple dual access queue
 *
 * Example:
 * @include concurrent_queue.cpp
 */
template <typename Type>
class ConcurrentQueue
: private boost::noncopyable
{
public:
	/**
	 * @brief Default constructor
	 */
	ConcurrentQueue(void);

	/**
	 * @brief Default destructor
	 */
	~ConcurrentQueue(void);

	/**
	 * @brief Handle for the producer to input from
	 * @param[in] input New data to put in the queue
	 * @return void
	 */
	void Produce(Type input);

	/**
	 * @brief Handle for the consumer to pull from
	 * @return The new data to process from the queue
	 */
	Type Consume(void);

	/**
	 * @brief Used to clear existing queue
	 * @return void
	 */
	void Clear(void);

	/**
	 * @brief Used to query the current container size
	 * @return The current number of messages in queue
	 */
	size_t Size(void);

private:
	/**
	 * @brief Handler of messages
	 */
	std::deque<Type> messages_;
	/**
	 * @brief Condition event for consumers to wait on
	 */
	boost::condition condition_;
	/**
	 * @brief Mutex to control access
	 */
	boost::mutex mutex_;
};

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//
/**
 * Nothing really right now
 */
template <typename Type>
ConcurrentQueue<Type>::ConcurrentQueue(void)
{
};

/**
 * The messages are assumed to be shared pointers
 * and as such we let them perform message cleanup
 */
template <typename Type>
ConcurrentQueue<Type>::~ConcurrentQueue(void)
{
	messages_.clear();
};

/**
 * @warning This function may block if another operation is in progress
 */
template <typename Type>
void ConcurrentQueue<Type>::Produce(Type input)
{
	{ /* forced scope */
        boost::mutex::scoped_lock lock(mutex_);
		messages_.push_back(input);
	}
    condition_.notify_one();
};

/**
 * @warning This function may block if another operation is in progress
 */
template <typename Type>
Type ConcurrentQueue<Type>::Consume(void)
{
	boost::mutex::scoped_lock lock(mutex_);
	while (messages_.empty())
		condition_.wait(mutex_);
	Type handle = messages_.front();
	messages_.pop_front();
	return handle;
};

/**
 * @warning This function may block if another operation is in progress
 */
template <typename Type>
void ConcurrentQueue<Type>::Clear(void)
{
	boost::mutex::scoped_lock lock(mutex_);
	messages_.clear();
};


/**
 * @warning This function may block if another operation is in progress
 */
template <typename Type>
size_t ConcurrentQueue<Type>::Size(void)
{
	boost::mutex::scoped_lock lock(mutex_);
	return messages_.size();
};

#endif
