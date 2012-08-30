for %%G in (..\Packages\*.nupkg) do ..\Tools\NuGet\NuGet.exe push -source http://packages.nuget.org/v1/ %%G %NUGET_ACCESS_KEY% >> push.log
