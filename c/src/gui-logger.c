/**
 * @file gui-logger.c
 * @brief Logging functions for gtk gui
 * @author Galen Collins
 */
#include "common.h"
#include "gui-logger.h"

int SILENT = 1;					/**< Global variable that indicates GUI or not */
static GtkTextBuffer *buffer;	/**< Global handle to the application log buffer */

/**
 * @brief Append text to the end of the GUI log
 * @param message The text to append
 * @return result of transaction
 *
 * @code
 *   gchar *message = "Adding to log";
 *
 *   gui_update_log(message);
 * @endcode
 */
void gui_update_log(const gchar *message)
{
	GtkTextIter iter;

	if (SILENT) {
		printf("%s\n", message);
	}
	else {
		gdk_threads_enter();
			gtk_text_buffer_get_end_iter(buffer, &iter);
			gtk_text_buffer_insert(buffer, &iter,
					message, strlen(message));
		gdk_threads_leave();
	}
}

/**
 * @brief Clear GUI log and set text
 * @param message The text to append
 * @return result of transaction
 *
 * @code
 *   gchar *message = "Resetting log";
 *
 *   gui_set_log(message);
 * @endcode
 */
void gui_set_log(const gchar *message)
{
	if (SILENT) {
		printf("\033[2J\033[1;1H"	\
			   "%s\n", message);
	}
	else {
		gdk_threads_enter();
			gtk_text_buffer_set_text(buffer, message, -1);
		gdk_threads_leave();
	}
}

/**
 * @brief Append error text to the end of the GUI log
 * @param message The text to append
 * @return result of transaction
 *
 * @code
 *   gchar *message = "Adding error to log";
 *
 *   gui_erro_log(message);
 * @endcode
 */
void gui_error_log(const gchar *message)
{
	GtkTextIter iter;

	if (SILENT) {
		fprintf(stderr, "\033[1;31m%s\033[0m\n",
				message);
	}
	else {
		gdk_threads_enter();
			gtk_text_buffer_get_end_iter(buffer, &iter);
			gtk_text_buffer_insert_with_tags_by_name(buffer, &iter,
					message, strlen(message),
					"error", NULL);
		gdk_threads_leave();
	}
}

/**
 * @brief Append error text to the end of the GUI log
 * @param textview The main text widget to initialize
 * @return 0 if successful, -1 if failure
 *
 * @code
 *  GladeXML * xml;
 *  GtkWidget *window;
 *
 * 	gui_signal_connect(xml, handle);
 * 	window = glade_xml_get_widget(handle->glade, "textview1");
 * 	gui_init_log(GTK_TEXT_VIEW(window));
 * @endcode
 */
int gui_init_log(GtkTextView *textview)
{
	GtkTextTag *tag;

	buffer = gtk_text_view_get_buffer(textview);
	gui_set_log("");	/* clear buffer */

	if (buffer) {
		tag = gtk_text_buffer_create_tag(buffer, "error",
				"foreground", "#FF0000",
				NULL);

		return (tag) ? 0 : -1;
	}
	return -1;
}

