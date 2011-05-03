/**
 * @file SocketClient.cpp
 * @brief Async socket client built with asio
 * @todo Refactor to use SocketSession
 */
#include "stdafx.h"
#include <iostream> /* for testing */
#include <boost/bind.hpp>
#include "SocketClient.h"
#include "SocketMessage.h"

using boost::asio::ip::tcp;

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//
/**
 * @param host The host to connect to
 * @param port The port which to connect to on host
 * @param scheduler The underlying os service
 * @param buffer The pooled buffer object
 * Initializes the scheduler and starts the async connect
 */
SocketClient::SocketClient(const std::string& host, const std::string& port,
	boost::asio::io_service& scheduler, socket_message_ptr buffer)
: scheduler_(scheduler),
  socket_(scheduler),
  buffer_(buffer)
{
	/* create an iterator to walk through resolved hosts */
    tcp::resolver resolver(scheduler_);
	tcp::resolver::query query(host, port);
	tcp::resolver::iterator endpoint_iterator = resolver.resolve(query);
	tcp::endpoint endpoint = *endpoint_iterator;

	/* start the connect loop */
	socket_.async_connect(endpoint,
		boost::bind(&SocketClient::HandleConnect, this,
			boost::asio::placeholders::error, ++endpoint_iterator));
}

/**
 * Everything should be automatically managed
 */
SocketClient::~SocketClient(void)
{
	handle_close();
}

/**
 * @param message The message to enque on the send list
 * @return void
 * posts the message so our threads will work on the message
 */
void SocketClient::Write(socket_message_ptr message)
{
	//std::cout << "[LOGGING] Client requested a send\n";
	//std::cout.write(message->get_raw_body(), message->Size());
	scheduler_.post(boost::bind(&SocketClient::write_callback,
		this, message)); /* is this copied or referenced ? */
}

/**
 * @return void
 * posts the request so a worker thread will perform the work
 */
void SocketClient::Close(void)
{
	//std::cout << "[LOGGING] Client requested to be closed\n";
	scheduler_.post(boost::bind(&SocketClient::handle_close, this));
}

//---------------------------------------------------------------------------//
// Protected
//---------------------------------------------------------------------------//
/**
 * @param message The received message
 * @return true to continue working, false otherwise
 */
bool SocketClient::ReadCallback(socket_message_ptr message)
{
	std::cout.write(message->get_raw_body(), message->Size());
	std::cout << "\n";
	return true;
}

//---------------------------------------------------------------------------//
// Private
//---------------------------------------------------------------------------//
/**
 * @param error Placeholder for a possible asio error
 * @return void
 */
void SocketClient::HandleConnect(const boost::system::error_code& error,
	boost::asio::ip::tcp::resolver::iterator endpoint_iterator)
{
	if (!error) {
	/* we connected, start the read loop */
		boost::asio::async_read(socket_,
			boost::asio::buffer(buffer_->get_raw_header(),
				SocketMessage::max_header_size),		/* asio buffer wrapper */
			boost::bind(&SocketClient::handle_read_header, this,
				boost::asio::placeholders::error));	/* callback handler */
	}
	/* try the next resolved host */
	else if (endpoint_iterator != tcp::resolver::iterator()) {
		socket_.close();
		tcp::endpoint endpoint = *endpoint_iterator;
		socket_.async_connect(endpoint,
			boost::bind(&SocketClient::HandleConnect, this,
				boost::asio::placeholders::error, ++endpoint_iterator));
	}
}

/**
 * @note If there is an error with the write, the connection is closed
 * @param error Placeholder for a possible asio error
 * @return void
 */
void SocketClient::handle_read_header(const boost::system::error_code& error)
{
	/* if the header decoded correctly, receive the body */
	if (!error && buffer_->decode_header()) {
		boost::asio::async_read(socket_,
			boost::asio::buffer(buffer_->get_raw_body(),
				buffer_->Size()),					/* asio buffer wrapper */
			boost::bind(&SocketClient::handle_read_body, this,
				boost::asio::placeholders::error));	/* callback handler */
	}
	else {
		handle_close();
	}
}

/**
 * @note If there is an error with the write, the connection is closed
 * @param error Placeholder for a possible asio error
 * @return void
 */
void SocketClient::handle_read_body(const boost::system::error_code& error)
{
	if (!error) {
		ReadCallback(buffer_); /* handle result */
		/* try to read the next message header */
		boost::asio::async_read(socket_,
			boost::asio::buffer(buffer_->get_raw_header(),
				SocketMessage::max_header_size),			/* asio buffer wrapper */
			boost::bind(&SocketClient::handle_read_header, this,
				boost::asio::placeholders::error));	/* callback handler */
	}
	else {
		handle_close();
	}
}

/**
 * @return void
 */
void SocketClient::write_callback(socket_message_ptr message)
{
	bool write_in_progress = !write_messages_.empty();
	write_messages_.push_back(message);

	if (!write_in_progress) {
		boost::asio::async_write(socket_,
			boost::asio::buffer(write_messages_.front()->get_raw_header(),
				write_messages_.front()->get_total_size()),
			boost::bind(&SocketClient::handle_write, this,
				boost::asio::placeholders::error));
	}
}

/**
 * @note If there is an error with the write, the connection is closed
 * @param error Placeholder for a possible asio error
 * @return void
 */
void SocketClient::handle_write(const boost::system::error_code& error)
{
	if (!error) {
		write_messages_.pop_front();	/* free message we just wrote */
		if (!write_messages_.empty()) {
			boost::asio::async_write(socket_,
				boost::asio::buffer(					/* asio buffer wrapper */
					write_messages_.front()->get_raw_header(),
					write_messages_.front()->get_total_size()),
				boost::bind(&SocketClient::handle_write, this,
					boost::asio::placeholders::error));	/* callback handler */
		}
	}
	else {
		handle_close();
	}
}

/**
 * The underlying implementation checks if the socket is 
 * open or not so a check before the close is redundant
 */
void SocketClient::handle_close(void)
{
	//std::cout << "[LOGGING] Client closing\n";
	socket_.close();
}
