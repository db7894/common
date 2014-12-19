import numpy as np

im = cv2.imread('images/image-maze.jpg')
X, Y, c = im.shape
xs  = int(X / 9.0)
ys  = int(Y / 9.0)
ysi = int(ys / 2.0)
xsi = int(xs / 2.0)

for x in range(0, X, xs):
    for y in range(0, Y, ys):
        cv2.circle(im, (x + xsi, y + ysi), 10, (0, 255, 0) 5)

cv2.imshow('image', im)

blue   = cv2.imshow('range2', cv2.inRange(im, (75,  0, 0), (135, 250, 250))) # 175..200
purple = cv2.imshow('range2', cv2.inRange(im, (140, 0, 0), (170, 250, 250))) # 275..325
red    = cv2.imshow('range2', cv2.inRange(im, (175, 0, 0), (200, 250, 250))) # 350..360

kernel = np.ones((5,5),np.uint8)
cv2.morphologyEx(im, cv2.MORPH_OPEN, kernel)
cv2.morphologyEx(im, cv2.MORPH_CLOSING, kernel)
