using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

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
    }
}
