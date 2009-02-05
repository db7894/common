/**
 * @file common.h
 * @brief This handles all the portability macros from autotools
 * @note http://www.opengroup.org/onlinepubs/000095399/mindex.html,
 * http://c-faq.com/index.html
 */
#ifndef _COMMON_H
#define _COMMON_H

#ifndef S_SPLINT_S /* so we can do splint analysis */

//---------------------------------------------------------------------------// 
// Test for autotools config.h
//---------------------------------------------------------------------------// 
#if HAVE_CONFIG_H
#	include <config.h>	/* autotools header */
#endif

//---------------------------------------------------------------------------// 
// Include system common headers
//---------------------------------------------------------------------------// 
// These should be common among all POSIX systems (except ctype I think)
//---------------------------------------------------------------------------// 
#include <errno.h>		/* current error */
#include <stdio.h>		/* read/write */
#include <sys/types.h>	/* base types */
#include <sys/stat.h>	/* for file-system */
#include <ctype.h>		/* isxxx functions */

//---------------------------------------------------------------------------// 
// Check for common functions
//---------------------------------------------------------------------------// 
#if HAVE_FCNTL_H
#	include <fcntl.h>		/* O_xxx Flags */
#endif

//---------------------------------------------------------------------------// 
// Local common headers
//---------------------------------------------------------------------------// 
// These are the only functions that I can warrent being globally available
// in common
//---------------------------------------------------------------------------// 
#include "language.h"	/* fot i18n */
#include "memoryx.h"	/* for safe xfuncs */

//---------------------------------------------------------------------------// 
// Test for string/memory functions
//---------------------------------------------------------------------------// 
//---------------------------------------------------------------------------// 
#if STDC_HEADERS
#	include <stdlib.h>
#	include <string.h>
#elif HAVE_STRING_H
#	include <strings.h>
#endif

//#ifndef HAVE_STRCHR
//#	define strchr(a,b)	index(a,b)
//#	define strrchr(a,b)	rindex(a,b)
//#endif

//---------------------------------------------------------------------------// 
// Test for common constants
//---------------------------------------------------------------------------// 
#ifndef O_BINARY
#	define O_BINARY			0
#endif

#ifndef TRUE
#	define TRUE				1
#endif

#ifndef FALSE
#	define FALSE			0
#endif

//---------------------------------------------------------------------------// 
// Add stdbool.h
//---------------------------------------------------------------------------// 
// http://www.opengroup.org/onlinepubs/000095399/basedefs/stdbool.h.html
//---------------------------------------------------------------------------// 
#ifndef __cplusplus
//typedef char _Bool					/**< boolean */
#	ifndef true
#		define true			1
#	endif

#	ifndef false
#		define false		0
#	endif
//#	define bool			_Bool
//#	define __bool_true_false_are_defined 1
#endif

//---------------------------------------------------------------------------// 
// Test for universal standard
//---------------------------------------------------------------------------// 
// Apparently windows does not have this
//---------------------------------------------------------------------------// 
#if HAVE_UNISTD_H
#	include <unistd.h>
#elif WIN32
#	include <io.h>
#endif

//---------------------------------------------------------------------------// 
// Test for strcpy
//---------------------------------------------------------------------------// 
// This is more than likely legacy, but I thought I would throw it in here
// anyways for older BSD systems.
//---------------------------------------------------------------------------// 
#if !HAVE_STRCPY
#	if HAVE_BCOPY
#		define strcpy(dest, src)   bcopy(src, dest, 1 + strlen(src))
#	else
		inline char *strcpy(char *d, char *s)
		{
			char *h = d;

			do {
				*h++ = *s;
			} while (*s++);

			return d;
		}
#	endif
#endif

//---------------------------------------------------------------------------// 
// Socket Constants
//---------------------------------------------------------------------------// 
// I really don't want to make every int a SOCKET, but I might have to if I
// want windows portability
//---------------------------------------------------------------------------// 
#if defined __linux__
#	include <sys/socket.h>
#	define INVALID_SOCKET	-1
#	define SOCKET_ERROR		-1
	typedef int SOCKET;
#elif WIN32
#	include <winsock.h>
#endif

//---------------------------------------------------------------------------// 
// Set Paths
//---------------------------------------------------------------------------// 
// I believe that there is a better way to do multi-platform device access,
// but right now this is the way I am doing it. The glade_xml_file should
// probobly not be in here, but I thought it would make porting easier.
//---------------------------------------------------------------------------// 
/**
 * @def SERIAL_DEVICE_BASE
 * @brief The portable base string for the serial device
 */
#if sun
#	define SERIAL_DEVICE_BASE	"/dev/ttya"
#elif linux
#	define SERIAL_DEVICE_BASE	"/dev/ttyS"
#endif

/**
 * @def GLADE_XML_FILE
 * @brief location of UI XML file relative to path in which program is running
 */
#define GLADE_XML_FILE "/usr/share/flasher/flasher.glade"

//---------------------------------------------------------------------------// 
// Debugging Functions
//---------------------------------------------------------------------------// 
// In order to use these, simply #define DEBUG in the file that you would
// like debugging turned on
//---------------------------------------------------------------------------// 

/**
 * @brief Quick function to print serial debug info
 * @param a The string to pring
 * @return void
 */
#ifdef DEBUG
#	define dprintf(a, ...)	fprintf(stderr, a, ##__VA_ARGS__)
#else
#	define dprintf(a, ...)
#endif

//---------------------------------------------------------------------------// 
// Static Code Analysis
//---------------------------------------------------------------------------// 
// In order to make splint happy, we need to do this so we don't get tons
// of stupid errors relating to splint not working with autoconf
//---------------------------------------------------------------------------// 
#else
#	include <config.h>			/* autotools header */
#	include <errno.h>			/* current error */
#	include <stdio.h>			/* read/write */
#	include <sys/types.h>		/* base types */
#	include <sys/stat.h>		/* for file-system */
#	include <fcntl.h>			/* O_xxx Flags */
#	include <ctype.h>			/* isxxx functions */
#	include <sys/socket.h>
#	include "memoryx.h"			/* for safe xfuncs */
#	include <stdlib.h>
#	include <string.h>
#	include <unistd.h>
	typedef int SOCKET;
#	define dprintf printf
#	define INVALID_SOCKET		-1
#	define SOCKET_ERROR			-1
#	define PACKAGE				"temp"
#endif

#endif

