using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct SpriteStruct
{
    public Vector2 Position;
    public Vector2 UV;
    public Vector4 Color;

    public const int SizeInBytes = 4 * (2 + 2 + 4);
}
