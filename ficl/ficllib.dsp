# Microsoft Developer Studio Project File - Name="ficllib" - Package Owner=<4>
# Microsoft Developer Studio Generated Build File, Format Version 6.00
# ** DO NOT EDIT **

# TARGTYPE "Win32 (x86) Static Library" 0x0104

CFG=ficllib - Win32 Debug Multithreaded DLL
!MESSAGE This is not a valid makefile. To build this project using NMAKE,
!MESSAGE use the Export Makefile command and run
!MESSAGE 
!MESSAGE NMAKE /f "ficllib.mak".
!MESSAGE 
!MESSAGE You can specify a configuration when running NMAKE
!MESSAGE by defining the macro CFG on the command line. For example:
!MESSAGE 
!MESSAGE NMAKE /f "ficllib.mak" CFG="ficllib - Win32 Debug Multithreaded DLL"
!MESSAGE 
!MESSAGE Possible choices for configuration are:
!MESSAGE 
!MESSAGE "ficllib - Win32 Debug Singlethreaded" (based on "Win32 (x86) Static Library")
!MESSAGE "ficllib - Win32 Debug Multithreaded" (based on "Win32 (x86) Static Library")
!MESSAGE "ficllib - Win32 Debug Multithreaded DLL" (based on "Win32 (x86) Static Library")
!MESSAGE "ficllib - Win32 Release Singlethreaded" (based on "Win32 (x86) Static Library")
!MESSAGE "ficllib - Win32 Release Multithreaded" (based on "Win32 (x86) Static Library")
!MESSAGE "ficllib - Win32 Release Multithreaded DLL" (based on "Win32 (x86) Static Library")
!MESSAGE 

# Begin Project
# PROP AllowPerConfigDependencies 0
# PROP Scc_ProjName "ficllib"
# PROP Scc_LocalPath "."
CPP=cl.exe
RSC=rc.exe

!IF  "$(CFG)" == "ficllib - Win32 Debug Singlethreaded"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "lib/debug/singlethreaded"
# PROP BASE Intermediate_Dir "lib/debug/singlethreaded"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "lib/debug/singlethreaded"
# PROP Intermediate_Dir "lib/debug/singlethreaded"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /YX /FD /GZ /c
# ADD CPP /nologo /W4 /Gm /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /YX /FD /GZ /Zm200 /c
# ADD BASE RSC /l 0x409 /d "_DEBUG"
# ADD RSC /l 0x409 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"lib/debug/singlethreaded/ficl.lib"

!ELSEIF  "$(CFG)" == "ficllib - Win32 Debug Multithreaded"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "lib/debug/multithreaded"
# PROP BASE Intermediate_Dir "lib/debug/multithreaded"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "lib/debug/multithreaded"
# PROP Intermediate_Dir "lib/debug/multithreaded"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /YX /FD /GZ /c
# ADD CPP /nologo /MTd /W4 /Gm /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /YX /FD /GZ /Zm200 /c
# ADD BASE RSC /l 0x409 /d "_DEBUG"
# ADD RSC /l 0x409 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"lib/debug/multithreaded/ficl.lib"

!ELSEIF  "$(CFG)" == "ficllib - Win32 Debug Multithreaded DLL"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 1
# PROP BASE Output_Dir "lib/debug/multithreaded_dll"
# PROP BASE Intermediate_Dir "lib/debug/multithreaded_dll"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 1
# PROP Output_Dir "lib/debug/multithreaded_dll"
# PROP Intermediate_Dir "lib/debug/multithreaded_dll"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /Gm /GX /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /YX /FD /GZ /c
# ADD CPP /nologo /MDd /W4 /Gm /ZI /Od /D "WIN32" /D "_DEBUG" /D "_MBCS" /D "_LIB" /YX /FD /GZ /Zm200 /c
# ADD BASE RSC /l 0x409 /d "_DEBUG"
# ADD RSC /l 0x409 /d "_DEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"lib/debug/multithreaded_dll/ficl.lib"

!ELSEIF  "$(CFG)" == "ficllib - Win32 Release Singlethreaded"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "lib/release/singlethreaded"
# PROP BASE Intermediate_Dir "lib/release/singlethreaded"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "lib/release/singlethreaded"
# PROP Intermediate_Dir "lib/release/singlethreaded"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /YX /FD /c
# ADD CPP /nologo /W4 /O2 /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D FICL_ROBUST=0 /YX /FD /Zm200 /c
# ADD BASE RSC /l 0x409 /d "NDEBUG"
# ADD RSC /l 0x409 /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"lib/release/singlethreaded/ficl.lib"

!ELSEIF  "$(CFG)" == "ficllib - Win32 Release Multithreaded"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "lib/release/multithreaded"
# PROP BASE Intermediate_Dir "lib/release/multithreaded"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "lib/release/multithreaded"
# PROP Intermediate_Dir "lib/release/multithreaded"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /YX /FD /c
# ADD CPP /nologo /MT /W4 /O2 /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D FICL_ROBUST=0 /YX /FD /Zm200 /c
# ADD BASE RSC /l 0x409 /d "NDEBUG"
# ADD RSC /l 0x409 /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"lib/release/multithreaded/ficl.lib"

!ELSEIF  "$(CFG)" == "ficllib - Win32 Release Multithreaded DLL"

# PROP BASE Use_MFC 0
# PROP BASE Use_Debug_Libraries 0
# PROP BASE Output_Dir "lib/release/multithreaded_dll"
# PROP BASE Intermediate_Dir "lib/release/multithreaded_dll"
# PROP BASE Target_Dir ""
# PROP Use_MFC 0
# PROP Use_Debug_Libraries 0
# PROP Output_Dir "lib/release/multithreaded_dll"
# PROP Intermediate_Dir "lib/release/multithreaded_dll"
# PROP Target_Dir ""
# ADD BASE CPP /nologo /W3 /GX /O2 /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /YX /FD /c
# ADD CPP /nologo /MD /W4 /O2 /D "WIN32" /D "NDEBUG" /D "_MBCS" /D "_LIB" /D FICL_ROBUST=0 /YX /FD /Zm200 /c
# ADD BASE RSC /l 0x409 /d "NDEBUG"
# ADD RSC /l 0x409 /d "NDEBUG"
BSC32=bscmake.exe
# ADD BASE BSC32 /nologo
# ADD BSC32 /nologo
LIB32=link.exe -lib
# ADD BASE LIB32 /nologo
# ADD LIB32 /nologo /out:"lib/release/multithreaded_dll/ficl.lib"

!ENDIF 

# Begin Target

# Name "ficllib - Win32 Debug Singlethreaded"
# Name "ficllib - Win32 Debug Multithreaded"
# Name "ficllib - Win32 Debug Multithreaded DLL"
# Name "ficllib - Win32 Release Singlethreaded"
# Name "ficllib - Win32 Release Multithreaded"
# Name "ficllib - Win32 Release Multithreaded DLL"
# Begin Group "Source Files"

# PROP Default_Filter "cpp;c;cxx;rc;def;r;odl;idl;hpj;bat"
# Begin Source File

SOURCE=.\bit.c
# End Source File
# Begin Source File

SOURCE=.\callback.c
# End Source File
# Begin Source File

SOURCE=.\compatibility.c
# End Source File
# Begin Source File

SOURCE=dictionary.c
# End Source File
# Begin Source File

SOURCE=double.c
# End Source File
# Begin Source File

SOURCE=.\extras.c
# End Source File
# Begin Source File

SOURCE=fileaccess.c
# End Source File
# Begin Source File

SOURCE=float.c
# End Source File
# Begin Source File

SOURCE=.\hash.c
# End Source File
# Begin Source File

SOURCE=.\lzuncompress.c
# End Source File
# Begin Source File

SOURCE=prefix.c
# End Source File
# Begin Source File

SOURCE=.\primitives.c
# End Source File
# Begin Source File

SOURCE=search.c
# End Source File
# Begin Source File

SOURCE=softcore.c
# End Source File
# Begin Source File

SOURCE=stack.c
# End Source File
# Begin Source File

SOURCE=.\system.c
# End Source File
# Begin Source File

SOURCE=tools.c
# End Source File
# Begin Source File

SOURCE=.\utility.c
# End Source File
# Begin Source File

SOURCE=vm.c
# End Source File
# Begin Source File

SOURCE=.\ficlplatform\win32.c
# End Source File
# Begin Source File

SOURCE=.\word.c
# End Source File
# End Group
# Begin Group "Header Files"

# PROP Default_Filter "h;hpp;hxx;hm;inl"
# Begin Source File

SOURCE=ficl.h
# End Source File
# Begin Source File

SOURCE=.\ficlcompatibility.h
# End Source File
# Begin Source File

SOURCE=.\ficllocal.h
# End Source File
# Begin Source File

SOURCE=.\ficltokens.h
# End Source File
# Begin Source File

SOURCE=.\ficlplatform\win32.h
# End Source File
# End Group
# End Target
# End Project
