/**
 * @file generic.h
 * @brief Data shared between simple rfc code
 */
#ifndef SAFE_GENERIC_RFC
#define SAFE_GENERIC_RFC

//---------------------------------------------------------------------------// 
// Types
//---------------------------------------------------------------------------// 
/**
 * @brief A common structure used to handle server connect information
 */
struct server_data {
	char *server;	/**< The server name (ascii) */
	char *port;		/**< The server port (ascii) */
};

/**
 * @brief A common structure used to handle protocol commands
 */
struct proto_cmd {
	char *cmd;	/**< the command string */
	char *pass;	/**< the postive result */
	char *fail;	/**< the negative result */
};

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------// 
char *proto_gen_write(struct server_data *s, char *data);
char *proto_gen_read(struct server_data *s);

int proto_read_test(int sock, char *test);
int proto_write_test(int sock, char *data, char *test);

int proto_read_test_data(int sock, char *test, char *buf);
int proto_write_test_data(int sock, char *data, char *test, char *buf);

#endif

