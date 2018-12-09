@echo off
setlocal

cd "%~dp0"

if exist "dist-examples" rmdir /s /q "dist-examples"

for /d %%a in ("Examples/WPF/*") do (
echo Examples/WPF/%%~a
mkdir "dist-examples/WPF/%%~a"
xcopy /s /d /q "Examples/WPF/%%~a/bin/Release" "dist-examples/WPF/%%~a\"
)

for /d %%a in ("Examples/WPF.SharpDX/*") do (
echo Examples/WPF.SharpDX/%%~a
mkdir "dist-examples/WPF.SharpDX/%%~a"
xcopy /s /d /q "Examples/WPF.SharpDX/%%~a/bin/Release" "dist-examples/WPF.SharpDX/%%~a\"
)


for /d %%a in ("Examples/SharpDX.Core/*") do (
echo Examples/SharpDX.Core/%%~a
mkdir "dist-examples/SharpDX.Core/%%~a"
xcopy /s /d /q "Examples/SharpDX.Core/%%~a/bin/Release" "dist-examples/SharpDX.Core/%%~a\"
)