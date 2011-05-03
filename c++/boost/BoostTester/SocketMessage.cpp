/**
 * @file SocketMessage.cpp
 * @brief Message used to wrap socket communication
 */
#include "stdafx.h"
#include "SocketMessage.h"

//---------------------------------------------------------------------------//
// Ctor/Dtor
//---------------------------------------------------------------------------//
/**
 * This will store a default initialized header
 */
SocketMessage::SocketMessage(void)
	: body_size_(max_header_size), header_(), buffer_()
{
	encode_header(); /* update header to reflect */
}

/**
 * Nothing really
 */
SocketMessage::~SocketMessage(void)
{
	/* nothing */
	//body_size_ = 0;
	//memset((void *)&header_, 0, max_header_size);
}

/**
 */
SocketMessage::SocketMessage(const SocketMessage& rhs)
{
	if (this != &rhs) {
		/* we don't need the entire buffer */
		this->body_size_ = rhs.body_size_;
		memcpy(this->buffer_, rhs.buffer_, this->body_size_);
		memcpy(&this->header_, &rhs.header_, sizeof(MessageHeader));
	}
}

/**
 */
SocketMessage& SocketMessage::operator=(const SocketMessage& rhs)
{
	if (this != &rhs) {
		/* we don't need the entire buffer */
		this->body_size_ = rhs.body_size_;
		memcpy(this->buffer_, rhs.buffer_, this->body_size_);
		memcpy(&this->header_, &rhs.header_, sizeof(MessageHeader));
	}
	return *this;
}


//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//

/**
 * @param type The id representing the message type
 * @param encrypted True if this packet should be encrypted
 * 
 * This is the only method that is concerned with extra variables in
 * the message header. All other methods simply default the entire
 * header to {0}.
 */
void SocketMessage::SetOptions(int type, bool encrypted)
{
	header_.type = type;
	header_.encrypted = encrypted;
}

/**
 * @return The current buffer size
 */
size_t SocketMessage::Size(void)
{
	return body_size_ - max_header_size;
}

/**
 * @return void
 */
void SocketMessage::Reset(void)
{
	/* in case we expand header we only have to change in one place */
	MessageHeader reset_ = {0};
	header_ = reset_;
	body_size_ = max_header_size;
	/* reseting the size should be enough, this will just steal time */
	//memset((buffer + HEADER_SIZE), 0, max_buffer_size);
	encode_header();
}

//---------------------------------------------------------------------------//
// Protected
//---------------------------------------------------------------------------//
/**
 * @return true if the header was valid, false otherwise
 * As a side effect, if the header is invalid, the message will be reset
 */
bool SocketMessage::decode_header(void)
{
	memcpy((void *)&header_, buffer_, max_header_size);
	if (header_.size > max_body_size) {
		Reset();
		return false;
	}
	body_size_ = header_.size + max_header_size;
	return true;
}

//---------------------------------------------------------------------------//
// Private
//---------------------------------------------------------------------------//
/**
 * @return void
 * Each call to this will update the header field and update the header
 * stored in the buffer.
 */
void SocketMessage::encode_header(void)
{
	header_.size = body_size_ - max_header_size;
	memcpy(buffer_, (void *)&header_, max_header_size);
}
