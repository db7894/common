/**
 * @file queue.c
 * @brief Implentation of a queue
 * @note Since we are using a dlist, our queue is actually 
 * top->next->next->bottom
 */
#include "common.h"
#include "queue.h"


/**
 * @brief Puts a new value on the queue
 * @param list The head of the queue
 * @param The data to push onto the queue
 * @return The new queue head
 */
squeue *queue_enqueue(squeue *list, void *data)
{
	return dlist_prepend(list, data);
}

/**
 * @brief Pulls a value off of the front of a queue
 * @param list The head of the queue
 * @param Point to store the data off of the queue
 * @return The new queue head
 */
squeue *queue_dequeue(squeue *list, void **data)
{
	squeue *tmp = list;

	list = dlist_remove_link(list, list->prev);
	*data = tmp->data;
	free(tmp);

	return list;
}

/**
 * @brief Checks the current head value without removing it
 * @param list The head of the queue
 * @return The data at the queue head
 */
void *queue_peek(squeue *list)
{
	return list->data;
}

/**
 * @brief Returns the current height of the queue
 * @param list The tail of the queue
 * @return The total size of the queue
 */
int queue_size(squeue *list)
{
	int size = 0;

	while (list) {
		size++;
		list = list->next;
	}

	return size;
}
