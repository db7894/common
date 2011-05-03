/**
 * @file RequestHandler.hpp
 * @brief Generic handler of reply callbacks for request input
 */
#pragma once

#ifndef REQUEST_HANDLER_H
#define REQUEST_HANDLER_H
//---------------------------------------------------------------------------//
// Includes
//---------------------------------------------------------------------------//
#include <map>
#include <boost/function.hpp>
#include <boost/utility.hpp>

//---------------------------------------------------------------------------//
// Types
//---------------------------------------------------------------------------//
/* nothing */

//---------------------------------------------------------------------------//
// Implementation
//---------------------------------------------------------------------------//

/**
 * @brief A generic request handler to simplify reply callbacks
 *
 * You must make sure to attach a handler with the id of -1 for
 * a generic 'invalid request' result.  Otherwise, a bad request
 * will throw.
 *
 * Example:
 * @include request_handler.cpp
 */
template <typename RequestType, typename ReplyType>
class RequestHandler
: private boost::noncopyable
{
public:
	/* Typedefs for readability */ 
	typedef boost::function<ReplyType(const RequestType&)> function_ptr;

public:
	/**
	 * @brief Constructor to initialize the handler container
	 */
	RequestHandler(void)
	{
	}

	/**
	 * @brief Destructor to clean the handler container
	 */
	~RequestHandler(void)
	{
		handlers_.clear();
	}

	/**
	 * @brief Helper function to retrieve current number of handlers
	 * @return The number of currently attached handlers
	 */
	size_t Size(void)
	{
		return handlers_.size();
	}

	/**
	 * @brief Add an id/function pair to handle
	 * @param handle A pair of a request id and the function to call for it
	 * @return *this
	 */
	RequestHandler& operator+=(std::pair<unsigned long,function_ptr> handle)
	{
		handlers_[handle.first] = handle.second;
		return *this;
	}

	/**
	 * @brief Add an id/function pair to handle
	 * @param handle A pair of a request id and the function to call for it
	 * @return *this
	 */
	RequestHandler& operator+(std::pair<unsigned long,function_ptr> handle)
	{
		return RequestHandler(*this) += handle;
	}

	/**
	 * @brief Remove a id/function pair from being handled
	 * @param id The id of the request type to remove
	 * @return *this
	 */
	RequestHandler& operator-=(const unsigned long id)
	{
		handlers_.erase(id);
		return *this;
	}

	/**
	 * @brief Remove a id/function pair from being handled
	 * @param id The id of the request type to remove
	 * @return *this
	 */
	RequestHandler& operator-(const unsigned long id)
	{
		return RequestHandler(*this) -= id;
	}

	/**
	 * @brief Handles a request using the supplied request handlers
	 * @param request Constant reference to a incoming request
	 * @param id The type of incoming request
	 * @return A constructed reply object
	 */
	ReplyType operator()(const RequestType& request, unsigned long id)
	{
		handler_type::iterator it = handlers_.find(id);
		if (it != handlers_.end())
			return (*it).second(request);
		return handlers_[-1](request); /* invalid request */
	}

private:
	/* Typedefs for readability */ 
	typedef std::map<int, function_ptr> handler_type;
	/**
	 * @brief The container to hold callback handlers
	 */
	handler_type handlers_;
};


#endif
