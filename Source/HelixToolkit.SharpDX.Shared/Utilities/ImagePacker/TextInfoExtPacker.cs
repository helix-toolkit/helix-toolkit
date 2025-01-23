using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;

#if !NETFX_CORE
namespace HelixToolkit.Wpf.SharpDX
#else
#if CORE
namespace HelixToolkit.SharpDX.Core
#else
namespace HelixToolkit.UWP
#endif
#endif
{
    namespace Utilities.ImagePacker
    {
        public sealed class TextLayoutInfo : IDisposable
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

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    TextLayout.Dispose();
                    disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            // ~ImagePacker() {
            //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            //   Dispose(false);
            // }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(true);
                // TODO: uncomment the following line if the finalizer is overridden above.
                // GC.SuppressFinalize(this);
            }
            #endregion
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
}
