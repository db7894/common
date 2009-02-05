/**
 * @file dll.h
 * @brief Implementation of a doubly linked list
 */
#ifndef _SAFE_DLL_H
#define _SAFE_DLL_H

//---------------------------------------------------------------------------// 
// Types
//---------------------------------------------------------------------------// 
typedef struct dlist dlist;

struct dlist {
	struct dlist *prev;
	struct dlist *next;
	void *data;
};

//---------------------------------------------------------------------------// 
// Function Declarations
//---------------------------------------------------------------------------// 
struct dlist *dlist_alloc(void);
void dlist_free(struct dlist *entry);
void dlist_destroy(struct dlist *list);

struct dlist *dlist_prepend(struct dlist *list, void *data);
struct dlist *dlist_append(struct dlist *list, void *data);
struct dlist *dlist_insert(struct dlist *list, void *data, int pos);
struct dlist *dlist_concat(struct dlist *list1, struct dlist *list2);

struct dlist *dlist_remove_link(struct dlist *list, struct dlist *link);
struct dlist *dlist_delete_link(struct dlist *list, struct dlist *link);

struct dlist *dlist_delete(struct dlist *list, void *data);
struct dlist *dlist_delete_all(struct dlist *list, void *data);

int dlist_length(struct dlist *list);
struct dlist *dlist_last(struct dlist *list);
struct dlist *dlist_first(struct dlist *list);

struct dlist *dlist_find(struct dlist *list, void *data);
int dlist_index(struct dlist *list, void *data);

struct dlist *dlist_nth(struct dlist *list, int index);
struct dlist *dlist_nth_prev(struct dlist *list, int index);
void *dlist_nth_data(struct dlist *list, int index);

struct dlist *dlist_copy(struct dlist *list);

#endif

