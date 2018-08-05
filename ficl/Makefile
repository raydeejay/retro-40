OBJECTS= dictionary.o system.o fileaccess.o float.o double.o prefix.o search.o softcore.o stack.o tools.o vm.o primitives.o bit.o lzuncompress.o unix.o utility.o hash.o callback.o word.o extras.o
HEADERS= ficl.h ficlplatform/unix.h
#
# Flags for shared library
#TARGET= -Dlinux  # riscos MOTO_CPU32 
SHFLAGS = -fPIC
CFLAGS= -g -O $(SHFLAGS) -Wall
CPPFLAGS= $(TARGET) -I.
CC = cc
LIB = ar cr
RANLIB = ranlib

MAJOR = 4
MINOR = 1.0

ficl: main.o $(HEADERS) libficl.a
	$(CC) $(CFLAGS) $(LDFLAGS) main.o -o ficl -L. -lficl -lm

lib: libficl.so.$(MAJOR).$(MINOR)

# static library build
libficl.a: $(OBJECTS)
	$(LIB) libficl.a $(OBJECTS)
	$(RANLIB) libficl.a

# shared library build
libficl.so.$(MAJOR).$(MINOR): $(OBJECTS)
	$(CC) $(LDFLAGS) -shared -Wl,-soname,libficl.so.$(MAJOR).$(MINOR) \
	-o libficl.so.$(MAJOR).$(MINOR) $(OBJECTS)
	ln -sf libficl.so.$(MAJOR).$(MINOR) libficl.so

main: main.o ficl.h sysdep.h libficl.so.$(MAJOR).$(MINOR)
	$(CC) $(CFLAGS) $(LDFLAGS) main.o -o main -L. -lficl -lm
	ln -sf libficl.so.$(MAJOR).$(MINOR) libficl.so.$(MAJOR)

# depend explicitly to help finding source files in another subdirectory,
# and repeat commands since gmake doesn't understand otherwise
ansi.o: ficlplatform/ansi.c $(HEADERS)
	$(CC) $(CFLAGS) $(CPPFLAGS) -c -o $@ $<
unix.o: ficlplatform/unix.c $(HEADERS)
	$(CC) $(CFLAGS) $(CPPFLAGS) -c -o $@ $<

#
#       generic object code
#
.SUFFIXES: .cxx .cc .c .o

.c.o:
	$(CC) $(CFLAGS) $(CPPFLAGS) -c -o $@ $<

.cxx.o:
	$(CPP) $(CXXFLAGS) $(CPPFLAGS) -c -o $@ $<

.cc.o:
	$(CPP) $(CXXFLAGS) $(CPPFLAGS) -c -o $@ $<
#
#       generic cleanup code
#
clean:
	rm -f *.o *.a libficl.* ficl
