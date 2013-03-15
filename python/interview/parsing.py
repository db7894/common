from collections import defaultdict
from collections import namedtuple
import sys

def generate_entries(log, keys, formats=None):
    ''' A helper generator to process a log file
    '''
    formats = formats or [str for i in range(len(keys))]
    Entry = namedtuple('Entry', keys)
    for line in log:
        values = line.split(',')
        values = (f(v) for f,v in zip(formats, values))
        yield Entry._make(values)

def max_service_instances(log):
    ''' Given the following log file format, find the maximum
    concurrent service instances::
        
        start_time,end_time,user_id,instances
    '''
    users = defaultdict(int) # range -> count
    keys  = ['start', 'end', 'id', 'size']
    fmts  = [int, int, str, int]
    for entry in generate_entries(log, keys, fmts):
        pass

def get_most_active_user():
    ''' Given the following log file format, find the user
    with the most concurrent service instances::
        
        start_time,end_time,user_id,instances
    '''
    users = defaultdict(dict) # user -> { range -> count }
    keys  = ['start', 'end', 'id', 'size']
    fmts  = [int, int, str, int]
    for entry in generate_entries(log, keys, fmts):
        pass

def max_concurrent_users(log):
    ''' Given the following log file format, find the maximum
    concurrent logged on users::
        
        time,user_id,<logon|logoff>

    First make it work locally, then distributedly
    '''
    total = 0
    users = set()
    keys  = ['time', 'user', 'is_logon']
    fmts  = [int, str, lambda s: s.lower() == 'logon']
    for entry in generate_entries(log, keys, fmts):
        if entry.is_logon and entry.user not in users:
            users.add(entry.user)
            total = max(total, len(users))
        else: users.remove(entry.user)
    return total

def get_users_at_time(log, query):
    ''' Given the following log file format, find the number
    of users logged on at the requested time (or range)::
        
        login_time,logoff_time,user_id

    Here we assume that the times will be converted to the
    required resolution (in this case seconds).
    '''
    import numpy as np
    times = np.ndarray(60 * 60 * 24, int)
    keys  = ['login', 'logoff', 'user']
    fmts  = [int, int, str]
    for entry in generate_entries(log, keys, fmts):
        times[entry.login:entry.logoff] += 1

    if isinstance(query, tuple): # range query
        return times[query[0]:query[1]].sum()
    return times[query]

def generate_log(count):
    ''' Generate a log file with the specified
    number of entries
    '''
    from random import randint
    for i in range(count):
        s = randint(0, 86400)
        e = randint(0, 86400)
        s,e = min(s,e), max(s, e)
        i = randint(0, 1000)
        yield "{},{},{}".format(s, e, i)
