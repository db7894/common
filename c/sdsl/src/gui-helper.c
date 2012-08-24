/**
 * @file gui-helper.c
 * @brief Helper functions for gtk gui
 * @author Galen Collins
 */
#include "common.h"
#include "gui-helper.h"

/**
 * @brief Displays an error dialog
 * @param message The message to prompt the user with
 * @return void
 *
 * We call error_message() any time we want to display an error message to the
 * user. It will both show an error dialog and log the error to the terminal
 * window.
 *
 * @code
 *  gchar *message  = "Error Message";
 *
 *  gui_error_prompt(message);
 * @endcode
 */
void gui_error_prompt(const gchar *message)
{
	GtkWidget *dialog;
	
	/* create an error message dialog and display modally to the user */
	dialog = gtk_message_dialog_new (NULL, 
			GTK_DIALOG_MODAL | GTK_DIALOG_DESTROY_WITH_PARENT,
			GTK_MESSAGE_ERROR,
			GTK_BUTTONS_OK,
			message);
        
	gtk_window_set_title(GTK_WINDOW (dialog), "Error!");
	gtk_dialog_run(GTK_DIALOG(dialog));      
	gtk_widget_destroy(dialog);         
}

/**
 * @brief Displays an warning dialog
 * @param message The message to prompt the user with
 * @return void
 *
 * @code
 *  gchar *message  = "Warning Message";
 *
 *  gui_warning_prompt(message);
 * @endcode
 */
void gui_warning_prompt(const gchar *message)
{
	GtkWidget *dialog;
	
	/* create an error message dialog and display modally to the user */
	dialog = gtk_message_dialog_new (NULL, 
			GTK_DIALOG_MODAL | GTK_DIALOG_DESTROY_WITH_PARENT,
			GTK_MESSAGE_WARNING,
			GTK_BUTTONS_OK,
			message);
        
	gtk_window_set_title(GTK_WINDOW(dialog), "Warning");
	gtk_dialog_run(GTK_DIALOG(dialog));      
	gtk_widget_destroy(dialog);         
}

/**
 * @brief Displays a yes/no prompt
 * @param message The message to prompt the user with
 * @return True for yes, False for no
 *
 * @code
 *  gchar *message = "Choose yes or no";
 *  gboolean result;
 *
 *  result = gui_yesno_prompt(message);
 *
 *  if (result)
 * 		printf("User selected yes\n");
 * 	else
 * 		printf("User selected no\n");
 * @endcode
 */
gboolean gui_yesno_prompt(const gchar *message)
{
	GtkWidget *dialog;
	gint result;
	
	dialog = gtk_message_dialog_new (NULL, 
			GTK_DIALOG_MODAL | GTK_DIALOG_DESTROY_WITH_PARENT,
			GTK_MESSAGE_QUESTION,
			GTK_BUTTONS_YES_NO,
			message);
        
	gtk_window_set_title(GTK_WINDOW(dialog), "Question?");
	result = gtk_dialog_run(GTK_DIALOG(dialog));      
	gtk_widget_destroy(dialog);

	return (result == GTK_RESPONSE_YES) ? TRUE : FALSE;

}

/**
 * @brief Retrieves a filename from the user
 * @param window Handle to the main window
 * @return The selected filename
 *
 * We call get_open_filename() when we want to get a filename to open from the
 * user. It will present the user with a file chooser dialog and return the 
 * newly allocated filename or NULL.
 *
 * @code
 *  GladeXML *xml;
 *  GtkWidget *window;
 *  gchar *handle;
 *
 * 	gui_signal_connect(xml, handle);
 * 	window = glade_xml_get_widget(handle->glade, "window1");
 * 	handle = gui_file_prompt(window);
 *
 * 	printf("Selected File: %s\n", handle);
 * @endcode
 */
gchar *gui_file_prompt(GtkWidget *window)
{
	GtkWidget *chooser;
	gchar     *filename=NULL;
	        
	chooser = gtk_file_chooser_dialog_new("Open File...",
				GTK_WINDOW(window),
				GTK_FILE_CHOOSER_ACTION_OPEN,
				GTK_STOCK_CANCEL, GTK_RESPONSE_CANCEL,
				GTK_STOCK_OPEN, GTK_RESPONSE_OK,
				NULL);
	                                       
	if (gtk_dialog_run(GTK_DIALOG (chooser)) == GTK_RESPONSE_OK) {
	        filename = gtk_file_chooser_get_filename(GTK_FILE_CHOOSER(chooser));
	}
	gtk_widget_destroy(chooser);

	return filename;
}

/**
 * @brief Prompts the user with an about dialog
 * @param window Handle to the main application window
 * @return void
 *
 * @code
 *  GladeXML *xml;
 *  GtkWidget *window;
 *
 * 	gui_signal_connect(xml, handle);
 * 	window = glade_xml_get_widget(handle->glade, "window1");
 * 	gui_about_prompt(window);
 * @endcode
 */
void gui_about_prompt(GtkWidget *window)
{
	static const gchar * const authors[] = {
		"Galen Collins <galen.collins@aperture.com>",
		NULL
	};

	static const gchar copyright[] = \
		"Copyright © 2008 Aperture Technologies";

	static const gchar comments[] = \
		"For More Information Please Visit The Main HP Website";

	gtk_show_about_dialog(GTK_WINDOW(window),
		"authors", authors,
		"comments", comments,
		"copyright", copyright,
		"version", "0.1",
		"website", "http://www.hp.com",
		"program-name", "HP Management Module Flash Utility",
		"logo-icon-name", GTK_STOCK_EDIT,
		NULL); 
}

