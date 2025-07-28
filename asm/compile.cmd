del *.exe
c:\windows\microsoft.net\framework64\v4.0.30319\csc /o+ /debug- /nologo /noconfig /utf8output /warn:4 /platform:x64 /target:exe *.cs
c:\windows\microsoft.net\framework64\v4.0.30319\ilasm.exe /EXE /QUIET /NOLOGO /OPTIMIZE /CLOCK /FOLD /PE64 /X64 /OUTPUT=%1.exe %1.asm
%1.exe