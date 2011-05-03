/**
 * @file SocketClient.h
 * @brief Async socket client built with asio
 */
#pragma once

#ifndef SOCKET_CLIENT_H
#define SOCKET_CLIENT_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <deque>
#include <boost/asio.hpp>
#include "globals.h"

//---------------------------------------------------------------------------//
// Interface
//---------------------------------------------------------------------------//
/**
 * @brief Interface for a socket client
 * @todo Introduce this to base classes so we can mock test
 */
struct ISocketClient 
{
	/**
	 * @brief Virtual Destructor
	 */
	virtual ~ISocketClient(void) {};

	/**
	 * @brief Appends a message to the client write queue
	 */
	virtual void Write(socket_message_ptr message) = 0;
	/**
	 * @brief Close the underlying socket
	 */
	virtual void Close(void) = 0;

protected:
	/**
	 * @brief Method called upon a complete recieve
	 */
	virtual bool ReadCallback(socket_message_ptr message) = 0;
};

//---------------------------------------------------------------------------//
// Implementation
//---------------------------------------------------------------------------//

/**
 * @brief Async socket client built with asio
 *
 * Example:
 * @include async_socket_client.cpp
 * @include async_socket_tester.cpp
 */
class SocketClient 
{
public:
	/**
	 * @brief Construct a socket to connect to host:port
	 */
	SocketClient(const std::string& host, const std::string& port,
		boost::asio::io_service& scheduler, socket_message_ptr buffer);
	/**
	 * @brief Default destructor
	 */
	~SocketClient(void);
	/**
	 * @brief Appends a message to the client write queue
	 */
	void Write(socket_message_ptr message);
	/**
	 * @brief Close the underlying socket
	 */
	void Close(void);

protected:
	/**
	 * @brief Method called upon a complete recieve
	 */
	virtual bool ReadCallback(socket_message_ptr message);

private:
	/**
	 * @brief Callback to start accept chain or process next resolved host
	 */
	void HandleConnect(const boost::system::error_code& error,
		boost::asio::ip::tcp::resolver::iterator endpoint_iterator);
	/**
	 * @brief Callback to read the next message header
	 */
	void handle_read_header(const boost::system::error_code& error);
	/**
	 * @brief Callback to read the body of the previous header
	 */
	void handle_read_body(const boost::system::error_code& error);
	/**
	 * @brief Callback to start writing the message queue
	 */
	void write_callback(socket_message_ptr message);
	/**
	 * @brief Callback to handle writing the message queue
	 */
	void handle_write(const boost::system::error_code& error);
	/**
	 * @brief Callback to close the underlying socket
	 */
	void handle_close(void);

private:
	typedef std::deque<socket_message_ptr> chat_message_queue;
	/**
	 * @brief The queue of messages currently queued to write
	 */
	chat_message_queue write_messages_;
	/**
	 * @brief Handle to the underlying operation system IO services
	 * @note make sure this is initialized first!
	 */
	boost::asio::io_service& scheduler_;
	/**
	 * @brief The underlying connected socket 
	 */
	boost::asio::ip::tcp::socket socket_;
	/**
	 * @brief Personal read buffer
	 */
	socket_message_ptr buffer_;
};

#endif
