/**
 * @file socketx.c
 * @brief Wrapper functions for all the socket i/o
 */
#include "common.h"
#include "socketx.h"
#include <netdb.h>

//---------------------------------------------------------------------------// 
// Socket Helpers
//---------------------------------------------------------------------------// 

/**
 * @brief Used to abstract away client socket setup
 * @param address The address to open a client to
 * @param port The port to open a client to
 * @param hints The pre-filled address hints structure
 * @return An opened socket
 *
 * @note This function exits if it cannot open a socket
 */
static SOCKET socket_open_client_hint(const char *address, const char *port, struct addrinfo *hints)
{
	struct addrinfo *base, *ai;
	SOCKET sock = INVALID_SOCKET;

	if (getaddrinfo(address, port, hints, &base) != 0) {
		perror("getaddrinfo");
		exit(1);
	}

	for (ai = base; ai != NULL; ai = ai->ai_next) {
		sock = socket(ai->ai_family, ai->ai_socktype, ai->ai_protocol);
		if (sock == INVALID_SOCKET) {
			continue;
		}
		if (connect(sock, ai->ai_addr, ai->ai_addrlen) != SOCKET_ERROR) {
			break;
		}
		socket_close(sock);
	}	
	freeaddrinfo(base);

	if (sock == INVALID_SOCKET) {
		exit(1);
	}

	return sock;
}

/**
 * @brief Used to abstract away server socket setup
 * @param port The port to open a client to
 * @param max The maximum number of clients to listen for
 * @param hints The pre-filled address hints structure
 * @return An opened socket
 *
 * @note This function exits if it cannot open a socket
 */
static SOCKET socket_open_server_hint(const char *port, int max, struct addrinfo *hints)
{
	struct addrinfo *base, *ai;
	SOCKET sock = INVALID_SOCKET;

	if (getaddrinfo(NULL, port, hints, &base) != 0) {
		perror("getaddrinfo");
		exit(1);
	}

	for (ai = base; ai != NULL; ai = ai->ai_next) {
		sock = socket(ai->ai_family, ai->ai_socktype, ai->ai_protocol);
		if (sock == INVALID_SOCKET) {
			continue;
		}

		/* options */

		if (bind(sock, ai->ai_addr, ai->ai_addrlen) < 0) {
			perror("bind");
			socket_close(sock);
			continue;
		}

		if (listen(sock, max) < 0) {
			perror("listen");
			socket_close(sock);
			continue;
		}
		break;
	}	
	freeaddrinfo(base);

	if (sock == INVALID_SOCKET) {
		exit(1);
	}

	return sock;
}

//---------------------------------------------------------------------------// 
// Socket Wrappers
//---------------------------------------------------------------------------// 
#if WIN32
/**
 * @brief Used to initialize winsock
 * @return 0 if successful, never returns on failure
 */
int socket_init(void)
{
	WSADATA wsa;

	if (WSAStartup(MAKEWORD(1,1), &wsa) != 0) {
		dprintf("WSAStartup Failed\n");
		exit(1);
	}
	return 0;
}

/**
 * @brief Used to clean-up winsock
 * @return 0 if successful, -1 on failure
 */
int socket_destroy(void)
{

	if (WSACleanup() == SOCKET_ERROR) {
		dprintf("WSACleanup Failed\n");
		return -1;
	}
	return 0;
}
#endif


/**
 * @brief Wrapper function to close a socket
 * @param sock The socket to close
 * @return 0 if successful, -1 if failure
 */
int socket_close(int sock)
{
#if WIN32
	if (closesocket(sock)) {
#else
	if (close(sock)) {
#endif
		perror("closesocket");
		return -1;
	}
	return 0;
}

/**
 * @brief Wrapper function to socket shutdown
 * @param sock The socket to close
 * @param how How to stop the socket
 * @return 0 if successful, -1 if failure
 *  - 0 stop rx
 *  - 1 stop tx
 *  - 2 stop both
 */
int socket_shutdown(int sock, int how)
{
	if (shutdown(sock, how)) {
		perror("shutdown");
		return -1;
	}
	return 0;
}

/**
 * @brief Wrapper function to socket shutdown
 * @param sock The socket to close
 * @param how How to stop the socket
 * @return 0 if successful, -1 if failure
 *  - SO_DEBUG
 *  - SO_BROADCAST
 *  - SO_REUSEADDR
 *  - SO_KEEPALIVE
 */
int socket_option(int sock, int opt, int val)
{
	if (setsockopt(sock, SOL_SOCKET, opt, (const void *)&val, sizeof(int))) {
		perror("setsockopt");
		return -1;
	}
	return 0;
}

//---------------------------------------------------------------------------// 
// Socket Send
//---------------------------------------------------------------------------// 

/**
 * @brief Utility function to send all data on a tcp socket
 * @param sock Previously opened tcp socket
 * @param buffer Data to send
 * @param size Pointer to size of data (will return amount sent)
 * @returns 0 if successful, -1 otherwise
 *
 * Unlike the other send, this one promises to send all your
 * data.  This can be checked by looking at the result of
 * the size variable.
 *
 * @code
 *    unsigned long address[4] = {0x00740001, 0x0, 0x0, 0x0};
 *    int socket = -1, size;
 *    char msg[] = "Test Message";
 *
 *    socket = open_tcp_client(address, 80, AF_INET);
 *    if (socket == -1)
 *       return -1;
 *    size = strlen(msg);
 *    socket_sendall(socket, msg, size);
 *    socket_close(socket);
 * @endcode
 */
int socket_send_all(int sock, char *buffer, int size)
{
	int flags = MSG_DONTWAIT;
	int n = 0, total = size;
	
	while (sock && (total > 0)) {
		n = send(sock, buffer + (size - total), total, flags);
		if (n == -1) break; /* bad send */
		total -= n;         /* how much is left to send */
 	}
 	return (n == -1) ? -1 : 0;
}

/**
 * @brief Sends some data to a client
 * @param sock Socket to send on
 * @param buffer Buffer to send
 * @param size Size of buffer
 * @return number of bytes sent, -1 if bad socket
 */
int socket_send(int sock, char *buffer, int size)
{
	int flags = 0;
	int n = 0;
	
    if (!sock)
        return -1;

	n = send(sock, buffer, size, flags);

 	return n;
}

/**
 * @brief Sends some data to a client w/o blocking
 * @param sock Socket to send on
 * @param buffer Buffer to send
 * @param size Size of buffer
 * @return number of bytes sent, -1 if bad socket
 */
int socket_send_fast(int sock, char *buffer, int size)
{
	int flags = MSG_DONTWAIT;
	int n = 0;

    if (!sock)
        return -1;
	
	n = send(sock, buffer, size, flags);

 	return n;
}

//---------------------------------------------------------------------------// 
// Socket Receive
//---------------------------------------------------------------------------// 

/**
 * @brief Recieves some data from a client
 * @param sock Socket to listen on
 * @param buffer Buffer to store data in
 * @param size Size of buffer
 * @return number of bytes read, -1 if bad socket
 */
int socket_recv(int sock, char *buffer, int size)
{
    int result = 0, flags = 0;

    if (!sock)
        return -1;

	result = recv(sock, buffer, size, flags);

    return result;
}

/**
 * @brief Recieves some data from a client w/o blocking
 * @param sock Socket to listen on
 * @param buffer Buffer to store data in
 * @param size Size of buffer
 * @return number of bytes read, -1 if bad socket
 */
int socket_recv_fast(int sock, char *buffer, int size)
{
    int result = 0, flags = MSG_DONTWAIT;

    if (!sock)
        return -1;

	result = recv(sock, buffer, size, flags);

    return result;
}

/**
 * @brief Recieves data from a client
 * @param sock Socket to listen on
 * @param buffer Buffer to store data in
 * @param size Size of buffer
 * @return +number of read bytes if successful, -1 otherwise
 *
 * This function attempts to read all data from the socket.
 * For this to work, you need to pass in a buffer that is 
 * big enough to get all the data that should be available.
 *
 * If there is more than it can hold, it will return what
 * it got. You need to be real sure to use this function,
 * otherwise, it is a segv waiting to happen.
 *
 * @code
 *    int socket = -1, host = -1, bytes = 1;
 *    char *buffer[50] = {0x0};
 *
 *    socket = open_tcp_server(NULL, 0, AF_INET, 0);
 *    if (socket == -1)
 *       return -1;
 *    socket_listen(socket, 5);
 *    host = socket_accept(socket, NULL);
 *
 *    bytes = socket_receive_loop(host, buffer, 50);
 *    socket_close(host);
 * @endcode
 */
char *socket_recv_all(int sock, int *size)
{
    int flags = MSG_DONTWAIT;						/* don't block */
	size_t c = 64, s = 0, r;
    char *p = xcalloc(c, sizeof(char));

    if (!sock) {
		*size = 0;
        return NULL;
	}

    do {
        r = recv(sock, &p[s], c - s - 1, flags);	/* grab available */
		if (r > 0) {								/* if data */
        	s += r;									/* increment buffer pointer */
			if (s >= (c - 1)) {						/* if buffer is filled */
				c *= 2;								/* increase size */
				p = xrealloc(p, c);
			}
		}
    } while (r > 0 || s == 0);   					/* continue till no more data */
	*size = (int )s;								/* return size */

    return p;
}


//---------------------------------------------------------------------------// 
// TCP / UDP Client Socket Openers
//---------------------------------------------------------------------------// 

/**
 * @brief Quickly opens a tcp client (with ip version)
 * @param address The address to open a client to
 * @param port The port to open a client to
 * @param af The selected AF_ constant
 * @return An opened socket
 */
int open_tcp_client_vers(const char *address, const char *port, int af)
{
	struct addrinfo hints;

	memset(&hints, 0, sizeof(hints));
	hints.ai_family = af;
	if (af != AF_INET || af != AF_INET6)
		hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_STREAM;

	return socket_open_client_hint(address, port, &hints);
}

/**
 * @brief Quickly opens a tcp client
 * @param address The address to open a client to
 * @param port The port to open a client to
 * @return An opened socket
 */
int open_tcp_client(const char *address, const char *port)
{
	return open_tcp_client_vers(address, port, AF_UNSPEC);
}

/**
 * @brief Quickly opens a udp client (with ip version)
 * @param address The address to open a client to
 * @param port The port to open a client to
 * @return An opened socket
 */
int open_udp_client_vers(const char *address, const char *port, int af)
{
	struct addrinfo hints;

	memset(&hints, 0, sizeof(hints));
	hints.ai_family = af;
	if (af != AF_INET || af != AF_INET6)
		hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_DGRAM;

	return socket_open_client_hint(address, port, &hints);
}

/**
 * @brief Quickly opens a udp client
 * @param address The address to open a client to
 * @param port The port to open a client to
 * @return An opened socket
 */
int open_udp_client(const char *address, const char *port)
{
	return open_udp_client_vers(address, port, AF_UNSPEC);
}

//---------------------------------------------------------------------------// 
// TCP / UDP Server Socket Openers
//---------------------------------------------------------------------------// 

/**
 * @brief Quickly opens a tcp server (with ip version)
 * @param port The port to open a client to
 * @param max The maximum number of supported connections
 * @return An opened socket
 */
int open_tcp_server_vers(const char *port, int max, int af)
{
	struct addrinfo hints;

	memset(&hints, 0, sizeof(hints));
	hints.ai_family = af;
	if (af != AF_INET || af != AF_INET6)
		hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_STREAM;
	hints.ai_flags = AI_PASSIVE;

	return socket_open_server_hint(port, max, &hints);
}

/**
 * @brief Quickly opens a tcp server
 * @param port The port to open a client to
 * @param max The maximum number of supported connections
 * @return An opened socket
 */
int open_tcp_server(const char *port, int max)
{
	return open_tcp_server_vers(port, max, AF_UNSPEC);
}

/**
 * @brief Quickly opens a udp server (with ip version)
 * @param port The port to open a client to
 * @param max The maximum number of supported connections
 * @return An opened socket
 */
int open_udp_server_vers(const char *port, int max, int af)
{
	struct addrinfo hints;

	memset(&hints, 0, sizeof(hints));
	hints.ai_family = af;
	if (af != AF_INET || af != AF_INET6)
		hints.ai_family = AF_UNSPEC;
	hints.ai_socktype = SOCK_DGRAM;
	hints.ai_flags = AI_PASSIVE;

	return socket_open_server_hint(port, max, &hints);
}

/**
 * @brief Quickly opens a udp server
 * @param port The port to open a client to
 * @param max The maximum number of supported connections
 * @return An opened socket
 */
int open_udp_server(const char *port, int max)
{
	return open_udp_server_vers(port, max, AF_UNSPEC);
}

//---------------------------------------------------------------------------// 
// Test Driver
//---------------------------------------------------------------------------// 
#ifdef SERVER
int main(void)
{
	int size, c, sock;
	char buffer[1024] = 
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n"
		"abcdefghijklmnopqrstuvwxyz\n";

	struct sockaddr client;

	c = open_tcp_server("6666", 5);
	while (1) {
		size = sizeof(client);
		sock = accept(c, &client, &size);
		size = strlen(buffer);
		if (sock > 0) {
			socket_send_all(sock, buffer, size);
			socket_close(sock);
		}
	}

	return 1;
}
#endif

#ifdef CLIENT
int main(void)
{
	int c, size;
	char *b;

	memset(buffer, 0, sizeof(buffer));

	c = open_tcp_client("127.0.0.1", "6666");
	b = socket_recv_all(c, *size);
	if (size > 0) {
		printf("Result:\n%s\n", b);
	}
	socket_close(sock);

	return 1;
}
#endif

