/**
 * @file main.c
 * @brief Portable flash application
 *
 * This file is part of the tutorial: Linux GUI Programming with GTK+ and Glade3
 * http://www.micahcarrick.com/12-24-2007/gtk-glade-tutorial-part-1.html
 *
 * Compile using:
 * gcc -Wall -g -o tutorial main.c -export-dynamic `pkg-config gtk+-2.0 libglade-2.0 --cflags --libs`      
*/

/* system includes */
#include "common.h"
#include <pthread.h>
#include <gtk/gtk.h>
#include <glade/glade.h>
#include <glib/gthread.h>

/* local includes */
#include "gui-helper.h"
#include "gui-events.h"
#include "gui-logger.h"
#include "gui-settings.h"
#include "ftp-client.h"
#include "serial-client.h"

/**
 * @brief Sets the silent (non-gui) status
 */
extern int SILENT;

/**
 * @brief Function to link all the signals with their callbacks
 * @param builder Handle to glade xml file
 * @param handle Handle to the application settings
 * @return 0 if successful, -1 if failure
 */
static int gui_signal_connect(GladeXML *builder, struct tag *handle)
{
	glade_xml_signal_connect_data(builder,
		"on_bHelp_clicked",
		(GtkSignalFunc)on_bHelp_clicked,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_cMethod_changed",
	   	(GtkSignalFunc)on_cMethod_changed,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_cFile_changed",
	   	(GtkSignalFunc)on_cFile_changed,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_cType_changed",
	  	(GtkSignalFunc)on_cType_changed,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_cPort_changed",
	   	(GtkSignalFunc)on_cPort_changed,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_cLang_changed",
	   	(GtkSignalFunc)on_cLang_changed,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_bHelp_clicked",
	   	(GtkSignalFunc)on_bHelp_clicked,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_bFlash_clicked",
	   	(GtkSignalFunc)on_bFlash_clicked,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_bClose_clicked",
	   	(GtkSignalFunc)on_bClose_clicked,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_bSetting_clicked",
	   	(GtkSignalFunc)on_bSetting_clicked,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_advCancel_clicked",
	   	(GtkSignalFunc)on_advCancel_clicked,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_advSave_clicked",
	   	(GtkSignalFunc)on_advSave_clicked,
		handle);
	glade_xml_signal_connect_data(builder,
		"on_window1_destroy",
	   	(GtkSignalFunc)on_window1_destroy,
		handle);

	return 0;
}

/**
 * @brief Main initializer for the gui
 * @param handle Handle to the application settings
 * @return 0 if successful, -1 if failure
 */
static int gui_main_init(struct tag *handle)
{
	GladeXML *xml;
	GtkWidget *window;
	
	/* use GtkBuilder to build our interface from the XML file */
	xml = glade_xml_new(GLADE_XML_FILE, NULL, NULL);
	if (!xml) {
		gui_error_prompt("Failed To Load Glade XML File");
		return -1;
	}
	
	handle->glade	= xml;
	handle->type	= FIRMWARE;
	handle->method	= SERIAL_UPLOAD;
	handle->port	= PORT_2;

	gui_signal_connect(xml, handle);/* connect our signals */
	window = glade_xml_get_widget(handle->glade, "textview1");
	gui_init_log(GTK_TEXT_VIEW(window));
	window = glade_xml_get_widget(handle->glade, "window1");
	gtk_widget_show(window);			/* show the main window */
	gui_set_lang(handle);				/* set language */
	gdk_threads_enter();
	gtk_main();							/* enter GTK+ main loop */                   
	gdk_threads_leave();
	                                                        
	return 0;
}

/**
 * @brief Quick help print wrapper
 * @param argv Arguments passed on the command line
 * @return void
 */
static void main_help(char *argv[])
{
	printf("Usage: %s [options]\n\n"										\
		   "Options:\n"														\
		   "   -h          Show this help message and exit\n"				\
		   "   -m METHOD   Upgrade method (s)erial|(n)etwork)\n"			\
		   "   -t type     File-type (f)irmware, (b)ootloader, (c)config\n"	\
		   "   -f FILE     File to load (image.bin|rom.bin|config.bin)\n"	\
		   "   -c HOST     Card for network upgrade\n"						\
		   "   -d PORT     Port number to transfer to\n"					\
		   "   -u USER     Username for network upgrade\n"					\
		   "   -p PASS     Password for network upgrade\n"					\
		   "   -s          Perform a silent (no-gui) upgrade\n",
	argv[0]);
}

/**
 * @brief Main entry point of the program
 * @param argc Number of command line arguments
 * @param argv Arguments passed on the command line
 * @return 0 if successful, 1 if failure
 */
int main(int argc, char *argv[])
{
	struct tag *handle;
	int c, i, result = 1;;

	language_init();					/* for i18n console */
	handle = g_slice_new(struct tag);	/* initialize structure */
	SILENT = 0;							/* default to gui */

	/* parse command line options */
	while ((c = getopt(argc, argv, "hsm:t:f:c:d:u:p:")) != -1) {
		switch (c) {
			case 'm':
				     if (optarg[0] == 's')	handle->method = SERIAL_UPLOAD;
				else if (optarg[0] == 'n')	handle->method = NETWORK_UPLOAD;
				else {
					main_help(argv);
					goto main_exit;
				}
				break;

			case 't':
				     if (optarg[0] == 'b')	handle->type = BOOTLOADER;
				else if (optarg[0] == 'f')	handle->type = FIRMWARE;
				else if (optarg[0] == 'c')	handle->type = CONFIGURATION;
				else {
					main_help(argv);
					goto main_exit;
				}
				break;

			case 'd':
				if ( (i = atoi(optarg)) >= 0)
					handle->port = i;
				break;

			case 's': SILENT = 1; break;
			case 'f': handle->file = optarg; break;
			case 'c': handle->host = optarg; break;
			case 'u': handle->user = optarg; break;
			case 'p': handle->pass = optarg; break;

			/* default is help */
			default:
			case 'h':
			case '?':
				main_help(argv);
				goto main_exit;
		}
	}

#if 0
	/* parse remaining arguments */
	for (i = optind; i < argc; i++) {
		printf("Other %s\n", argv[i]);
	}
#endif

	/* perform gui upgrade */
	if (!SILENT) {
		g_thread_init(NULL);
		gdk_threads_init();
		gtk_init(&argc, &argv);				/* initialize GTK+ libraries */

		if (gui_main_init(handle))			/* load the application gui */
			goto main_exit;
	}
	/* perform console upgrade */
	else {
		loader_launcher(handle);
		pthread_exit(NULL);
	}
	result = 0;								/* good upload if we got here */

main_exit:
	g_slice_free(struct tag, handle);		/* free allocated memory */
        
	return result;
}

