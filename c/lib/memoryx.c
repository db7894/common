/**
 * @file memoryx.c
 * @brief Safe wrappers for memory functions
 */
#include "common.h"

/**
 * @brief Quick exit function
 * @param str The error message
 * @return void
 * @note This should only be used for system errors
 *
 * @code
 *   char *handle = "System Error, Quitting"
 *
 *   error_and_die(handle);
 * @endcode
 */
void error_and_die(char *str)
{
	fprintf(stderr, str);
	exit(1);
}

/**
 * @brief Quick wrapper for malloc
 * @param size The size in bytes of memory to allocate
 * @return pointer to new memory
 * @note If the malloc fails, this will exit
 *
 * @code
 *   char *handle = NULL;
 *   size_t size = 40;
 *
 *   handle = xmalloc(size);
 *   ...
 *   free(handle);
 * @endcode
 */
void *xmalloc(size_t size)
{
	void *handle = malloc(size);

	if (handle == NULL && size != 0)
		error_and_die("Memory Exhausted");
	return handle;
}

/**
 * @brief Quick wrapper for malloc
 * @param size The size in bytes of memory to allocate
 * @return pointer to new memory
 * @note If the malloc fails, this will exit
 *
 * @code
 *   char *handle = NULL;
 *   size_t size = 40;
 *
 *   handle = xzalloc(size);
 *   ...
 *   free(handle);
 * @endcode
 */
void *xzalloc(size_t size)
{
	void *handle = xmalloc(size);

	memset(handle, 0, size);
	return handle;
}

/**
 * @brief Quick wrapper for realloc
 * @param size The size in bytes of memory to allocate
 * @param handle Handle to previously allocated memory
 * @return pointer to new memory
 * @note If the malloc fails, this will exit
 *
 * @code
 *   char *handle = NULL;
 *   size_t size1 = 10;
 *   size_t size2 = 40;
 *
 *   handle = xmalloc(size1);
 *   ...
 *   handle = xrealloc(size2);
 *   ...
 *   free(handle);
 * @endcode
 */
void *xrealloc(void *handle, size_t size)
{
	handle = realloc(handle, size);

	if (handle == NULL && size != 0)
		error_and_die("Memory Exhausted");
	return handle;
}

/**
 * @brief Quick wrapper for calloc
 * @param size The size in bytes of memory to allocate
 * @param nmem The number of size elements to allocate
 * @return pointer to new memory
 * @note If the malloc fails, this will exit
 *
 * @code
 *   char *handle = NULL;
 *   size_t size = 40;
 *   size_t nmem = sizeof(char);
 *
 *   handle = xcalloc(size, nmem);
 *   ...
 *   free(handle);
 * @endcode
 */
void *xcalloc(size_t size, size_t nmem)
{
	void *handle = calloc(nmem, size);

	if (handle == NULL && size != 0 && nmem != 0)
		error_and_die("Memory Exhausted");
	return handle;
}

