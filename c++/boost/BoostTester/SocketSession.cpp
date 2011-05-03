/**
 * @file SocketSession.cpp
 * @brief Handler for a client session
 */
#include "StdAfx.h"
#include <boost/bind.hpp>
/* for logging */
//#include <iostream>
#include "SocketSession.h"
#include "ConnectionManager.h"
#include "SocketMessage.h"
#include "Protocol.h"

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//

/**
 */
SocketSession::SocketSession(boost::asio::io_service &io_service, 
	boost::shared_ptr<ConnectionManager> manager, socket_message_ptr buffer,
	socket_protocol_ptr protocol)
: socket_(io_service),
  manager_(manager),
  buffer_(buffer),
  protocol_(protocol),
  destroyed_(false)
{
}

/**
 * Uses Destroy to clean up
 */
SocketSession::~SocketSession()
{
	//std::cout << "[LOGGING] Client Deleted\n";
	Destroy();
}

/**
 * Starts the read loop (this does not block)
 */
void SocketSession::start(void)
{
	//std::cout << "[LOGGING] Client session started\n";
	/*
	if (HandleConnect()) {
		start_read();
	}
	*/
	boost::asio::async_read(socket_, boost::asio::buffer(
			buffer_->get_raw_header(), SocketMessage::max_header_size),
		boost::bind(&SocketSession::handle_read_header, shared_from_this(),
			boost::asio::placeholders::error,
			boost::asio::placeholders::bytes_transferred));
}

/**
 * The underlying implementation checks if the socket is 
 * open or not and calls shutdown(both) on the socket
 */
void SocketSession::stop(void)
{
	//std::cout << "[LOGGING] Client session forcefully stopped\n";
	socket_.close();
}

/**
 * If a send loop is not in progress, we can start one; otherwise
 * we will simply post to the write queue.
 */
void SocketSession::write(socket_message_ptr message)
{
	bool write_in_progress = !write_messages_.empty();
	write_messages_.push_back(message);

	/* if we are currently in a write loop, it will pick this
	 * up, otherwise we need to start the loop again
	 */
	//std::cout << "[LOGGING] Client session send" << (*message) << "\n";
	if (!write_in_progress) {
		boost::asio::async_write(socket_,
			boost::asio::buffer(write_messages_.front()->get_raw_header(),
				write_messages_.front()->get_total_size()),
			boost::bind(&SocketSession::handle_write, shared_from_this(),
			boost::asio::placeholders::error));
	}
}

//---------------------------------------------------------------------------//
// Protected
//---------------------------------------------------------------------------//

/**
 * @warning This does not work with the reference counting
 */
boost::asio::ip::tcp::socket& SocketSession::socket(void)
{
	//std::cout << "[LOGGING] Client session raw handle get\n";
	return socket_;
}

//---------------------------------------------------------------------------//
// Private
//---------------------------------------------------------------------------//
/**
 * @todo Investigate ?possible? race condition
 * If a read and a write are in progress at the same time and the client
 * disconnects, both operations could hit the error condition in their
 * respective handlers and then call Destroy() together. They could then both
 * possibly pass the sentinal check and attempt to remove themself from the
 * manager twice which will cause an error.
 */
void SocketSession::Destroy(void)
{
	/* maybe lock this ? */
	if (!destroyed_) {
	/*
		std::cout << "[LOGGING] Client session disconnected\n";
		HandleDisconnect()) {
	*/
		destroyed_ = true;
		manager_->stop(shared_from_this());
	}
}

/**
 */
void SocketSession::handle_read_header(const boost::system::error_code& error,
	size_t bytes_transferred)
{
	if (!error && buffer_->decode_header()) {
		boost::asio::async_read(socket_, boost::asio::buffer(
				buffer_->get_raw_body(), buffer_->Size()),
			boost::bind(&SocketSession::handle_read_body, shared_from_this(),
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred));
	}
	else { 	/* we assume the client disconnected */
		Destroy();
	}
}

/**
 */
void SocketSession::handle_read_body(const boost::system::error_code& error,
	size_t bytes_transferred)
{
	if (!error) {
		protocol_->Handle(buffer_);
		boost::asio::async_read(socket_, boost::asio::buffer(
			buffer_->get_raw_header(), SocketMessage::max_header_size),
			boost::bind(&SocketSession::handle_read_header, shared_from_this(),
				boost::asio::placeholders::error,
				boost::asio::placeholders::bytes_transferred));
	}
	else { 	/* we assume the client disconnected */
		Destroy();
	}
}

/**
 * This will continue to loop until the write queue is empty
 */
void SocketSession::handle_write(const boost::system::error_code& error)
{
	if (!error) {
		write_messages_.pop_front(); /* clear message we just wrote */
		if (!write_messages_.empty()) {
			boost::asio::async_write(socket_,
				boost::asio::buffer(write_messages_.front()->get_raw_header(),
					write_messages_.front()->get_total_size()),
				boost::bind(&SocketSession::handle_write, shared_from_this(),
				boost::asio::placeholders::error));
		}
	}
	else { 	/* we assume the client disconnected */
		Destroy();
	}
}
