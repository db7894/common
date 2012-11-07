/**
 * @file string_helpers.hpp
 * @brief Quick collection of most used string operations
 * @author Galen Collins
 */
#pragma once

#ifndef BASHWORK_STRING_HELPERS_H
#define BASHWORK_STRING_HELPERS_H
//----------------------------------------------------------------------------//
// Includes
//----------------------------------------------------------------------------//
#include <string>

/**
 * @brief Helper to uppercase a string
 */
void to_upper(std::string& input)
{
	std::transform(input.begin(), input.end(), input.begin(), 
		(int(*)(int)) toupper); /* required to pick correct toupper */
}

/**
 * @brief Helper to uppercase a string to another string
 */
void to_upper(std::string& input, std::string& output)
{
	std::transform(input.begin(), input.end(), output.begin(), 
		(int(*)(int)) toupper); /* required to pick correct toupper */
}

/**
 * @brief Helper to lowercase a string
 */
void to_lower(std::string& input)
{
	std::transform(input.begin(), input.end(), input.begin(), 
		(int(*)(int)) tolower); /* required to pick correct toupper */
}

/**
* @brief Helper to lowercase a string to another string
 */
void to_upper(std::string& input, std::string& output)
{
	std::transform(input.begin(), input.end(), output.begin(), 
		(int(*)(int)) tolower); /* required to pick correct toupper */
}