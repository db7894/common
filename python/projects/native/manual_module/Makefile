all: test libexample.so example_module

libexample.so: library/libexample.c
	$(CC) -Wall -g -fPIC -shared -o $@ $? -lc

test: test_main.c library/libexample.o
	$(CC) -o $@ $?

example_module: example_module.c
	python setup.py build

clean:
	rm -f test libexample.o libexample.so *.pyc
	rm -rf build
