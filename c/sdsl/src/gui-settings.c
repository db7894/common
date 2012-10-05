/**
 * @file gui-settings.c
 * @brief Functions to reset various aspects of the gui
 * @author Galen Collins
*/
/* system includes */
#include "common.h"
#include <gtk/gtk.h>
#include <glade/glade.h>

/* local includes */
#include "gui-settings.h"

/**
 * @brief Wrapper function for bold label reset
 * @param x Handle to glade xml
 * @param e The element to set
 * @param s String to set
 */
static inline void _gui_set_bold_label(GladeXML *x, gchar *e, const gchar *s)
{
	gchar buffer[70]; 
	GtkWidget *w = glade_xml_get_widget(x, e);

	sprintf(buffer, "<span size='large' weight='bold'>%s</span>", s);
	gtk_label_set_markup(GTK_LABEL(w), buffer);
}

/**
 * @brief Wrapper function for label reset
 * @param x Handle to glade xml
 * @param e The element to set
 * @param s String to set
 */
static inline void _gui_set_label(GladeXML *x, gchar *e, const gchar *s)
{
	GtkWidget *w = glade_xml_get_widget(x, e);

	gtk_label_set_text(GTK_LABEL(w), s);
}

/**
 * @brief Wrapper function for combo box text reset
 * @param x Handle to glade xml
 * @param e The element to set
 * @param s Array of elements to set
 */
static inline void _gui_set_combo(GladeXML *x, gchar *e, const gchar *s[])
{
	GtkListStore *model;
	GtkWidget *w;
	GtkTreeIter iter;
	gint i = 0;

	model = gtk_list_store_new(1, G_TYPE_STRING);
	for (i = 0; s[i]; i++) {
		gtk_list_store_append(GTK_LIST_STORE(model), &iter);
		gtk_list_store_set(GTK_LIST_STORE(model), &iter, 0, s[i], -1);
	}
	w = glade_xml_get_widget(x, e);
	gtk_combo_box_set_model(GTK_COMBO_BOX(w), GTK_TREE_MODEL(model));
	g_object_unref(model);
}

/**
 * @brief Wrapper function for bold label reset
 * @param x Handle to glade xml
 * @param e The element to set
 * @param s String to set
 */
static inline void _gui_init_combobox(GladeXML *x)
{
	GtkWidget *w;

	w = glade_xml_get_widget(x, "cLang");
	gtk_combo_box_remove_text(GTK_COMBO_BOX(w), 0);

	w = glade_xml_get_widget(x, "cType");
	gtk_combo_box_remove_text(GTK_COMBO_BOX(w), 0);

	w = glade_xml_get_widget(x, "cMethod");
	gtk_combo_box_remove_text(GTK_COMBO_BOX(w), 0);

	w = glade_xml_get_widget(x, "cPort");
	gtk_combo_box_remove_text(GTK_COMBO_BOX(w), 0);
}

/**
 * @brief Main initializer for the gui
 * @param handle Handle to the application settings
 * @return 0 if successful, -1 if failure
 *
 * @code
 *   void lang_change_callback(struct tag *handle)
 *   {
 *       gui_set_lang(handle);
 *   }
 * @endcode
 */
int gui_set_lang(struct tag *handle)
{
	const gchar *clang_text[7] =
   	{
		_("English"), _("Spanish"),
	   	_("German"),  _("Italian"),
	   	_("French"),  _("Japanese"),
	   	NULL
	};

	const gchar *ctype_text[4] =
   	{
		_("Bootloader"), _("Firmware"), _("Configuration"),
	   	NULL
	};

	const gchar *cport_text[5] =
   	{
		_("Port 1"), _("Port 2"),
	   	_("Port 3"), _("Port 4"),
	   	NULL
	};

	const gchar *cmethod_text[3] =
   	{
		_("Serial Upgrade"), _("Network Upgrade"),
	   	NULL
	};

	_gui_set_bold_label(handle->glade, "lHeader", _("Instructions"));
	_gui_set_label(handle->glade, "lMethod",	_("Upgrade Type"));
	_gui_set_label(handle->glade, "lType",		_("File Type"));
	_gui_set_label(handle->glade, "lFile",		_("File Name"));
	_gui_set_label(handle->glade, "lSetting",	_("Type Settings"));
	_gui_set_label(handle->glade, "lHost",		_("Network Host"));
	_gui_set_label(handle->glade, "lUser",		_("Network Username"));
	_gui_set_label(handle->glade, "lPass",		_("Network Password"));
	_gui_set_label(handle->glade, "lPort",		_("Serial Port"));
	_gui_set_label(handle->glade, "lLang",		_("Language"));
	_gui_set_label(handle->glade, "lInstruct",
	_("Select a upgrade method, a file to upgrade with, the type of file, and optionally a serial port to transfer on.  Then simply click flash."));

	_gui_init_combobox(handle->glade);
	_gui_set_combo(handle->glade, "cLang", clang_text);
	_gui_set_combo(handle->glade, "cType", ctype_text);
	_gui_set_combo(handle->glade, "cPort", cport_text);
	_gui_set_combo(handle->glade, "cMethod", cmethod_text);

	return 0;
}

