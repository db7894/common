/**
 * @file timex.c
 * @brief Safe wrappers for time functions
 */
#ifndef SAFE_TIME
#define SAFE_TIME

//---------------------------------------------------------------------------// 
// Function Prototypes
//---------------------------------------------------------------------------// 
int time_alarm(unsigned int secs);
void time_sleep(unsigned int secs, unsigned int nsecs);

#endif

