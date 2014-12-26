#include <stdio.h>
#include "example.h"

int main (void)
{
    char *data;
    size_t size = 100;

    data = get_data_of_size(size);
    print_data(data, size);
}
