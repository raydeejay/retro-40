@echo off

if "%1" == "clean" goto CLEAN

if exist makesoftcore.exe goto SKIPCL
cl /Zi /Od makesoftcore.c ..\lzcompress.c ..\bit.c
goto MAKESOFTCORE

:SKIPCL
echo makesoftcore.exe exists, skipping building it.

:MAKESOFTCORE
echo on
makesoftcore softcore.fr ifbrack.fr prefix.fr ficl.fr jhlocal.fr marker.fr oo.fr classes.fr string.fr win32.fr ficllocal.fr fileaccess.fr
goto EXIT

:CLEAN
del *.obj
del makesoftcore.exe
del ..\softcore.c

:EXIT
