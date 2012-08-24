/**
 * @file filex.h
 * @brief Wrappers for casual io operations
 */
#ifndef _SAFE_IO
#define _SAFE_IO

#ifdef HAVE_DIRENT_H
#	include <dirent.h>	/* for type constants */
#endif

//---------------------------------------------------------------------------// 
// Function Declarations
//---------------------------------------------------------------------------// 
int reply_wait(int sock, int time);
void nonblock(int sock, int state);

void file_close(FILE *file);
int file_exists(char *file);
int file_perms(char *file);

int file_read(char *file, char **handle);
void file_write(char *file, char *handle, int size);
void file_append(char *file, char *handle, int size);

int fs_mkdir(const char *path);
int fs_rmdir(const char *path);
int fs_remove(const char *path);
int fs_mknod(const char *path, int mode, int dev);
int fs_scandir(char *path);

int device_open(const char *device, int mode);

#endif

