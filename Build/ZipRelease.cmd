set dir=..\Output\NET45\Examples
del /S /Q %dir%\*.pdb 
del /S /Q %dir%\*.vshost.exe 
del /S /Q %dir%\*.manifest 
del /S /Q %dir%\*.config
del /S %dir%\HelixToolkit.*.xml
del /S %dir%\PropertyTools.*.xml
del /S %dir%\NAudio.*.xml
"C:\Program Files\7-Zip\7z.exe" a -r ..\Output\HelixToolkit-%1.zip ..\Output\*.* > ZipRelease.log