/**
 * @file timex.c
 * @brief Safe wrappers for time functions
 */
#include "common.h"
#include <time.h>
#include <sys/time.h>

/**
 * @brief Creates a timer alarm to go off
 * @param secs The number of seconds to delay
 * @return 0 if successful, -1 if failure
 *
 * You need to do the following before calling this:
 * @code
 * volatile sig_atomic_t keep_going = 1;
 * signal(SIGALRM, catch_alarm);
 *
 * void catch_alarm (int sig)
 * {
 *   keep_going = 0; 
 *   signal(sig, catch_alarm);
 * }
 * @endcode
 */
int time_alarm(unsigned int secs)
{
	struct itimerval old, new;

	new.it_interval.tv_usec	= 0;
	new.it_interval.tv_sec	= 0;
	new.it_value.tv_usec	= 0;
	new.it_value.tv_sec		= (long int)secs;

	if (setitimer(ITIMER_REAL, &new, &old))
		return -1;
	return 0;
}

/**
 * @brief Creates a timer alarm to go off
 * @param secs The number of seconds to delay
 * @return 0 if successful, -1 if failure
 */
void time_sleep(unsigned int secs, unsigned int nsecs)
{
	struct timespec new, left;

	if (secs > 0) {
		time_t end = time(NULL) + (time_t)secs;
		do {
			sleep(secs);
		} while (time(NULL) < end);
	}

	if (nsecs > 0) {
		left.tv_sec = 0;
		left.tv_nsec = nsecs;
		do { 
			new.tv_sec	= 0;
			new.tv_nsec	= left.tv_nsec;
			left.tv_nsec = nanosleep(&new, &left);
		} while (left.tv_nsec != 0);
	}
}
