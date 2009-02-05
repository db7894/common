/**
 * @file sll.h
 * @brief Implementation of a singly linked list
 */
#ifndef _SAFE_SLL_H
#define _SAFE_SLL_H

//---------------------------------------------------------------------------// 
// Types
//---------------------------------------------------------------------------// 
typedef struct slist slist;

struct slist {
	struct slist *next;
	void *data;
};

//---------------------------------------------------------------------------// 
// Function Declarations
//---------------------------------------------------------------------------// 
struct slist *slist_alloc(void);
void slist_free(struct slist *entry);
void slist_destroy(struct slist *head);

struct slist *slist_prepend(struct slist *head, void *data);
struct slist *slist_append(struct slist *head, void *data);
struct slist *slist_insert(struct slist *head, void *data, int pos);
struct slist *slist_concat(struct slist *list1, struct slist *list2);

struct slist *slist_remove_link(struct slist *head, struct slist *link);
struct slist *slist_delete_link(struct slist *head, struct slist *link);

struct slist *slist_delete(struct slist *head, void *data);
struct slist *slist_delete_all(struct slist *head, void *data);

int slist_length(struct slist *head);
struct slist *slist_last(struct slist *head);

struct slist *slist_find(struct slist *head, void *data);
int slist_index(struct slist *head, void *data);
struct slist *slist_nth(struct slist *head, int index);
void *slist_nth_data(struct slist *head, int index);

struct slist *slist_copy(struct slist *head);

#endif

