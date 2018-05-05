@echo off
nuget pack HelixToolkit\HelixToolkit.nuspec -Properties "version=2.1.0"
nuget pack HelixToolkit.Wpf\HelixToolkit.Wpf.nuspec -Properties "version=2.1.0"
nuget pack HelixToolkit.Wpf.SharpDX\HelixToolkit.Wpf.SharpDX.nuspec -Properties "version=2.1.0"

copy "HelixToolkit.Wpf.SharpDX.2.1.0.nupkg" "C:\Frame\packages\HelixToolkit.Wpf.SharpDX.2.1.0"
copy "HelixToolkit.Wpf.2.1.0.nupkg" "C:\Frame\packages\HelixToolkit.Wpf.2.1.0"
copy "HelixToolkit.2.1.0.nupkg" "C:\Frame\packages\HelixToolkit.2.1.0"

set ACM_FRAME_BUILD=C:\Acutus\bin\Frame\%ACMFRAME_VERSION%
copy "HelixToolkit\bin\Release\HelixToolkit.*" "C:\Frame\packages\HelixToolkit.2.1.0\lib\portable-net45+win8+wpa81+wp8"
copy "HelixToolkit.Wpf\bin\Release\HelixToolkit.Wpf.*" "C:\Frame\packages\HelixToolkit.Wpf.2.1.0\lib\net45"
copy "HelixToolkit.Wpf.SharpDX\bin\Release\HelixToolkit.Wpf.SharpDX.*" "C:\Frame\packages\HelixToolkit.Wpf.SharpDX.2.1.0\lib\net45"

copy "HelixToolkit.Wpf.SharpDX.2.1.0\lib\net45\HelixToolkit.Wpf.SharpDX.*" "%ACMFRAME_BUILD%\Debug"
copy "HelixToolkit.Wpf.SharpDX.2.1.0\lib\net45\HelixToolkit.Wpf.SharpDX.*" "%ACMFRAME_BUILD%\Release"