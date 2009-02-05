#!/bin/bash
cd ..
find . -name *.h >>cscope.files.bak
find . -name *.c >>cscope.files.bak
grep -v SCCS cscope.files.bak >cscope.files
cscope -b
ctags -R .

