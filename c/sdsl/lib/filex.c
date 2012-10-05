/**
 * @file filex.c
 * @brief Wrappers for casual io operations
 */
#include <sys/stat.h>
#include "common.h"
#include "filex.h"

//---------------------------------------------------------------------------// 
// FD manipulation
//---------------------------------------------------------------------------// 

/**
 * @brief Waits for input number of seconds for a reply
 * @param sock The socket to wait for a reply on
 * @param time The number of seconds to wait
 * @return -1 if a socket error, 0 if timeout, 1 if successful recieve
 * @see ping
 *
 * @code
 *  int sock, bytes = 10;
 *  char buffer[10];
 *
 *  sock = open_udp_server(NULL, 0x6666, AF_INET);
 *  if (sock < 0)
 *    return -1;
 *  
 *  if (socket_reply_wait(sock, 5)) {
 *      bytes = socket_receive_udp(sock, buffer, bytes, NULL);
 *      if (bytes < 0) {
 *          return -1;
 *      }
 *      printf("S[%s]\n", buffer);
 *  }
 * @endcode
 */
int reply_wait(int sock, int time)
{
	struct timeval timeout;
	fd_set files;
	
	FD_ZERO(&files);
	FD_SET(sock, &files);
	
	timeout.tv_sec  = time;
	timeout.tv_usec = 0;
	
	return (select(FD_SETSIZE, (fd_set *)&files, /* only concerned with input */
		(fd_set *)0, (fd_set *)0, &timeout));   
}

/**
 * @brief Make stdin/stdout blocking or non-blocking
 * @param fd The file descriptor to block
 * @param state TRUE for non-blocking FALSE for blocking
 * @return void
 * @note This code is taken from .../bsp/customize/dialog.c
 *
 * @code
 *  int pause = 5, menu_fd = 0;
 *  char choice;
 *
 *  nonblock(menu_fd, TRUE);
 *  if (kbhit(menu_fd, pause)) {
 *  	choice = getc(menu_fd);
 *  	if (choice >= ' ' || choice <= '~')
 *  		process(choice);
 *  }
 *  nonblock(menu_fd, FALSE);
 * @endcode
 */
void nonblock(int sock, int state)
{
	int flags;

	if (state) {
		flags = fcntl(sock, F_GETFL, 0);
		fcntl(sock, F_SETFL, flags | O_NONBLOCK);
	}
	else {
		flags = fcntl(sock, F_GETFL, 0);
		fcntl(sock, F_SETFL, flags & (~O_NONBLOCK));
	}
}

//---------------------------------------------------------------------------// 
// Quick file open/close
//---------------------------------------------------------------------------// 

/**
 * @brief Wrapper for file open
 * @param file The filename to open
 * @param mode The mode to open the file
 * @return FILE if the file opens, NULL otherwise
 */
FILE *file_open(const char *file, char *mode)
{
	FILE *output = fopen(file, mode);

	if (output == NULL)
		return NULL;
	return output;
}

/**
 * @brief Wrapper for file close
 * @param file The filename to close
 * @return void
 */
void file_close(FILE *file)
{
	fclose(file);
}

/**
 * @brief Wrapper for temporary file create
 * @return File handle to the temporary file
 */
FILE *file_temp(void)
{
	return tmpfile();
}

//---------------------------------------------------------------------------// 
// Quick file properties
//---------------------------------------------------------------------------// 

/**
 * @brief Tests to see if a file exists
 * @param file The filename to test
 * @return 1 if file exists, 0 if it does not
 */
int file_exists(char *file)
{
	FILE *output = file_open(file, "rb");

	if (output == NULL)
		return 0;
	file_close(output);
	return 1;
}

/**
 * @brief Quickly get the size of a file
 * @param file The filename to get the size of
 * @return -1 if the file does not exist, otherwise the size in chars
 */
size_t file_size(char *file)
{
	struct stat info;

	if (file_exists(file)) {
		if (stat(file, &info)) {
			perror("stat");
			info.st_size = -1;
		}
	}

	return info.st_size;
}

/**
 * @brief Quickly get the size of a file
 * @param file The filename to get the size of
 * @return -1 if file does not exist, otherwise:
 *  - S_IFSOCK   0140000   socket
 *  - S_IFLNK    0120000   symbolic link
 *  - S_IFREG    0100000   regular file
 *  - S_IFBLK    0060000   block device
 *  - S_IFDIR    0040000   directory
 *  - S_IFCHR    0020000   character device
 *  - S_IFIFO    0010000   FIFO
 */
mode_t file_type(char *file)
{
	struct stat info;

	if (file_exists(file)) {
		if (stat(file, &info)) {
			perror("stat");
			info.st_mode = -1;
		}
	}

	//return (info.st_mode & 0xFFFF0000);
	return info.st_mode;
}

/**
 * @brief Quickly get the size of a file
 * @param file The filename to get the size of
 * @return 0xxx mode for the file
 */
int file_perms(char *file)
{
	struct stat info;

	if (file_exists(file)) {
		if (stat(file, &info)) {
			perror("stat");
			return -1;
		}
	}
	return (info.st_mode & (S_IRWXU | S_IRWXG | S_IRWXO));
}

//---------------------------------------------------------------------------// 
// Quick file read/write/append 
//---------------------------------------------------------------------------// 

/**
 * @brief Reads an entire file into memory
 * @param file The filename to read
 * @return The file pointer if successful, NULL if failure
 */
int file_read(char *file, char **handle)
{
	int size = file_size(file);
	FILE *input = file_open(file, "rb");

	if (input) {
		*handle = (char *)xcalloc(size, sizeof(char));
		fread(*handle, sizeof(char), size, input);
		file_close(input);
	}

	return size;
}

/**
 * @brief Writes contents of buffer to file
 * @param file The filename to write
 * @return void
 */
void file_write(char *file, char *handle, int size)
{
	FILE *output = file_open(file, "wb");

	if (output) {
		fwrite(handle, sizeof(char), size, output);
		file_close(output);
	}
}

/**
 * @brief Appends contents of buffer to file
 * @param file The filename to append
 * @return void
 */
void file_append(char *file, char *handle, int size)
{
	FILE *output = file_open(file, "ab");

	if (output) {
		fwrite(handle, sizeof(char), size, output);
		file_close(output);
	}
}

//---------------------------------------------------------------------------// 
// Quick file-system functions
//---------------------------------------------------------------------------// 

/**
 * @brief Gets the current working directory
 * @return String representing the current working directory
 */
char *fs_getcwd(void)
{
	int size = 100;
	char *buffer = (char *) xmalloc(size);

	while (1) {
		if (getcwd(buffer, size))
			return buffer;
		size *= 2;
		buffer = (char *)xrealloc(buffer, size);
	}
}

/**
 * @brief Makes a current directory at path
 * @param path The path to start at
 * @return 0 if successful, -1 if failure
 */
int fs_mkdir(const char *path)
{
	mode_t mode = 0755;

	if (mkdir(path, mode) != 0) {
		perror("mkdir");
		return -1;
	}
	return 0;
}

/**
 * @brief Removes the directory at path
 * @param path The directory to delete
 * @return 0 if successful, -1 if failure
 */
int fs_rmdir(const char *path)
{
	if (rmdir(path) != 0) {
		perror("rmdir");
		return -1;
	}
	return 0;
}

/**
 * @brief Removes the file at path
 * @param path The file to delete
 * @return 0 if successful, -1 if failure
 */
int fs_remove(const char *path)
{
	if (remove(path) != 0) {
		perror("remove");
		return -1;
	}
	return 0;
}

/**
 * @brief Removes the file at path
 * @param path The file to create
 * @param mode The attributes of the file
 * @param dev The device type of the file
 * @return 0 if successful, -1 if failure
 * - S_IFCHR This is the file type constant of a character-oriented device file.
 * - S_IFBLK This is the file type constant of a block-oriented device file.
 * - S_IFIFO This is the file type constant of a FIFO or pipe. 
 */
int fs_mknod(const char *path, int mode, int dev)
{
	if (mknod(path, mode, dev) != 0) {
		perror("mknod");
		return -1;
	}
	return 0;
}

static int scan_filter(const struct dirent *dir)
{
	if (dir->d_name[0] == '.')
		return 0;
	return 1;
}
     
int fs_scandir(char *path)
{
#ifndef WIN32
	struct dirent **eps;
	int n, cnt;
	
	n = scandir(path, &eps, scan_filter, alphasort);
	if (n >= 0) {
		for (cnt = 0; cnt < n; ++cnt)
			puts(eps[cnt]->d_name);
	}
	else
		perror("scandir");
#endif
	return 0;
}


//---------------------------------------------------------------------------// 
// Special Files
//---------------------------------------------------------------------------// 

int device_open(const char *device, int mode)
{
#if 0
	int m, f, fd;

	m = mode | O_NONBLOCK;

	for (f = 0; f < 5; f++) {
		fd = open(device, m, 0600);
		if (fd >= 0)
			break;
	}
	if (fd < 0)
		return fd;
	/* Reset original flags. */
	if (m != mode)
		fcntl(fd, F_SETFL, mode);
	return fd;
#endif
	return 0;
}

