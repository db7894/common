#ifndef _LIB_EXAMPLE_H
#define _LIB_EXAMPLE_H

/**
 * @summary Retrieve a new block of data
 * @param size The size of the data to retrieve
 * @return A pointer to the new block of data
 */
char* get_data_of_size(size_t size);

/**
 * @summary Print the supplied data to console
 * @param data The data to print to console
 * @param size The size of the data to print
 */
void print_data(char *data, int size);

#endif
