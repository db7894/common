/**
 * @file gui-helper.h
 * @brief Header file for gtk gui common functions
 * @author Galen Collins
 */
#ifndef GUI_HELPER_H
#define GUI_HELPER_H

#include <gtk/gtk.h>

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------// 
void gui_error_prompt(const gchar *message);
void gui_warning_prompt(const gchar *message);
gboolean gui_yesno_prompt(const gchar *message);
gchar *gui_file_prompt(GtkWidget *window);
void gui_about_prompt(GtkWidget *window);

#endif

