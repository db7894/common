#!/usr/bin/env python
import sys

(lastk, total, count) = (None, 0.0, 0)

for line in sys.stdin:
    (key, val) = line.split("\t")

    if lastk and lastk != key:
        print lastk + "\t" + str(total / count)
        (total, count) = (0.0, 0)
    (lastk, total, count) = key, total + float(val), count + 1 
print lastk + "\t" + str(total / count)
