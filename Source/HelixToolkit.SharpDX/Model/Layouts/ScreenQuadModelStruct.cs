using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ScreenQuadModelStruct
{
    public Matrix mWorld;
    public Vector4 BottomLeft;
    public Vector4 BottomRight;
    public Vector4 TopLeft;
    public Vector4 TopRight;

    public Vector2 TexTopLeft;
    Vector2 pad2;
    public Vector2 TexTopRight;
    Vector2 pad3;
    public Vector2 TexBottomLeft;
    Vector2 pad0;
    public Vector2 TexBottomRight;
    Vector2 pad1;

    public const int SizeInBytes = 4 * (4 * 4 + 4 * 8);
}
