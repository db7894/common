#---------------------------------------------------------------------------# 
# Main program code listing
#---------------------------------------------------------------------------# 
noinst_LTLIBRARIES = libcsdl.la
libcsdl_la_SOURCES = sll.c sll.h	\
					 dll.c dll.h	\
					 cll.c cll.h	\
					 stack.c stack.h

libcsdl_la_LIBADD = $(srcdir)/../lib/libslib.la
libcsdl_la_CPPFLAGS = -Wall -I$(srcdir)/../include/

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
#noinst_PROGRAMS = test
#test_SOURCES = test.c
#
#test_LDADD = $(srcdir)/../lib/libslib.la $(srcdir)/../csdl/libcsdl.la
#test_CPPFLAGS = -Wall -I$(srcdir)/../include/
#---------------------------------------------------------------------------# 
# Do we enable debug printing
#---------------------------------------------------------------------------# 
#if DEBUG
#lib_test_CPPFLAGS		+= -DDEBUG
#endif
#
