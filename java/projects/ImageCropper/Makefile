SRC_PATH = .
SRC_EXT  = cc
OUT_DIR  = .
SOURCES  = $(shell find $(SRC_PATH)/ -name '*.$(SRC_EXT)')
OBJECTS  = $(SOURCES:%.cc=%.o)
PROJECT  = cropper
LIBRARY  = libImageCropper.dylib

JAVAC    = javac
JAVAH    = javah
CC       = clang++
CXX      = clang++
CFLAGS   = `pkg-config --cflags opencv` -I /usr/local/include/opencv2
CFLAGS  += -std=c++11 -Wall
CFLAGS  += -I /System/Library/Frameworks/JavaVM.framework/Headers
LIBS     = `pkg-config --libs opencv` -L /usr/local/lib
LIBS    += -lboost_system -lboost_filesystem -lboost_program_options

all: $(PROJECT) $(LIBRARY)

clean:
	@rm -f $(OBJECTS) $(PROJECT) $(LIBRARY)

$(PROJECT): $(OBJECTS)
	$(CC) $(lFLAGS) $(LIBS) -o $(OUT_DIR)/$@ $^

$(LIBRARY): $(OBJECTS)
	$(CC) -shared $(lFLAGS) $(LIBS) -o $(OUT_DIR)/$@ $^

.cc.o:
	$(CC) $(CFLAGS) $(INCLUDES) -o $(OUT_DIR)/$@ -c $<

%.class : %.java
	$(JAVAC) $<

%.h : %.class
	$(JAVAH) -jni $<
