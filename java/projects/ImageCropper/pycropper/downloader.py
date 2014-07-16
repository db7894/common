import os
import json
import urllib
import random
from cStringIO import StringIO
from joblib import Parallel, delayed
from PIL import Image
from optparse import OptionParser

#------------------------------------------------------------
# logging
#------------------------------------------------------------

import logging
logger = logging.getLogger(__name__)

#------------------------------------------------------------
# utiliites
#------------------------------------------------------------

def retrieve_image(path, link):
    ''' Given an image link, download it to memory
    and return it and a path it can be saved to.

    :param path: The path to save the image to
    :param link: The link to the image to save
    :returns: (path to save image to, in memory image)
    '''
    link = link.replace('_C.jpeg?', '.jpeg?')
    name = link.split('/')[-1].split('?')[0]
    path = os.path.join(path, name)

    try:
        if not os.path.exists(path):
            logger.debug("Downloading {}".format(len(name)))
            data = urllib.urlopen(link).read()
            return (path, Image.open(StringIO(data)))
    except Exception, ex:
        logger.exception("Download failed for {}".format(path))
    return (path, None)

def save_image(path, link, **kwargs):
    ''' Given an image link, save it to the supplied path

    :param path: The path to save the image to
    :param link: The link to the image to save
    :param size: The amount to resize the image
    :returns: The path to the saved image
    '''
    size = kwargs.get('size', 0.25)
    path, image = retrieve_image(path, link)
    if image:
        image = image.resize([int(s * size) for s in image.size])
        image.save(path)
    return path

#---------------------------------------------------------------------------# 
# initialize our program settings
#---------------------------------------------------------------------------# 

def get_options():
    ''' A helper method to parse the command line options

    :returns: The options manager
    '''
    parser = OptionParser()

    parser.add_option("-p", "--path",
        help="The path to append to download files to",
        dest="path", default=".")

    parser.add_option("-d", "--debug",
        help="Enable debug tracing",
        action="store_true", dest="debug", default=False)

    parser.add_option("-s", "--sample",
        help="The sample size to download",
        dest="sample", default=1.0)

    parser.add_option("-i", "--input",
        help="The input database to operate with",
        dest="database", default=None)

    (opt, arg) = parser.parse_args()
    return opt

def sample_data(data, rate=1.0):
    ''' Given a dataset, randomly sample it by
    the supplied rate to return a subset of the
    input.

    :param data: The data to sample
    :param rate: The rate at which to sample
    :returns: The sampled dataset
    '''
    if rate == 1.0: return data
    size = int(rate * len(data) + 0.5)
    return random.sample(data, size)

#---------------------------------------------------------------------------# 
# main runner
#---------------------------------------------------------------------------# 

if __name__ == "__main__":
    option = get_options()
    output = os.path.abspath(option.path)
    links  = json.load(open(option.database))
    links  = sample_data(links, option.sample)
    paths  = Parallel(n_jobs=10)(delayed(save_image)(path, link) for link in links)
    logger.debug("Downloaded {} files".format(len(paths)))
