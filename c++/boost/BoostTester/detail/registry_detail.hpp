/**
 * @file registry_detail.hpp
 * @brief Quick wrapper around win32 registry api
 * @author Galen Collins
 * @todo Work on the Ansi vs Unicode mess
 * @code
 * #ifdef UNICODE 
 * #  define FunctionEx FunctionExW 
 * #else 
 * #  define FunctionEx FunctionExA 
 * #endif
 * @endcode
 */
#pragma once

#ifndef SCOTTRADE_REGISTRY_DETAIL_H
#define SCOTTRADE_REGISTRY_DETAIL_H

#include <winreg.h>
#include <boost/lexical_cast.hpp>

/**
 * @brief Bucket to store all nasty OS specific junk
 */
namespace detail
{
//---------------------------------------------------------------------------//
// Constants
//---------------------------------------------------------------------------//
static const HKEY REGISTRY_ROOT			= HKEY_LOCAL_MACHINE;
static const char* SCOTTRADE_ROOT		= "Software\\Scottrade\\";
static const int MAX_REGISTRY_STRING	= 255;

/**
 * @brief Opens a registy handle if it exists
 * @param key The registry key to open
 * @return A registry handle if success, NULL otherwise
 */
static inline HKEY registry_open(const char* key)
{
	std::string total = std::string(SCOTTRADE_ROOT) + std::string(key);
	HKEY handle = NULL;
	
	RegOpenKeyExA(REGISTRY_ROOT, total.data(), 0, KEY_READ, &handle); 
	return handle;
}

/**
 * @brief Opens a registry handle or creates it if it doesn't exist
 * @param key The registry key to open
 * @return A registry handle if success, NULL otherwise
 */
static inline HKEY registry_create(const char* key)
{
	std::string total = std::string(SCOTTRADE_ROOT) + std::string(key);
	HKEY handle = NULL;

	/* if key doesn't exist, we create it */
	RegCreateKeyExA(REGISTRY_ROOT, total.data(),
		NULL, NULL, NULL, KEY_ALL_ACCESS, NULL, &handle, NULL); 
	return handle;
}

/**
 * @brief Quick helper to close a registry handle
 * @param handle The registry handle to close
 * @return void
 */
static inline void registry_close(HKEY& handle)
{
	if (handle) {
		RegCloseKey(handle);
	}
}

/**
 * @brief Quick helper to reset the value of a key
 * @param key The registry key to open
 * @param name The element under key to work with
 * @return true if the value was reset, false otherwise
 */
static inline bool registry_delete(const char* key, const char* name)
{
	HKEY handle = registry_create(key);
	bool result = false;

	if (handle) {
		if (RegDeleteValueA(handle, name) == ERROR_SUCCESS) {
			result = true;
		}
	}
	registry_close(handle);

	return result;
}

/**
 * @brief Windows API wrapper to read a fixed sized value from the registry
 * @param key The registry key to open
 * @param name The element under key to work with
 * @return Value read on success, -1 on failure
 */
static inline int registry_read_fixed(const char* key, const char* name)
{
	HKEY handle = registry_open(key);
	int output = -1;
	DWORD name_type;
	DWORD size = MAX_REGISTRY_STRING;

	RegQueryValueExA(handle, name, NULL, &name_type, (LPBYTE)&output, &size);
	registry_close(handle);

	return output;
}

/**
 * @brief Windows API wrapper to read a variable sized value from the registry
 * @param key The registry key to open
 * @param name The element under key to work with
 * @return Value read on success, "" on failure
 */
static inline std::string registry_read_variable(const char* key, const char* name)
{
	HKEY handle = registry_open(key);
	std::string output;
	char buffer[MAX_REGISTRY_STRING];
	DWORD name_type;
	DWORD size = MAX_REGISTRY_STRING;

	if (   RegQueryValueExA(handle, name, NULL, &name_type, (LPBYTE)buffer, &size)
		== ERROR_SUCCESS) {
			output = &buffer[0];
	}
	registry_close(handle);

	return output;
}

/**
 * @brief Windows API wrapper to write a fixed sized value to the registry
 * @param key The registry key to open
 * @param name The element under key to work with
 * @param value The data to write to key\name
 * @return true if success, false otherwise
 */
static inline bool registry_write_fixed(const char* key, const char* name,
	DWORD value)
{
	bool result = false;
	HKEY handle = registry_create(key);

	if (handle) {
		if (RegSetValueExA(handle, name, NULL, REG_DWORD, /* type */
				(const BYTE *)&value, sizeof(DWORD)) == ERROR_SUCCESS) {
			result = true;
		}
	}
	registry_close(handle);

	return result;
}

/**
 * @brief Windows API wrapper to write a variable sized value to the registry
 * @param key The registry key to open
 * @param name The element under key to work with
 * @param value The data to write to key\name
 * @return true if success, false otherwise
 */
static inline bool registry_write_variable(const char* key, const char* name,
	const char* value)
{
	bool result = false;
	HKEY handle = registry_create(key);
	DWORD size = strlen(value);

	if (handle) {
		if (RegSetValueExA(handle, name, NULL, REG_SZ, /* type */
				(LPBYTE)value, size) == ERROR_SUCCESS) {
			result = true;
		}
	}
	registry_close(handle);

	return result;
}

/**
 * @brief Windows API wrapper to enumerate a name-value pair
 * @param key The registry key to open
 * @param index The index into the value array
 * @return pair<name,value> on success, pair<"",""> on error
 */
static inline std::pair<std::string, std::string> registry_enum(
	const char* key, int index)
{
	HKEY handle = registry_open(key);
	DWORD type;
	DWORD name = MAX_REGISTRY_STRING;
	DWORD size = MAX_REGISTRY_STRING;
	char name_buffer[MAX_REGISTRY_STRING]	= {0};
	char value_buffer[MAX_REGISTRY_STRING]	= {0};

	long status = RegEnumValueA(handle, index, (LPSTR)name_buffer, &name,
		NULL, &type, (LPBYTE)value_buffer, &size);
	if (status == ERROR_SUCCESS) {
		switch (type) {
			case REG_SZ:
				return std::make_pair(std::string(name_buffer),
					std::string(value_buffer));
			case REG_DWORD:
				return std::make_pair(std::string(name_buffer),
					std::string(boost::lexical_cast<std::string>(
						*(int *)&value_buffer[0])));
			default: break; /* other types are ignored */
		}
	}
	registry_close(handle);

	return std::make_pair(std::string(""),std::string(""));
}

/**
 * @brief Counts the number of immediate subkeys in a given key
 * @note This is not recursive
 * @param key The registry key to open
 * @return The number of subkeys in a key, or zero
 */
static inline int registry_count_keys(const char* key)
{
	DWORD result = 0;
	HKEY handle = detail::registry_open(key);

	if (RegQueryInfoKeyA(handle, NULL, NULL, NULL, &result, NULL, NULL, NULL,
		NULL, NULL, NULL, NULL) != ERROR_SUCCESS) {
		result = 0;
	}

	detail::registry_close(handle);
	return result;
}

/**
 * @brief Counts the number of immediate values in a given key
 * @param key The registry key to open
 * @return The number of values contained in a key, or zero
 */
static inline int registry_count_values(const char* key)
{
	DWORD result = 0;
	HKEY handle = detail::registry_open(key);

	if (RegQueryInfoKeyA(handle, NULL, NULL, NULL, NULL, NULL, NULL, &result,
		NULL, NULL, NULL, NULL) != ERROR_SUCCESS) {
		result = 0;
	}

	detail::registry_close(handle);
	return result;
}

} /* end of namespace */
#endif

