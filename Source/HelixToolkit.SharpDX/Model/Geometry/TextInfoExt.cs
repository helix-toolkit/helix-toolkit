using SharpDX;
using SharpDX.DirectWrite;

namespace HelixToolkit.SharpDX;

public class TextInfoExt : TextInfo
{
    public string FontFamily { get; set; } = "Arial";
    public FontWeight FontWeight { get; set; } = FontWeight.Normal;
    public FontStyle FontStyle { get; set; } = FontStyle.Normal;
    public Vector4 Padding = Vector4.Zero;
    public int Size { get; set; } = 12;
}
