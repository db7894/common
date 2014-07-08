import os
import random
import urllib
from cStringIO import StringIO
import pandas as pd
from PIL import Image
from joblib import Parallel, delayed

#
# TODO joblib
#
size   = 0.25
output = 'validate'
images = pd.read_csv('image-test-data.csv')
images = images.URL.tolist()
count  = len(os.listdir(output))
needed = 1000

while count < needed:
    try:
        url  = random.choice(images)
        path = url.split('?')[0].rsplit('/', 1)[-1]
        path = os.path.join(output, path)
        if os.path.exists(path):
            print "exists: " + path
            continue

        print "downloading: " + path
        data  = urllib.urlopen(url).read()
        image = Image.open(StringIO(data))
        image = image.resize([int(s * size) for s in image.size])
        image.save(path)
        count += 1
    except ex: print ex.message
