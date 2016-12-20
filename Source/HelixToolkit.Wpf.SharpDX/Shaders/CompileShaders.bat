@echo off
call "%DXSDK_DIR%Utilities\bin\dx_setenv.cmd"
pushd %~dp0
rem fxc /Od /Zi /T fx_5_0 /I ./../ /Fo ..\Resources\_default.bfx Default.fx
rem fxc /T fx_5_0 /I ./../ /Fo ..\Resources\_default.bfx Default.fx
fxc /T fx_5_0 /I ./../ /Fo ..\Resources\_default.bfx Tessellation.fx
fxc /D SSAO /D DEFERRED_MSAA /T fx_5_0 /I ./../ /Fo ..\Resources\_deferred.bfx Deferred.fx
popd
pause