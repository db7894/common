#---------------------------------------------------------------------------# 
# Main program code listing
#---------------------------------------------------------------------------# 
AM_CPPFLAGS = -DLOCALEDIR=\"$(localedir)\"

bin_PROGRAMS = flasher
flasher_SOURCES = gui-settings.c gui-events.c	\
				   gui-helper.c gui-logger.c	\
				   ftp-client.c serial-client.c	\
				   main.c						\
				   gui-events.h gui-helper.h	\
				   gui-logger.h gui-settings.h	\
				   serial-client.h ftp-client.h
flasher_LDADD = $(LIBINTL) $(CURL_LIBS) $(GTK_LIBS) $(GLADE_LIBS) $(srcdir)/../lib/libslib.la
flasher_CPPFLAGS = -Wall $(GTK_CFLAGS) $(GLADE_CFLAGS) $(CURL_CFLAGS) -I$(srcdir)/../include

#---------------------------------------------------------------------------# 
# Do we compile unit tests
#---------------------------------------------------------------------------# 
noinst_PROGRAMS =
if UNIT_TEST
noinst_PROGRAMS += net-test serial-test
endif

#---------------------------------------------------------------------------# 
# Unit test code listing
#---------------------------------------------------------------------------# 
net_test_SOURCES = gui-logger.c gui-logger.h		\
				   ftp-client.c ftp-client.h
net_test_LDADD = $(LIBINTL) $(CURL_LIBS) $(GTK_LIBS) $(srcdir)../lib/libslib.la
net_test_CPPFLAGS = -DUNIT_TEST $(GTK_CFLAGS) $(CURL_CFLAGS) -I$(srcdir)/../include

serial_test_SOURCES = gui-logger.c gui-logger.h			\
					  serial-client.c serial-client.h
serial_test_LDADD = $(LIBINTL) $(GTK_LIBS) $(srcdir)/../lib/libslib.la
serial_test_CPPFLAGS = -DUNIT_TEST $(GTK_CFLAGS) -I$(srcdir)/../include

#---------------------------------------------------------------------------# 
# Do we enable debug printing
#---------------------------------------------------------------------------# 
if DEBUG
flasher_CPPFLAGS		+= -DDEBUG
net_test_CPPFLAGS		+= -DDEBUG
serial_test_CPPFLAGS	+= -DDEBUG
endif

#---------------------------------------------------------------------------# 
# Extra Data To Carry Along
#---------------------------------------------------------------------------# 
sharedir = $(pkgdatadir)/flasher
dist_share_DATA = ../share/flasher.glade ../share/hp-header.png ../share/hp-icon.png

