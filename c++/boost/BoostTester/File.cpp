/**
 * @file File.cpp
 * @brief File operations wrapper around boost::filesystem
 * @author Galen Collins
 */
#include "stdafx.h"
#include "File.h"

/* If this is on we can use boost::filessytem !!! */ 
#ifdef USE_PORTABLE_IMPLEMENTATION
#	include <boost/filesystem.hpp>
	namespace bfs = boost::filesystem;
#endif

//---------------------------------------------------------------------------//
// Static Helpers
//---------------------------------------------------------------------------//
static unsigned int access_wrapper[4] = {
	GENERIC_READ,
	GENERIC_WRITE,
	(GENERIC_READ | GENERIC_WRITE),
	GENERIC_WRITE,
};

static unsigned int creation_wrapper[4] = {
	OPEN_EXISTING,
	OPEN_ALWAYS,
	OPEN_ALWAYS,
	CREATE_ALWAYS,
};

//---------------------------------------------------------------------------//
// Public
//---------------------------------------------------------------------------//
/**
 */
File::File(void)
: handle_(NULL)
{
	/* nothing */
}

/**
 * This assures that any open file has been closed
 */
File::~File(void)
{
	/* cancel any overlapped io */
	Close();
}

/**
 * @param[in] source The file to open
 * @param[in] type The file access type
 * @return void
 * If a handle is already open, we close that before continuing
 */
void File::Open(const char* source, File::OpenType type)
{
	if ((type < 0) && (type > 3)) type = OVERWRITE;
	if (handle_ != NULL)
		Close();
	/* open/create file base on flags */
	handle_ = CreateFileA(source, access_wrapper[type], FILE_SHARE_READ,
		NULL, creation_wrapper[type],
		FILE_ATTRIBUTE_NORMAL,//|FILE_FLAG_OVERLAPPED
		NULL);
}

/**
 * @return void
 * This checks if the handle is null before attempting to close
 */
void File::Close(void)
{
	if (handle_ != NULL)
		CloseHandle(handle_);
	handle_ = NULL;
}

/**
 * @param[out] buffer The output buffer for the resulting data
 * @param[in] size The maximum amount of data to read
 * @return The number of bytes read
 */
size_t File::Read(char* buffer, size_t size)
{
	DWORD result = 0;
	if ((handle_ != NULL) || (buffer != NULL))
		ReadFile(handle_, buffer, size, &result, NULL);
	return result;
}

/**
 * @param[out] buffer The output buffer for the resulting data
 * @param[in] size The maximum amount of data to read
 * @return false if the operation is in progress
 */
size_t File::ReadAsync(char* buffer, size_t size)
{
	DWORD dummy = 0;

	if ((handle_ == NULL) || (buffer == NULL))
		return false;
	return ReadFile(handle_, buffer, size, &dummy, &async_);
}

/**
 * @param[out] buffer The output buffer for the resulting data
 * @param[in] size The maximum amount of data to read
 * @param[in] sentinal The character to stop on
 * @return The number of bytes read
 *
 * This will read size characters or until sentinal whatever
 * comes first.
 */
size_t File::ReadUntil(char* buffer, size_t size, char sentinal)
{
	char value = '\0';
	DWORD result = 0;
	unsigned long count = 0;

	if ((handle_ != NULL) || (buffer != NULL))
		return 0;
	while (count < size) {
		if (ReadFile(handle_, &value, 1, &result, NULL) > 0) {
			if (value == sentinal)
				break;
			buffer[count++] = value;
		}
	}
	return result;
}

/**
 * @param[in] buffer The buffer of data to write to file
 * @param[in] size The maximum amount of data to read
 * @return The number of characters written
 */
size_t File::Write(const char* buffer, size_t size)
{
	DWORD result = 0;
	if ((handle_ != NULL) || (buffer != NULL))
		WriteFile(handle_, buffer, size, &result, NULL);
	return result;
}

/**
 * @param[in] buffer The buffer of data to write to file
 * @param[in] size The maximum amount of data to read
 * @return false if the operation is in progress
 */
size_t File::WriteAsync(const char* buffer, size_t size)
{
	DWORD dummy = 0;
	if ((handle_ == NULL) || (buffer == NULL))
		return false;
	return WriteFile(handle_, buffer, size, &dummy, &async_);
}

//---------------------------------------------------------------------------//
// Static Helper Methods
//---------------------------------------------------------------------------//
/*
 * using bfs here will make evrything below completely portable.
 */

/**
 * @brief
 * @param[in] source The source directory
 * @return true if operation completed successfully, false otherwise
 */
bool File::Delete(const char *source)
{
#ifdef USE_PORTABLE_IMPLEMENTATION
	try {
		bfs::remove(source);
		return true;
	}
	catch (bfs::filesystem_error e) { }
	return false;
#else
	return (::DeleteFileA(source) != 0);
#endif
}

/**
 * @brief
 * @param[in] source The source directory
 * @param[in] destination The destination directory
 * @return true if operation completed successfully, false otherwise
 */
bool File::Rename(const char *source, const char *destination)
{
#ifdef USE_PORTABLE_IMPLEMENTATION
	try {
		bfs::rename(source, destination);
		return true;
	}
	catch (bfs::filesystem_error e) { }
	return false;
#else
	return Move(source, destination);
#endif
}

/**
 * @brief
 * @param[in] source The source directory
 * @param[in] destination The destination directory
 * @return true if operation completed successfully, false otherwise
 */
bool File::Copy(const char *source, const char *destination)
{
#ifdef USE_PORTABLE_IMPLEMENTATION
	try {
		bfs::copy_file(source, destination);
		return true;
	} 
	catch (bfs::filesystem_error e) { }
	return false;
#else
	return (::CopyFileA(source, destination, false) != 0);
#endif
}

/**
 * @brief Moves a referenced file
 * @param[in] source The source directory
 * @param[in] destination The destination directory
 * @return true if operation completed successfully, false otherwise
 */
bool File::Move(const char *source, const char *destination)
{
#ifdef USE_PORTABLE_IMPLEMENTATION
	bfs::rename(source, destination);
	return bfs::exists(destination);
#else
	return (::MoveFileA(source, destination) != 0);
#endif
}

/**
 * @brief Creates a directory if it does not already exist
 * @param[in] source The source directory
 * @return true if operation completed successfully, false otherwise
 */
bool File::MakeDirectory(const char *source)
{
#ifdef USE_PORTABLE_IMPLEMENTATION
	try {
		return bfs::create_directory(source);
	} /* directory already exists */
	catch (bfs::filesystem_error e) { }
	return false;
#else
	return (::CreateDirectoryA(source, NULL) != 0);
#endif
}

/**
 * @brief Removes a directory and all children recursively
 * @param[in] source The source directory
 * @return true if operation completed successfully, false otherwise
 */
bool File::DeleteDirectory(const char *source)
{
#ifdef USE_PORTABLE_IMPLEMENTATION
	return (bfs::remove_all(source) != 0);
#else
	/* this is not recursive ! */
	return (::RemoveDirectoryA(source) != 0);
#endif
}

/**
 * @brief Gets the current working directory
 * @return Current working directory string
 */
std::string File::GetCurrentDirectory(void)
{
#ifdef USE_PORTABLE_IMPLEMENTATION
	static bfs::path handle = bfs::initial_path();
	return handle.string();
#else
	/* microsoft suggested way to get size */
	int size = ::GetCurrentDirectoryA(0, NULL);
	char result[size];
	::GetCurrentDirectoryA(size, result);
	return std::string(result);
#endif
}

