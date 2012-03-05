set dir=..\Output\Examples
del /S /Q %dir%\*.pdb 
del /S /Q %dir%\*.vshost.exe 
del /S /Q %dir%\*.manifest 
del /S /Q %dir%\*.config
"C:\Program Files\7-Zip\7z.exe" a -r ..\Output\HelixToolkit-Examples-%1.zip %dir%\*.* > CopyAllExamples.log