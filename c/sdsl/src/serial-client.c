/**
 * @file serial-client.c
 * @brief A quick serial upload function
 * @author Galen Collins
 *
 * Compile With:
 * gcc -DUNIT_TEST -o client serial-client.c (unit test)
 * gcc serial-client.c (object)
 */

/* system includes */
#include "common.h"
#include <termios.h>
#include <pthread.h>

/* local includes */
#include "gui-logger.h"
#include "serial-client.h"

/**
 * @brief Sets up the serial port for uploading to
 * @param port Serial port to setup loading to
 * @return -1 if failure, serial fd if successful
 */
static int serial_init(char *port)
{
	int fd;
	struct termios term;

	fd = open(port, (O_RDWR | O_NOCTTY | O_NONBLOCK)); 
	if (fd < 0)
		return -1;

	/* initialize term settings */
	tcgetattr(fd, &term);

	term.c_lflag		&= ~(ICANON | ECHO | ECHOE | ISIG); /* no input processing */
	term.c_oflag		&= ~OPOST;							/* no output processing */
	term.c_cflag		&= ~PARENB;							/* disable parity */
	term.c_cflag		&= ~CSTOPB;							/* 1 stop bit(s) */
	term.c_cflag		&= ~CSIZE;							/* clear character size */
	term.c_cflag		&= ~CRTSCTS;						/* disable hardware flow control */
	term.c_cflag		|= CS8;								/* set character size */
	term.c_cflag		|= CREAD | CLOCAL;					/* turn on READ & ignore ctrl lines */
	//term.c_iflag		&= ~(IXON | IXOFF | IXANY);			/* disable software flow control */	
	term.c_iflag		 = 0x1;
	term.c_cc[VTIME]	 = 0;								/* don't hold for time out */
	term.c_cc[VMIN]		 = 0;								/* don't block */
	cfsetispeed(&term, B115200);							/* set input baud */
	cfsetospeed(&term, B115200);							/* set output baud */
	tcsetattr(fd, TCSANOW, &term);

	return fd;
}

/**
 * @brief Waits for a string to appear to continue
 * @param handle The serial socket handle
 * @param input The string to wait for
 * @return -1 if failure, 0 if successful
 */
static int serial_wait(int handle, char *input)
{
	int size, index = 0, count = 0;
	static char buffer[1000];	/* for the big post message */

	memset(buffer, 0, sizeof(buffer));
	do {
		size = read(handle, (void *)&buffer[index], 64);
		if (size > 0) {
			gui_update_log(".");
			count = 0;
			index += size;
			if (strstr(buffer, input))
				return 0;
		}
		sleep(1);
	} while (count++ < 15);

	dprintf("[%s]", buffer);

	return -1;
}

/**
 * @brief Issues the reset command and waits for result
 * @param handle The serial socket handle
 * @return -1 if failure, 0 if successful
 */
static int serial_cmd_reset(int handle)
{
	static char *skip = "\n";
	static char *cmd  = "reset\n";


	write(handle, skip, strlen(skip));	/* to clear old command */
	usleep(500000);						/* allow serial to catch up */
	write(handle, cmd,	strlen(cmd));	/* to issue reset */
	sleep(5);							/* allow boot menu to run */

	if (serial_wait(handle, "the Service menu"))
		return -1;
	sleep(6);							/* wait for menu to time out */

	return 0;
}

/**
 * @brief Pre upload testing stage
 * @param handle The serial socket handle
 * @return -1 if failure, 0 if successful
 */
static int serial_cmd_pre(int handle)
{
	char *cmd = "UpLoad1\n";

	write(handle, cmd,	sizeof(cmd));
	if (serial_wait(handle, ">"))
		return -1;
	return 0;
}

/**
 * @brief Post upload testing stage
 * @param handle The serial socket handle
 * @return -1 if failure, 0 if successful
 */
static int serial_cmd_post(int handle)
{
	int result = -1;

	/* wait for upload to finish */
	if (!serial_wait(handle, ">")) {
		sleep(5);	/* give it some time to copy */
		result = serial_wait(handle, "reset");
	}

	return result;
}

/**
 * @brief Reads an entire file into memory
 * @param file The filename to read
 * @return The file pointer if successful, NULL if failure
 */
static int read_file(char *file, char **handle)
{
	FILE *input = fopen(file, "rb");
	struct stat info;

	/* test if file exists */
	if (!input) {
		info.st_size = -1;
		goto sclient_cleanup;
	}

	/* get the file size of the local file */
	if (stat(file, &info)) {
		info.st_size = -1;
		goto sclient_cleanup;
	}

	/* allocate memory of the file */
	*handle = (char *)xcalloc(info.st_size, sizeof(char));
	fread(*handle, sizeof(char), info.st_size, input);

sclient_cleanup:
	fclose(input);
	return info.st_size;
}

/**
 * @brief Uploads the specified file with the passed in settings
 * @param input Settings to control the upload
 * @return void
 *
 * @code
 *  struct serial_upload *settings;
 *  pthread_t thread;
 *  
 *  settings = (struct serial_upload *)xmalloc(sizeof(struct serial_upload));
 *  strcpy(settings->port, "/dev/ttyS1");
 *  strcpy(settings->file, "./image.bin");
 *  pthread_create(&thread, NULL, serial_upload, (void *)settings);
 *  pthread_exit(NULL);
 * @endcode
 */
void *serial_upload(void *input)
{
	int result = -1;
	int size, index = 0;
	char *buffer = NULL, error[250];
	struct serial_upload *settings = (struct serial_upload *)input;
	int handle = serial_init(settings->port);

	dprintf("P[%s]\nF[%s]\n", settings->port, settings->file);

	gui_update_log(_("Initializing\n"));
	size = read_file(settings->file, &buffer);
	if (!buffer || (size <= 0)) {
		sprintf(error, _("File [%s] Does Not Exist\n"), settings->file);
		gui_error_log(error);
		goto clean_up_serial;
	}

	gui_update_log(_("Resetting the card..."));
	if (serial_cmd_reset(handle))
		goto clean_up_serial;
	gui_update_log(_("Done!\n"));

	if (serial_cmd_pre(handle))
		goto clean_up_serial;

	sprintf(error, _("Sending Firmware [%d bytes]..."), size);
	gui_update_log(error);
	do {
		result = write(handle, (void *)&buffer[index], size);
		if (result > 0) {
			size  -= result;
			index += result;
		}
	} while (size > 0);
	gui_update_log(_("Done!\n"));

	gui_update_log(_("Testing Result..."));
	if (serial_cmd_post(handle)) {
		result = -1;
		goto clean_up_serial;
	}
	result = 0;
	gui_update_log(_("Upload Successful\n"));

clean_up_serial:
	if (handle > 0) close(handle);
	if (!buffer)	free(buffer);
	free(input);
	if (result)
		gui_error_log(_("Upload Failed\n"));
#ifndef UNIT_TEST
	extern int stop_progress_bar;
	stop_progress_bar = 1;
#endif
	pthread_exit(NULL);
}

#ifdef UNIT_TEST

int main(void)
{
	struct serial_upload *settings;
	pthread_t thread;

	settings = (struct serial_upload *)xmalloc(sizeof(struct serial_upload));
	strcpy(settings->port, "/dev/ttyS1");
	strcpy(settings->file, "./image.bin");
	pthread_create(&thread, NULL, serial_upload, (void *)settings);
	pthread_exit(NULL);
}

#endif

