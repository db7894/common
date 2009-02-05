/**
 * @file gui-logger.h
 * @brief Header file for gtk gui logging
 * @author Galen Collins
 */
#ifndef GUI_LOGGER_H
#define GUI_LOGGER_H

#include <gtk/gtk.h>

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------// 
void gui_update_log(const gchar *message);
void gui_set_log(const gchar *message);
void gui_error_log(const gchar *message);
int  gui_init_log(GtkTextView *textview);

#endif

