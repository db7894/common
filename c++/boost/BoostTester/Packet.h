/**
 * @file Packet.h
 * @brief Definitions for scottrade packets
 */
#pragma once

#ifndef SOCKET_PACKET_H
#define SOCKET_PACKET_H
//---------------------------------------------------------------------------//
// Types
//---------------------------------------------------------------------------//

/**
 * @brief Scottrade chosen packet header
 */
struct MessageHeader
{
	unsigned long type;	/**< The unique id for this packet */
	unsigned long size;	/**< The size of the following packet */
	bool encrypted;		/**< True if this packet is encrypted */
};

#endif
