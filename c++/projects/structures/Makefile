SRC_PATH = .
SRC_EXT  = cc
OUT_DIR  = .
SOURCES  = $(shell find $(SRC_PATH)/ -name '*.$(SRC_EXT)')
OBJECTS  = $(SOURCES:%.cc=%)

#CC      = clang++
#CXX     = clang++
#CFLAGS  = -std=c++11 -stdlib=libc++ 
#LIBS    = -lboost_system -lboost_filesystem -lboost_program_options
CC      = g++-4.9
CXX     = g++-4.9
CFLAGS  = -std=c++11 -mcx16 -ggdb -Wall

all: $(OBJECTS)

clean:
	@rm -f $(OBJECTS)

% : %.cc
	$(CC) $(CFLAGS) $(LIBS) -o $(OUT_DIR)/$@ $<


