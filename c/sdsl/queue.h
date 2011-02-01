/**
 * @file queue.h
 * @brief Implentation of a queue
 */
#ifndef SIMPLE_QUEUE_H
#define SIMPLE_QUEUE_H

#include "dll.h"

//---------------------------------------------------------------------------// 
// Types 
//---------------------------------------------------------------------------// 
typedef struct dlist squeue;

//---------------------------------------------------------------------------// 
// Function Declarations
//---------------------------------------------------------------------------// 
squeue *queue_enqueue(squeue *list, void *data);
squeue *queue_dequeue(squeue *list, void **data);
void *queue_peek(squeue *list);
int queue_size(squeue *list);

#endif
