/**
 * @file memoryx.h
 * @brief Header file for safe memory functions
 */
#ifndef SAFE_MEMORY_FUNCS
#define SAFE_MEMORY_FUNCS

//---------------------------------------------------------------------------// 
// Function Declarations
//---------------------------------------------------------------------------// 
void *xmalloc(size_t size);
void *xzalloc(size_t size);
void *xrealloc(void *handle, size_t size);
void *xcalloc(size_t size, size_t nmem);

#endif

