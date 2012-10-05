all:example
clean:
	rm -f example > /dev/null

example: example.c
	gcc -rdynamic -o $@ $^ -ldl
