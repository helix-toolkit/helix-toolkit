using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct CubeFaceCamera
{
    public Matrix View;
    public Matrix Projection;
    public const int SizeInBytes = 4 * 4 * 4;
}
