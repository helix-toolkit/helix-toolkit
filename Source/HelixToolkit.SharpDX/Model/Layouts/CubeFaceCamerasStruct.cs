using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct CubeFaceCamerasStruct
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public CubeFaceCamera[] Cameras;
    public const int SizeInBytes = CubeFaceCamera.SizeInBytes * 6;
}
