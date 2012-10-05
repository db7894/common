/**
 * @file stack.c
 * @brief Implentation of a stack
 * @note Since we are using a dlist, our stack is actually 
 * top->next->next->bottom
 */
#include "common.h"
#include "stack.h"


/**
 * @brief Puts a new value on the stack
 * @param list The head of the stack
 * @param The data to push onto the stack
 * @return The new stack head
 */
sstack *stack_push(sstack *list, void *data)
{
	return dlist_prepend(list, data);
}

/**
 * @brief Pops a value off of the stack
 * @param list The head of the stack
 * @param Point to store the data off of the stack
 * @return The new stack head
 */
sstack *stack_pop(sstack *list, void **data)
{
	sstack *tmp = list;

	list = dlist_remove_link(list, list->prev);
	*data = tmp->data;
	free(tmp);

	return list;
}

/**
 * @brief Checks the value without popping
 * @param list The head of the stack
 * @return The data at the stack head
 */
void *stack_peek(sstack *list)
{
	return list->data;
}

/**
 * @brief Returns the current height of the stack
 * @param list The tail of the stack
 * @return The total size of the stack
 */
int stack_height(sstack *list)
{
	int size = 0;

	while (list) {
		size++;
		list = list->next;
	}

	return size;
}
