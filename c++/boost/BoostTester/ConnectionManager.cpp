/**
 * @file ConnectionManager.cpp
 * @brief Used to maintain collection of current connections
 */
#include "StdAfx.h"
#include "ConnectionManager.h"
#include "SocketSession.h"
#include <boost/bind.hpp>

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//

/**
 */
ConnectionManager::ConnectionManager()
	: connections_()
{
}

/**
 * @warning This may block
 */
void ConnectionManager::start(socket_session_ptr session)
{
	//std::cout << "[LOGGING] Manager adding new session\n";
	{ /* forced scope */
		boost::lock_guard<boost::mutex> guard(locker_);
		connections_.insert(session);
	}
	session->start();
}

/**
 * @warning This may block
 */
void ConnectionManager::stop(socket_session_ptr session)
{
	//std::cout << "[LOGGING] Manager removing old session\n";
	{ /* forced scope */
		boost::lock_guard<boost::mutex> guard(locker_);
		connections_.erase(session);
	}
	session->stop();
}

/**
 * @warning This may block
 */
void ConnectionManager::stop_all(void)
{
	//std::cout << "[LOGGING] Manager removing all sessions\n";
	boost::lock_guard<boost::mutex> guard(locker_);
	std::for_each(connections_.begin(), connections_.end(),
		boost::bind(&SocketSession::stop, _1));
	connections_.clear();
}

/**
 * @warning This may block
 */
int ConnectionManager::count(void)
{
	boost::lock_guard<boost::mutex> guard(locker_);
	return connections_.size();
}

/**
 * @warning This may block
 */
void ConnectionManager::write_all(socket_message_ptr message)
{
	//std::cout << "[LOGGING] Manager broadcasting to all sessions\n";
	boost::lock_guard<boost::mutex> guard(locker_);
	std::for_each(connections_.begin(), connections_.end(),
		boost::bind(&SocketSession::write, _1, message));
}

//---------------------------------------------------------------------------//
// Private
//---------------------------------------------------------------------------//
/* nothing */
