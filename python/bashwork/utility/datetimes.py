from datetime import datetime

def datetime_to_epoch(timestamp):
    ''' Given a python datetime object, return the milliseconds
    since the epoch in UTC.

    :param timestamp: The timestamp to convert
    :returns: The milliseconds since the epoch.
    '''
    if isinstance(timestamp, datetime):
        epoch = datetime.utcfromtimestamp(0)
        delta = timestamp - epoch
        return int(delta.total_seconds() * 1000)
    return timestamp
