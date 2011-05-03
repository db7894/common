/**
 * @file Producer.hpp
 * @brief Base class for a generic producer
 */
#pragma once

#ifndef SCOTTRADE_PRODUCER_H
#define SCOTTRADE_PRODUCER_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <boost/shared_ptr.hpp>
#include "ConcurrentQueue.hpp"

//---------------------------------------------------------------------------//
// Interface
//---------------------------------------------------------------------------//
/**
 * @brief Interface a producer must implement
 */
template <typename Type>
class IProducer
{
public:
	/**
	 * @brief Handle method to add more data to the queue
	 * @param[in] input A new message to be consumed
	 * @return void
	 */
	virtual void Produce(boost::shared_ptr<Type> input) = 0;
	/**
	 * @brief Method to be called by available consumers
	 * @return The next availabe message
	 */
	virtual boost::shared_ptr<Type> Consume(void) = 0;
};

//---------------------------------------------------------------------------//
// Implementation
//---------------------------------------------------------------------------//
/**
 * @brief A generic one message one user producer
 *
 * Example:
 * @include producer_consumer.cpp
 */
template <typename Type>
class Producer : public IProducer<Type>
{
public:
	/**
	 * @brief Default constructor
	 */
	Producer(void);
	/**
	 * @brief Default destructor
	 */
	virtual ~Producer(void);
	/**
	 * @brief Handle method to add more data to the queue
	 */
	void Produce(boost::shared_ptr<Type> input);
	/**
	 * @brief Method to be called by available consumers
	 */
	boost::shared_ptr<Type> Consume(void);
	/**
	 * @brief Returns the current size of the message queue
	 */
	size_t Size(void);

private:
	ConcurrentQueue<boost::shared_ptr<Type> > queue_;
};

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//

/**
 * @brief Default constructor
 */
template <typename Type>
Producer<Type>::Producer(void)
{
	/* nothing */
};

/**
 * @note can be used polymorphically
 */
template <typename Type>
Producer<Type>::~Producer(void)
{
	/* queue will handle freeing */
	queue_.Clear();
};

/**
 * @param input Data to append to the consumer queue
 * @return void
 * @note A call on this may quickly block
 */
template <typename Type>
void Producer<Type>::Produce(boost::shared_ptr<Type> input)
{
	queue_.Produce(input);
};

/**
 * @return The next element to consume
 * @note A call on this will block until there is another message
 */
template <typename Type>
boost::shared_ptr<Type> Producer<Type>::Consume(void)
{
	return queue_.Consume();
};

/**
 * @return The nubmer of items left in the producer queue
 * @note A call on this may quickly block
 */
template <typename Type>
size_t Producer<Type>::Size(void)
{
	return queue_.Size();
};

//---------------------------------------------------------------------------//
// Private
//---------------------------------------------------------------------------//
/* none */

#endif
