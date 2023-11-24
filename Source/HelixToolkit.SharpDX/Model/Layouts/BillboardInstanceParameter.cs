using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct BillboardInstanceParameter
{
    public Color4 DiffuseColor;
    public Vector2 TexCoordScale;
    public Vector2 TexCoordOffset;
    public const int SizeInBytes = 4 * (4 + 2 + 2);
}
