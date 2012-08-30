mkdir ..\Packages\HelixToolkit\lib
set dest=..\Packages\HelixToolkit\lib\NET40
mkdir %dest%

copy ..\Output\HelixToolkit.Wpf.dll %dest%
copy ..\Output\HelixToolkit.Wpf.xml %dest%
copy ..\Output\HelixToolkit.Wpf.pdb %dest%
copy ..\license.txt ..\Packages\HelixToolkit
..\Tools\NuGet\NuGet.exe pack ..\Packages\HelixToolkit\HelixToolkit.nuspec -OutputDirectory ..\Packages > pack.log
