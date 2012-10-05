/**
 * @file stringx.h
 * @brief Extended string functions
 */

#ifndef SAFE_STRING
#define SAFE_STRING

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------//

int isnull_string(void *buffer, int length);
int isalpha_string(char *input);
int isalnum_string(char *input);
int isdigit_string(char *input);
int ishex_string(char *input);

char *trim(char *str);
char *trim_left(char *str);
char *trim_right(char *str);
char *to_lower_str(char *str);
char *to_upper_str(char *str);

int strcmp_nocase(const char *a, const char *b);
char *filter_str(char *input, char *bad);
char *strip(char *input, char *bad);
char *split(char *input, const char *x, char **split, int *size);

#endif
