/**
 * @file serial-client.h
 * @brief Header file for quick serial upload
 * @author Galen Collins
 */
#ifndef SERIAL_CLIENT_H
#define SERIAL_CLIENT_H

//---------------------------------------------------------------------------// 
// Constants
//---------------------------------------------------------------------------// 
//#define UNIT_TEST		/**< Uncomment to create unit test */

/**
 * @brief The portable base string for the serial device
 */
#define SERIAL_DEVICE_BASE	"/dev/ttyS"


//---------------------------------------------------------------------------// 
// Types
//---------------------------------------------------------------------------// 
/**
 * @brief A handle to hold relevant ftp information
 */
struct serial_upload {
	char port[25];	/**< The port to upload to */
	char file[100];	/**< The filename to upload */
};

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------// 
void *serial_upload(void *input);

#endif

