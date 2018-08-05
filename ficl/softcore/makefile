SOURCES = softcore.fr ifbrack.fr prefix.fr ficl.fr jhlocal.fr marker.fr oo.fr classes.fr string.fr ficllocal.fr fileaccess.fr

../softcore.c: makesoftcore $(SOURCES)
	makesoftcore $(SOURCES)

makesoftcore: makesoftcore.c ../lzcompress.c ../bit.c
	$(CC) $(CFLAGS) $(CPPFLAGS) -I.. -o makesoftcore makesoftcore.c ../lzcompress.c ../bit.c

clean: 
	- rm ../softcore.c *.o makesoftcore

