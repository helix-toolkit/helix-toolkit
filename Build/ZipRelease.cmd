mkdir ..\Output\Release
mkdir ..\Output\Release\NET40
mkdir ..\Output\Release\NET45

copy ..\Output\NET45\HelixToolkit.??? ..\Output\Release
copy ..\Output\NET45\HelixToolkit.Wpf.??? ..\Output\Release\NET45
copy ..\Output\NET40\HelixToolkit.Wpf.??? ..\Output\Release\NET40
copy ..\Output\NET45\HelixToolkit.Wpf.Input.??? ..\Output\Release\NET45
copy ..\Output\NET40\HelixToolkit.Wpf.Input.??? ..\Output\Release\NET40
copy ..\Output\NET45\HelixToolkit.Wpf.SharpDX.??? ..\Output\Release\NET45
copy ..\Output\NET40\HelixToolkit.Wpf.SharpDX.??? ..\Output\Release\NET40

"C:\Program Files\7-Zip\7z.exe" a -r ..\Output\HelixToolkit-%1.zip ..\Output\Release\*.* > ZipRelease.log