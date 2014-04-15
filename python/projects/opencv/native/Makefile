SRC_PATH = .
SRC_EXT  = cc
OUT_DIR  = .
SOURCES  = $(shell find $(SRC_PATH)/ -name '*.$(SRC_EXT)')
OBJECTS  = $(SOURCES:%.cc=%)

CC     = g++
CXX    = g++
CFLAGS = `pkg-config --cflags opencv`
LIBS   = `pkg-config --libs opencv`

all: $(OBJECTS)

clean:
	@rm -f $(OBJECTS)

% : %.cc
	$(CC) $(CFLAGS) $(LIBS) -o $(OUT_DIR)/$@ $<


