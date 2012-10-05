#---------------------------------------------------------------------------# 
# Initialize
#---------------------------------------------------------------------------# 
AC_INIT([common], [0.1], [bashwork@gmail.com])
AC_CONFIG_AUX_DIR([build-aux])
AM_INIT_AUTOMAKE([-Wall -Werror foreign])
AC_CONFIG_SRCDIR([src/main.c])
AM_GNU_GETTEXT_VERSION([0.16.1])
AM_GNU_GETTEXT([external])

#---------------------------------------------------------------------------# 
# Initialize Libtool
#---------------------------------------------------------------------------# 
AM_PROG_LIBTOOL

#---------------------------------------------------------------------------# 
# Parse arguments
#---------------------------------------------------------------------------# 
AC_ARG_WITH(ut,
	AC_HELP_STRING(
		[--with-ut],
		[Pass this flag to compile unit test code]
	),
	unit_test=1,
	unit_test=0
)
AM_CONDITIONAL([UNIT_TEST], [test "$unit_test" = 1])

AC_ARG_WITH(debug,
	AC_HELP_STRING(
		[--with-debug],
		[Pass this flag to compile debug code]
	),
	debug_test=1,
	debug_test=0
)
AM_CONDITIONAL([DEBUG], [test "$debug_test" = 1])

#AM_CONDITIONAL(CYGWIN, test "x$CYGWIN" = "xyes")

#---------------------------------------------------------------------------# 
# Check for programs
#---------------------------------------------------------------------------# 
AC_PROG_CC
AC_CHECK_PROGS([PKG_CONFIG], [pkg-config],[:])
if test "$PKG_CONFIG" = ":"; then
	  AC_MSG_ERROR([pkg-config not found, apt-get install pkg-config])
fi

#---------------------------------------------------------------------------# 
# Check for libraries
#---------------------------------------------------------------------------# 
AC_CHECK_LIB([pthread], [pthread_create],,
	AC_MSG_ERROR([pthread library not found]))

AC_CHECK_LIB([gthread-2.0], [g_thread_init],,
	AC_MSG_ERROR([gthread library not found]))

#---------------------------------------------------------------------------# 
# Check for packaged libraries
#---------------------------------------------------------------------------# 
PKG_CHECK_MODULES(XML, libxml-2.0 >= 2.0)

PKG_CHECK_MODULES(CURL, libcurl)
AC_SUBST(CURL_LIBS)
AC_SUBST(CURL_CFLAGS)

PKG_CHECK_MODULES(GTK, gtk+-2.0 >= 2.6.0)
AC_SUBST(GTK_LIBS)
AC_SUBST(GTK_CFLAGS)

PKG_CHECK_MODULES(GLADE, libglade-2.0 >= 2.6.0)
AC_SUBST(GLADE_LIBS)
AC_SUBST(GLADE_CFLAGS)

#---------------------------------------------------------------------------# 
# Check for headers
#---------------------------------------------------------------------------# 
AC_HEADER_STDC
AC_CHECK_HEADERS([errno.h stdlib.h stdio.h string.h time.h])
AC_CHECK_HEADERS([unistd.h getopt.h strings.h ctype.h fcntl.h])
AC_CHECK_HEADERS([socket.h netdb.h dirent.h termios.h])
AC_CHECK_HEADERS([sys/stat.h sys/time.h dirent.h])
AC_CHECK_HEADERS([curl/curl.h],,AC_MSG_ERROR([unable to find libcurl header files]))

#---------------------------------------------------------------------------# 
# Check for library functions
#---------------------------------------------------------------------------# 
AC_FUNC_MALLOC
AC_CHECK_FUNCS([strcpy strlen strchr strstr usleep sleep])
AC_CHECK_FUNCS([realloc calloc memset])

#---------------------------------------------------------------------------# 
# Check for types, structures, and compiler characteristics
#---------------------------------------------------------------------------# 
AC_TYPE_SIZE_T
AC_C_CONST
AC_CHECK_TYPES([FILE])
AC_CHECK_MEMBERS([struct stat.st_size])

#---------------------------------------------------------------------------# 
# Output results
#---------------------------------------------------------------------------# 
AC_CONFIG_HEADERS([config.h])
AC_CONFIG_FILES([
	Makefile
   	gtk/Makefile
   	lib/Makefile
   	rfc/Makefile
   	sdsl/Makefile
	doc/Makefile
   	po/Makefile.in
])
AC_OUTPUT
