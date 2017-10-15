@echo off
call "%DXSDK_DIR%Utilities\bin\dx_setenv.cmd"
pushd %~dp0
rem fxc /Od /Zi /T fx_5_0 /I ./../ /Fo ..\Resources\_default.bfx Default.fx
rem fxc /T fx_5_0 /I ./../ /Fo ..\Resources\_default.bfx Default.fx
"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64\fxc" /T fx_5_0 /I ./../ /Fo ..\Resources\_default.bfx Tessellation.fx
"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64\fxc" /D SSAO /D DEFERRED_MSAA /T fx_5_0 /I ./../ /Fo ..\Resources\_deferred.bfx Deferred.fx
popd
pause