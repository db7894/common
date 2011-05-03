/**
 * @file SocketServer.h
 * @brief Async socket server written with boost::asio
 */
#pragma once

#ifndef SOCKET_SERVER_H
#define SOCKET_SERVER_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <vector>
#include <boost/asio.hpp>
#include <boost/thread.hpp>
#include <boost/function.hpp>
#include <boost/shared_ptr.hpp>
#include "globals.h"

//---------------------------------------------------------------------------//
// Inteface
//---------------------------------------------------------------------------//
/**
 * @brief Interface for a socket server
 * @todo Introduce this to base classes so we can mock test
 */
struct ISocketServer
{
	/**
	 * @brief Virtual Destructor
	 */
	virtual ~ISocketServer(void) {};

	/**
	 * @brief Starts the IO service and underlying threads
	 * @param threads The number of io worker threads to start (default 2)
	 * @return void
	 */
	virtual void Start(std::size_t threads) = 0;

	/**
	 * @brief Stops the IO service and underlying threads
	 * @return void
	 */
	virtual void Stop(void) = 0;
};

//---------------------------------------------------------------------------//
// Base Socket Server
//---------------------------------------------------------------------------//
/**
 * @brief Portable async socket server base class 
 * @note This is implemented using boost::asio
 *
 * Example:
 * @include async_socket_server.cpp
 */
class SocketServer
: private boost::noncopyable
{
public:
	/**
	 * @brief Constructor for a Socket Server
	 * @param port The port for the server to listen on
	 * @param protocol The server underlying protocol
	 * @param heartbeat Set to true to enable a connection heartbeat (default true)
	 */
	SocketServer(short port, socket_protocol_ptr protocol, bool heartbeat=true);
	/**
	 * @brief Default destructor
	 */
	~SocketServer();
	/**
	 * @brief Starts the IO service and underlying threads
	 * @param threads The number of io worker threads to start (default 2)
	 * @return void
	 */
	void Start(std::size_t threads = 2);
	/**
	 * @brief Stops the IO service and underlying threads
	 * @return void
	 */
	void Stop(void);

private:
	/**
	 * @brief The event handler for a new client connection
	 * @param Error holder for a possible asio error
	 * @return void
	 */
	void HandleAccept(const boost::system::error_code& error);
	/**
	 * @brief Thread worker function used to supply a client heartbeat
	 * @return void
	 */
	void HandleKeepAlive(void);

private:	
	/* 
	 * Be wary about changing this order as the initialization
	 * on many of these objects depends on already initialized
	 * objects or data supplied by already initialized objects
	 */

	/**
	 * @brief Handle to the underlying operation system IO services
	 */
	boost::asio::io_service scheduler_;
	/**
	 * @brief Handle to the socket acceptor
	 */
	boost::asio::ip::tcp::acceptor acceptor_;
	/**
	 * @brief Handle to the session protocol
	 */
	socket_protocol_ptr protocol_;
	/**
	 * @brief Handle to the socket acceptor
	 */
	socket_manager_ptr manager_;
	/**
	 * @brief Handle to the heartbeat work timer
	 */
	boost::asio::deadline_timer keep_alive_timer_;
	/**
	 * @brief Handle to the object pool factory
	 */
	boost::shared_ptr<ObjectFactory<SocketMessage> > factory_;
	/**
	 * @brief Handle to manage the current io worker threads
	 */
	boost::thread_group thread_manager_;
	/**
	 * @brief Handle pointer to the next session
	 */
	socket_session_ptr new_session_;
};

#endif
