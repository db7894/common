#include <Python.h>
#include "../library/example.h"

static PyObject* wrap_get_data_of_size(PyObject* self, PyObject* args)
{
    char *data;
    unsigned int size;
 
    if (!PyArg_ParseTuple(args, "I", &size))
        return NULL;
 
    data = get_data_of_size(size);
    return PyString_FromStringAndSize(data, size);
}
 
static PyMethodDef ExampleMethods[] =
{
     {"get_data_of_size", wrap_get_data_of_size, METH_VARARGS, "Get a string of variable length"},
     {NULL, NULL, 0, NULL},
};
 
PyMODINIT_FUNC inittestmodule(void)
{
     (void) Py_InitModule("example", ExampleMethods);
}
