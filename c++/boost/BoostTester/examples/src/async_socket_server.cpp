/**
 * @file async_socket_server.cpp
 * @brief Example of an async socket server
 */
/* windows candy */
#include <windows.h>
/* boost candy */
#include <boost/lexical_cast.hpp>
#include <boost/function.hpp>
/* our candy */
#include "SocketServer.h"

/**
 * @brief Function to run on signaled to exit
 * @return void
 */
boost::function0<void> console_ctrl_function;

/**
 * @brief Console handler function
 * @param ctrl_type The type of signal to handle
 * @return true if we handled the signal correctly
 */
BOOL WINAPI console_ctrl_handler(DWORD ctrl_type)
{
  switch (ctrl_type)
  {
  case CTRL_C_EVENT:
  case CTRL_BREAK_EVENT:
  case CTRL_CLOSE_EVENT:
  case CTRL_SHUTDOWN_EVENT:
    console_ctrl_function();
    return TRUE;
  default:
    return FALSE;
  }
}

/**
 * @brief Test Driver
 * @param argc The number of command line arguments
 * @param argv An array of the command line arguments
 * @return Always 0
 */
int main(int argc, char* argv[])
{
	try {
		if (argc != 2) {
			std::cerr << "Usage: " << argv[0] << " <port>\n";
			return -1;
		}
		/* initialize server */
		int port = boost::lexical_cast<int>(argv[1]);
		SocketServer server(port, false);

		/* what a system interrupt will call */
		console_ctrl_function = boost::bind(&SocketServer::Stop, &server);
		SetConsoleCtrlHandler(console_ctrl_handler, TRUE);

 		/* block on service threads */
		server.Start();
	}
	catch (std::exception& e) {
		std::cerr << "Exception: " << e.what() << "\n";
	}

	return 0;
}

