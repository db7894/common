/**
 * @file Packet.h
 * @brief Definitions for base packets
 */
#pragma once

#ifndef SOCKET_PACKET_H
#define SOCKET_PACKET_H
//---------------------------------------------------------------------------//
// Types
//---------------------------------------------------------------------------//

/**
 * @brief Base chosen packet header
 */
struct MessageHeader
{
	unsigned long type;	/**< The unique id for this packet */
	unsigned long size;	/**< The size of the following packet */
	bool encrypted;		/**< True if this packet is encrypted */
};

#endif
