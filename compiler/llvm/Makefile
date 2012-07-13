all:compiler
compiler: compiler.cpp
	c++ -ggdb -g -O3 $< `llvm-config-2.8 --cppflags --ldflags --libs core` -ldl -o $@

clean:
	rm compiler > /dev/null
