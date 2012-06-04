#!/usr/bin/env python
import sys

index1 = int(sys.argv[1])
index2 = int(sys.argv[2])
for line in sys.stdin:
    f = line.split(",")
    print "UniqValueCount:" + f[index1] + "\t" + f[index2]

