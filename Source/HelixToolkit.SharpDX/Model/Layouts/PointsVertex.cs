using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct PointsVertex
{
    public Vector4 Position;
    public Color4 Color;
    public const int SizeInBytes = 4 * (4 + 4);
}
