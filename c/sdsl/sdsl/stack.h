/**
 * @file stack.h
 * @brief Implentation of a stack
 */
#ifndef SIMPLE_STACK_H
#define SIMPLE_STACK_H

#include "dll.h"

//---------------------------------------------------------------------------// 
// Types 
//---------------------------------------------------------------------------// 
typedef struct dlist sstack;

//---------------------------------------------------------------------------// 
// Function Declarations
//---------------------------------------------------------------------------// 
sstack *stack_push(sstack *list, void *data);
sstack *stack_pop(sstack *list, void **data);
void *stack_peek(sstack *list);
int stack_height(sstack *list);

#endif
