import os
from pydmtx import DataMatrix
from PIL import Image

#------------------------------------------------------------
# write a data matrix barcode
#------------------------------------------------------------
path = os.path.abspath('.')
path = os.path.join(path, 'hello.png')

matrix = DataMatrix()
matrix.encode("Hello, world!")
matrix.save(path, "png")

#------------------------------------------------------------
# read a data matrix Barcode
#------------------------------------------------------------
matrix = DataMatrix()
image  = Image.open(path)

print matrix.decode(image.size[0], image.size[1], buffer(image.tostring()))
print matrix.count()
print matrix.message(1)
print matrix.stats(1)
