using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Media = System.Windows.Media;
namespace HelixToolkit.Wpf.SharpDX.Extensions
{
    internal static class BitmapExtension
    {
        public static BitmapSource ToBitmapSource(this TextBlock element, bool freeze = true)
        {
            var target = new RenderTargetBitmap((int)(element.Width), (int)(element.Height), 96, 96, System.Windows.Media.PixelFormats.Pbgra32);
            var brush = new System.Windows.Media.VisualBrush(element);

            var visual = new System.Windows.Media.DrawingVisual();
            var drawingContext = visual.RenderOpen();


            drawingContext.DrawRectangle(brush, null, new Rect(new System.Windows.Point(0, 0),
                new System.Windows.Point(element.Width, element.Height)));

            drawingContext.Close();
            target.Render(visual);
            if (freeze)
            {
                target.Freeze();
            }
            return target;
        }

        public static BitmapSource StringToBitmapSource(this string str, int fontSize, System.Windows.Media.Color foreground, System.Windows.Media.Color background)
        {
            TextBlock tbX = new TextBlock();
            tbX.FontFamily = new System.Windows.Media.FontFamily("Consolas");
            tbX.Foreground = new System.Windows.Media.SolidColorBrush(foreground);
            tbX.Background = new System.Windows.Media.SolidColorBrush(background);
            tbX.TextAlignment = TextAlignment.Center;
            tbX.FontSize = fontSize;
            tbX.FontStretch = FontStretches.Normal;
            tbX.FontWeight = FontWeights.Medium;
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

            return new Size(formattedText.Width, formattedText.Height);
        }
    }
}
