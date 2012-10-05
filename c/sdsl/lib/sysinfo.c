/**
 * @file sysinfo.c
 * @brief Wrapper functions for system information
 */
#include "common.h"
#include "sysinfo.h"

//---------------------------------------------------------------------------// 
// Network Information
//---------------------------------------------------------------------------// 

/**
 * @brief Retrieve the current host name
 * @return Pointer to the current host name
 * @note The memory must be freed after use
 */
char *sys_hostname(void)
{
	size_t size = 64;
	char *name = (char *)xmalloc(size);

	do {
		if (gethostname(name, size)) {
			size *= 2;
			name = (char *)xrealloc(name, size);
		}
		else {
			break;
		}
	} while (1);

	return name;
}

/**
 * @brief Retrieve the current domain name
 * @return Pointer to the current domain name
 * @note The memory must be freed after use
 */
char *sys_domainname(void)
{
	size_t size = 64;
	char *name = (char *)xmalloc(size);

	do {
		if (getdomainname(name, size)) {
			size *= 2;
			name = (char *)xrealloc(name, size);
		}
		else {
			break;
		}
	} while (1);

	return name;
}

#if 0
int main(void)
{
	char *h = sys_hostname();
	char *i = sys_domainname();
	printf("%s\n", h);
	printf("%s\n", i);
	free(h);
	free(i);

	return 0;
}
#endif
