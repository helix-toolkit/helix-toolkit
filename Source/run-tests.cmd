@echo off
setlocal

cd "%~dp0"

if '%Configuration%' == '' if not '%1' == '' set Configuration=%1
if '%Configuration%' == '' set Configuration=Debug

dotnet tool update dotnet-reportgenerator-globaltool --tool-path packages

if exist bld\coverage rd /s /q bld\coverage
md bld\coverage

if exist bld\TestResults rd /s /q bld\TestResults

dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings --results-directory bld\TestResults --no-build
if %ERRORLEVEL% neq 0 exit /b %ERRORLEVEL%

setlocal EnableDelayedExpansion
cd "%~dp0"
set _filelist=,
for /f "delims=|" %%f in ('dir /b bld\TestResults\') do (
  set "_filelist=!_filelist!;bld\TestResults\%%f\coverage.cobertura.xml"
)
set _filelist=%_filelist:,;=%
echo %_filelist%

packages\reportgenerator -reports:"%_filelist%" -reporttypes:Html;Badges -targetdir:bld\coverage -verbosity:Info
if %ERRORLEVEL% neq 0 exit /b %ERRORLEVEL%
