from datetime import date, datetime
from dateutil import rrule
from dateutil.parser import parse

class Holidays(object):
    ''' Can be used to generate a collection of the requested
    holiday dates for the previously configured years.
    '''

    def __init__(self, **kwargs):
        ''' Initializes a new instance of the holiday generator
        '''
        st = kwargs.get('start', date(2000, 1, 1))
        en = kwargs.get('end',   date(2014, 12, 1))
        rules = {
            'new_years'      : { 'bymonth' :1,  'bymonthday' :  1 },
            'valentines'     : { 'bymonth' :2,  'bymonthday' : 14 },
            'saint_patricks' : { 'bymonth' :3,  'bymonthday' : 17 },
            'halloween'      : { 'bymonth' :10, 'bymonthday' : 31 },
            'independence'   : { 'bymonth' :7,  'bymonthday' :  4 },
            'may'            : { 'bymonth' :5,  'bymonthday' :  1 },
            'cinco_de_mayo'  : { 'bymonth' :5,  'bymonthday' :  5 },
            'christmas'      : { 'bymonth' :12, 'bymonthday' : 25 },
            'mothers'        : { 'bymonth' :5,  'byweekday'  : rrule.SU(2)  },
            'fathers'        : { 'bymonth' :6,  'byweekday'  : rrule.SU(3)  },
            'memorial'       : { 'bymonth' :5,  'byweekday'  : rrule.MO(-1) },
            'thanksgiving'   : { 'bymonth' :11, 'byweekday'  : rrule.TH(4)  },
            'labor'          : { 'bymonth' :9,  'byweekday'  : rrule.MO(1)  },
        }

        # Compute the rule for the supplied dates and then cache the result
        # as an attribute (TODO make read only)
        for key, params in rules.items():
            rule = rrule.rrule(freq=rrule.YEARLY, dtstart=st, until=en, **params) 
            setattr(self, key,  set(day for day in rule))
