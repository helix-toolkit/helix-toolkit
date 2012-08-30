mkdir ..\Packages\HelixToolkit\lib
mkdir ..\Packages\HelixToolkit\lib\NET40

copy ..\Output\HelixToolkit.Wpf.dll ..\Packages\HelixToolkit\lib\NET40
copy ..\Output\HelixToolkit.Wpf.xml ..\Packages\HelixToolkit\lib\NET40
copy ..\Output\HelixToolkit.Wpf.pdb ..\Packages\HelixToolkit\lib\NET40
copy ..\license.txt ..\Packages\HelixToolkit

set EnableNuGetPackageRestore=true
..\Tools\NuGet\NuGet.exe pack ..\Packages\HelixToolkit\HelixToolkit.nuspec -OutputDirectory ..\Packages > pack.log
