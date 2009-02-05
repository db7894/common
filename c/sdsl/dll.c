/**
 * @file dll.c
 * @brief Implementation of a doubly linked list
 */
#include "common.h"
#include "dll.h"

//---------------------------------------------------------------------------// 
// Initialization Methods
//---------------------------------------------------------------------------// 
/**
 * @brief Allocate a new list list
 * @return void
 */
struct dlist *dlist_alloc(void)
{
	return xmalloc(sizeof(struct dlist));
}

/**
 * @brief Free a list list
 * @param list The list to free (data and list element)
 * @return void
 */
void dlist_free(struct dlist *entry)
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
void dlist_destroy(struct dlist *list)
{
	struct dlist *h = list->next;

	do {
		h = list->next;
		dlist_free(h);
	} while (h != list);
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
struct dlist *dlist_prepend(struct dlist *list, void *data)
{
	struct dlist *next = dlist_alloc();

	next->data = data;
	next->next = list;

	if (list) {
		next->prev = list->prev;
		if (next->prev)
			next->prev->next = next;
		list->prev = next;
	}
	else
		next->prev = NULL;

	return next;
}

/**
 * @brief Append a list element to the end of the list
 * @param list The list list pointer
 * @param data The data to add to the list
 * @return The new list list
 */
struct dlist *dlist_append(struct dlist *list, void *data)
{
	struct dlist *last = NULL;
	struct dlist *next = dlist_alloc();

	next->data = data;
	next->next = NULL;

	if (list) {
		last = dlist_last(list);
		last->next = next;
		next->prev = last;
		return list;
	}
	else {
		next->prev = NULL;
		return next;
	}
}

/**
 * @brief Insert a list element to the specified position
 * @param list The list list pointer
 * @param data The data for the new list
 * @param pos The position to put the new list
 * @return The new list list
 */
struct dlist *dlist_insert(struct dlist *list, void *data, int pos)
{
	struct dlist *tmp = NULL, *next = NULL;

	if (pos < 0)
		return dlist_append(list, data);
	else if (pos == 0)
		return dlist_prepend(list, data);

	tmp = dlist_nth(list, pos);
	if (!tmp)
		return dlist_append(list, data);

	next = dlist_alloc();
	next->data = data;
	next->prev = tmp->prev;

	if (tmp->prev)
		tmp->prev->next = next;
	next->next = tmp;
	tmp->prev = next;

	return (tmp == list) ? next : list;
}

/**
 * @brief Concatinates two lists together
 * @param list1 The first half of the list
 * @param list2 The second half of the list
 * @return The new list list
 */
struct dlist *dlist_concat(struct dlist *list1, struct dlist *list2)
{
	struct dlist *tmp = NULL;

	if (list2) {
		tmp = dlist_last(list1);
		if (tmp) {
			tmp->next = list2;
		}
		else
			list1 = list2;
		list2->prev = tmp;
	}

	return list1;	
}

//---------------------------------------------------------------------------// 
// Delete Methods
//---------------------------------------------------------------------------// 
// I need to finish these
//---------------------------------------------------------------------------// 
/**
 * @brief Pulls an list out of a list without freeing it
 * @param list The list list pointer
 * @param link The list to delete
 * @return The new list lister
 */
static struct dlist *_remove_link(struct dlist *list, struct dlist *link)
{
	if (link) {
		if (link->prev)
			link->prev->next = link->next;
		if (link->next)
			link->next->prev = link->prev;

		if (link == list)
			list = list->next;
		link->next = NULL;
		link->prev = NULL;
	}
	return list;
}

/**
 * @brief Pulls an list out of a list without freeing it
 * @param list The list list pointer
 * @param link The list to delete
 * @return The new list lister
 */
struct dlist *dlist_remove_link(struct dlist *list, struct dlist *link)
{
	return _remove_link(list, link);
}

/**
 * @brief Pulls an list out of a list and frees it
 * @param list The list list pointer
 * @param link The list to delete
 * @return The new list lister
 */
struct dlist *dlist_delete_link(struct dlist *list, struct dlist *link)
{
	list = _remove_link(list, link);
	dlist_free(link);

	return list;
}

/**
 * @brief Remove a list list matching data
 * @param list The list list pointer
 * @param data The data to match
 * @return The new list lister
 */
struct dlist *dlist_delete(struct dlist *list, void *data)
{
   	struct dlist *t = list;

	while (t) {
		if (t->data == data) {
			if (t->prev)
				t->prev->next = t->next;
			if (t->next)
				t->next->prev = t->prev;
			if (t == list)
				list = list->next;

			dlist_free(t);
			break;
		}
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
struct dlist *dlist_delete_all(struct dlist *list, void *data)
{
   	struct dlist *n = NULL, *t = list;

	while (t) {
		if (t->data != data)
			t = t->next;
		else 	{
			n = t->next;

			if (t->prev)
				t->prev->next = n;
			else
				list = n;
			if (n)
				n->prev = t->prev;
			if (t == list)
				list = list->next;

			dlist_free(t);
			t = n;
		}
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
int dlist_length(struct dlist *list)
{
	int size;

	for (size = 0; list != NULL; size++)
		list = list->next;

	return size;
}

/**
 * @brief Return the last list in a list
 * @param list The list to get the size of
 * @return the size of the list
 */
struct dlist *dlist_last(struct dlist *list)
{
	if (list) {
		while (list->next)
			list = list->next;
	}

	return list;
}

/**
 * @brief Return the first list in a list
 * @param list The list to get the size of
 * @return the size of the list
 */
struct dlist *dlist_first(struct dlist *list)
{
	if (list) {
		while (list->prev)
			list = list->prev;
	}

	return list;
}

/**
 * @brief Return the list element with matching data
 * @param list The list to get the size of
 * @param data The pointer to search for
 * @return the size of the list
 */
struct dlist *dlist_find(struct dlist *list, void *data)
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
int dlist_index(struct dlist *list, void *data)
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
 * @param list The list to scan through
 * @param index The postion of the list to extract
 * @return the requested element or NULL if the index is too large
 */
struct dlist *dlist_nth(struct dlist *list, int index)
{
	while ((index-- > 0) && list)
		list = list->next;

	return list;
}

/**
 * @brief Return the requested previous member of the list
 * @param list The list to scan through
 * @param index The postion of the list to extract
 * @return the requested element or NULL if the index is too large
 */
struct dlist *dlist_nth_prev(struct dlist *list, int index)
{
	while ((index-- > 0) && list)
		list = list->prev;

	return list;
}

/**
 * @brief Return the requested member of the list's data
 * @note This returns NULL of the index is too large
 * @param list The list to get the size of
 * @return The data for that list in the list
 */
void *dlist_nth_data(struct dlist *list, int index)
{
	while ((index-- > 0) && list)
		list = list->next;

	return (list) ? list->data : (void *)NULL;
}

/**
 * @brief Shallow copy of the list passed in
 * @param list The list to copy
 * @return the newly created list
 */
struct dlist *dlist_copy(struct dlist *list)
{
	struct dlist *next = NULL, *last = NULL;

	/* first one */
	if (list) {
		next = dlist_alloc();
		next->data = list->data;
		next->prev = NULL;
		last = next;
		list = list->next;

		/* the rest */
		while (list) {
			next->next = dlist_alloc();
			next->next->prev = next;
			next = next->next;
			next->data = list->data;
			list = list->next;
		}
		next->next = NULL;
	}

	return last;
}

