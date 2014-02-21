import math
import time

class IncrementCounter(object):
    ''' Given a clock with second precision, record the number of 
    times that increment has been called in the past second, minute,
    hour, and day (say as an interrupted handler).
    '''

    def __init__(self):
        self.update = self.__time()
        self.counts = [0] *  17 # 2 ** 17
        # get a binary counter to determine when to shift

    def __time(self):
        ''' This is a simple wrapper around time to return
        an integer instead of a floating point number
        '''
        return int(time.time())

    def increment(self):
        ''' The general idea is that we will shift the counts forward
        by the power of two number of seconds that have elapsed between
        consecutive updates. If we are currently in the same second, we
        will simply update the current count.
        '''
        now  = self.__time()
        diff = now - self.update
        if diff != 0:
            shift = int(math.log(diff, 2) + 0.5)
            shifts = self.counts[0:len(self.counts) - shift]
            shifts = [1] + [0] * (shift - 1) + shifts
        else: self.counts[0] += 1
        self.update = now

    def in_past_second(self):
        ''' sum(2**i for i in range(0)) == 1 '''
        return self.counts[0]
   
    def in_past_minute(self):
        ''' sum(2**i for i in range(7)) == 63 '''
        return sum(self.counts[i] for i in range(7))

    def in_past_hour(self):
        ''' sum(2**i for i in range(12)) == 4095 '''
        return sum(self.counts[i] for i in range(12))

    def in_past_day(self):
        ''' sum(2**i for i in range(17)) == 131071 '''
        return sum(self.counts[i] for i in range(17))

if __name__ == "__main__":
    c = IncrementCounter()
    for i in range(60): c.increment()
    print "called inc(60) for second: %d" % c.in_past_second()
    for i in range(60): c.increment(); time.sleep(1)
    print "called inc(60) for minute: %d" % c.in_past_minute()
