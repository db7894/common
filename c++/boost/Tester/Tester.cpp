/**
 * @file async_socket_server.cpp
 * @brief Example of an async socket server
 */
/* windows candy */
#include "stdafx.h"
#include <windows.h>
/* boost candy */
#include <boost/lexical_cast.hpp>
#include <boost/function.hpp>
/* our candy */
#include "Protocol.h"
#include "SocketServer.h"
#include "SocketSession.h"

/* example protocol */
class EchoProtocol : public IProtocol
{
public:
	EchoProtocol(socket_session_ptr session)
		: session_(session)
	{}

	virtual ~EchoProtocol(void)
	{}

	virtual bool Handle(socket_message_ptr message)
	{
		session_->write(message);
		return true;
	}

private:
	socket_session_ptr session_;
};

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
 * @return Always 0
 */
int main(void)
{
	try {
		/* initialize server */
		boost::shared_ptr<IProtocol> protocol(new EchoProtocol);
		SocketServer server(7, protocol, false);

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

