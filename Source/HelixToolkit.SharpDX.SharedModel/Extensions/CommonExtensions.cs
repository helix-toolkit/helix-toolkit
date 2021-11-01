/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
#if !CORE
using System;
using D2D = SharpDX.Direct2D1;
#if NETFX_CORE 
using Windows.UI.Text;
using Media = Windows.UI.Xaml.Media;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.Foundation;
#elif WINUI
using Windows.UI.Text;
using Microsoft.UI.Text;
using Media = Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
#else
using Media = System.Windows.Media;
using System.Windows;
#endif
using System.Linq;

#if NETFX_CORE

namespace HelixToolkit.UWP.Extensions
#elif WINUI
namespace HelixToolkit.WinUI.Extensions
#else
namespace HelixToolkit.Wpf.SharpDX.Extensions
#endif
{
    public static class CommonExtensions
    {
        public static global::SharpDX.DirectWrite.FontWeight ToDXFontWeight(this FontWeight fontWeight)
        {
#if NETFX_CORE || WINUI
            var w = fontWeight.Weight;
            if (w == FontWeights.Black.Weight)
            {
                return global::SharpDX.DirectWrite.FontWeight.Black;
            }
            else if (w == FontWeights.Bold.Weight)
            {
                return global::SharpDX.DirectWrite.FontWeight.Bold;
            }
            else if (w == FontWeights.ExtraBlack.Weight)
            {
                return global::SharpDX.DirectWrite.FontWeight.ExtraBlack;
            }
            else if (w == FontWeights.ExtraBold.Weight)
            {
                return global::SharpDX.DirectWrite.FontWeight.ExtraBold;
            }
            else if (w == FontWeights.ExtraLight.Weight)
            {
                return global::SharpDX.DirectWrite.FontWeight.ExtraLight;
            }
            else if (w == FontWeights.Light.Weight)
            {
                return global::SharpDX.DirectWrite.FontWeight.Light;
            }
            else if (w == FontWeights.Medium.Weight)
            {
                return global::SharpDX.DirectWrite.FontWeight.Medium;
            }
            else if (w == FontWeights.Normal.Weight)
            {
                return global::SharpDX.DirectWrite.FontWeight.Normal;
            }
            else if (w == FontWeights.SemiBold.Weight)
            {
                return global::SharpDX.DirectWrite.FontWeight.SemiBold;
            }
            else if (w == FontWeights.Thin.Weight)
            {
                return global::SharpDX.DirectWrite.FontWeight.Thin;
            }
            else
            {
                return global::SharpDX.DirectWrite.FontWeight.Normal;
            }
#else
            if (fontWeight == FontWeights.Black)
            {
                return global::SharpDX.DirectWrite.FontWeight.Black;
            }
            else if (fontWeight == FontWeights.Bold)
            {
                return global::SharpDX.DirectWrite.FontWeight.Bold;
            }
            else if (fontWeight == FontWeights.DemiBold)
            {
                return global::SharpDX.DirectWrite.FontWeight.DemiBold;
            }
            else if (fontWeight == FontWeights.ExtraBlack)
            {
                return global::SharpDX.DirectWrite.FontWeight.ExtraBlack;
            }
            else if (fontWeight == FontWeights.ExtraBold)
            {
                return global::SharpDX.DirectWrite.FontWeight.ExtraBold;
            }
            else if (fontWeight == FontWeights.ExtraLight)
            {
                return global::SharpDX.DirectWrite.FontWeight.ExtraLight;
            }
            else if (fontWeight == FontWeights.Heavy)
            {
                return global::SharpDX.DirectWrite.FontWeight.Heavy;
            }
            else if (fontWeight == FontWeights.Light)
            {
                return global::SharpDX.DirectWrite.FontWeight.Light;
            }
            else if (fontWeight == FontWeights.Medium)
            {
                return global::SharpDX.DirectWrite.FontWeight.Medium;
            }
            else if (fontWeight == FontWeights.Normal)
            {
                return global::SharpDX.DirectWrite.FontWeight.Normal;
            }
            else if (fontWeight == FontWeights.Regular)
            {
                return global::SharpDX.DirectWrite.FontWeight.Regular;
            }
            else if (fontWeight == FontWeights.SemiBold)
            {
                return global::SharpDX.DirectWrite.FontWeight.SemiBold;
            }
            else if (fontWeight == FontWeights.Thin)
            {
                return global::SharpDX.DirectWrite.FontWeight.Thin;
            }
            else if (fontWeight == FontWeights.UltraBlack)
            {
                return global::SharpDX.DirectWrite.FontWeight.UltraBlack;
            }
            else if (fontWeight == FontWeights.UltraBold)
            {
                return global::SharpDX.DirectWrite.FontWeight.UltraBold;
            }
            else if (fontWeight == FontWeights.UltraLight)
            {
                return global::SharpDX.DirectWrite.FontWeight.UltraLight;
            }
            else
            {
                return global::SharpDX.DirectWrite.FontWeight.Normal;
            }
#endif
        }

        public static global::SharpDX.DirectWrite.FontStyle ToDXFontStyle(this FontStyle style)
        {
#if NETFX_CORE || WINUI
            if (style == FontStyle.Italic)
            {
                return global::SharpDX.DirectWrite.FontStyle.Italic;
            }
            else if (style == FontStyle.Normal)
            {
                return global::SharpDX.DirectWrite.FontStyle.Normal;
            }
            else if (style == FontStyle.Oblique)
            {
                return global::SharpDX.DirectWrite.FontStyle.Oblique;
            }
            else
            {
                return global::SharpDX.DirectWrite.FontStyle.Normal;
            }
#else
            if (style == FontStyles.Italic)
            {
                return global::SharpDX.DirectWrite.FontStyle.Italic;
            }
            else if (style == FontStyles.Normal)
            {
                return global::SharpDX.DirectWrite.FontStyle.Normal;
            }
            else if (style == FontStyles.Oblique)
            {
                return global::SharpDX.DirectWrite.FontStyle.Oblique;
            }
            else
            {
                return global::SharpDX.DirectWrite.FontStyle.Normal;
            }
#endif
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
                    return D2D.ExtendMode.Wrap;
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
                    return D2D.Gamma.Linear;
            }
        }

        public static D2D.Brush ToD2DBrush(this Media.Brush brush, global::SharpDX.Direct2D1.RenderTarget target)
        {
            if (brush is Media.SolidColorBrush solid)
            {
                return new global::SharpDX.Direct2D1.SolidColorBrush(target, solid.Color.ToColor4());
            }
            else if (brush is Media.LinearGradientBrush linear)
            {
                return new D2D.LinearGradientBrush(target,
                    new D2D.LinearGradientBrushProperties() { StartPoint = linear.StartPoint.ToVector2(), EndPoint = linear.EndPoint.ToVector2() },
                    new D2D.GradientStopCollection
                    (
                        target,
                        linear.GradientStops.Select(x => new D2D.GradientStop() { Color = x.Color.ToColor4(), Position = (float)x.Offset }).ToArray(),
                        linear.ColorInterpolationMode.ToD2DColorInterpolationMode(),
                        linear.SpreadMethod.ToD2DExtendMode()
                    )
                    );
            }
#if NETFX_CORE || WINUI
#else
            else if (brush is Media.RadialGradientBrush radial)
            {
                return new D2D.RadialGradientBrush(target,
                    new D2D.RadialGradientBrushProperties()
                    {
                        Center = radial.Center.ToVector2(),
                        GradientOriginOffset = radial.GradientOrigin.ToVector2(),
                        RadiusX = (float)radial.RadiusX,
                        RadiusY = (float)radial.RadiusY
                    },
                    new D2D.GradientStopCollection
                    (
                        target,
                        radial.GradientStops.Select(x => new D2D.GradientStop() { Color = x.Color.ToColor4(), Position = (float)x.Offset }).ToArray(),
                        radial.ColorInterpolationMode.ToD2DColorInterpolationMode(),
                        radial.SpreadMethod.ToD2DExtendMode()
                    ));
            }
#endif
            else
            {
                throw new NotImplementedException("Brush does not support yet.");
            }
        }

        public static D2D.CapStyle ToD2DCapStyle(this Media.PenLineCap cap)
        {
            switch (cap)
            {
                case Media.PenLineCap.Flat:
                    return D2D.CapStyle.Flat;
                case Media.PenLineCap.Round:
                    return D2D.CapStyle.Round;
                case Media.PenLineCap.Square:
                    return D2D.CapStyle.Square;
                case Media.PenLineCap.Triangle:
                    return D2D.CapStyle.Triangle;
                default:
                    return D2D.CapStyle.Flat;
            }
        }

        public static D2D.LineJoin ToD2DLineJoin(this Media.PenLineJoin lineJoin)
        {
            switch (lineJoin)
            {
                case Media.PenLineJoin.Bevel:
                    return D2D.LineJoin.Bevel;
                case Media.PenLineJoin.Miter:
                    return D2D.LineJoin.Miter;
                case Media.PenLineJoin.Round:
                    return D2D.LineJoin.Round;
                default:
                    return D2D.LineJoin.Bevel;
            }
        }
#if !NETFX_CORE && !WINUI
        public static D2D.DashStyle ToD2DDashStyle(this Media.DashStyle style)
        {
            if (style == Media.DashStyles.Dash)
            {
                return D2D.DashStyle.Dash;
            }
            else if (style == Media.DashStyles.DashDot)
            {
                return D2D.DashStyle.DashDot;
            }
            else if (style == Media.DashStyles.DashDotDot)
            {
                return D2D.DashStyle.DashDotDot;
            }
            else if (style == Media.DashStyles.Dot)
            {
                return D2D.DashStyle.Dot;
            }
            else
            {
                return D2D.DashStyle.Solid;
            }
        }
#else

#endif

        public static global::SharpDX.DirectWrite.TextAlignment ToD2DTextAlignment(this TextAlignment alignment)
        {
            switch (alignment)
            {
                case TextAlignment.Center:
                    return global::SharpDX.DirectWrite.TextAlignment.Center;
                case TextAlignment.Left:
                    return global::SharpDX.DirectWrite.TextAlignment.Leading;
                case TextAlignment.Right:
                    return global::SharpDX.DirectWrite.TextAlignment.Trailing;
                case TextAlignment.Justify:
                    return global::SharpDX.DirectWrite.TextAlignment.Justified;
                default:
                    return global::SharpDX.DirectWrite.TextAlignment.Leading;
            }
        }

        public static global::SharpDX.DirectWrite.FlowDirection ToD2DFlowDir(this FlowDirection direction)
        {
            switch (direction)
            {
                case FlowDirection.LeftToRight:
                    return global::SharpDX.DirectWrite.FlowDirection.LeftToRight;
                case FlowDirection.RightToLeft:
                    return global::SharpDX.DirectWrite.FlowDirection.RightToLeft;
                default:
                    return global::SharpDX.DirectWrite.FlowDirection.LeftToRight;
            }
        }
    }
}
#endif