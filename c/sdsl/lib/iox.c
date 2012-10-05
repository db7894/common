/**
 * @file iox.c
 * @brief Wrappers for casual io operations
 */
#include "common.h"
#include "iox.h"
#include <signal.h>
#include <sys/wait.h>	/* for waitpid */

//---------------------------------------------------------------------------// 
// Pipes / FD clones
//---------------------------------------------------------------------------// 

/**
 * @brief Creates a pipe between two file descriptors
 * @param file An array of two fds to pipe together
 * @return 0 if successful, -1 if failure
 */
int io_pipe(int file[])
{
	if (pipe(file)) {
		perror("pipe");
		return -1;
	}
	return 0;
}

/**
 * @brief Creates a pipe between two file descriptors
 * @param file An array of two fds to pipe together
 * @return 0 if successful, -1 if failure
 */
int io_fifo(const char *file, int mode)
{
	int fd;

	if ((fd = mkfifo(file, (mode_t)mode)) == -1) {
		perror("mkfifo");
		return -1;
	}
	return fd;
}

//---------------------------------------------------------------------------// 
// Processes
//---------------------------------------------------------------------------// 

/**
 * @brief Quick wrapper to fork a function
 * @param func Fork child callback
 * @param arg Argument pointer to pass to child
 * @return 0 if successful, -1 if failure
 */
int proc_fork(void (*func)(void *), void *arg)
{
	int status = 0;
	pid_t pid;

	pid = fork();
	if (pid == 0) {
		func(arg);
		exit(1);
	}
	else if (pid < 0) {
		status = -1;
	}

	return status;
}

/**
 * @brief Quick wrapper to fork a function and wait for result
 * @param func Fork child callback
 * @param arg Argument pointer to pass to child
 * @return 0 if successful, -1 if failure
 */
int proc_fork_wait(void (*func)(void *), void *arg)
{
	int status = -1;
	pid_t pid;

	pid = fork();
	if (pid == 0) {
		func(arg);
		exit(1);
	}
	else if (pid < 0) {
		status = -1;
	}
	if (waitpid(pid, &status, 0) != pid)
		status = -1;

	return status;
}

//---------------------------------------------------------------------------// 
// Signals
//---------------------------------------------------------------------------// 
/**
 * @brief A quick function to raise a signal locally
 * @param sig Signal to raise
 * @return 0 if successful, -1 if failure
 */
int signal_raise(int sig)
{
	if (raise(sig)) {
		perror("raise");
		return -1;
	}
	return 0;
}

/**
 * @brief A quick function to send a signal to another program
 * @param pid Process to send the signal to
 * @param sig Signal to raise
 * @return 0 if successful, -1 if failure
 */
int signal_send(int pid, int sig)
{
	if (kill((pid_t)pid, sig)) {
		perror("raise");
		return -1;
	}
	return 0;
}

/**
 * @brief A quick function to initialize an  array of signals
 * @param signals Handle to signals and their handlers
 * @return 0 if successful, -1 if failure
 */
int signal_setup(struct signals *sig)
{
	int i, result = 0;

	for (i = 0; sig[i].func != NULL; i++) {
		if (signal(sig[i].sig, sig[i].func) == SIG_IGN) {
			if (signal(sig[i].sig, SIG_IGN) == SIG_ERR)
				result = -1;
		}
	}
	return result;
}

//---------------------------------------------------------------------------// 
// Shell Execute
//---------------------------------------------------------------------------// 

/**
 * @brief Wrapper for running a shell command and reading resulting data
 * @param command Command to execute
 * @param data Handle for resulting data
 * @return exit value of command
 * @note The data must be freed after it is used
 */
int io_rshell(const char *command, char **data)
{
	char *h;
	size_t size = 0, cur = 256;
	FILE *file;

	if ((file = popen(command, "r")) == NULL) {
		perror("popen");
		return -1;
	}

	h = xmalloc(cur);
	do {
		size += fread((void *)&h[size], sizeof(char), cur - size - 1, file);
		if (size >= (cur - 1)) {
			cur *= 2;
			h = xrealloc(h, cur);
		}
	} while(!feof(file));
	*data = h;

	return pclose(file);
}

/**
 * @brief Wrapper for running a shell command and write data to command
 * @param command Command to execute
 * @param data Extra data to write to opened command
 * @return exit value of command
 */
int io_wshell(const char *command, const char *data[])
{
	int i;
	FILE *file = popen(command, "w");

	if (file == NULL) {
		perror("popen");
		return -1;
	}
	for (i = 0; data[i]; i++) {
		fprintf(file, "%s", data[i]);
	}	
	return pclose(file);
}

