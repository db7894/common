/**
 * @file SocketSession.h
 * @brief Handler for a client session
 */
#pragma once

#ifndef SOCKET_SESSION_H
#define SOCKET_SESSION_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <deque>
#include <vector>
#include <boost/asio.hpp>
#include <boost/shared_ptr.hpp>
#include <boost/enable_shared_from_this.hpp>
#include "globals.h"

//---------------------------------------------------------------------------//
// Inteface
//---------------------------------------------------------------------------//
/**
 * @brief Interface for a socket session
 * @todo Introduce this to base classes so we can mock test
 */
struct ISocketSession
{
	/**
	 * @brief Virtual Destructor
	 */
	virtual ~ISocketSession(void) {};

	/**
	 * @brief Quick function to start up all the aync event chains
	 * @return void
	 */
	virtual void start(void) = 0;

	/**
	 * @brief Quick function to shutdown the socket session
	 * @return void
	 */
	virtual void stop(void) = 0;

	/**
	 * @brief Enqueue a message to be sent to a client
	 * @param message The message to send off
	 * @return void
	 */
	virtual void write(socket_message_ptr message) = 0;

protected:
	friend SocketServer;
	/**
	 * @brief Returns the raw socket for this client
	 * @return A raw unmanaged socket handle
	 */
	virtual boost::asio::ip::tcp::socket& socket(void) = 0;
};

//---------------------------------------------------------------------------//
// Implementation
//---------------------------------------------------------------------------//

/**
 * @brief Handler for a client session
 */
class SocketSession
: public boost::enable_shared_from_this<SocketSession>,
  private boost::noncopyable
{
	friend SocketServer;

public:
	/**
	 * @brief Constructor to initialize the underlying socket
	 * @param io_service The underlying scheduler
	 * @param manager The connection manager holding this session
	 * @param buffer The pooled buffer object to use for reading
	 * @param protocol The protocol implementation
	 */
	SocketSession(boost::asio::io_service &io_service,
		socket_manager_ptr manager, socket_message_ptr buffer,
		socket_protocol_ptr protocol);

	/**
	 * @brief Debug destructor
	 */
	~SocketSession(void);

	/**
	 * @brief Quick function to start up all the aync event chains
	 * @return void
	 */
	void start(void);

	/**
	 * @brief Quick function to shutdown the socket session
	 * @return void
	 */
	void stop(void);

	/**
	 * @brief Enqueue a message to be sent to a client
	 * @param message The message to send off
	 * @return void
	 */
	void write(socket_message_ptr message);

protected:
	/**
	 * @brief Returns the raw socket for this client
	 * @return A raw unmanaged socket handle
	 */
	boost::asio::ip::tcp::socket& socket(void);

private:
	/**
	 * @brief Callback for a socket message receive event
	 * @param error Handle for a possible asio error
	 * @param bytes_transferred Number of bytes actually transferred
	 * @return void
	 */
	void handle_read_header(const boost::system::error_code& error,
		size_t bytes_transferred);

	/**
	 * @brief Callback for a valid header receive event
	 * @param error Handle for a possible asio error
	 * @param bytes_transferred Number of bytes actually transferred
	 * @return void
	 */
	void handle_read_body(const boost::system::error_code& error,
		size_t bytes_transferred);

	/**
	 * @brief Callback for a socket write event
	 * @param error Handle for a possible asio error
	 * @return void
	 */
	void handle_write(const boost::system::error_code& error);

	/**
	 * @brief Handles removing session from connection manager
	 * @return void
	 */
	void Destroy(void);

private:
	/**
	 * @brief Sentinal to check whether the session is dirty still
	 */
	bool destroyed_;
	/**
	 * @brief The underlying socket used for communication
	 */
	boost::asio::ip::tcp::socket socket_;

	/**
	 * @brief The write message queue
	 */
	std::deque<socket_message_ptr> write_messages_;

	/**
	 * @brief The protocol implementation
	 */
	 socket_protocol_ptr protocol_;

	/**
	 * @brief Handle to connection manager
	 */
	socket_manager_ptr manager_;

	/**
	 * @brief Handle to our personal read buffer
	 */
	socket_message_ptr buffer_;
};

#endif
