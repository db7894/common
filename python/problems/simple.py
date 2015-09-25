#!/usr/bin/env python
'''
A collection of simple interview questions that don't really fit into
a specific group.
'''

def angle_between_clock_arms(hour, minute):
    ''' Given a time, compute the angle between the
    two hands on an analog clock.

    :param hour: The hour of the time
    :param minute: The minute of the time
    :returns: The angle between the two times
    '''
    m_value = (360 / 60) * minute
    h_value = (360 / 12) * hour + m_value / 12.0
    return abs(m_value - h_value)
