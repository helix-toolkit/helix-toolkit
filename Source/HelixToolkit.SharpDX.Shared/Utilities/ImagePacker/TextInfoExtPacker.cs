using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX.Utilities.ImagePacker
#else
namespace HelixToolkit.UWP.Utilities.ImagePacker
#endif
{
    public struct TextLayoutInfo
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
                using (var brush = new SolidColorBrush(target, t.Background))
                    target.FillRectangle(location, brush);
                using (var brush = new SolidColorBrush(target, t.Foreground))
                    target.DrawTextLayout(new Vector2(location.Left + text.Value.Padding.X, location.Top + text.Value.Padding.Y),
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
}
