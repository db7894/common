OBJ_DIR = ../build
OBJ_BIN = test_example example.o example.so

all: ${OBJ_BIN}

example.so: example.o
	$(CC) -Wall -g -ggdb -fPIC -shared -o $@ $? -lc

test_example: test_example.c example.o

clean:
	rm -f ${OBJ_BIN}
