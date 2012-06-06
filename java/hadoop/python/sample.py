#!/usr/bin/env python
import sys
from random import random

percent = int(sys.argv[1]) * 0.01
for line in sys.stdin:
    if percent >= random():
        print line.strip()

