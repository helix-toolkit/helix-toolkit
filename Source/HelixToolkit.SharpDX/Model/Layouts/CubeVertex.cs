using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct CubeVertex
{
    public Vector4 Position;
    public const int SizeInBytes = 4 * 4;
}
