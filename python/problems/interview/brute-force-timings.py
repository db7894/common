import math
import timeit

def brute_force_timer(base):
    ''' Given a base, see how long a brute force solution
    will take for a given power of that base::

        1 year:      ~ 10^12 ~= 2^40
        10 years:    ~ 10^13 ~= 2^43
        100 years:   ~ 10^14 ~= 2^46

    :param base: The base to time brute force solutions for
    '''
    limit = int(math.log(1e10) / math.log(base))
    for exponent in range(1, limit+1):
        count = 0
        command = 'for i in range(%d**%d): count = i*i' % (base, exponent)
        seconds = timeit.timeit(command, number = 1)
        print('Loop %d^%d in %.2fs' % (base, exponent, seconds))

print('-' * 10)
brute_force_timer(2)
print('-' * 10)
brute_force_timer(10)
print('-' * 10)
