/**
 * @file SocketServer.cpp
 * @brief Async socket server written with boost::asio
 */
#include "stdafx.h"
#include <boost/bind.hpp>
#include <boost/date_time/posix_time/posix_time.hpp>
#include "SocketServer.h"
#include "SocketSession.h"
#include "ObjectFactory.hpp"
#include "ConnectionManager.h"
#include "SocketMessage.h"

using boost::asio::ip::tcp;

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//

/**
 * This initializes the accept chain and the optional heartbeat timer
 */
SocketServer::SocketServer(short port, socket_protocol_ptr protocol, bool heartbeat)
: scheduler_(),
  acceptor_(scheduler_, tcp::endpoint(tcp::v4(), port)),
  manager_(new ConnectionManager()),
  protocol_(protocol),
  factory_(new ObjectFactory<SocketMessage>(128)),
  new_session_(new SocketSession(scheduler_, manager_,
		factory_->create(), protocol_)),
  keep_alive_timer_(scheduler_, boost::posix_time::seconds(1))
{
	/* start accept chain (main thread) */
	acceptor_.async_accept(new_session_->socket(),
		boost::bind(&SocketServer::HandleAccept, this,
			boost::asio::placeholders::error));

	/* start heartbeat thread */
	if (heartbeat) {
		keep_alive_timer_.async_wait(
			boost::bind(&SocketServer::HandleKeepAlive, this));
	}
}

/**
 * This simply calls stop if it has not already been called
 */
SocketServer::~SocketServer()
{
	Stop();
}

/**
 * This call will block so make sure it is the last thing you call
 * or start it in its own thread.
 *
 * @code
 *  boost::asio::io_service io_service;
 *  SocketServer server(io_service, 12345);
 *  boost::thread t(boost::bind(&SocketServer::Start, &server, 2));
 *  ...
 *  server.Stop();
 * @endcode
 */
void SocketServer::Start(std::size_t threads)
{
	//std::cout << "[LOGGING] Server starting: " << threads << "\n";
	for (std::size_t i = 0; i < threads; ++i) {
		thread_manager_.create_thread(
			boost::bind(&boost::asio::io_service::run, &scheduler_));
	}
	thread_manager_.join_all();
}

/**
 * @note This does not block
 * Makes sure that all connections are closed and uses the io_service
 * to stop remaining threads/timers.
 */
void SocketServer::Stop(void)
{
	//std::cout << "[LOGGING] Server stopping\n";
	manager_->stop_all();	/* close all active connections */
	//acceptor_.close();	/* stop accept listener */
	scheduler_.stop();		/* close io_service handle */
}

//---------------------------------------------------------------------------//
// Private
//---------------------------------------------------------------------------//

/**
 * This will create a new SocketSession for each new client and adds to
 * the manager.
 */
void SocketServer::HandleAccept(const boost::system::error_code& error)
{
	if (!error) {
		//std::cout << "[LOGGING] Server accepted new client: " << hostname << "\n";
		manager_->start(new_session_);

		/* create a new session for user connection */
		new_session_.reset(new SocketSession(scheduler_,
			manager_, factory_->create(), protocol_));
		/* continue accept chain */
		acceptor_.async_accept(new_session_->socket(),
			boost::bind(&SocketServer::HandleAccept, this,
			boost::asio::placeholders::error));
	}
}

/**
 * This runs every 1 second and sends the result of GetBuffer to 
 * all available clients in the SocketServer connection handler
 */
void SocketServer::HandleKeepAlive(void)
{
	/* issue heartbeat */
	//std::cout << "[LOGGING] Server issuing heartbeat\n";
	static const char* ping = "Testing Message\n";
	socket_message_ptr message = factory_->create();
	message->Append(ping, strlen(ping));
	manager_->write_all(message);

	/* restart the timer */
	keep_alive_timer_.expires_at(keep_alive_timer_.expires_at()
		+ boost::posix_time::seconds(1));
	keep_alive_timer_.async_wait(
		boost::bind(&SocketServer::HandleKeepAlive, this));
}

