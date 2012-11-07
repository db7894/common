/**
 * @file globals.h
 * @brief Types and forward declarations used throughout
 *
 * This should decouple header file dependency, speed up compilation, and
 * not require everthing to rebuild when something small is changed.
 */
#pragma once

#ifndef BASHWORK_GLOBALS_H
#define BASHWORK_GLOBALS_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <boost/shared_ptr.hpp>

//---------------------------------------------------------------------------//
// Logging
//---------------------------------------------------------------------------//
/* add logging globals here */

//---------------------------------------------------------------------------//
// Forward Declarations
//---------------------------------------------------------------------------//
template <typename Type> class ObjectFactory;
class ConnectionManager;
class SocketServer;
class SocketClient;
class SocketSession;
class SocketMessage;
class IProtocol;

//---------------------------------------------------------------------------//
// Shared Wrappers
//---------------------------------------------------------------------------//
typedef boost::shared_ptr<SocketSession> socket_session_ptr;
typedef boost::shared_ptr<SocketMessage> socket_message_ptr;
typedef boost::shared_ptr<ConnectionManager> socket_manager_ptr;
typedef boost::shared_ptr<IProtocol> socket_protocol_ptr;

#endif