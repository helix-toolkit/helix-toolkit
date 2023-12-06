using SharpDX.DirectWrite;
using SharpDX;

namespace HelixToolkit.SharpDX.Utilities.ImagePacker;

public readonly struct TextLayoutInfo
{
    public readonly TextLayout TextLayout;
    public readonly Color4 Foreground;
    public readonly Color4 Background;
    public readonly Vector4 Padding;
    public TextLayoutInfo(TextLayout layout, Color4 foreground, Color4 background, Vector4 padding)
    {
        TextLayout = layout;
        Foreground = foreground;
        Background = background;
        Padding = padding;
    }
}
