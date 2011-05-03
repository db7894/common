/**
 * @file Protocol.h
 * @brief Base implementation of a protocol
 */
#pragma once

#ifndef SOCKET_PROTOCOL_H
#define SOCKET_PROTOCOL_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include "globals.h"

//---------------------------------------------------------------------------//
// Types
//---------------------------------------------------------------------------//
/* nothing */

//---------------------------------------------------------------------------//
// Implementation
//---------------------------------------------------------------------------//

/**
 * @brief Interface for a Protocol
 */
class IProtocol
{
public:
	/**
	 * @brief Base implementation of a protocol
	 */
	virtual ~IProtocol() {};

	/**
	 * @brief Implement request processing logic here
	 * @param message The received message
	 * @return True to continue working, false to disconnect
	 */
	virtual bool HandleReceive(socket_message_ptr message) = 0;

	/**
	 * @brief Implement connection logic here
	 * @return True to continue working, false to disconnect
	 */
	virtual bool HandleConnect(void) = 0;

	/**
	 * @brief Implement disconnection logic here
	 * @return True to continue working, false to disconnect
	 */
	virtual bool HandleDisconnect(void) = 0;
};

#endif
