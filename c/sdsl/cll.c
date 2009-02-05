/**
 * @file cll.c
 * @brief Implementation of a circular doubly linked list
 */
#include "common.h"
#include "cll.h"

//---------------------------------------------------------------------------// 
// Initialization Methods
//---------------------------------------------------------------------------// 
/**
 * @brief Allocate a new list list
 * @return void
 */
struct clist *clist_alloc(void)
{
	return xmalloc(sizeof(struct clist));
}

/**
 * @brief Free a list entry
 * @param list The list to free (data and list element)
 * @return void
 */
void clist_free(struct clist *entry)
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
 * @todo This is not right
 */
void clist_destroy(struct clist *list)
{
	struct clist *h = list->next;

	do {
		h = list->next;
		clist_free(h);
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
struct clist *clist_prepend(struct clist *list, void *data)
{
	struct clist *next = clist_alloc();

	next->data = data;
	next->next = list;

	if (list) {
		next->prev = list->prev;
		if (next->prev)
			next->prev->next = next;
		list->prev = next;
	}
	else
		next->prev = next;

	return next;
}

/**
 * @brief Append a list element to the end of the list
 * @param list The list list pointer
 * @param data The data to add to the list
 * @return The new list list
 */
struct clist *clist_append(struct clist *list, void *data)
{
	struct clist *next = clist_alloc();

	next->data = data;
	next->next = list;

	if (list) {
		next->prev = list->prev;
		if (next->prev)
			next->prev->next = next;
		list->prev = next;
	}
	else
		next->prev = next;

	return next;
}

/**
 * @brief Insert a list element to the specified position
 * @param list The list list pointer
 * @param data The data for the new list
 * @param pos The position to put the new list
 * @return The new list list
 */
struct clist *clist_insert(struct clist *list, void *data, int pos)
{
	struct clist *tmp = NULL, *next = NULL;

	if (pos < 0)
		return clist_append(list, data);
	else if (pos == 0)
		return clist_prepend(list, data);

	tmp = clist_nth(list, pos);
	if (!tmp)
		return clist_append(list, data);

	next = clist_alloc();
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
struct clist *clist_concat(struct clist *list1, struct clist *list2)
{
	struct clist *tmp = NULL;

	if (list2) {
		if (list1) {
			list1->prev->next = list2;
			list2->prev->next = list1;

			tmp = list2->prev;
			list2->prev = list1->prev;
			list1->prev = tmp;
		}
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
 * @brief Pulls an list out of a list without freeing it
 * @param list The list list pointer
 * @param link The list to delete
 * @return The new list lister
 */
static struct clist *_remove_link(struct clist *list, struct clist *link)
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
struct clist *clist_remove_link(struct clist *list, struct clist *link)
{
	return _remove_link(list, link);
}

/**
 * @brief Pulls an list out of a list and frees it
 * @param list The list list pointer
 * @param link The list to delete
 * @return The new list lister
 */
struct clist *clist_delete_link(struct clist *list, struct clist *link)
{
	list = _remove_link(list, link);
	clist_free(link);

	return list;
}

/**
 * @brief Remove a list list matching data
 * @param list The list list pointer
 * @param data The data to match
 * @return The new list lister
 */
struct clist *clist_delete(struct clist *list, void *data)
{
   	struct clist *t = list;

	while (t) {
		if (t->data == data) {
			if (t->prev)
				t->prev->next = t->next;
			if (t->next)
				t->next->prev = t->prev;
			if (t == list)
				list = list->next;

			clist_free(t);
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
struct clist *clist_delete_all(struct clist *list, void *data)
{
   	struct clist *n = NULL, *t = list;

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

			clist_free(t);
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
int clist_length(struct clist *list)
{
	struct clist *t = list;
	int size = 0;

	if (list) {
		do {
			t = list->next;
			size++;
		} while (t != list);
	}

	return size;
}

/**
 * @brief Return the last list in a list
 * @param list The list to get the size of
 * @return the size of the list
 */
struct clist *clist_last(struct clist *list)
{
	return list->prev;
}

/**
 * @brief Return the list element with matching data
 * @param list The list to get the size of
 * @param data The pointer to search for
 * @return the size of the list
 */
struct clist *clist_find(struct clist *list, void *data)
{
	struct clist *t = NULL;

	if (list) {
		t = list;
		do {
			if (t->data == data)
				break;
			t = t->next;
		} while (t != list);
	}

	return t;
}

/**
 * @brief Return the index of the requested data
 * @param list The list to get the size of
 * @param data The pointer to search for
 * @return The index of the matching list element
 */
int clist_index(struct clist *list, void *data)
{
	struct clist *t = NULL;
	int i = 0;

	if (list) {
		t = list;
		do {
			if (t->data == data)
				return i;
			i++;
			t = t->next;
		} while (t != list);
	}

	return -1;
}

/**
 * @brief Return the requested member of the list
 * @param list The list to scan through
 * @param index The postion of the list to extract
 * @return the requested element
 */
struct clist *clist_nth(struct clist *list, int index)
{
	while ((index-- > 0) && list)
		list = list->next;

	return list;
}

/**
 * @brief Return the requested previous member of the list
 * @param list The list to scan through
 * @param index The postion of the list to extract
 * @return the requested element
 */
struct clist *clist_nth_prev(struct clist *list, int index)
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
void *clist_nth_data(struct clist *list, int index)
{
	while ((index-- > 0) && list)
		list = list->next;

	return (list) ? list->data : (void *)NULL;
}

/**
 * @brief Shallow copy of the list passed in
 * @param list The list to copy
 * @return the newly created list
 * @todo Finish This
 */
struct clist *clist_copy(struct clist *list)
{
	struct clist *next = NULL, *last = NULL;

	/* first one */
	if (list) {
		next = clist_alloc();
		next->data = list->data;
		next->prev = NULL;
		last = next;
		list = list->next;

		/* the rest */
		while (list) {
			next->next = clist_alloc();
			next->next->prev = next;
			next = next->next;
			next->data = list->data;
			list = list->next;
		}
		next->next = NULL;
	}

	return last;
}

