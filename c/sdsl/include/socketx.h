/**
 * @file socketx.h
 * @brief Functional units for tag network library
 */
#ifndef _SAFE_SOCKET
#define _SAFE_SOCKET

//---------------------------------------------------------------------------// 
// Function Declarations
//---------------------------------------------------------------------------// 
int socket_close(int sock);
int socket_shutdown(int sock, int how);
int socket_option(int sock, int opt, int val);

int socket_send_all(int sock, char *buffer, int size);
int socket_send(int sock, char *buffer, int size);
int socket_send_fast(int sock, char *buffer, int size);

char *socket_recv_all(int sock, int *size);
int socket_recv(int sock, char *buffer, int size);
int socket_recv_fast(int sock, char *buffer, int size);

int open_tcp_client_vers(const char *address, const char *port, int af);
int open_tcp_client(const char *address, const char *port);

int open_udp_client_vers(const char *address, const char *port, int af);
int open_udp_client(const char *address, const char *port);

int open_tcp_server_vers(const char *port, int max, int af);
int open_tcp_server(const char *port, int max);

int open_udp_server_vers(const char *port, int max, int af);
int open_udp_server(const char *port, int max);

//---------------------------------------------------------------------------// 
// OS Specific
//---------------------------------------------------------------------------// 
#if WIN32
int socket_init(void);
int socket_destroy(void);
#endif

#endif

