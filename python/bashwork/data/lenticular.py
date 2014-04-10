from PIL import Image

a = Image.open('image.jpg')
b = a.transpose(Image.FLIP_LEFT_RIGHT)
c = Image.open('image2.jpg')

width  = 250
images = [a, c, b]
size   = (a.size[0] * 3, a.size[1])
output = Image.new(a.mode, size) 
point  = 0

for xidx in range(0, a.size[0] - width, width):
    for image in images:
        band = image.crop((xidx, 0, xidx + width, image.size[1]))
        output.paste(band, (point, 0))
        point += width
output.show()
