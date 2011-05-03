/**
 * @file ObjectFactory.hpp
 * @brief Wrapper around an object pool to automatically return unused object
 */
#pragma once

#ifndef OBJECT_FACTORY_H
#define OBJECT_FACTORY_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <boost/pool/object_pool.hpp>
#include <boost/shared_ptr.hpp>
#include <boost/bind.hpp>
//#include <boost/thread/mutex.hpp> /* for the mutex */

/**
 * @brief Wrapper around an object pool to automatically return unused object
 *
 * Example usage:
 * @include async_socket_client.cpp
 */
template <typename T>
class ObjectFactory
{
public:
	/**
	 * @brief Initialize the pool
	 * @param size The default size of the object pool (default 512)
	 */
	explicit ObjectFactory(int size=512)
		: pool_(size)
	{};

	/**
	 * @brief Clean up the object pool
	 * @note The pool automatically cleans up after itself
	 */
	//~ObjectFactory()
	//{
	//}

	/**
	 * @brief Create an object from the pool that is automatically returned
	 * This uses boost shared_ptr to call our release function when the 
	 * reference count dips.
	 */
	boost::shared_ptr<T> create(void)
	{
		//boost::lock_guard<boost::mutex> guard(locker_);
		return boost::shared_ptr<T>(pool_.construct(),
			boost::bind(&ObjectFactory<T>::release, this, _1));
	};

private:
	/**
	 * @brief Custom destructor for the object to return message to the pool
	 * @return p Pointer to the object to return to the pool
	 * @return void
	 */
	void release(T* p)
	{
		//boost::lock_guard<boost::mutex> guard(locker_);
		pool_.free(p);
	};

private:
	/**
	 * @brief Lock used to control access to the connection manager
	 */
	//boost::mutex locker_;
	/**
	 * @brief Handle to underlying pool
	 */
	boost::object_pool<T> pool_;
};

#endif
