#include <stdio.h>
#include "regex.h"

/**
 *
 * @param regexp The regular expression to match with
 * @param text The text to match against the regexp
 * @return 1 if a match was found, 0 otherwise
 */
int match(char *regexp, char *text) {
    if (regexp[0] == '^')
        return matchhere(regexp + 1, text);

    do {
        if (matchhere(regexp, text))
            return 1;
    } while (*text++ != '\0');
    return 0;
}

/**
 *
 * @param regexp The regular expression to match with
 * @param text The text to match against the regexp
 * @return 1 if a match was found, 0 otherwise
 */
int matchstar(int c, char *regexp, char *text) {
    do {
        if (matchhere(regexp, text))
            return 1;
    } while (*text != '\0' && (*text++ == c || c == '.'));
    return 0;
}

/**
 *
 * @param regexp The regular expression to match with
 * @param text The text to match against the regexp
 * @return 1 if a match was found, 0 otherwise
 */
int matchhere(char *regexp, char *text) {
    if (regexp[0] == '\0')
        return 1;
    if (regexp[0] == '*')
        return matchstar(regexp[0], regexp + 2, text);
    if (regexp[0] == '$' && regexp[1] == '\0')
        return (*text == '\0');
    if (*text != '\0' && (regexp[0] == '.' || regexp[0] == *text))
        return matchhere(regexp + 1, text + 1);
    return 0;
}
