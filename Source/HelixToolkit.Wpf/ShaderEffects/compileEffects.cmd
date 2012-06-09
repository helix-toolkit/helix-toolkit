REM Requires DirectX SDK

path %path%;"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Utilities\bin\x64"

fxc /T ps_2_0 /E main /Fo AnaglyphEffect.ps AnaglyphEffect.fx
pause