/**
 * @file stringx.c
 * @brief Extended string functions
 */

/**
 * System Includes
 */
#include "common.h"
#include "stringx.h"
#ifdef HAVE_CTYPE_H
#	include <ctype.h>	/* for isxxx functions */
#endif

//---------------------------------------------------------------------------// 
// String Checking
//---------------------------------------------------------------------------// 

/**
 * @brief Checks if string is [0]
 * @param buffer Buffer to check
 * @param length Length of buffer to check
 * @return 1 if successful, 0 otherwise
 *
 * @code
 *  char buffer[] = "\0\0\0\0\0 \0\0\0";
 *  int length    = 9;
 *
 *  if (isnull_string(buffer, 9)
 *     puts("String is null");
 *  else
 *     puts("String is not-null");
 * @endcode
 */
int isnull_string(void *buffer, int length)
{
	int i;
	char *p = (char *)buffer;

	for (i = 0; i < length; i++) {
		if (p[i])
			return 0;
	}
	return 1;
}

/**
 * @brief Checks if string is [a-zA-Z]
 * @param input String to check
 * @return 1 if successful, 0 otherwise
 *
 * @code
 *  char buffer[] = "abdSDFdcs";
 *
 *  if (isalpha_string(buffer)
 *     puts("String is only alpha");
 *  else
 *     puts("String is not only alpha");
 * @endcode
 */
int isalpha_string(char *input)
{
	char *p = input;
	if (input == NULL)
		return 0;
	do {
		if (!isalpha(*p))
			return 0;
	} while (*++p);
	return 1;
}

/**
 * @brief Checks if string is [a-zA-Z0-9_]
 * @param input String to check
 * @return 1 if successful, 0 otherwise
 *
 * @code
 *  char buffer[] = "abdSDF123";
 *
 *  if (isalnum_string(buffer)
 *     puts("String is only alphanumeric");
 *  else
 *     puts("String is not only alphanumeric");
 * @endcode
 */
int isalnum_string(char *input)
{
	char *p = input;
	if (input == NULL)
		return 0;
	do {
		if (!(isalnum(*p) || *p == '_'))
			return 0;
	} while (*++p);
	return 1;
}

/**
 * @brief Checks if string is [0-9]
 * @param input String to check
 * @return 1 if successful, 0 otherwise
 *
 * @code
 *  char buffer[] = "1FFF23";
 *
 *  if (isdigit_string(buffer)
 *     puts("String is only digits");
 *  else
 *     puts("String is not only digits");
 * @endcode
 */
int isdigit_string(char *input)
{
	char *p = input;
	if (input == NULL)
		return 0;
	do {
		if (!isdigit(*p))
			return 0;
	} while (*++p);
	return 1;
}

/**
 * @brief Checks if string is [0-9a-fA-F]
 * @param input String to check
 * @return 1 if successful, 0 otherwise
 *
 * Note, this also checks for the starting 0[xX]
 *
 * @code
 *  char buffer[] = "0xAfeeF9x";
 *
 *  if (isdigit_string(buffer)
 *     puts("String is hex");
 *  else
 *     puts("String is not hex");
 * @endcode
 */
int ishex_string(char *input)
{
	char *p = input;
	if (input == NULL)
		return 0;

	if (    input[0] == '0'
		&& (input[1] == 'x' || input[1] == 'X'))
		p += 2;

	do {
		if (!(   isdigit(*p)
			  || (*p >= 'a' && *p <= 'f')
			  || (*p >= 'A' && *p <= 'F')))
			return 0;
	} while (*++p);
	return 1;
}

//---------------------------------------------------------------------------// 
// String Manipulation
//---------------------------------------------------------------------------// 
/**
 * @brief Trims space characters on left and right side of string
 * @param str String trim
 * @return pointer to resulting string
 *
 * Note: this function changes the value of the
 * string in memory, do not pass values from the heap
 *
 * @code
 *  char buffer[] = "   Empty Space  ";
 *
 *  printf("Before: %s\n", buffer);
 *  printf("After:  %s\n", trim(buffer));
 * @endcode
 */
char *trim(char *str)
{
	size_t left, size = strlen(str);

	while (size && isspace(str[size-1]))
		size--;

	if (size) {
		left = strspn(str, " \n\r\t\v");
		if (left){
			size -= left;
			memmove(str, str + left, size);
		}
	}
	str[size] = '\0'; /* start of text */
	return str;
}

/**
 * @brief Trims space characters on left size of string
 * @param str String trim
 * @return pointer to resulting string
 *
 * Note: this function changes the value of the
 * string in memory, do not pass values from the heap
 *
 * @code
 *  char buffer[] = "   Empty Space  ";
 *
 *  printf("Before: %s\n", buffer);
 *  printf("After:  %s\n", trim_left(buffer));
 * @endcode
 */
char *trim_left(char *str)
{
	size_t left, size = strlen(str);

	if (size) {
		left = strspn(str, " \n\r\t\v");
		if (left)
			memmove(str, str + left, size - left);
	}
	return str;
}

/**
 * @brief Trims space characters on right side of string
 * @param str String trim
 * @return pointer to resulting string
 *
 * Note: this function changes the value of the
 * string in memory, do not pass values from the heap
 *
 * @code
 *  char buffer[] = "   Empty Space  ";
 *
 *  printf("Before: %s\n", buffer);
 *  printf("After:  %s\n", trim_right(buffer));
 * @endcode
 */
char *trim_right(char *str)
{
	size_t size = strlen(str);

	while (size && isspace(str[size-1]))
	   	size--;
	str[size] = '\0'; /* start of text */

	return str;
}

/**
 * @brief Converts a string to lowercase
 * @param str String to convert to lowercase
 * @return Pointer to lowercase string
 *
 * Note: this function changes the value of the
 * string in memory, do not pass values from the heap
 *
 * @code
 *  char buffer[] = "TeSt stRING";
 *
 *  printf("Before: %s\n", buffer);
 *  printf("After:  %s\n", to_lower_str(buffer));
 * @endcode
 */
char *to_lower_str(char *str)
{
	char *c;

	for (c = str; *c; ++c)
		*c = tolower(*c);

	return str;
}

/**
 * @brief Converts a string to uppercase
 * @param str String to convert to uppercase
 * @return Pointer to uppercase string
 *
 * Note: this function changes the value of the
 * string in memory, do not pass values from the heap
 *
 * @code
 *  char buffer[] = "TeSt stRING";
 *
 *  printf("Before: %s\n", buffer);
 *  printf("After:  %s\n", to_upper_str(buffer));
 * @endcode
 */
char *to_upper_str(char *str)
{
	char *c;

	for (c = str; *c; ++c)
		*c = toupper(*c);

	return str;
}


/**
* @brief Tests whether filenames are the same (case insensitive)
* @param a First string to match against
* @param b Second string to match against
* @return 1 if successful match, 0 if not
 *
 * @code
 *  char a[] = "TeSt stRING";
 *  char b[] = "TesT STrIng";
 *
 *  if (strcmp_nocase(a, b))
 *     puts("The Strings Match");
 *  else
 *     puts("The Strings Do Not Match");
 * @endcode
*/
int strcmp_nocase(const char *a, const char *b)
{
	do {
		if (tolower(*a) != tolower(*b))
            return 0;
	} while (*a++ && *b++);

    return 1;
}

/**
* @brief Filters the passed in characters from input
* @param input String to scan for bad values
* @param bad String of bad characters to test for 
* @return pointer to result string
 *
 * The following Will output: ".h1.Bad string..h1."
 * @code
 *  char a[] = "<h1>Bad string</h1>";
 *  char b[] = "<>\\/()";
 *
 *  printf("%s\n", filter_str(a, b);
 * @endcode
*/
char *filter_str(char *input, char *bad)
{
	char *b, *a = input;

	do {
		b = bad;
		do {
			if (*a == *b) {
				*a = '.';
				break;
			}
		} while (*++b);
	} while (*++a);

	return input;
}

/**
* @brief Strips the passed in characters from input
* @param input String to scan for bad values
* @param bad String of bad characters to test for 
* @return pointer to result string, or 0 if bad malloc
 *
 * The following Will output: "h1Badstringh1"
 * @code
 *  char a[] = "<h1>Bad string</h1>";
 *  char b[] = "<>\\/() ";
 *
 *  printf("%s\n", strip(a, b);
 * @endcode
*/
char *strip(char *input, char *bad)
{
	char *handle, *b, *a = input;
	int skip;

	handle = b = (char *)malloc(strlen(input));
	if (b == NULL)
		return (char *)0;

	do {
		skip = 0;
		do {
			if (*a == bad[skip]) {
				skip = -1;
			   	break;
			}
		} while (bad[++skip]);
		if (skip != -1) *b++ = *a;
	} while (*++a);

	strcpy(input, handle);
	free(handle);

	return input;
}

/**
* @brief Splits the input string into tokens
* @param input String to split
* @param x String of characters to split by
* @param split Array of character pointers to store tokens
* @param size The number of slots available in split
* @return Number of tokens found or -1 if error
* @warning This function will alter the input string, however,
* it will not alter the pointer location.
* @note This will always return n-1 of size to prevent returning
* untokenized strings.
*
* The following Will output: "A\nB\nC\nD\nE\nF\nG"
* @code
*  #define MAX_TOKENS 5
*
*  char x[] = "A B^C	D^E^F^G";
*  char *tmp, *handle[MAX_TOKENS];
*  int i, n;

*  tmp = x;  
*  do {
*      n = MAX_TOKENS;
*      tmp = split(tmp, "\t^ ", handle, &n);
*      
*      for (i=0; i < n; i++)
*          printf("%s\n", handle[i]);
*  } while (n != 0);
* @endcode
*/
char *split(char *input, const char *x, char **split, int *size)
{
	int i = 0, n;
	char *handle = input;
	
	if (*handle == '\0')
		goto split_clean;
	
	split[i++] = handle;
	
	do {
		n = 0;
		while(x[n] != '\0') {
			if (*handle == x[n++]) {
				*(handle++) = '\0';
				if ((i + 1) >= *size)
					goto split_clean;
				split[i++] = handle;
			}
		}
	} while (*handle++);

split_clean:
	*size = i;	
	return handle;
}

