#!/usr/bin/env python
'''
'''
import os
from random import randint, choice
from sys import exit
from optparse import OptionParser
from datetime import datetime
from PIL import Image, ImageDraw, ImageFont

#--------------------------------------------------------------------------#
# initialize logging
#--------------------------------------------------------------------------#

import logging
logger = logging.getLogger(__name__)

#--------------------------------------------------------------------------#
# constants
#--------------------------------------------------------------------------#
# * FONT is the font that is used to print text on the image
# * SIZES are the sizes that will be used to create images
# * FORMAT is the image format that the images will be saved as
#--------------------------------------------------------------------------#

FONT   = "/System/Library/Fonts/HelveticaNeueDeskUI.ttc"
SIZES  = [ (4096, 4096), (2048, 2048), (1024, 1024), (512, 515), (256, 256), (128, 128) ]
FORMAT = "jpeg"

try:
    FONTS = { size:ImageFont.truetype(FONT, int(size * 2.0/3)) for size, _ in SIZES }
except Exception, ex:
    print "Cannot find a valid font, exiting!"
    exit(-1)

#--------------------------------------------------------------------------#
# utility methods
#--------------------------------------------------------------------------#

def create_image(size, color, text):
    ''' Given a collection of parameters, create the
    supplied image and save it to the supplied path.

    :param size: The size of the image to create
    :param color: The color of the image to create
    :param text: The text to draw on top of the image
    :returns: The created image
    '''
    other  = tuple(255 - v for v in color)
    font   = FONTS[size[0]]
    center = tuple(s // 2 - f // 2 for s, f in zip(size, font.getsize(text)))
    image  = Image.new('RGB', size, color)
    paint  = ImageDraw.Draw(image)
    paint.text(center, text, fill=other, font=font)
    return image

def get_random_color():
    ''' Generates a random RGB color

    :returns: (R, G, B) tuple of a random color
    '''
    return (randint(0, 256), randint(0, 256), randint(0,256))

def create_images(directory, count):
    ''' Given a few parameters, create a collection of images
    and save them to disk.

    :param directory: The directory to save the images to
    :param count: The number of images to create
    :param size: The size of the images to create
    '''
    if not os.path.exists(directory):
        os.mkdir(directory)

    for i in range(count):
        name  = "image-%s-%d.%s" % (datetime.now().strftime("%d-%m-%y::%H:%m:%S"), i, FORMAT)
        path  = os.path.join(directory, name)
        size  = choice(SIZES)
        color = get_random_color()
        logger.debug("creating image %s", name)
        image = create_image(size, color, str(i))
        image.save(path)

#--------------------------------------------------------------------------#
# main methods
#--------------------------------------------------------------------------#

def parse_options():
    ''' The main function for this script '''
    parser = OptionParser()

    parser.add_option("-v", "--verbose",
        help="Enable debug tracing",
        action="store_true", dest="verbose", default=False)

    parser.add_option("-o", "--output",
        help="The output directory to write the images to",
        dest="output", default="images")

    parser.add_option("-c", "--count",
        help="The number of images to create", type="int",
        dest="count", default=100)

    (opt, arg) = parser.parse_args()
    return opt, parser

def main():
    ''' The main runner function
    '''
    option, parser = parse_options()

    if option.verbose:
        try:
            logger.setLevel(logging.DEBUG)
    	    logging.basicConfig()
        except Exception, ex:
    	    print "Logging is not supported on this system"

    try:
        create_images(option.output, option.count)
    except Exception, ex:
        logger.exception("An error occurred while generating the images")
        parser.print_help()

if __name__ == "__main__":
    exit(main())
