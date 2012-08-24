/**
 * @file iox.h
 * @brief Wrappers for casual io operations
 */
#ifndef SAFE_PIPE
#define SAFE_PIPE

//---------------------------------------------------------------------------// 
// Types
//---------------------------------------------------------------------------// 
typedef void (*fptr)(void *);	/**< Generic function pointer */

/**
 * @brief Used as a handle for signal_setup
 */
struct signals {
	int sig;
	void (*func)(int sig);
};

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------// 

int io_pipe(int file[]);
int io_fifo(const char *file, int mode);

int proc_fork(void (*func)(void *), void *arg);
int proc_fork_wait(void (*func)(void *), void *arg);

int signal_raise(int sig);
int signal_send(int pid, int sig);
int signal_setup(struct signals[]);

int io_rshell(const char *command, char **data);
int io_wshell(const char *command, const char *data[]);

#endif

