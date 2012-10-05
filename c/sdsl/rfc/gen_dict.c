/**
 * @file generic.c
 * @brief Quick protocol wrapper code
 *
 * The DICT URL scheme is used to refer to definitions or word lists
 * available using the DICT protocol:
 *
 * dict://<user>;<auth>@<host>:<port>/d:<word>:<database>:<n>
 * dict://<user>;<auth>@<host>:<port>/m:<word>:<database>:<strat>:<n>
 *
 * The "/d" syntax specifies the DEFINE command (section 3.2), whereas
 * the "/m" specifies the MATCH command (section 3.3).
 *
 */

#include "common.h"
#include "socketx.h"

//---------------------------------------------------------------------------// 
// Constants
//---------------------------------------------------------------------------// 
/**
 * @brief Listing of readable result codes
 */
enum {
	/* errors */
	SERVER_DOWN			= 420,
	SERVER_SHUTDOWN		= 421,
	BAD_COMMAND			= 500,
	BAD_PARAM			= 501,
	NO_COMMAND			= 502,
	NO_PARAM			= 503,
	ACCESS_DENIED		= 530,

	NEXT_WORD			= 151,
};

/**
 * @brief Listing of available commands
 */
const struct proto_cmd commands[] =
{
	{ "CLIENT srfc-lib\r\n",	"250", "000"},	/* dict hello */
	{ "HELP\r\n",				"113", "000"},	/* dict serv help */
	{ "DEFINE %s %s\r\n",		"150", "552"},	/* '!' for one '*' for all */
	{ "MATCH %s %s %s\r\n",		"150", "552"},	/* '!' for one '*' for all */
	{ "SHOW DB\r\n",			"110", "554"},	/* show all databases */
	{ "SHOW STRATEGIES\r\n",	"111", "555"},	/* show all databases */
	{ "SHOW INFO %s\r\n",		"112", "550"},	/* show database info */
	{ "SHOW SERVER\r\n",		"114", "000"},	/* show server info */
	{ "STATUS\r\n",				"210", "000"},	/* show server status */
	{ "\r\n.\r\n",				"000", "000"},	/* end of definition */
	{ "QUIT\r\n",				"221", "000"}	/* quit */
};

/**
 * @brief Listing of available commands
 */
enum
{
	CLIENT,
	HELP,
	DEFINE,
	MATCH,
	SHOW_DB,
	SHOW_STRAT,
	SHOW_INFO,
	SHOW_SERVER,
	STATUS,
	EOD,
	QUIT,
};

/**
 * @brief Listing of lookup strategies
 */
char const *dict_strategy[] =
{
	"exact",
	"prefix",
	"substring",
	"suffix",
	"re",
	"regexp",
	"soundex",
	"lev",
	"word"
};

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
static int proto_dict_quick(int sock, int cmd, char *b)
{
	int r;
	char *h, *d;

	d = xmalloc(strlen(user) + 3 * sizeof(char));
	sprintf(d, "%s\r\n", commands[cmd]->cmd);

	if (b)	r = proto_write_test_data(sock, d, commands[cmd]->pass, b);
	else	r =proto_read_test(sock, commands[cmd]->pass);

	free(d);	/* free command string */

	return r;
}

//---------------------------------------------------------------------------// 
// Quick wrappers
//---------------------------------------------------------------------------// 
/**
 * @brief wrapper to issue a dict status command
 * @param sock the socket to r/w to 
 * @param b the buffer to store result to
 * @return 0 if successful, -1 if failure
 */
int proto_dict_status(int sock, char **b)
{
	return proto_dict_quick(sock, dict_commands[STATUS], *b);
}

/**
 * @brief wrapper to issue a dict client command
 * @param sock the socket to r/w to 
 * @return 0 if successful, -1 if failure
 */
int proto_dict_client(int sock)
{
	return proto_dict_quick(sock, dict_commands[CLIENT], NULL);
}

/**
 * @brief wrapper to issue a dict help command
 * @param sock the socket to r/w to 
 * @param b the buffer to store result to
 * @return 0 if successful, -1 if failure
 */
int proto_dict_help(int sock, char **b)
{
	return proto_dict_quick(sock, dict_commands[CLIENT], *b);
}

/**
 * @brief wrapper to issue a dict showdb command
 * @param sock the socket to r/w to 
 * @param b the buffer to store result to
 * @return 0 if successful, -1 if failure
 */
int proto_dict_showdb(int sock, char **b)
{
	return proto_dict_quick(sock, dict_commands[SHOW_DB], *b);
}

/**
 * @brief wrapper to issue a dict status command
 * @param sock the socket to r/w to 
 * @param b the buffer to store result to
 * @return 0 if successful, -1 if failure
 */
int proto_dict_status(int sock, char **b)
{
	b = proto_dict_quick(sock, dict_commands[STATUS], *b);
}

/**
 * @brief wrapper to issue a dict server command
 * @param sock the socket to r/w to 
 * @param b the buffer to store result to
 * @return 0 if successful, -1 if failure
 */
int proto_dict_server(int sock, char **b)
{
	b = proto_dict_quick(sock, dict_commands[SHOW_SERVER], *b);
}

/**
 * @brief wrapper to issue a dict strat command
 * @param sock The socket to r/w to 
 * @param b The buffer to store result to
 * @return 0 if successful, -1 if failure
 */
int proto_dict_strat(int sock, char **b)
{
	b = proto_dict_quick(sock, dict_commands[SHOW_STRAT], *b);
}

//---------------------------------------------------------------------------// 
// Core Commands
//---------------------------------------------------------------------------// 

/**
 * @brief wrapper to issue a dict match command
 * @param sock The socket to r/w to 
 * @return 0 if successful, -1 if failure
 */
int proto_dict_match(int sock)
{
}

/**
 * @brief wrapper to issue a dict define command
 * @param sock The socket to r/w to 
 * @return 0 if successful, -1 if failure
 */
int proto_dict_define(int sock)
{
}

//---------------------------------------------------------------------------// 
// Init wrappers
//---------------------------------------------------------------------------// 

/**
 * @brief Quick wrapper to close a dict session
 * @param s Server info structure
 * @return Socket if successful, -1 if failure
 * @note the default port is 2628
 */
int proto_dict_quit(int sock)
{
	int r;
	char *b = proto_dict_quick(sock, dict_commands[CLIENT], &r);

	free(b);
	socket_close(sock);

	return (r == QUIT_INFO) 0 : -1;
}

/**
 * @brief Quick wrapper to initialize a dict session
 * @param s Server info structure
 * @return Socket if successful, -1 if failure
 * @note the default port is 2628
 */
int proto_dict_init(struct server_info *s)
{
	int r = 0, sock = -1;

	sock = open_tcp_client(s->host, s->port);
	if (sock) {
		r = proto_read_test(sock, "220");
		r |= proto_dict_client(sock);
		if (r) {
			proto_dict_quit(sock);
		}
	}
	return sock;
}

//---------------------------------------------------------------------------// 
// Test Driver
//---------------------------------------------------------------------------// 
#if 0
int main(void)
{
	printf("%s\n", proto_time("time-a.nist.gov"));
	printf("%s\n", proto_whois("127.0.0.1", "lansing"));

	return 0;
}
#endif

