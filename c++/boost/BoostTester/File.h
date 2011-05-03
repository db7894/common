/**
 * @file File.h
 * @brief File operations wrapper around boost::filesystem
 * @author Galen Collins
 */
#pragma once

#ifndef SOCKET_SERVER_FILE_H
#define SOCKET_SERVER_FILE_H
//----------------------------------------------------------------------------//
// Includes
//----------------------------------------------------------------------------//
#include <windows.h>
#include <sstream>

//---------------------------------------------------------------------------//
// Types
//---------------------------------------------------------------------------//
/**
 * @brief Uncomment to use portable filesystem operations
 * As of right now, this only includes the static filesystem
 * helper functions. Reading/Writing will have to come later
 */
#define USE_PORTABLE_IMPLEMENTATION

//---------------------------------------------------------------------------//
// Definition
//---------------------------------------------------------------------------//

/**
 * @brief File operations wrapped around boost::filesystem
 */
class File
{
public:
	/**
	 * @brief Constants for access method
	 */
	typedef enum {
		READ = 0,
		WRITE, READWRITE, OVERWRITE
	} OpenType;

public:
	/**
	 * @brief Default constructor
	 */
	File(void);
	/**
	 * @brief RAII constructor
	 */
	File(const char* file, File::OpenType type);
	/**
	 * @brief Common destructor
	 */
	~File(void);
	/**
	 * @brief Opens a file and maintains the handle
	 */
	void Open(const char* source, File::OpenType type);
	/**
	 * @brief Closes the currently opend file handle
	 */
	void Close(void);
	/**
	 * @brief Reads size bytes from the file
	 */
	size_t Read(char *buffer, size_t size);
	/**
	 * @brief Reads size bytes from the file
	 */
	size_t ReadAsync(char *buffer, size_t size);
	/**
	 * @brief Reads from the file until a given character is seen
	 */
	size_t ReadUntil(char *buffer, size_t size, char sentinal);
	/**
	 * @brief Writes the given buffer to file
	 */
	size_t Write(const char* buffer, size_t size);
	/**
	 * @brief Writes the given buffer to file
	 */
	size_t WriteAsync(const char* buffer, size_t size);
	/**
	 * @brief Member template for streaming to the output file
	 */
	template <typename Type>
	File& operator<<(const Type input);

public:
	/**
	 * @brief Wrapper to rename a file
	 */
	static bool Rename(const char *source, const char *destination);
	/**
	 * @brief Wrapper to delete a file
	 */
	static bool Delete(const char *source);
	/**
	 * @brief Wrapper to copy a file
	 */
	static bool Copy(const char *source, const char *destination);
	/**
	 * @brief Wrapper to move a file
	 */
	static bool Move(const char *source, const char *destination);
	/**
	 * @brief Wrapper to make a new directory
	 */
	static bool MakeDirectory(const char *source);
	/**
	 * @brief Wrapper to recursively delete a directory
	 */
	static bool DeleteDirectory(const char *source);
	/**
	 * @brief Wrapper to return the current directory
	 */
	static std::string GetCurrentDirectory(void);

private:
	HANDLE handle_;
    OVERLAPPED async_;
};

//---------------------------------------------------------------------------//
// Stream Specializations
//---------------------------------------------------------------------------//
/**
 * @brief Generic stream interface
 * @note This is very slow as it uses streamstream
 * @param input The data to stream to the output file
 * @return reference to this
 */
template <typename Type>
inline File& File::operator<<(const Type input)
{
	std::ostringstream stream;
	stream << input;
	Write(stream.str().c_str(), stream.str().length());
	return *this;
}

/**
 * @brief Stream specialization for a char
 * @param input The data to stream to the output file
 * @return reference to this
 */
template <>
inline File& File::operator<<(const char input)
{
	char output[1] = {input};
	Write(output, 1);
	return *this;
}

/**
 * @brief Stream specialization for a char*
 * @param input The data to stream to the output file
 * @return reference to this
 */
template <>
inline File& File::operator<<(const char* input)
{
	Write(input, strlen(input));
	return *this;
}

/**
 * @brief Stream specialization for an int
 * @param input The data to stream to the output file
 * @return reference to this
 */
template <>
inline File& File::operator<<(const int input)
{
	char output[64] = {0};
	_snprintf_s(output, sizeof(output), sizeof(output), "%d", input);
	Write(output, strlen(output));
	return *this;
}

/**
 * @brief Stream specialization for a double
 * @param input The data to stream to the output file
 * @return reference to this
 */
template <>
inline File& File::operator<<(const double input)
{
	char output[64] = {0};
	_snprintf_s(output, sizeof(output), sizeof(output), "%0.03f", input);
	Write(output, strlen(output));
	return *this;
}

#endif
