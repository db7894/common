from Xlib import X, display
from time import sleep
from random import randint

#------------------------------------------------------------ 
# initialize handles
#------------------------------------------------------------ 
d = display.Display()
s = d.screen()
root = s.root
(w, h) = s.width_in_pixels, s.height_in_pixels

while True:
    point = (randint(0, w), randint(0,h))
    root.warp_pointer(*point)
    d.sync()
    sleep(1)
