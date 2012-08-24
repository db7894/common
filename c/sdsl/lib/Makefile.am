#---------------------------------------------------------------------------# 
# Main program code listing
#---------------------------------------------------------------------------# 
AM_CPPFLAGS = -DLOCALEDIR=\"$(localedir)\"

noinst_LTLIBRARIES = libslib.la
libslib_la_SOURCES = filex.c filex.h		\
				 	 iox.c iox.h			\
				 	 language.c language.h	\
				 	 memoryx.c memoryx.h	\
					 timex.c timex.h		\
				 	 socketx.c socketx.h	\
				 	 sysinfo.c sysinfo.h	\
				 	 stringx.c stringx.h	\
				 	 common.h bitops.h		\
					 gettext.h


libslib_la_LIBADD =
libslib_la_CPPFLAGS = -Wall -I$(srcdir)/../include

#---------------------------------------------------------------------------# 
# Do we compile unit tests
#---------------------------------------------------------------------------# 
#noinst_PROGRAMS =
#if UNIT_TEST
#noinst_PROGRAMS += lib-test
#endif
#
#---------------------------------------------------------------------------# 
# Unit test code listing
#---------------------------------------------------------------------------# 
#lib_test_SOURCES = language.c language.h		\
#				   gui-logger.c gui-logger.h	\
#				   memory.c memory.h			\
#				   ftp-client.c ftp-client.h
#lib_test_CPPFLAGS = -DUNIT_TEST
#
#---------------------------------------------------------------------------# 
# Do we enable debug printing
#---------------------------------------------------------------------------# 
#if DEBUG
#lib_test_CPPFLAGS		+= -DDEBUG
#endif
#
