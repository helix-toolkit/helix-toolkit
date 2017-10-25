using System;
using D2D = SharpDX.Direct2D1;
using Media = System.Windows.Media;
using System.Windows;
using System.Linq;

namespace HelixToolkit.Wpf.SharpDX.Extensions
{
    public static class CommonExtensions
    {
        public static global::SharpDX.DirectWrite.FontWeight ToDXFontWeight(this System.Windows.FontWeight fontWeight)
        {
            if(fontWeight == System.Windows.FontWeights.Black)
            {
                return global::SharpDX.DirectWrite.FontWeight.Black;
            }
            else if(fontWeight == System.Windows.FontWeights.Bold)
            {
                return global::SharpDX.DirectWrite.FontWeight.Bold;
            }
            else if (fontWeight == System.Windows.FontWeights.DemiBold)
            {
                return global::SharpDX.DirectWrite.FontWeight.DemiBold;
            }
            else if (fontWeight == System.Windows.FontWeights.ExtraBlack)
            {
                return global::SharpDX.DirectWrite.FontWeight.ExtraBlack;
            }
            else if (fontWeight == System.Windows.FontWeights.ExtraBold)
            {
                return global::SharpDX.DirectWrite.FontWeight.ExtraBold;
            }
            else if (fontWeight == System.Windows.FontWeights.ExtraLight)
            {
                return global::SharpDX.DirectWrite.FontWeight.ExtraLight;
            }
            else if (fontWeight == System.Windows.FontWeights.Heavy)
            {
                return global::SharpDX.DirectWrite.FontWeight.Heavy;
            }
            else if (fontWeight == System.Windows.FontWeights.Light)
            {
                return global::SharpDX.DirectWrite.FontWeight.Light;
            }
            else if (fontWeight == System.Windows.FontWeights.Medium)
            {
                return global::SharpDX.DirectWrite.FontWeight.Medium;
            }
            else if (fontWeight == System.Windows.FontWeights.Normal)
            {
                return global::SharpDX.DirectWrite.FontWeight.Normal;
            }
            else if (fontWeight == System.Windows.FontWeights.Regular)
            {
                return global::SharpDX.DirectWrite.FontWeight.Regular;
            }
            else if (fontWeight == System.Windows.FontWeights.SemiBold)
            {
                return global::SharpDX.DirectWrite.FontWeight.SemiBold;
            }
            else if (fontWeight == System.Windows.FontWeights.Thin)
            {
                return global::SharpDX.DirectWrite.FontWeight.Thin;
            }
            else if (fontWeight == System.Windows.FontWeights.UltraBlack)
            {
                return global::SharpDX.DirectWrite.FontWeight.UltraBlack;
            }
            else if (fontWeight == System.Windows.FontWeights.UltraBold)
            {
                return global::SharpDX.DirectWrite.FontWeight.UltraBold;
            }
            else if (fontWeight == System.Windows.FontWeights.UltraLight)
            {
                return global::SharpDX.DirectWrite.FontWeight.UltraLight;
            }
            else
            {
                throw new ArgumentException("FontWeight not found.");
            }
        }

        public static global::SharpDX.DirectWrite.FontStyle ToDXFontStyle(this FontStyle style)
        {
            if(style == FontStyles.Italic)
            {
                return global::SharpDX.DirectWrite.FontStyle.Italic;
            }
            else if(style == FontStyles.Normal)
            {
                return global::SharpDX.DirectWrite.FontStyle.Normal;
            }
            else if(style == FontStyles.Oblique)
            {
                return global::SharpDX.DirectWrite.FontStyle.Oblique;
            }
            else
            {
                throw new ArgumentException("FontStyle not found.");
            }
        }

        public static D2D.ExtendMode ToD2DExtendMode(this Media.GradientSpreadMethod mode)
        {
            switch (mode)
            {
                case Media.GradientSpreadMethod.Pad:
                    return D2D.ExtendMode.Clamp;
                case Media.GradientSpreadMethod.Reflect:
                    return D2D.ExtendMode.Mirror;
                case Media.GradientSpreadMethod.Repeat:
                    return D2D.ExtendMode.Wrap;
                default:
                    throw new ArgumentException("GradientSpreadMethod cannot convert to Direct2D ExtendMode");
            }
        }

        public static D2D.Gamma ToD2DColorInterpolationMode(this Media.ColorInterpolationMode mode)
        {
            switch (mode)
            {
                case Media.ColorInterpolationMode.ScRgbLinearInterpolation:
                    return D2D.Gamma.Linear;
                case Media.ColorInterpolationMode.SRgbLinearInterpolation:
                    return D2D.Gamma.StandardRgb;
                default:
                    throw new ArgumentException("ColorInterpolationMode cannot convert to Direct2D Gama");
            }
        }

        public static D2D.Brush ToD2DBrush(this Media.Brush brush, global::SharpDX.Direct2D1.RenderTarget target)
        {
            if(brush is Media.SolidColorBrush)
            {
                return new global::SharpDX.Direct2D1.SolidColorBrush(target, (brush as Media.SolidColorBrush).Color.ToColor4());
            }
            else if(brush is Media.LinearGradientBrush)
            {
                var b = brush as Media.LinearGradientBrush;
                return new D2D.LinearGradientBrush(target,
                    new D2D.LinearGradientBrushProperties() { StartPoint = b.StartPoint.ToVector2(), EndPoint = b.EndPoint.ToVector2() },
                    new D2D.GradientStopCollection
                    (
                        target,
                        b.GradientStops.Select(x => new D2D.GradientStop() { Color = x.Color.ToColor4(), Position = (float)x.Offset }).ToArray(),
                        b.ColorInterpolationMode.ToD2DColorInterpolationMode(),
                        b.SpreadMethod.ToD2DExtendMode()
                    )
                    );
            }
            else if(brush is Media.RadialGradientBrush)
            {
                var b = brush as Media.RadialGradientBrush;
                return new D2D.RadialGradientBrush(target,
                    new D2D.RadialGradientBrushProperties() { Center = b.Center.ToVector2(), GradientOriginOffset = b.GradientOrigin.ToVector2(), RadiusX = (float)b.RadiusX, RadiusY = (float)b.RadiusY },
                    new D2D.GradientStopCollection
                    (
                        target,
                        b.GradientStops.Select(x => new D2D.GradientStop() { Color = x.Color.ToColor4(), Position = (float)x.Offset }).ToArray(),
                        b.ColorInterpolationMode.ToD2DColorInterpolationMode(),
                        b.SpreadMethod.ToD2DExtendMode()
                    ));
            }
            else
            {
                throw new NotImplementedException("Brush does not support yet.");
            }
        }
    }
}
