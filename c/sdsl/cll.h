/**
 * @file cll.h
 * @brief Implementation of a doubly linked list
 */
#ifndef _SAFE_CLL_H
#define _SAFE_CLL_H

//---------------------------------------------------------------------------// 
// Types
//---------------------------------------------------------------------------// 
typedef struct clist clist;

struct clist {
	struct clist *prev;
	struct clist *next;
	void *data;
};

//---------------------------------------------------------------------------// 
// Function Declarations
//---------------------------------------------------------------------------// 
struct clist *clist_alloc(void);
void clist_free(struct clist *entry);
void clist_destroy(struct clist *list);

struct clist *clist_prepend(struct clist *list, void *data);
struct clist *clist_append(struct clist *list, void *data);
struct clist *clist_insert(struct clist *list, void *data, int pos);
struct clist *clist_concat(struct clist *list1, struct clist *list2);

struct clist *clist_remove_link(struct clist *list, struct clist *link);
struct clist *clist_delete_link(struct clist *list, struct clist *link);

struct clist *clist_delete(struct clist *list, void *data);
struct clist *clist_delete_all(struct clist *list, void *data);

int clist_length(struct clist *list);
struct clist *clist_last(struct clist *list);
struct clist *clist_first(struct clist *list);

struct clist *clist_find(struct clist *list, void *data);
int clist_index(struct clist *list, void *data);

struct clist *clist_nth(struct clist *list, int index);
struct clist *clist_nth_prev(struct clist *list, int index);
void *clist_nth_data(struct clist *list, int index);

struct clist *clist_copy(struct clist *list);

#endif

