using D2D = SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;

#if false
#elif WINUI
using Media = Microsoft.UI.Xaml.Media;
#elif WPF
using System.Windows;
using Media = System.Windows.Media;
#else
#error Unknown framework
#endif

#if false
#elif WINUI
namespace HelixToolkit.WinUI.SharpDX.Extensions;
#elif WPF
namespace HelixToolkit.Wpf.SharpDX.Extensions;
#else
#error Unknown framework
#endif

public static class CommonExtensions
{
    public static global::SharpDX.DirectWrite.FontWeight ToDXFontWeight(this FontWeight fontWeight)
    {
#if false
#elif WINUI
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
#elif WPF
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
#else
#error Unknown framework
#endif
    }

    public static global::SharpDX.DirectWrite.FontStyle ToDXFontStyle(this FontStyle style)
    {
#if false
#elif WINUI
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
#elif WPF
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
#else
#error Unknown framework
#endif
    }

    public static D2D.ExtendMode ToD2DExtendMode(this Media.GradientSpreadMethod mode)
    {
        return mode switch
        {
            Media.GradientSpreadMethod.Pad => D2D.ExtendMode.Clamp,
            Media.GradientSpreadMethod.Reflect => D2D.ExtendMode.Mirror,
            Media.GradientSpreadMethod.Repeat => D2D.ExtendMode.Wrap,
            _ => D2D.ExtendMode.Wrap,
        };
    }

    public static D2D.Gamma ToD2DColorInterpolationMode(this Media.ColorInterpolationMode mode)
    {
        return mode switch
        {
            Media.ColorInterpolationMode.ScRgbLinearInterpolation => D2D.Gamma.Linear,
            Media.ColorInterpolationMode.SRgbLinearInterpolation => D2D.Gamma.StandardRgb,
            _ => D2D.Gamma.Linear,
        };
    }

    public static D2D.Brush ToD2DBrush(this Media.Brush brush, global::SharpDX.Direct2D1.RenderTarget target)
    {
        if (brush is Media.SolidColorBrush solid)
        {
            return new global::SharpDX.Direct2D1.SolidColorBrush(target, solid.Color.ToColor4().ToStruct<Color4, RawColor4>());
        }
        else if (brush is Media.LinearGradientBrush linear)
        {
            return new D2D.LinearGradientBrush(target,
                new D2D.LinearGradientBrushProperties() { StartPoint = linear.StartPoint.ToRawVector2(), EndPoint = linear.EndPoint.ToRawVector2() },
                new D2D.GradientStopCollection
                (
                    target,
                    linear.GradientStops.Select(x => new D2D.GradientStop() { Color = x.Color.ToRawColor4(), Position = (float)x.Offset }).ToArray(),
                    linear.ColorInterpolationMode.ToD2DColorInterpolationMode(),
                    linear.SpreadMethod.ToD2DExtendMode()
                )
                );
        }
#if false
#elif WINUI
#elif WPF
        else if (brush is Media.RadialGradientBrush radial)
        {
            return new D2D.RadialGradientBrush(target,
                new D2D.RadialGradientBrushProperties()
                {
                    Center = radial.Center.ToRawVector2(),
                    GradientOriginOffset = radial.GradientOrigin.ToRawVector2(),
                    RadiusX = (float)radial.RadiusX,
                    RadiusY = (float)radial.RadiusY
                },
                new D2D.GradientStopCollection
                (
                    target,
                    radial.GradientStops.Select(x => new D2D.GradientStop() { Color = x.Color.ToRawColor4(), Position = (float)x.Offset }).ToArray(),
                    radial.ColorInterpolationMode.ToD2DColorInterpolationMode(),
                    radial.SpreadMethod.ToD2DExtendMode()
                ));
        }
#else
#error Unknown framework
#endif
        else
        {
            throw new NotImplementedException("Brush does not support yet.");
        }
    }

    public static D2D.CapStyle ToD2DCapStyle(this Media.PenLineCap cap)
    {
        return cap switch
        {
            Media.PenLineCap.Flat => D2D.CapStyle.Flat,
            Media.PenLineCap.Round => D2D.CapStyle.Round,
            Media.PenLineCap.Square => D2D.CapStyle.Square,
            Media.PenLineCap.Triangle => D2D.CapStyle.Triangle,
            _ => D2D.CapStyle.Flat,
        };
    }

    public static D2D.LineJoin ToD2DLineJoin(this Media.PenLineJoin lineJoin)
    {
        return lineJoin switch
        {
            Media.PenLineJoin.Bevel => D2D.LineJoin.Bevel,
            Media.PenLineJoin.Miter => D2D.LineJoin.Miter,
            Media.PenLineJoin.Round => D2D.LineJoin.Round,
            _ => D2D.LineJoin.Bevel,
        };
    }

#if false
#elif WINUI
    public static D2D.DashStyle ToD2DDashStyle(this D2D.DashStyle style)
    {
        return style;
    }
#elif WPF
    public static D2D.DashStyle ToD2DDashStyle(this Media.DashStyle? style)
    {
        if (style is null)
        {
            return D2D.DashStyle.Solid;
        }

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
#error Unknown framework
#endif
    public static global::SharpDX.DirectWrite.TextAlignment ToD2DTextAlignment(this TextAlignment alignment)
    {
        return alignment switch
        {
            TextAlignment.Center => global::SharpDX.DirectWrite.TextAlignment.Center,
            TextAlignment.Left => global::SharpDX.DirectWrite.TextAlignment.Leading,
            TextAlignment.Right => global::SharpDX.DirectWrite.TextAlignment.Trailing,
            TextAlignment.Justify => global::SharpDX.DirectWrite.TextAlignment.Justified,
            _ => global::SharpDX.DirectWrite.TextAlignment.Leading,
        };
    }

    public static global::SharpDX.DirectWrite.FlowDirection ToD2DFlowDir(this FlowDirection direction)
    {
        return direction switch
        {
            FlowDirection.LeftToRight => global::SharpDX.DirectWrite.FlowDirection.LeftToRight,
            FlowDirection.RightToLeft => global::SharpDX.DirectWrite.FlowDirection.RightToLeft,
            _ => global::SharpDX.DirectWrite.FlowDirection.LeftToRight,
        };
    }
}
