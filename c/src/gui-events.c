/**
 * @file gui-events.c
 * @brief Callback functions for gui actions
 * @author Galen Collins
 */
#include "common.h"
#include <pthread.h>
#include <glade/glade.h>

#include "gui-events.h"
#include "gui-helper.h"
#include "gui-settings.h"
#include "gui-logger.h"

#include "ftp-client.h"
#include "serial-client.h"

/**
 * @brief A quick hack to start and stop the progress bar
 * @todo I should probobly use a condition flag
 */
volatile int stop_progress_bar = 1;

/**
 * @brief A wrapper for starting the serial/ftp upload
 * @param handle Handle to the application data
 * @return void
 */
static void *loader_progress(void *input) 
{
	GtkWidget *widget;
	struct tag *handle = (struct tag *)input;

   	widget = glade_xml_get_widget(handle->glade, "progress1");
	
	stop_progress_bar = 0;
	gdk_threads_enter();
		gtk_progress_bar_set_pulse_step(GTK_PROGRESS_BAR(widget), 0.1);
	gdk_threads_leave();

	while (!stop_progress_bar) {
		gdk_threads_enter();
			gtk_progress_bar_pulse(GTK_PROGRESS_BAR(widget));
		gdk_threads_leave();
		usleep(100000);
	}
	gdk_threads_enter();
		gtk_progress_bar_set_fraction(GTK_PROGRESS_BAR(widget), 0.0);
	gdk_threads_leave();
	pthread_exit(0);
}

/**
 * @brief A wrapper for starting the serial/ftp upload
 * @param handle Handle to the application data
 * @return void
 */
void loader_launcher(struct tag *handle) 
{
	pthread_t thread;
	static gchar *names[3] = {"rom.bin", "image.bin", "config.bin"};
	struct ftp_upload *nflags;
	struct serial_upload *sflags;
	int start = 1;

	/* test upload type */
	if (   (handle->method == SERIAL_UPLOAD)
		&& (handle->type != FIRMWARE)) {
		gui_error_log(_("Can only upgrade firmware over serial\n"));
		start = 0;
	}

	/* test upload port */
	if (   (handle->method == SERIAL_UPLOAD)
		&& (handle->port < 0 || handle->port > MAX_SERIAL_PORTS)) {
		handle->port = 0;
	}

	/* test network information */
	if (   (handle->method == NETWORK_UPLOAD)
		&& (handle->user == NULL || handle->pass == NULL || handle->host == NULL)) {
		gui_error_log(_("Missing information for selected method\n"));
		start = 0;
	}

	/* test file type */
	if (strstr(handle->file, names[handle->type]) == NULL) {
		gui_error_log(_("Incorrect file type for selected method\n"));
		start = 0;
	}

	/* if all the tests passed, perform upgrade */
	if (start) {
		if (handle->method == NETWORK_UPLOAD) {
			nflags = (struct ftp_upload *)xmalloc(sizeof(struct ftp_upload));

			strncpy(nflags->host, handle->host, 50);
			strncpy(nflags->user, handle->user, 25);
			strncpy(nflags->pass, handle->pass, 25);
			strncpy(nflags->file, handle->file, 100);

			pthread_create(&thread, NULL, ftp_upload, (void *)nflags);
		}
		else if (handle->method == SERIAL_UPLOAD) {
			sflags = (struct serial_upload *)xmalloc(sizeof(struct serial_upload));

			sprintf(sflags->port, "%s%d",  SERIAL_DEVICE_BASE, handle->port);
			strncpy(sflags->file, handle->file, 100);

			pthread_create(&thread, NULL, serial_upload, (void *)sflags);
		}
	}
	else {
		stop_progress_bar = 1;
	}
}

/**
 * @brief Handler for the upload method drop-down
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_cMethod_changed(GtkComboBox *w, struct tag *handle)
{
	gint result;

	result = gtk_combo_box_get_active(w);

	if (result >= 0)
		handle->method = result;
}

/**
 * @brief Handler for the filename selector
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_cFile_changed(GtkFileChooser *w, struct tag *handle)
{
	gchar *result;

	result = gtk_file_chooser_get_filename(w);

	if (result)
		handle->file = result;
}

/**
 * @brief Handler for the upload type drop-down
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_cType_changed(GtkComboBox *w, struct tag *handle)
{
	gint result;

	result = gtk_combo_box_get_active(w);

	if (result >= 0)
		handle->type = result;
}

/**
 * @brief Handler for the serial port drop-down
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_cPort_changed(GtkComboBox *w, struct tag *handle)
{
	gint result;

	result  = gtk_combo_box_get_active(w);

	if (result >= 0)
		handle->port = result;
}

/**
 * @brief Handler for the language drop-down
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_cLang_changed(GtkComboBox *w, struct tag *handle)
{
	gint result;

	result  = gtk_combo_box_get_active(w);

	if (result >= 0) {
		language_set(result);
		gui_set_lang(handle);
	}
}

/**
 * @brief Handler for the help button
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_bHelp_clicked(GtkButton *w, struct tag *handle)
{
	GtkWidget *window = glade_xml_get_widget(handle->glade, "window1");

	gui_about_prompt(window);
}

/**
 * @brief Handler for apply button(flash card)
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_bFlash_clicked(GtkButton *w, struct tag *handle)
{
	pthread_t thread;

	pthread_create(&thread, NULL, loader_progress, (void *)handle);
	loader_launcher(handle);
}

/**
 * @brief Handler for the close button
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_bClose_clicked(GtkButton *w, struct tag *handle)
{
	gtk_main_quit();
}

/**
 * @brief Handler for the settings button
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_bSetting_clicked(GtkButton *w, struct tag *handle)
{
	GtkWidget *window = glade_xml_get_widget(handle->glade, "advanced");

	gtk_widget_show(window);
}

/**
 * @brief Handler for the advanced->close button
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_advCancel_clicked(GtkButton *w, struct tag *handle)
{
	GtkWidget *window = glade_xml_get_widget(handle->glade, "advanced");

	gtk_widget_hide(window);
}

/**
 * @brief Handler for the advanced->save button
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_advSave_clicked(GtkButton *w, struct tag *handle)
{
	GtkWidget *widget;
	gint result;

   	widget = glade_xml_get_widget(handle->glade, "cPort");
	result = gtk_combo_box_get_active(GTK_COMBO_BOX(widget));

	if (result >= 0)
		handle->port = result;

   	widget = glade_xml_get_widget(handle->glade, "tHost");
	handle->host = (gchar *)gtk_entry_get_text(GTK_ENTRY(widget));

   	widget = glade_xml_get_widget(handle->glade, "tPass");
	handle->pass = (gchar *)gtk_entry_get_text(GTK_ENTRY(widget));

   	widget = glade_xml_get_widget(handle->glade, "tUser");
	handle->user = (gchar *)gtk_entry_get_text(GTK_ENTRY(widget));

   	widget = glade_xml_get_widget(handle->glade, "advanced");
	gtk_widget_hide(widget);
}

/**
 * @brief The main handler for the window close
 * @param w The main window handle
 * @param handle Handle to the application data
 * @return void
 */
void on_window1_destroy(GtkWidget *w, gpointer handle)
{
	gtk_main_quit();
}

