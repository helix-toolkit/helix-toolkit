@echo off
call "%DXSDK_DIR%Utilities\bin\dx_setenv.cmd"
pushd %~dp0
fxc /Od /Zi /T fx_5_0 /I ./../ /Fo ..\Resources\_custom.bfx Default.fx
popd
pause