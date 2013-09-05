mkdir ..\Packages\HelixToolkit\lib
mkdir ..\Packages\HelixToolkit\lib\NET40
mkdir ..\Packages\HelixToolkit\lib\NET45

copy ..\Output\NET40\HelixToolkit.Wpf.dll ..\Packages\HelixToolkit\lib\NET40
copy ..\Output\NET40\HelixToolkit.Wpf.xml ..\Packages\HelixToolkit\lib\NET40
copy ..\Output\NET40\HelixToolkit.Wpf.pdb ..\Packages\HelixToolkit\lib\NET40
copy ..\Output\NET45\HelixToolkit.Wpf.dll ..\Packages\HelixToolkit\lib\NET45
copy ..\Output\NET45\HelixToolkit.Wpf.xml ..\Packages\HelixToolkit\lib\NET45
copy ..\Output\NET45\HelixToolkit.Wpf.pdb ..\Packages\HelixToolkit\lib\NET45
copy ..\license.txt ..\Packages\HelixToolkit

set EnableNuGetPackageRestore=true
..\Tools\NuGet\NuGet.exe pack ..\Packages\HelixToolkit\HelixToolkit.nuspec -OutputDirectory ..\Packages > pack.log
