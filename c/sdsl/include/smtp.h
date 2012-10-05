/**
 * @file smtp.h
 * @brief Implementation of a quick smtp forwarder
 * @note http://www.ietf.org/rfc/rfc0821.txt
 */
#ifndef _SAFE_SMTP_
#define _SAFE_SMTP_
#include "generic.h"

//---------------------------------------------------------------------------// 
// Types
//---------------------------------------------------------------------------// 
struct smtp_address {
	char *from;
	char *subject;
	char *data;
	char **email;
};

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------// 
int smtp_connect(char *host, char *port);
int smtp_send(int sock, struct smtp_address *addr);
int smtp_close(int sock);
int smtp_main(struct server_data *s, struct smtp_address *a);

int smtp_finger(int sock, char *user);
int smtp_reset(int sock);

#endif

