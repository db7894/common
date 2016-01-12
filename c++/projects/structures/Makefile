SRC_PATH = .
SRC_EXT  = cc
OUT_DIR  = .
SOURCES  = $(shell find $(SRC_PATH)/ -name '*.$(SRC_EXT)')
OBJECTS  = $(SOURCES:%.cc=%)

# clang flags
CC      = clang++
CXX     = clang++
CFLAGS  = -std=c++11 -stdlib=libc++ -mcx16

# gdb flags
#CC      = g++-5
#CXX     = g++-5
#CFLAGS  = -std=c++11 -mcx16 -ggdb -Wall
#LIBS    = -lboost_system -lboost_filesystem -lboost_program_options

all: $(OBJECTS)

clean:
	@rm -f $(OBJECTS)
	@rm -rf *.dSYM

% : %.cc
	$(CC) $(CFLAGS) $(LIBS) -o $(OUT_DIR)/$@ $<


