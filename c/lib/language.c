/**
 * @file language.c
 * @brief Quick functions to control application language
 * @author Galen Collins
 */

#include <locale.h>
#include "language.h"

#if HAVE_CONFIG_H
#	include <config.h>	/* autotools header */
#endif

/**
 * @brief Initialize an application for i18n
 * @return 0 if successful, -1 if failure
 */
int language_init(void)
{
	(void)setlocale(LC_ALL,	"");
	(void)bindtextdomain(PACKAGE, LOCALEDIR);
	(void)textdomain(PACKAGE);

	return 0;
}

/**
 * @brief Dynamically alters the current language settings
 * @param lang The constant for the new langauge to set
 * @return 0 if successful, -1 if failure
 */
int language_set(int lang)
{
	static char *setting[] = {"en", "es", "ge", "it", "fr", "ja" };
	char *local;

	if (lang >= 0 && lang < MAX_LANGUAGES) {
		local = setlocale(LC_ALL, setting[lang]);
		return 0;
	}
	return -1;
}

/**
 * @brief Gets the current language setting string
 * @return The language string if successful, NULL if failure
 */
char *language_get(void)
{
	char *local;

	local = setlocale(LC_ALL, NULL);
	if (!local)
		return (char *)NULL;
	return local;
}

