mkdir ..\Packages\HelixToolkit\lib
copy ..\Output\HelixToolkit.Wpf.dll ..\Packages\HelixToolkit\lib
copy ..\Output\HelixToolkit.Wpf.xml ..\Packages\HelixToolkit\lib
copy ..\Output\HelixToolkit.Wpf.pdb ..\Packages\HelixToolkit\lib
copy ..\license.txt ..\Packages\HelixToolkit
nuget.exe pack ..\Packages\HelixToolkit\HelixToolkit.nuspec -OutputDirectory ..\Packages > pack.log
