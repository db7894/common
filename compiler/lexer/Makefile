all: lex.yy.o

lex.yy.c: lex.1
	flex lex.1

lex.yy.o: lex.yy.c
clean:
	rm -f lex.yy.o lex.yy.c > /dev/null
