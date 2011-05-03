/**
 * @file async_socket_client.cpp
 * @brief Example of a async socket client writer
 */
#include <iostream>
/* boost candy */
#include <boost/thread.hpp>
#include <boost/asio.hpp>
#include <boost/bind.hpp>
#include <boost/scoped_ptr.hpp>
/* our candy */
#include "SocketMessage.h"
#include "SocketClient.h"
#include "ObjectFactory.hpp"

int main(int argc, char* argv[])
{
	try {
		if (argc != 3) {
			std::cerr << "Usage: " << argv[0] << " <host> <port>\n";
			return -1;
		}

		boost::thread_group thread_manager;
		boost::asio::io_service scheduler;
		boost::scoped_ptr<ObjectFactory<SocketMessage> > 
			factory(new ObjectFactory<SocketMessage>(8));

		/* initialize the client */	
		boost::scoped_ptr<SocketClient> client(
			new SocketClient(argv[1], argv[2],
				scheduler,				/* underlying io service */
				factory->create()));	/* the pooled read buffer */
		/* create a background service handler */
		thread_manager.create_thread(
				boost::bind(&boost::asio::io_service::run, &scheduler));

		/* main dispatcher loop */
		bool keep_running = true;
		do {
			std::string input;
			std::cin >> input;
			if (input == "exit") {
				keep_running = false;
			} else {
				/* message is returned to the pool when send is finished */
				socket_message_ptr message = factory->create();
				message->Append(input.data(), input.length());
				client->Write(message); /* this is queued and does not block */
			}
		} while (keep_running);

		/* clean-up */
		scheduler.stop();			/* everything will shutdown with this */
		thread_manager.join_all();	/* be nice and wait for our thread */
	}
	catch (std::exception& e) {
		std::cerr << "Exception: " << e.what() << "\n";
	}

	return 0;
}

