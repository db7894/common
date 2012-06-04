#!/usr/bin/env python
import sys

index = int(sys.argv[1])
value = (0, None)
for line in sys.stdin:
    fields = line.strip().split(",")
    if fields[index].isdigit():
        v = int(fields[index])
        value = max(value, (v, line))
else: print "%s\t%s" % value

