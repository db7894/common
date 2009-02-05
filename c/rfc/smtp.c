/**
 * @file smtp.c
 * @brief Implementation of a quick smtp forwarder
 * @note http://www.ietf.org/rfc/rfc0821.txt
 */

#include "smtp.h"
#include "common.h"
#include "socketx.h"

//---------------------------------------------------------------------------// 
// Constants
//---------------------------------------------------------------------------// 

/**
 * @brief Readable index into the smtp_commands array
 */
enum {
	HELO,
	SENDER,
	RECEIVER,
	MESSAGE,
	QUIT,
	RESET,
	USER
};

/**
 * @brief Handle of smtp commands available
 */
const char *smtp_commands[] = {
	"HELO localhost\r\n",	/* initiate communication */
	"MAIL FROM:%s\r\n",		/* set sender */
	"RCPT TO:%s\r\n",		/* recipient(s) */
	"DATA\r\n",				/* start data stream */
	"QUIT\r\n",				/* quit session */
	"RSET\r\n",				/* restart data session */
	"VRFY\r\n",				/* verify a user exists */
};

/**
 * @brief Formatted string of email header
 */
const char *smtp_header = 
	"Date: %s\n"		\
	"From: %s\n"		\
	"To: %s\n"			\
	"Subject: %s\n\n"	\
	"%s"				\
	"\r\n.\r\n";

/**
 * @brief Readable return codes
 */
enum {
	GOOD_HELO	= 220,	/* good login */
	GOOD_QUIT	= 221,	/* good disconnect */
	GOOD_CMD	= 250,	/* successful command */
	GOOD_FWD	= 251,	/* user not found, will forward */
	GOOD_DATA	= 354,	/* ready for data */
	/* anything 500 > is assumed an error */
};

//---------------------------------------------------------------------------// 
// SMTP Helpers
//---------------------------------------------------------------------------// 
/**
 * @brief A quick expect function
 * @param sock The socket to listen on
 * @param result The data to wait for
 * @return 0 if successful, -1 if failure
 */
static int smtp_check(int sock, int result)
{
	int r;
	char *buffer = NULL;

	buffer = socket_recv_all(sock, &r);
	sscanf(buffer, "%d %*s", &r);
	free(buffer);

	return (r == result) ? 0 : -1;
}

/**
 * @brief Wrapper for send and check
 * @param sock The socket to listen on
 * @param data The data to send out
 * @param result The data to wait for
 * @return 0 if successful, -1 if failure
 */
static int smtp_send_check(int sock, char *data, int result)
{
	if (socket_send_all(sock, data, strlen(data)) == 0)
		return smtp_check(sock, result);
	return -1;
}

//---------------------------------------------------------------------------// 
// SMTP Extra Functions
//---------------------------------------------------------------------------// 
/**
 * @brief Wrapper to reset a session
 * @param sock The previously opened socket
 * @return 0 if successful, -1 if failure
 */
int smtp_reset(int sock)
{
	return smtp_send_check(sock,
		   	(char *)smtp_commands[RESET],
		   	GOOD_CMD);
}

/**
 * @brief Wrapper to reset a session
 * @param sock The previously opened socket
 * @return 0 if successful, -1 if failure
 */
int smtp_finger(int sock, char *user)
{
	int size  = strlen(user) + strlen(smtp_commands[USER]);
	char *buffer = xmalloc(size);

	sprintf(buffer, smtp_commands[USER], user);
	size = smtp_send_check(sock, buffer, GOOD_CMD);

	free(buffer);
	return size;
}

//---------------------------------------------------------------------------// 
// SMTP Main Functions
//---------------------------------------------------------------------------// 
/**
 * @brief Wrapper for smtp_connect
 * @param host The hostname to connect to
 * @param port The port to connect to (25)
 * @return 0 if successful, -1 if failure
 */
int smtp_connect(char *host, char *port)
{
	int c;

	c = open_tcp_client(host, port);
	if (smtp_check(c, GOOD_HELO)) {
		socket_close(c);
		return -1;
	}

	if (smtp_send_check(c, (char *)smtp_commands[HELO], GOOD_CMD)) {
		socket_close(c);
		return -1;
	}
	return c;
}

/**
 * @brief Sends an email
 * @param sock The previously opened socket
 * @param addr The email handling structure
 * @return 0 if successful, -1 if failure
 */
int smtp_send(int sock, struct smtp_address *addr)
{
	int i, size, result = -1;
	char *msg;

	size = strlen(addr->data) + strlen(smtp_header) + strlen(addr->subject)
		 + strlen(addr->from);
	msg = xmalloc(size);

	/* add sender */
	sprintf(msg, smtp_commands[SENDER], addr->from);
	if (smtp_send_check(sock, msg, GOOD_CMD))
		goto smtp_cleanup;

	/* add receivers */
	for (i = 0; addr->email[i]; i++) {
		sprintf(msg, smtp_commands[RECEIVER], addr->email[i]);
		if (smtp_send_check(sock, msg, GOOD_CMD))
			goto smtp_cleanup;
	}

	if (smtp_send_check(sock, (char *)smtp_commands[MESSAGE], GOOD_DATA))
			goto smtp_cleanup;

	/* data */
	sprintf(msg, smtp_header,
			"23 Oct 81 11:22:33",
			addr->from,
			addr->email[0],
			addr->subject,
			addr->data
	);

	if (smtp_send_check(sock, msg, GOOD_CMD))
			goto smtp_cleanup;

smtp_cleanup:
	return result;
}

/**
 * @brief Wrapper to close and check email
 * @param sock The previously opened socket
 * @return 0 if successful, -1 if failure
 */
int smtp_close(int sock)
{
	int result = 0;

	if (smtp_send_check(sock, (char *)smtp_commands[QUIT], GOOD_QUIT)) {
		result = -1;
	}
	socket_close(sock);

	return result;
}

/**
 * @brief Wrapper to send an email
 * @param s The server settings
 * @param a The email settings
 * @return 0 if successful, -1 if failure
 */
int smtp_main(struct server_data *s, struct smtp_address *a)
{
	int result = -1;
	int sock;

	if ((sock = smtp_connect(s->server, s->port))) {
		smtp_send(sock, a);
		smtp_close(sock);
		result = 0;
	}

	return result;
}

//---------------------------------------------------------------------------// 
// Test Driver
//---------------------------------------------------------------------------// 
#if 0
int main(void)
{
	char *emails[] = {
		"lansing@sandbox.net",
	   	"root@sandbox.net",
	   	NULL
	};

	struct smtp_address addr = {
		"galen.collins@aperture.com",
		"Testing Email",
		"This is the total message",
		emails
	};

	struct server_data serv = {
		"127.0.0.1",
		"25",
	};

	return smtp_main(&serv, &addr);
}
#endif

