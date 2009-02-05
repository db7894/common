/**
 * @file sll.c
 * @brief Implementation of a singly linked list
 */
#include "common.h"
#include "sll.h"

//---------------------------------------------------------------------------// 
// Initialization Methods
//---------------------------------------------------------------------------// 
/**
 * @brief Allocate a new list entry
 * @return void
 */
struct slist *slist_alloc(void)
{
	return xmalloc(sizeof(struct slist));
}

/**
 * @brief Free a list entry
 * @param entry The entry to free (data and list element)
 * @return void
 */
void slist_free(struct slist *entry)
{
	if (entry) {
		if (entry->data)
			free(entry->data);
		free(entry);
	}
}

/**
 * @brief Free and entire list
 * @param list The starting point of a list to destory
 * @return void
 */
void slist_destroy(struct slist *list)
{
	struct slist *h = list;

	while (h) {
		list = h;
		h = list->next;
		slist_free(list);
	}
}

//---------------------------------------------------------------------------// 
// Add Methods
//---------------------------------------------------------------------------// 

/**
 * @brief Add a list element to the start of the list
 * @param list The list list pointer
 * @param data The data to add to the list
 * @return The new list list
 */
struct slist *slist_prepend(struct slist *list, void *data)
{
	struct slist *next = slist_alloc();

	next->data = data;
	next->next = list;

	return next;
}

/**
 * @brief Append a list element to the end of the list
 * @param list The list list pointer
 * @param data The data to add to the list
 * @return The new list list
 */
struct slist *slist_append(struct slist *list, void *data)
{
	struct slist *last = NULL;
	struct slist *next = slist_alloc();

	next->data = data;
	next->next = NULL;

	if (list) {
		last = slist_last(list);
		last->next = next;
		return list;
	}
	else
		return next;
}

/**
 * @brief Append a list element to the end of the list
 * @param list The list list pointer
 * @param data The data for the new entry
 * @param pos The position to put the new entry
 * @return The new list list
 */
struct slist *slist_insert(struct slist *list, void *data, int pos)
{
	struct slist *prev = NULL, *tmp = NULL;
	struct slist *next;

	if (pos < 0)
		return slist_append(list, data);
	else if (pos == 0)
		return slist_prepend(list, data);

	next = slist_alloc();
	next->data = data;

	if (!list) {
		next->next = NULL;
		return next;
	}

	tmp = list;

	while ((pos -- > 0) && tmp) {
		prev = tmp;
		tmp = tmp->next;
	}

	if (tmp) {
		next->next = prev->next;
		tmp->next = next;
	}
	else {
		next->next = list;
		list = next;
	}

	return list;
}

/**
 * @brief Concatinates two lists together
 * @param list1 The first half of the list
 * @param list2 The second half of the list
 * @return The new list list
 */
struct slist *slist_concat(struct slist *list1, struct slist *list2)
{
	if (list2) {
		if (list1)
			slist_last(list1)->next = list2;
		else
			list1 = list2;
			
	}

	return list1;	
}

//---------------------------------------------------------------------------// 
// Delete Methods
//---------------------------------------------------------------------------// 
// I need to finish these
//---------------------------------------------------------------------------// 
/**
 * @brief Pulls an entry out of a list without freeing it
 * @param list The list list pointer
 * @param link The entry to delete
 * @return The new list lister
 */
static struct slist *_remove_link(struct slist *list, struct slist *link)
{
   	struct slist *p = NULL;
	struct slist *t = list;

	while (t) {
		if (t == link) {
			/* check if we are the list */
			if (p)
				p->next = t->next;
			if (list == t)
				list = list->next;
		}
		p = t;
		t = t->next;
	}

	return list;
}

/**
 * @brief Pulls an entry out of a list without freeing it
 * @param list The list list pointer
 * @param link The entry to delete
 * @return The new list lister
 */
struct slist *slist_remove_link(struct slist *list, struct slist *link)
{
	return _remove_link(list, link);
}

/**
 * @brief Pulls an entry out of a list and frees it
 * @param list The list list pointer
 * @param link The entry to delete
 * @return The new list lister
 */
struct slist *slist_delete_link(struct slist *list, struct slist *link)
{
	list = _remove_link(list, link);
	slist_free(link);

	return list;
}

/**
 * @brief Remove a list entry matching data
 * @param list The list list pointer
 * @param data The data to match
 * @return The new list lister
 */
struct slist *slist_delete(struct slist *list, void *data)
{
   	struct slist *p = NULL, *t = list;

	while (t) {
		if (t->data == link) {
			/* check if we are the list */
			if (p)
				p->next = t->next;
			else 
				list = t->next;
			slist_free(t);
			break;
		}
		p = t;
		t = t->next;
	}

	return list;
}

/**
 * @brief Remove all list entries matching data
 * @param list The list list pointer
 * @param data The data to match
 * @return The new list lister
 */
struct slist *slist_delete_all(struct slist *list, void *data)
{
   	struct slist *b = NULL, *p = NULL, *t = list;

	while (t) {
		if (t->data == link) {
			b = t->next;	/* b/c we are not sure who grabs the next entry */
			if (p)			/* check if we are the list */
				p->next = t->next;
			else 
				list = list->next;
			slist_free(t);
			t = b;
		}
		p = t;
		t = t->next;
	}

	return list;
}

//---------------------------------------------------------------------------// 
// Properties
//---------------------------------------------------------------------------// 
/**
 * @brief Return the current length of the list
 * @param list The list to get the size of
 * @return the size of the list
 */
int slist_length(struct slist *list)
{
	int size;

	for (size = 0; list != NULL; size++)
		list = list->next;

	return size;
}

/**
 * @brief Return the current length of the list
 * @param list The list to get the size of
 * @return the size of the list
 */
struct slist *slist_last(struct slist *list)
{
	if (list) {
		while (list->next)
			list = list->next;
	}

	return list;
}

/**
 * @brief Return the list element with matching data
 * @param list The list to get the size of
 * @param data The pointer to search for
 * @return the size of the list
 */
struct slist *slist_find(struct slist *list, void *data)
{
	while (list) {
		if (list->data == data)
			break;
		list = list->next;
	}

	return list;
}

/**
 * @brief Return the index of the requested data
 * @param list The list to get the size of
 * @param data The pointer to search for
 * @return The index of the matching list element
 */
int slist_index(struct slist *list, void *data)
{
	int i = 0;

	while (list) {
		if (list->data == data)
			return i;
		i++;
		list = list->next;
	}

	return -1;
}

/**
 * @brief Return the requested member of the list
 * @note This returns NULL of the index is too large
 * @param list The list to get the size of
 * @return the size of the list
 */
struct slist *slist_nth(struct slist *list, int index)
{
	while ((index-- > 0) && list)
		list = list->next;

	return list;
}

/**
 * @brief Return the requested member of the list's data
 * @note This returns NULL of the index is too large
 * @param list The list to get the size of
 * @return The data for that entry in the list
 */
void *slist_nth_data(struct slist *list, int index)
{
	while ((index-- > 0) && list)
		list = list->next;

	return (list) ? list->data : (void *)NULL;
}

/**
 * @brief Shallow copy of the list passed in
 * @param list The list to copy
 * @return the newly created list
 * @todo I can do better than this
 */
struct slist *slist_copy(struct slist *list)
{
	struct slist *t, *f = NULL, *n = NULL;
	struct slist *h = list;

	/* first one */
	if (list) {
		n = slist_alloc();
		n->next = NULL;
		n->data = h->data;
		f = n;
		h = h->next;
	}

	/* the rest */
	while (h != NULL) {
		t = slist_alloc();
		t->data = h->data;
		t->next = NULL;

		f->next = t;
		f = f->next;
		h = h->next;
	}

	return n;
}

//---------------------------------------------------------------------------// 
// Unit Test
//---------------------------------------------------------------------------// 
#if 0
int main(void)
{
	struct slist *head = NULL, *h = NULL;

	head = slist_append(head, (void *)"Galen");
	head = slist_append(head, (void *)"Billy");
	head = slist_append(head, (void *)"Johnny");

	for (h = head; h != NULL;  h = h->next) {
		printf("%s\n", (char *)h->data);
	}

	//slist_destroy(head);

	return 0;
}
#endif
