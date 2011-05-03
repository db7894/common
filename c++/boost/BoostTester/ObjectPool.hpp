/**
 * @file ObjectPool.hpp
 * @brief Implements a generic object pool manager
 * @warning As of now this is more than likely too slow to use for massive
 * collections
 */
#pragma once

#ifndef OBJECT_POOL_H
#define OBJECT_POOL_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <vector>
#include <boost/thread/mutex.hpp> /* for the mutex */
#include <boost/thread/thread.hpp>

//---------------------------------------------------------------------------//
// Types
//---------------------------------------------------------------------------//
/* none */


//---------------------------------------------------------------------------//
// Definition
//---------------------------------------------------------------------------//

/**
 * @brief A generic object pool
 */
template <typename Type>
class ObjectPool
: private boost::noncopyable
{
public:

	/**
	 * @brief The constructor for an object pool
	 */
	ObjectPool(int size = 10);

	/**
	 * @brief Destroy the held pool of objects
	 */
	~ObjectPool();

	/**
	 * @brief Returns the current size of the object pool
	 */
	size_t size(void);

	/**
	 * @brief Returns an element back to the pool
	 */
	void free(Type* item);

	/**
	 * @brief Used to retrieve the next free element from the pool
	 */
	Type* construct(void);

	/**
	 * @brief Removes all elements in the object pool
	 */
	void clear(void);

private:
	/* Typedefs for readability */ 
	typedef std::pair<bool, Type*>	object_t;
	typedef std::vector<object_t>	object_pool_t;

	/**
	 * @brief The pool handle object
	 */
	object_pool_t pool_;

	/**
	 * @brief Lock used to enable thread-safe operation
	 */
	boost::mutex locker_;
};

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//

/**
 * @brief The constructor for an object pool
 * @param size The number of elements to start out with (default 10)
 */
template <typename Type>
ObjectPool<Type>::ObjectPool(int size = 10)
	: pool_(size)
{
	for (int i = 0; i < size; ++i) {
		Type* handle(new Type());
		pool_[i] = std::make_pair(false, handle);
	}
}

/**
 * @brief Destroy the held pool of objects
 */
template <typename Type>
ObjectPool<Type>::~ObjectPool()
{
	/*
	object_pool_t::iterator	end = pool_.end();
	for (object_pool_t::iterator result = pool_.begin();
		result != end; ++result)
		delete result->second;
	*/
	std::for_each(pool_.begin(), pool_.end(),
		boost::bind(boost::lambda::delete_ptr(),
			boost::bind(&object_pool_t::second, _1)));
	pool_.clear();
}

/**
 * @brief Returns the current size of the object pool
 * @return The current number of elements in the object pool
 */
template <typename Type>
size_t ObjectPool<Type>::size(void)
{
	boost::lock_guard<boost::mutex> guard(locker_);
	return pool_.size();
}

/**
 * @brief Returns an element back to the pool
 * @param item A pointer to the currently held element
 * @return void
 *
 * If the returned item is not currently in the object pool,
 * it is disregarded, otherwise its position is marked as 
 * unused.
 */
template <typename Type>
void ObjectPool<Type>::free(Type* item)
{
	boost::lock_guard<boost::mutex> guard(locker_);
	object_pool_t::iterator	end = pool_.end();
	for (object_pool_t::iterator result = pool_.begin();
		result != end; ++result) {
		if (result->second == item) {
			result->first = false;
			break;
		}
	}
	/* is this really better?
	object_pool_t::iterator result = std::find_if(
		pool_.begin(), pool_.end(),
		boost::bind<bool>(&std::equal_to<Type*>(), item,
			boost::bind(&object_pool_t::second, _1)));
	if (result != pool_.end())
		result->first = false;
	*/
}

/**
 * @brief Used to retrieve the next free element from the pool
 * @return A pointer to the handled type
 *
 * If there is not an available free element, one is created and
 * appended to the pool.
 */
template <typename Type>
Type* ObjectPool<Type>::construct(void)
{
	boost::lock_guard<boost::mutex> guard(locker_);
	object_pool_t::iterator end = pool_.end();
	for (object_pool_t::iterator result = pool_.begin();
		result != end; ++result) {
		if (result->first == false) {
			result->first = true;
			return result->second;
		}
	}
	/* is this really better?
	object_pool_t::iterator result = std::find_if(
		pool_.begin(), pool_.end(),
		boost::bind<bool>(&std::equal_to<bool>(), false,
			boost::bind(&object_pool_t::first, _1)));
	if (result != pool_.end()) {
		result->first = true;
		return result->second;
	}
	*/
	Type* handle(new Type());
	pool_.push_back(std::make_pair(true, handle));
	return handle;
}

/**
 * @brief Removes all elements in the object pool
 * @return void
 */
template <typename Type>
void ObjectPool<Type>::clear(void)
{
	boost::lock_guard<boost::mutex> guard(locker_);
	pool_.clear();
}

//---------------------------------------------------------------------------//
// Private
//---------------------------------------------------------------------------//
/* nothing */

#endif
