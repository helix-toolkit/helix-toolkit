@echo off
setlocal

cd "%~dp0"

set DOCFX_SERVE=--serve
if '%CI%' == 'True' set DOCFX_SERVE=

echo Update docfx
dotnet tool update docfx --tool-path packages --verbosity quiet

echo Build documentation
rem packages\docfx init -q -o Documentation
packages\docfx Documentation\docfx.json %DOCFX_SERVE%

