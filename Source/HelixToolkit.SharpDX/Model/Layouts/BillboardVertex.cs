using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct BillboardVertex
{
    public Vector4 Position;
    public Color4 Foreground;
    public Color4 Background;
    public Vector2 TexTL;
    public Vector2 TexBR;
    public Vector2 OffTL;
    public Vector2 OffTR;
    public Vector2 OffBL;
    public Vector2 OffBR;
    public const int SizeInBytes = 4 * (4 * 3 + 2 * 6);
}
