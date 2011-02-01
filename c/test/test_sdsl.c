#include <assert.h>
#include <stdio.h>
#include "common.h"
#include "stack.h"
#include "queue.h"

typedef struct {
    char first;
    int  second;

} test_data;

/**
 * @summary A test of the sdsl stack implementation
 */
void test_memory(void)
{
    int i;

    printf("Starting Memory Tests...");
    for (i = 0; i <= 10; ++i) {
        test_data *data = (test_data*)xmalloc(sizeof(test_data));
        data = xfree(data);
        assert(data == NULL);
    }
    printf("Passed\n");
}

/**
 * @summary A test of the sdsl stack implementation
 */
void test_stack(void)
{
    int i = 0;
    test_data* data = NULL;
    sstack *stack = (sstack *)clist_alloc();

    printf("Starting Stack Tests...");
    for (i = 0; i <= 10; ++i) {
        data = (test_data *)xmalloc(sizeof(test_data));
        data->first  = (char)(65 + i);
        data->second = i;
        stack = stack_push(stack, data);
    }

    for (i = 10; i >= 0; --i) {
        stack = stack_pop(stack, (void**)&data);
        assert(data->first == (char)(65 + i));
        assert(data->second == i);
        xfree(data);
    }
    clist_free(stack);
    printf("Passed\n");
}

/**
 * @summary A test of the sdsl queue implementation
 */
void test_queue(void)
{
    int i = 0;
    test_data* data = NULL;
    squeue *queue = (squeue*)clist_alloc();

    printf("Starting Queue Tests...");
    for (i = 0; i <= 10; ++i) {
        data = (test_data*)xmalloc(sizeof(test_data));
        data->first  = (char)(65 + i);
        data->second = i;
        queue = queue_enqueue(queue, data);
    }

    for (i = 0; i <= 10; ++i) {
        queue = queue_dequeue(queue, (void**)&data);
        assert(data->first == (char)(65 + i));
        assert(data->second == i);
        xfree(data);
    }
    clist_free(queue);
    printf("Passed\n");
}

/**
 * @summary The main test runner
 */
int main(void)
{
    test_memory();
    test_stack();
    test_queue();

    return 0;
}
