call DeletePdbFiles.cmd ..\Output\NET45\Examples
call DeletePdbFiles.cmd ..\Output\NET40\Examples

del /S /Q ..\Output\NET40\Examples

"C:\Program Files\7-Zip\7z.exe" a -r ..\Output\HelixToolkit-%1.zip ..\Output\*.* > ZipRelease.log