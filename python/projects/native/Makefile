DIRS = $(wildcard */)

all: ${DIRS}

.PHONY: force

${DIRS}: force
	$(MAKE) -C $@
