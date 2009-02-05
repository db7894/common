/**
 * @file language.h
 * @brief Header file for application language functions
 * @author Galen Collins
 */
#ifndef LANGUAGE_H
#define LANGUAGE_H

#include "gettext.h"

//---------------------------------------------------------------------------// 
// Types
//---------------------------------------------------------------------------// 
/**
 * @brief Sets the path to look for the language domain
 * @note Set to NULL to use the default path
 */
#define LOCALEDIR		"."

/**
 * @brief Constants for readable language constants
 */
enum language
{
	ENGLISH,
	SPANISH,
	GERMAN,
	ITALIAN,
	FRENCH,
	JAPANESE,

	MAX_LANGUAGES
};

//---------------------------------------------------------------------------// 
// Macros
//---------------------------------------------------------------------------// 
/**
 * @brief Wrapper for text conversion
 * @param text String to translate
 * @return Translated string
 */
#define _(text)	gettext(text)

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------// 
int language_init(void);
int language_set(int lang);
char *language_get(void);

#endif

