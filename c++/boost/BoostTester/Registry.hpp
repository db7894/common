/**
 * @file Registry.hpp
 * @brief Quick wrapper functions to read/write to a registry
 * @author Galen Collins
 */
#pragma once

#ifndef BASHWORK_REGISTRY_H
#define BASHWORK_REGISTRY_H
//----------------------------------------------------------------------------//
// Includes
//----------------------------------------------------------------------------//
#include "detail/registry_detail.hpp"
#include <map>

//----------------------------------------------------------------------------//
// Interface
//----------------------------------------------------------------------------//
/**
 * @brief Easy to use helpers for reading from a registry
 * @todo Maybe make helpers a static pimpl for portability or extendability
 * to other types of systems (gconf, ini file, etc)
 * @code
 * #include "Registry.hpp"
 * 
 * static const char* test_key	= "Testing";
 * static const char* test_name	= "Test";
 * 
 * int main(void)
 * {
 *	std::string result = "";
 * 	std::string input = "Testing";
 * 	Registry::Write(test_key, test_name, input);
 * 	std::string output = Registry::Read(test_key, test_name, result);
 * 	std::cout << output  << "\n";
 * 	Registry::Delete(test_key, test_name);
 * 
 * 	return 0;
 * }
 * @endcode
 */
class Registry
{
public:
	/* read functions */
	static std::string Read(const char* key, const char* name,
		const std::string& Default);
	static const char* Read(const char* key, const char* name,
		const char* Default);
	static int   Read(const char* key, const char* name, int Default);
	static bool  Read(const char* key, const char* name, bool Default);

	/* write functions */
	static bool Write(const char* key, const char* name,
		const std::string& value);
	static bool Write(const char* key, const char* name, const char* value);
	static bool Write(const char* key, const char* name, int value);
	static bool Write(const char* key, const char* name, bool value);

	/* clear functions */
	static bool Delete(const char* key, const char* name);
	static bool Delete(const char* key, const std::string& name);

	/* the others */
	static int KeyCount(const char* key);
	static int ValueCount(const char* key);
	static std::map<std::string, std::string> Enumerate(const char* key);
};

//----------------------------------------------------------------------------//
// Read Implementations
//----------------------------------------------------------------------------//
/**
 * @brief Read a registry value
 * @param key The registry key to open
 * @param name The element under key to work with
 * @param Default The value to return on error
 * @return The returned value, or Default if the result is ""
 */
inline std::string Registry::Read(const char* key, const char* name,
	const std::string& Default)
{
	std::string result = detail::registry_read_variable(key, name);
	return (result == "") ? Default : result;
}

/**
 * @brief Read a registry value
 * @param key The registry key to open
 * @param name The element under key to work with
 * @param Default The value to return on error
 * @return The returned value, or Default if the result is ""
 */
inline const char* Registry::Read(const char* key, const char* name,
	const char* Default)
{
	std::string result = detail::registry_read_variable(key, name);
	return (result == "") ? Default : result.c_str();	
}

/**
 * @brief Read a registry value
 * @param key The registry key to open
 * @param name The element under key to work with
 * @param Default The value to return on error
 * @return The returned value, or Default if the result is -1
 */
inline int Registry::Read(const char* key, const char* name, int Default)
{
	int  result = detail::registry_read_fixed(key, name);
	return (result == -1) ? Default : result;
}

/**
 * @brief Read a registry value
 * @param key The registry key to open
 * @param name The element under key to work with
 * @param Default The value to return on error
 * @return true if the key has a value != 0, false otherwise
 */
inline bool Registry::Read(const char* key, const char* name, bool Default)
{
	int  result = detail::registry_read_fixed(key, name);
	return (result == -1) ? Default : (result != 0);
}

//----------------------------------------------------------------------------//
// Write Implementations
//----------------------------------------------------------------------------//
/**
 * @brief Write a registry value
 * @param key The registry key to open
 * @param name The element under key to work with
 * @param value The data to write to key\name
 * @return true if write was successful, false otherwise
 */
inline bool Registry::Write(const char* key, const char* name,
	const std::string& value)
{
	return detail::registry_write_variable(key, name, value.c_str());
}

/**
 * @brief Write a registry value
 * @param key The registry key to open
 * @param name The element under key to work with
 * @param value The data to write to key\name
 * @return true if write was successful, false otherwise
 */
inline bool Registry::Write(const char* key, const char* name,
	const char* value)
{
	return detail::registry_write_variable(key, name, value);
}

/**
 * @brief Write a registry value
 * @param key The registry key to open
 * @param name The element under key to work with
 * @param value The data to write to key\name
 * @return true if write was successful, false otherwise
 */
inline bool Registry::Write(const char* key, const char* name, int value)
{
	return detail::registry_write_fixed(key, name, value);
}

/**
 * @brief Write a registry value
 * @param key The registry key to open
 * @param name The element under key to work with
 * @param value The data to write to key\name
 * @return true if write was successful, false otherwise
 */
inline bool Registry::Write(const char* key, const char* name, bool value)
{
	return detail::registry_write_fixed(key, name, value);
}

//----------------------------------------------------------------------------//
// Clear Implementations
//----------------------------------------------------------------------------//

/**
 * @brief Deletes a registry value
 * @param key The registry key to open
 * @param name The element under key to work with
 * @return true if the value has been reset, false otherwise
 */
inline bool Registry::Delete(const char* key, const char* name)
{
	return detail::registry_delete(key, name);
}

/**
 * @brief Deletes a registry value
 * @param key The registry key to open
 * @param name The element under key to work with
 * @return true if the value has been reset, false otherwise
 */
inline bool Registry::Delete(const char* key, const std::string& name)
{
	return detail::registry_delete(key, name.c_str());
}

//----------------------------------------------------------------------------//
// Implementations
//----------------------------------------------------------------------------//

/**
 * @brief Used to query the number of sub-keys in a key
 * @note This is not recursive
 * @param key The registry key to open
 * @return The number of subkeys in a key, or zero
 */
inline int Registry::KeyCount(const char* key)
{
	return detail::registry_count_keys(key);
}

/**
 * @brief Used to query the number of values in a key
 * @param key The registry key to open
 * @return The number of values contained in a key, or zero
 */
inline int Registry::ValueCount(const char* key)
{
	return detail::registry_count_values(key);
}

/**
 * @brief Used to completely enumerate a list of key values
 * @param key The registry key to open
 * @return A map of values <name, value>
 */
inline std::map<std::string, std::string>
	Registry::Enumerate(const char* key)
{
	std::map<std::string, std::string> buffer;

	bool keep_running = true;
	int index = 0; /* enumeration index */
	do {
		std::pair<std::string, std::string> output =
			detail::registry_enum(key, index++);
		if ((output.first == "") && (output.second == "")) {
			keep_running = false;
		} else { buffer.insert(output); }
	} while (keep_running);
	return buffer;
}

#endif

