/**
 * @file ftp-client.h
 * @brief Header file for quick ftp upload
 * @author http://curl.haxx.se/lxr/source/docs/examples/ftpget.c
 * @note This only performs plain-text authentication, so passwords
 * are sent in the clear.
 */
#ifndef FTP_CLIENT_H
#define FTP_CLIENT_H

//---------------------------------------------------------------------------// 
// Constants
//---------------------------------------------------------------------------// 
//#define UNIT_TEST		/**< Uncomment to create unit test */


//---------------------------------------------------------------------------// 
// Types
//---------------------------------------------------------------------------// 
/**
 * @brief A handle to hold relevant ftp information
 */
struct ftp_upload {
	char host[50];	/**< The hostname and path */
	char pass[25];	/**< The ftp password */
	char user[25];	/**< The ftp username */
	char file[100];	/**< The filename to upload */
};

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------// 
void *ftp_upload(void *input);

#endif

