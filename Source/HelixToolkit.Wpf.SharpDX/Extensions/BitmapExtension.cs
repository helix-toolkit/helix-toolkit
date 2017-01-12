using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Media = System.Windows.Media;
namespace HelixToolkit.Wpf.SharpDX
{
    public static class BitmapExtension
    {
        public static BitmapSource ToBitmapSource(this TextBlock element, bool freeze = true)
        {
            var target = new RenderTargetBitmap((int)(element.RenderSize.Width), (int)(element.RenderSize.Height), 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            target.Render(element);
            if (freeze)
            {
                target.Freeze();
            }
            return target;
        }

        public static BitmapSource StringToBitmapSource(this string str, int fontSize, System.Windows.Media.Color foreground,
            System.Windows.Media.Color background)
        {
            return StringToBitmapSource(str, fontSize, foreground, background,
                new System.Windows.Media.FontFamily("Arial"));
        }

        public static BitmapSource StringToBitmapSource(this string str, int fontSize, System.Windows.Media.Color foreground,
            System.Windows.Media.Color background, Media.FontFamily fontFamily)
        {
            return StringToBitmapSource(str, fontSize, foreground, background,
               fontFamily, FontWeights.Normal);
        }

        public static BitmapSource StringToBitmapSource(this string str, int fontSize, System.Windows.Media.Color foreground,
            System.Windows.Media.Color background, Media.FontFamily fontFamily, FontWeight fontWeight)
        {
            return StringToBitmapSource(str, fontSize, foreground, background,
               fontFamily, fontWeight, FontStyles.Normal, new Thickness(0));
        }

        public static BitmapSource StringToBitmapSource(this string str, int fontSize, System.Windows.Media.Color foreground,
            System.Windows.Media.Color background, Media.FontFamily fontFamily, FontWeight fontWeight, FontStyle fontStyle, Thickness padding)
        {
            TextBlock tbX = new TextBlock();
            tbX.FontFamily = fontFamily;
            tbX.Foreground = new System.Windows.Media.SolidColorBrush(foreground);
            tbX.Background = new System.Windows.Media.SolidColorBrush(background);
            tbX.TextAlignment = TextAlignment.Center;
            tbX.FontSize = fontSize;
            tbX.FontStretch = FontStretches.Normal;
            tbX.FontWeight = fontWeight;
            tbX.FontStyle = fontStyle;
            tbX.Padding = padding;
            tbX.Text = str;
            var size = tbX.MeasureString();
            tbX.Width = size.Width;
            tbX.Height = size.Height;
            tbX.Measure(new Size(size.Width, size.Height));
            tbX.Arrange(new Rect(new Size(size.Width, size.Height)));
            return tbX.ToBitmapSource();
        }

        public static Size MeasureString(this TextBlock textBlock)
        {
            var formattedText = new Media.FormattedText(
                textBlock.Text,
                CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight,
                new Media.Typeface(textBlock.FontFamily, textBlock.FontStyle, textBlock.FontWeight, textBlock.FontStretch),
                textBlock.FontSize,
                Media.Brushes.Black);

            return new Size(formattedText.Width + textBlock.Padding.Left + textBlock.Padding.Right, formattedText.Height + textBlock.Padding.Top + textBlock.Padding.Bottom);
        }

        public static MemoryStream ToMemoryStream(this BitmapSource writeBmp)
        {
            var outStream = new MemoryStream();
            BitmapEncoder enc = new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(writeBmp));
            enc.Save(outStream);
            outStream.Position = 0;
            return outStream;
        }

        public static byte[] ToByteArray(this System.Windows.Media.Imaging.BitmapSource bitmapSource)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(ms);
                return ms.ToArray();
            }
        }
    }
}
