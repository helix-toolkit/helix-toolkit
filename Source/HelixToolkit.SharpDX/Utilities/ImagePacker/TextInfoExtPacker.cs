using SharpDX.Direct2D1;
using SharpDX;
using SharpDX.Mathematics.Interop;

namespace HelixToolkit.SharpDX.Utilities.ImagePacker;

/// <summary>
/// Draw and pack a list of <see cref="TextInfoExt"/> into a single large bitmap
/// </summary>
public sealed class TextInfoExtPacker : SpritePackerBase<TextInfoExt, TextLayoutInfo>
{
    public TextInfoExtPacker(IDevice2DResources deviceResources) : base(deviceResources)
    {
    }

    protected override void DrawOntoOutputTarget(WicRenderTarget target)
    {
        foreach (var text in ItemArray)
        {
            var location = ImagePlacement[text.Key];
            var t = text.Value;
            using (var brush = new SolidColorBrush(target, t.Background.ToStruct<Color4, RawColor4>()))
                target.FillRectangle(location.ToStruct<RectangleF, RawRectangleF>(), brush);
            using (var brush = new SolidColorBrush(target, t.Foreground.ToStruct<Color4, RawColor4>()))
                target.DrawTextLayout(new RawVector2(location.Left + text.Value.Padding.X, location.Top + text.Value.Padding.Y),
                    t.TextLayout, brush, DrawTextOptions.None);
        }
    }
    
    protected override KeyValuePair<int, TextLayoutInfo>[] GetArray(IEnumerable<TextInfoExt> items)
    {
        return items.Select((x, i) =>
        {
            var textLayout = BitmapExtensions
            .GetTextLayoutMetrices(x.Text, deviceRes2D, x.Size, x.FontFamily,
            x.FontWeight, x.FontStyle);
            return new KeyValuePair<int, TextLayoutInfo>(i,
                new TextLayoutInfo(textLayout, x.Foreground, x.Background, x.Padding));
        }).ToArray();
    }

    protected override Size2F GetSize(TextLayoutInfo value)
    {
        return new Size2F(value.TextLayout.Metrics.Width + value.Padding.X + value.Padding.Z,
            value.TextLayout.Metrics.Height + value.Padding.Y + value.Padding.W);
    }
}
