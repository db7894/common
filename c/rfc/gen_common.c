/**
 * @file generic.c
 * @brief Quick protocol wrapper code
 */

#include "common.h"
#include "socketx.h"

//---------------------------------------------------------------------------// 
// Quick protocal implementations
//---------------------------------------------------------------------------// 
/**
 * @brief Implementation of a quick whois query
 * @param host The host to query
 * @param user The user to query
 * @return The resulting user information
 * @note http://www.ietf.org/rfc/rfc0954.txt
 */
char *proto_whois(char *host, char *user)
{
	char *h, *d;
	struct server_data addr = { host, "43"};

	d = xmalloc(strlen(user) + 3 * sizeof(char));
	sprintf(d, "%s\r\n", user);

	h = proto_gen_write(&addr, user);
	free(d);

	return h;
}

/**
 * @brief Implementation of a quick time query
 * @param host The host to query
 * @return The current server time
 * @note http://www.ietf.org/rfc/rfc0.txt
 */
char *proto_time(char *host)
{
	struct server_data addr = { host, "13"};

	return proto_gen_read(&addr);
}

/**
 * @brief Implementation of a quick finger query
 * @param host The host to query
 * @param user The user to query
 * @return The resulting user information
 * @note http://www.ietf.org/rfc/rfc1288.txt
 *
 * The user value can be
 * - A username
 * - part of a username
 * - an email address
 * - empty (for all users)
 */
char *proto_finger(char *host, char *user)
{
	char *h, *d;
	struct server_data addr = { host, "79"};

	if (!user) {
		d = xmalloc(3 * sizeof(char));
		strcpy(d, "\r\n");
	}
	else {
		d = xmalloc(strlen(user) + 3 * sizeof(char));
		sprintf(d, "%s\r\n", user);
	}

	h = proto_gen_write(&addr, d);
	free(d);

	return h;
}

//---------------------------------------------------------------------------// 
// Test Driver
//---------------------------------------------------------------------------// 
#if 0
int main(void)
{
	printf("%s\n", proto_time("time-a.nist.gov"));
	printf("%s\n", proto_whois("127.0.0.1", "lansing"));
	printf("%s\n", proto_finger("127.0.0.1", "lansing"));

	return 0;
}
#endif

