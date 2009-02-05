/**
 * @file generic.c
 * @brief Quick protocol wrapper code
 */

#include "common.h"
#include "generic.h"
#include "socketx.h"

//---------------------------------------------------------------------------// 
// Quick Protocal Implementors
//---------------------------------------------------------------------------// 

/**
 * @brief Wrapper to send a generic query
 * @param s The server settings
 * @param user The data to query with
 * @return The resulting information, or NULL
 */
char *proto_gen_write(struct server_data *s, char *data)
{
	int r;
	char *buffer = NULL;
	int sock;

	sock = open_tcp_client(s->server, s->port);
	if (socket_send_all(sock, data, strlen(data)) == 0) {
		buffer = socket_recv_all(sock, &r);
	}
	socket_close(sock);
	free(data);

	return buffer;
}

/**
 * @brief Wrapper to read from a generic socket
 * @param s The server settings
 * @return The resulting information, or NULL
 */
char *proto_gen_read(struct server_data *s)
{
	char *buffer = NULL;
	int r, sock;

	sock = open_tcp_client(s->server, s->port);
	buffer = socket_recv_all(sock, &r);
	socket_close(sock);

	return buffer;
}

//---------------------------------------------------------------------------// 
// Protocal Helpers
//---------------------------------------------------------------------------// 

/**
 * @brief Wrapper to read and test from a socket and return data
 * @param sock The socket to read from
 * @param test The string to test for
 * @param buf Pointer location for result
 * @return 0 if successful, -1 if failure
 *
 * This function first does a recv all and then
 * performs a search on the resulting data for
 * the first occurance of the result data. It
 * then creates a new buffer just past that
 * result and returns it.
 */
int proto_read_test_data(int sock, char *test, char *buf)
{
	char *b = NULL, *c = NULL;
	int r = -1;

	b = socket_recv_all(sock, &r);
	if (b && r > 0) {
		c = strstr(b, test);
		if (c != NULL) {
			c += strlen(test);			/* jump past result */
			buf = xmalloc(strlen(c));	/* copy to new buffer */
			strcpy(buf, c);
		}
		free(b);
	}

	return (c == NULL) ? -1 : 0;
}

/**
 * @brief Wrapper to read and test from a socket and return data
 * @param sock The socket to read from
 * @param data The data to send before test
 * @param test The string to test for
 * @param buf Pointer location for result
 * @return 0 if successful, -1 if failure
 *
 * This function first does a recv all and then
 * performs a search on the resulting data for
 * the first occurance of the result data. It
 * then creates a new buffer just past that
 * result and returns it.
 */
int proto_write_test_data(int sock, char *data, char *test, char *buf)
{

	if (socket_send_all(sock, data, strlen(data)) == 0) {
		return proto_read_test_data(sock, test, buf);
	}
	return -1;
}

/**
 * @brief Wrapper to read and test from a socket
 * @param sock The socket to read from
 * @param test The string to test for
 * @return 0 if successful, -1 if failure
 */
int proto_read_test(int sock, char *test)
{
	char *b = NULL, *c = NULL;
	int r = -1;

	b = socket_recv_all(sock, &r);
	if (b && r > 0) {
		c = strstr(b, test);
		free(b);
	}

	return (c == NULL) ? -1 : 0;
}

/**
 * @brief Wrapper to read and test from a socket
 * @param sock The socket to read from
 * @param data The data to send before test
 * @param test The string to test for
 * @return 0 if successful, -1 if failure
 */
int proto_write_test(int sock, char *data, char *test)
{

	if (socket_send_all(sock, data, strlen(data)) == 0) {
		return proto_read_test(sock, test);
	}
	return -1;
}
