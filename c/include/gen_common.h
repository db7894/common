/**
 * @file generic.h
 * @brief Data shared between simple rfc code
 */
#ifndef SAFE_GENERIC_COMMON
#define SAFE_GENERIC_COMMON

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------// 

char *proto_whois(char *host, char *user);
char *proto_finger(char *host, char *user);
char *proto_time(char *host);

#endif

