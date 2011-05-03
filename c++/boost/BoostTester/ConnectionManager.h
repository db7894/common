/**
 * @file ConnectionManager.h
 * @brief Used to maintain collection of current connections
 */
#pragma once

#ifndef CONNECTION_MANAGER_H
#define CONNECTION_MANAGER_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <set>
#include <boost/utility.hpp>
#include <boost/thread/mutex.hpp> /* for the mutex */
#include "globals.h"

//---------------------------------------------------------------------------//
// Interface
//---------------------------------------------------------------------------//
/**
 * @brief Interface for a socket connection manager
 * @todo Introduce this to base classes so we can mock test
 */
struct IConnectionManager
{
	/**
	 * @brief Virtual Destructor
	 */
	virtual ~IConnectionManager(void) {}

	/**
	 * @brief Adds a new client to the active clients list
	 * @param session The client session object to add
	 * @return void
	 */
	virtual void start(socket_session_ptr session) = 0;

	/**
	 * @brief Removes a client from the active clients list
	 * @param session The client session object to remove
	 * @return void
	 */
	virtual void stop(socket_session_ptr session) = 0;

	/**
	 * @brief Removes all clients from the active clients list
	 * @return void
	 */
	virtual void stop_all(void) = 0;

	/**
	 * @brief Returns the current number of active clients
	 * @return The number of active clients
	 */
	virtual int count(void) = 0;

	/**
	 * @brief Writes a message to all active clients
	 * @param message The data to send to all clients
	 * @return void
	 */
	virtual void write_all(socket_message_ptr message) = 0;
};

//---------------------------------------------------------------------------//
// Definition
//---------------------------------------------------------------------------//

/**
 * @brief Wrapper class to handle list of current clients
 */
class ConnectionManager
: private boost::noncopyable
{
public:
	/**
	 * @brief Constructor to inialize the connection set
	 */
	ConnectionManager(void);

	/**
	 * @brief Adds a new client to the active clients list
	 * @param session The client session object to add
	 * @return void
	 */
	void start(socket_session_ptr session);

	/**
	 * @brief Removes a client from the active clients list
	 * @param session The client session object to remove
	 * @return void
	 */
	void stop(socket_session_ptr session);

	/**
	 * @brief Removes all clients from the active clients list
	 * @return void
	 */
	void stop_all(void);

	/**
	 * @brief Returns the current number of active clients
	 * @return The number of active clients
	 */
	int count(void);

	/**
	 * @brief Writes a message to all active clients
	 * @param message The data to send to all clients
	 * @return void
	 */
	void write_all(socket_message_ptr message);

private:
	/**
	 * @brief Lock used to control access to the connection manager
	 */
	boost::mutex locker_;
	/**
	 * @brief Set used to handle unique collection of clients
	 */
	std::set<socket_session_ptr> connections_;
};

#endif