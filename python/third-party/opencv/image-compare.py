#!/usr/bin/env python
screenshot = driver.get_screenshot_as_png()

image_url = creative.payload['ImageUrl']
ad_image = requests.get(image_url).content

img_array = numpy.asarray(bytearray(ad_image), dtype=numpy.uint8)
img1 = cv2.imdecode(img_array, 0)
img_array = numpy.asarray(bytearray(screenshot), dtype=numpy.uint8)
img2 = cv2.imdecode(img_array, 0)

# use a scale-invariant feature transform algorithm to find the ad
sift = cv2.SIFT()
kp1, des1 = sift.detectAndCompute(img1, None)
kp2, des2 = sift.detectAndCompute(img2, None)
bf = cv2.BFMatcher()

matches = bf.knnMatch(des1, des2, k=2)

good = []
for m,n in matches:
    if m.distance < 0.75 * n.distance:
        good.append([m])

# draw matches and output to a third image
img3 = cv2.drawMatchesKnn(img1, kp1, img2, kp2, good, flags=2)
return_value, image_analysis_array = cv2.imencode('.png', img3)
pytest.screenshots.append(image_analysis_array.tostring())
