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
    import numpy as np
    times = np.ndarray(60 * 60 * 24, int)
    keys  = ['startup', 'shutdown', 'id', 'size']
    fmts  = [int, int, str, int]
    for entry in generate_entries(log, keys, fmts):
        times[entry.startup:entry.shutdown] += entry.size
    return times.max()


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
    return times[query]


def get_users_at_time_II(log, query):
    ''' Given the following log file format, find the number
    of users logged on at the requested time (or range)::
        
        login_time,logoff_time,user_id

    Here we assume that the times will be stored as HH:MM:SS
    or simply as a list of [hh, mm, ss]

    TODO this needs a hueristic to decide which way to go:

    * 12:00:00 - 12:30:00 (increase minutes)
    * 12:59:59 -  1:00:01  (increase seconds)
    * 12:00:01 -  1:59:59  (increase hours)
    '''
    Node = namedtuple('Node', ['value', 'childs'])
    fact = lambda: node(0, defaultdict(fact))
    root = Node(0, defaultdict(fact))

    keys  = ['login', 'logoff', 'user']
    fmts  = [int, int, str]
    for entry in generate_entries(log, keys, fmts):
        # update hour counts
        hourr, hourl = entry.logoff[0], entry.login[0]
        hourd = hourr - hourl
        if hourd == 24: root.value += 1
        elif hourd > 0:
            for i in range(hourl, hourr + 1):
                root.childs[i].value += 1
        else: root.childs[hourl].value += 1

        # reduce minute counts
        minr, minl = entry.logoff[1], entry.login[1]
        for i in range(0, minl):
            root.childs[hourl].childs[i].value -= 1
        for i in range(minr, 60):
            root.childs[hourr].childs[i].value -= 1

        # reduce second counts
        secr, secl = entry.logoff[2], entry.login[2]
        for i in range(0, secl):
            root.childs[hourl].childs[minl].childs[i].value -= 1
        for i in range(secr, 60):
            root.childs[hourr].childs[minr].childs[i].value -= 1

    # sum down the tree for the query
    times, count = root, 0
    while times:
        count += times.value
        times = times.children.get(query.pop(), None)
    return count


def get_users_at_time_III(log, query):
    ''' Given the following log file format, find the number
    of users logged on at the requested time (or range)::
        
        login_time,logoff_time,user_id

    Here we assume that the times will be converted to the
    required resolution (in this case seconds).
    '''
    times = []
    keys  = ['login', 'logoff', 'user']
    fmts  = [int, int, str]
    for entry in generate_entries(log, keys, fmts):
        pass # store in hash

    index, count = 0, 0
    while query:
        if query & 0x01: count += times[index]
        index, query = index << 1, query >> 1
    return count


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


if __name__ == "__main__":
    import doctest
    doctest.testmod()
