/**
 * @file SocketMessage.h
 * @brief Message used to wrap socket communication
 * @todo Make this a pimpl as the implementation will more than likely change
 * over time, however, we will have to loose the two templated constructors.
 */
#pragma once

#ifndef BASHWORK_SOCKET_MESSAGE_H
#define BASHWORK_SOCKET_MESSAGE_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include "Packet.h"
#include "globals.h"

//---------------------------------------------------------------------------//
// Interface
//---------------------------------------------------------------------------//

/**
 * @brief Interface for a Protocol
 */
class SocketMessage
{
	/**
	 * @brief So we can write to the underlying buffer
	 */
	friend SocketSession;
	friend SocketClient;

protected:
	/**
	 * @brief Class specific constants
	 */
	enum {
		max_header_size = sizeof(MessageHeader),/**< Header size compile generated */
		max_body_size	= 256,					/**< Size of largest packet */
	};

public:
	/**
	 * @brief Default Constructor
	 */
	SocketMessage(void);
	/**
	 * @brief Constructor from a message type
	 */
	template <typename Type>
	SocketMessage(const Type& input);
	/**
	 * @brief Constructor from a message type with size
	 */
	template <typename Type>
	SocketMessage(const Type& input, size_t size);
	/**
	 * @brief Default Destructor
	 */
	~SocketMessage(void);
	/**
	 * @brief Default Destructor
	 * @param copy Constant socket message to copy
	 * @return This handle
	 */
	SocketMessage(const SocketMessage& copy);
	/**
	 * @brief Assign operator
	 * @param assign Constant socket message to assign with
	 * @return void
	 */
	SocketMessage& operator=(const SocketMessage& assign);

	/**
	 * @brief Used to set the packet options
	 */
	void SetOptions(int type, bool encrypted);
	/**
	 * @brief Returns the current size of the message body
	 */
	size_t Size(void);
	/**
	 * @brief Resets the Message to a default state
	 */
	void Reset(void);
	/**
	 * @brief Adds data to the buffer
	 */
	template <typename Type>
	void Append(const Type& input, size_t size);
	/**
	 * @brief Adds data to the buffer
	 */
	template <typename Type>
	void Append(const Type* input, size_t size);
	/**
	 * @brief Adds data to the buffer
	 */
	template <typename Type>
	SocketMessage& operator+=(const Type& input);
	/**
	 * @brief Adds data to the buffer
	 */
	template <typename Type>
	SocketMessage& operator+(const Type& input);

protected:
	/**
	 * @brief Tests the received header for validity
	 */
	bool decode_header(void);

	/* NOTE: These are defined here so we can unit test the final
	 * packet. Moving them to the implementation file will cause
	 * a linker error in the unit tester.
	 */

	/**
	 * @brief Tests the received header for validity
	 * @warning Writes past max_header_size will overwrite the packet body
	 * @return A pointer to the header body
	 */
	char* get_raw_header(void)	{ return buffer_; }
	/**
	 * @brief Tests the received header for validity
	 * @return A pointer to the packet body
	 */
	char* get_raw_body(void)	{ return (buffer_ + max_header_size); }
	/**
	 * @brief Returns the entire packet length
	 * @return The current buffer size
	 */
	size_t get_total_size(void)	{ return body_size_; }

private:
	/**
	 * @brief Used to update the message header after an append
	 */
	void encode_header(void);

private:
	/**
	 * @brief Handle to the current message header
	 */
	MessageHeader header_;
	/**
	 * @brief The current size of the message body
	 */
	size_t body_size_;
	/**
	 * @brief The current message buffer
	 */
	char buffer_[max_header_size + max_body_size];
};

//---------------------------------------------------------------------------//
// Method Templates
//---------------------------------------------------------------------------//

/**
 * @param input The POD to append to the buffer
 */
template <typename Type>
SocketMessage::SocketMessage(const Type& input)
	: body_size_(max_header_size), header_(), buffer_()
{
	Append(input, sizeof(input));
}

/**
 * @param input The POD to append to the buffer
 * @param size How of input to add to the buffer
 */
template <typename Type>
SocketMessage::SocketMessage(const Type& input, size_t size)
	: body_size_(max_header_size), header_(), buffer_()
{
	Append(input, size);
}

/**
 * @param input The POD to append to the buffer
 * @param size How of input to add to the buffer
 * @return void
 */
template <typename Type>
void SocketMessage::Append(const Type& input, size_t size)
{
	if ((size + body_size_) > max_body_size)
		size = max_body_size - body_size_;
	memcpy((buffer_ + body_size_), (void *)&input, size);
	body_size_ += size;
	encode_header(); /* update header to reflect */
}

/**
 * Specialized for pointers
 * @param input The POD to append to the buffer
 * @param size How of input to add to the buffer
 * @return void
 */
template <typename Type>
void SocketMessage::Append(const Type* input, size_t size)
{
	if ((size + body_size_) > max_body_size)
		size = max_body_size - body_size_;
	memcpy((buffer_ + body_size_), (void *)input, size);
	body_size_ += size;
	encode_header(); /* update header to reflect */
}


/**
 * @param input The POD to append to the buffer
 * @return *this
 */
template <typename Type>
SocketMessage& SocketMessage::operator+=(const Type& input)
{
	Append(input, sizeof(input));
	return *this;
}

/**
 * @param input The POD to append to the buffer
 * @return *this
 */
template <typename Type>
SocketMessage& SocketMessage::operator+(const Type& input)
{
	return SocketMessage(*this) += input;
}

#endif
