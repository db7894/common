#include <stdio.h>
#include <stdlib.h>
#include <dlfcn.h>

typedef void* dynamic_method;
typedef void* dynamic_library;

/**
 * @summary Loads a dynamic library
 * @param library The library to load
 * @returns A handle to the loaded library
 */
dynamic_library load_dynamic_library(char *library)
{
    char *error;

    dlerror(); /* clear existing errors */
    dynamic_library handle = dlopen(library, RTLD_LAZY);
    if (!handle) {
        fprintf(stderr, "%s\n", dlerror());
        exit(EXIT_FAILURE);
    }
    return handle;
}

/**
 * @summary Unloads a currently loaded library
 * @param method The method to retrieve
 * @param library The library to unload
 */
void unload_dynamic_library(dynamic_library handle)
{
    if (handle != NULL) {
        dlerror(); /* clear existing errors */
        dlclose(handle);
    }
}

/**
 * @summary Gets a handle to the specified method
 * @param method The method to retrieve
 * @param lib The library to get the method from
 * @returns A handle to the requested method
 */
dynamic_method get_dynamic_method(char *method,
    dynamic_library library)
{
    char *error;

    dlerror(); /* clear existing errors */
    dynamic_method handle = dlsym(library, method);
    if ((error = dlerror()) != NULL) {
        fprintf(stderr, "%s\n", dlerror());
        exit(EXIT_FAILURE);
    }
    return handle;
}

/**
 * @summary main starting point
 * @param argc The number of command line args
 * @param argv The actual command line args
 * @return The result of the program
 */
int main(int argc, char **argv)
{
    dynamic_library library = load_dynamic_library("libm.so");
    dynamic_method method = get_dynamic_method("cos", library);
    printf("%f\n", ((double (*)(double))method)(2.0));
    unload_dynamic_library(library);

    return 0;
}
