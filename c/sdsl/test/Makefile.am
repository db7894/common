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
noinst_PROGRAMS = test
test_SOURCES = test_sdsl.c

test_LDADD = $(srcdir)/../lib/libslib.la $(srcdir)/../sdsl/libsdsl.la
test_CPPFLAGS = -Wall -I$(srcdir)/../include/ -I$(srcdir)/../sdsl/ -ggdb

#---------------------------------------------------------------------------# 
# Do we enable debug printing
#---------------------------------------------------------------------------# 
#if DEBUG
#test_CPPFLAGS		+= -DDEBUG
#endif

