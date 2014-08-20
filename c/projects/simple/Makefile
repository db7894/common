SRC_EXT  = c
SRC_PATH = .
SOURCES  = $(shell find $(SRC_PATH)/ -name '*.$(SRC_EXT)')
OBJECTS  = $(SOURCES:%.$(SRC_EXT)=%)
LDLIBS  += -lreadline

all: $(OBJECTS)

clean:
	@rm -f $(OBJECTS)
