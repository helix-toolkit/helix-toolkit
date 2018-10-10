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
                var location = imagePlacement[text.Key];
                var t = text.Value;
                using (var brush = new SolidColorBrush(target, t.Background))
                    target.FillRectangle(location, brush);
                using (var brush = new SolidColorBrush(target, t.Foreground))
                    target.DrawTextLayout(new Vector2(location.Left, location.Top), t.TextLayout, brush, DrawTextOptions.None);
            }
        }

        protected override KeyValuePair<int, TextLayoutInfo>[] GetArray(IEnumerable<KeyValuePair<int, TextInfoExt>> items)
        {
            return items.Select(x =>
            {
                var textLayout = BitmapExtensions
                .GetTextLayoutMetrices(x.Value.Text, deviceRes2D, x.Value.Size, x.Value.FontFamily,
                x.Value.FontWeight, x.Value.FontStyle);
                return new KeyValuePair<int, TextLayoutInfo>(x.Key,
                    new TextLayoutInfo(textLayout, x.Value.Foreground, x.Value.Background, x.Value.Padding));
            }).ToArray();
        }

        protected override Size2F GetSize(TextLayoutInfo value)
        {
            return new Size2F(value.TextLayout.Metrics.Width, value.TextLayout.Metrics.Height);
        }
    }
}
