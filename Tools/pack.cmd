mkdir ..\Packages\HelixToolkit\lib
copy ..\Output\HelixToolkit.Wpf.dll ..\Packages\HelixToolkit\lib
copy ..\Output\HelixToolkit.Wpf.xml ..\Packages\HelixToolkit\lib
nuget.exe pack ..\Packages\HelixToolkit\HelixToolkit.nuspec -OutputDirectory ..\Packages > pack.log
