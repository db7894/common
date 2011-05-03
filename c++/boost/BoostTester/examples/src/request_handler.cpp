/**
 * @file request_handler.cpp
 * @brief Example of using the request handler
 */
#include <iostream>
#include <boost/shared_ptr.hpp>
#include "RequestHandler.hpp"

/* dummy data messages */
struct Request { std::string message; };
struct Reply   { std::string message; };
typedef boost::shared_ptr<Reply> reply_t;

/* example success callback function */
reply_t goodCallback(const Request& request)
{
	reply_t result(new Reply());
	result->message = "[Good] " + request.message;
	return result;
}

/* example failure callback function */
reply_t errorCallback(const Request& request)
{
	reply_t result(new Reply());
	result->message = "[Error] " + request.message;
	return result;
}

/* our test runner */ 
int main(void)
{
	/* initialize objects */
	Request request = {"Testing Message"};
	RequestHandler<Request, reply_t> handler;

	/* make sure we attach the various requests
	 * as well as the error handler to -1
	 */
	handler += std::make_pair(-1, &errorCallback);
	handler += std::make_pair(0, &goodCallback);
	handler += std::make_pair(1, &goodCallback);

	/* try and run some requests */
	for (int i = 0; i < 10; ++i) {
		reply_t reply(handler(request, i%31));
		std::cout << reply->message << "\n";
	}	
	return 0;
}

