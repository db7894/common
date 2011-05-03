/**
 * @file stdafx.h
 * @brief Header file for common includes/defines/types
 * include file for standard system include files, or project specific
 * include files that are used frequently, but are changed infrequently
 */
#pragma once

#ifndef SOCKET_SERVER_STDFX_H
#define SOCKET_SERVER_STDFX_H

//---------------------------------------------------------------------------//
// Global Includes
//---------------------------------------------------------------------------//

//---------------------------------------------------------------------------//
// Windows Defines
//---------------------------------------------------------------------------//
/**
 * @brief excludes rarely-used headers/etc
 */
#define WIN32_LEAN_AND_MEAN

/**
 * @brief Specifies the minimum required platform
 */
#ifndef _WIN32_WINNT
//#	define _WIN32_WINNT 0x0600	/* windows-vista */
//#	define _WIN32_WINNT 0x0502	/* 2k3 w/SP1 or XP w/SP2 */
#	define _WIN32_WINNT 0x0501	/* 2k3 or XP*/
//#	define _WIN32_WINNT 0x0501	/* 2k */
#endif

//---------------------------------------------------------------------------//
// Global Types
//---------------------------------------------------------------------------//
/* nothing */

//---------------------------------------------------------------------------//
// Global Pragma Disables
//---------------------------------------------------------------------------//
#pragma warning(disable:4351)	/* new behaviour of default initialization */

#endif