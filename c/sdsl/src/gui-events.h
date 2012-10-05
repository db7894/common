/**
 * @file gui-events.h
 * @brief Header file for gtk gui common functions
 * @author Galen Collins
 */
#ifndef GUI_EVENTS_H
#define GUI_EVENTS_H

#include <gtk/gtk.h>

//---------------------------------------------------------------------------// 
// Types
//---------------------------------------------------------------------------// 
/**
 * @brief Handle to all the pertinant program settings
 */
struct tag
{
	gchar *file;		/**< The file to upload */
	gchar *user;		/**< The username to authenticate with */
	gchar *pass;		/**< The password to authenticate with */
	gchar *host;		/**< The hostname to upload to */
	gint type;			/**< The file-type to upload */
	gint port;			/**< The serial upload port */
	gint method;		/**< The uploading method */
	GladeXML *glade;	/**< Handle to needed gui accessors */
};

/**
 * @brief Readable constants for the upload file type
 */
enum flash_type
{
	BOOTLOADER,
	FIRMWARE,
	CONFIGURATION
};

/**
 * @brief Readable constants for the flash method
 */
enum flash_method
{
	SERIAL_UPLOAD,
	NETWORK_UPLOAD
};

/**
 * @brief Readable constants for the serial port
 */
enum flash_port
{
	PORT_1,
	PORT_2,
	PORT_3,
	PORT_4,

	MAX_SERIAL_PORTS
};

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------// 
void loader_launcher(struct tag *handle);

void on_cMethod_changed(GtkComboBox *w, struct tag *handle);
void on_cFile_changed(GtkFileChooser *w, struct tag *handle);
void on_cType_changed(GtkComboBox *w, struct tag *handle);
void on_cPort_changed(GtkComboBox *w, struct tag *handle);
void on_cLang_changed(GtkComboBox *w, struct tag *handle);
void on_bHelp_clicked(GtkButton *w, struct tag *handle);
void on_bFlash_clicked(GtkButton *w, struct tag *handle);
void on_bClose_clicked(GtkButton *w, struct tag *handle);
void on_bSetting_clicked(GtkButton *w, struct tag *handle);
void on_advCancel_clicked(GtkButton *w, struct tag *handle);
void on_advSave_clicked(GtkButton *w, struct tag *handle);
void on_window1_destroy(GtkWidget *w, gpointer handle);

#endif

