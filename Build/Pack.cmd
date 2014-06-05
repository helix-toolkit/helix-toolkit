mkdir ..\Packages\HelixToolkit\lib
mkdir ..\Packages\HelixToolkit\lib\portable-NET4+wp71+win8

copy ..\Output\NET40\HelixToolkit.??? ..\Packages\HelixToolkit\lib\portable-NET4+wp71+win8
copy ..\license.txt ..\Packages\HelixToolkit

mkdir ..\Packages\HelixToolkit.Wpf\lib
mkdir ..\Packages\HelixToolkit.Wpf\lib\NET40
mkdir ..\Packages\HelixToolkit.Wpf\lib\NET45

copy ..\Output\NET40\HelixToolkit.Wpf.??? ..\Packages\HelixToolkit.Wpf\lib\NET40
copy ..\Output\NET45\HelixToolkit.Wpf.??? ..\Packages\HelixToolkit.Wpf\lib\NET45
copy ..\license.txt ..\Packages\HelixToolkit.Wpf

mkdir ..\Packages\HelixToolkit.Wpf.SharpDX\lib
mkdir ..\Packages\HelixToolkit.Wpf.SharpDX\lib\NET40
mkdir ..\Packages\HelixToolkit.Wpf.SharpDX\lib\NET45

copy ..\Output\NET40\HelixToolkit.Wpf.SharpDX.??? ..\Packages\HelixToolkit.Wpf\lib\NET40
copy ..\Output\NET45\HelixToolkit.Wpf.SharpDX.??? ..\Packages\HelixToolkit.Wpf\lib\NET45
copy ..\license.txt ..\Packages\HelixToolkit.Wpf.SharpDX

set EnableNuGetPackageRestore=true
..\Source\.nuget\NuGet.exe pack ..\Packages\HelixToolkit\HelixToolkit.nuspec -OutputDirectory ..\Packages
..\Source\.nuget\NuGet.exe pack ..\Packages\HelixToolkit.Wpf\HelixToolkit.Wpf.nuspec -OutputDirectory ..\Packages
..\Source\.nuget\NuGet.exe pack ..\Packages\HelixToolkit.Wpf.SharpDX\HelixToolkit.Wpf.SharpDX.nuspec -OutputDirectory ..\Packages
