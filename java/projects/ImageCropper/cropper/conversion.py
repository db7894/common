'''
A collection of convertion functions from one rectangle
form to another rectangle form:

* rectangle width   - (x, y, w, h)
* rectangle 2 point - (x1, y1, x2, y2)
* rectangle 4 point - [tl, tr, br, bl]
'''
def point2_to_width(rectangle):
    ''' Given a rectangle described by the upper
    left point and lower right point, convert it
    to a form described by the upper left point and
    a width and height.

    :param rectangle: The rectangle to convert
    :returns: The converted rectangle
    '''
    x1, y1, x2, y2 = rectangle
    return (x1, y1, x2 - x1, y2 - y1)

def point2_to_point4(rectangle):
    ''' Given a rectangle described by the upper
    left point and lower right point, convert it
    to a form described by four points (tl, tr, br, bl).

    :param rectangle: The rectangle to convert
    :returns: The converted rectangle
    '''
    x1, y1, x2, y2 = rectangle
    return [(x1, y2), (x2, y1), (x2, y2), (x1, y2)]

def point4_to_point2(rectangle):
    ''' Given a rectangle described by four points
    left point and lower right point, convert it
    to a form described by the upper left point and
    the bottom right point.

    :param rectangle: The rectangle to convert
    :returns: The converted rectangle
    '''
    x1, y1 = rectangle[0]
    x2, y2 = rectangle[3]
    return (x1, y1, x2, y2)

def point4_to_width(rectangle):
    ''' Given a rectangle described by four points
    convert it to a form described by the upper left
    point and a width and height.

    :param rectangle: The rectangle to convert
    :returns: The converted rectangle
    '''
    x1, y1 = rectangle[0]
    x2, y2 = rectangle[3]
    return (x1, y1, x2 - x1, y2 - y1)

def width_to_point2(rectangle):
    ''' Given a rectangle described by the upper
    left point and a width and height convert it
    to a form described by the upper left point
    and lower right point.

    :param rectangle: The rectangle to convert
    :returns: The converted rectangle
    '''
    x, y, w, h = rectangle
    return (x, y, x + w, y + h)

def width_to_point4(rectangle):
    ''' Given a rectangle described by the upper
    left point and a width and height convert it
    to a form described by four points (tl, tr, br, bl).

    :param rectangle: The rectangle to convert
    :returns: The converted rectangle
    '''
    x, y, w, h = rectangle
    return [(x, y), (x + w, y), (x + w, y + h), (x, y + h)]
