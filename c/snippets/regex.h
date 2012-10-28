#ifndef REGEX_H
#define REGEX_H

int match(char *regexp, char *text);
int matchstar(int c, char *regexp, char *text);
int matchhere(char *regexp, char *text);

#endif
