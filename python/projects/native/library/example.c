#include <stdlib.h>
#include <stdio.h>

char* get_data_of_size(size_t size)
{
    return calloc(size, sizeof(char));
}

void print_data(char *data, int size)
{
    int i;
    for (i = 0; i < size; i++)
        printf("%x (%c),", data[i], data[i]);
    printf("\n");
}
