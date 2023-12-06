using SharpDX;
using System.Runtime.InteropServices;

namespace HelixToolkit.SharpDX;

/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct ScreenDuplicationModelStruct
{
    public Vector4 TopRight;
    public Vector4 TopLeft;
    public Vector4 BottomRight;
    public Vector4 BottomLeft;

    public Vector2 TexTopRight;
    private Vector2 pad0;
    public Vector2 TexTopLeft;
    private Vector2 pad1;
    public Vector2 TexBottomRight;
    private Vector2 pad2;
    public Vector2 TexBottomLeft;
    private Vector2 pad3;

    public Vector4 CursorTopRight;
    public Vector4 CursorTopLeft;
    public Vector4 CursorBottomRight;
    public Vector4 CursorBottomLeft;

    public const int SizeInBytes = 4 * 4 * 12;
}
