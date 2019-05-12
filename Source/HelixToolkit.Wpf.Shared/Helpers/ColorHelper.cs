// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColorHelper.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Helper methods for Color.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf
{
    using System;
    using System.Globalization;
    using System.Windows.Media;

    /// <summary>
    /// Helper methods for <see cref="Color"/>.
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// The undefined color #00000000.
        /// </summary>
        public static Color UndefinedColor = Color.FromArgb(0, 0, 0, 0);

        /// <summary>
        /// Change the alpha value of a color
        /// </summary>
        /// <param name="c">
        /// </param>
        /// <param name="alpha">
        /// </param>
        /// <returns>
        /// </returns>
        public static Color ChangeAlpha(this Color c, byte alpha)
        {
            return Color.FromArgb(alpha, c.R, c.G, c.B);
        }

        /// <summary>
        /// Changes the intensity.
        /// </summary>
        /// <param name="c">
        /// The c.
        /// </param>
        /// <param name="factor">
        /// The factor.
        /// </param>
        /// <returns>
        /// </returns>
        public static Color ChangeIntensity(this Color c, double factor)
        {
            var hsv = ColorToHsv(c);
            hsv[2] *= factor;
            if (hsv[2] > 1.0)
            {
                hsv[2] = 1.0;
            }

            return HsvToColor(hsv);
        }

        /// <summary>
        /// Calculates the difference between two <see cref="Color"/>s
        /// </summary>
        /// <param name="c1">
        /// </param>
        /// <param name="c2">
        /// </param>
        /// <returns>
        /// L2-norm in RGBA space
        /// </returns>
        public static double ColorDifference(Color c1, Color c2)
        {
            // http://en.wikipedia.org/wiki/Color_difference
            // http://mathworld.wolfram.com/L2-Norm.html
            double dr = (c1.R - c2.R) / 255.0;
            double dg = (c1.G - c2.G) / 255.0;
            double db = (c1.B - c2.B) / 255.0;
            double da = (c1.A - c2.A) / 255.0;
            double e = dr * dr + dg * dg + db * db + da * da;
            return Math.Sqrt(e);
        }

        /// <summary>
        /// Convert a <see cref="Color"/> to a hexadecimal string.
        /// </summary>
        /// <param name="color">
        /// </param>
        /// <returns>
        /// The color to hex.
        /// </returns>
        public static string ColorToHex(Color color)
        {
            return string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts from a <see cref="Color"/> to HSV values (double)
        /// </summary>
        /// <param name="color">
        /// </param>
        /// <returns>
        /// Array of [Hue,Saturation,Value] in the range [0,1]
        /// </returns>
        public static double[] ColorToHsv(Color color)
        {
            byte r = color.R;
            byte g = color.G;
            byte b = color.B;

            double h = 0, s, v;

            double min = Math.Min(Math.Min(r, g), b);
            v = Math.Max(Math.Max(r, g), b);
            double delta = v - min;

            if (v == 0.0)
            {
                s = 0;
            }
            else
            {
                s = delta / v;
            }

            if (s == 0)
            {
                h = 0.0;
            }
            else
            {
                if (r == v)
                {
                    h = (g - b) / delta;
                }
                else if (g == v)
                {
                    h = 2 + (b - r) / delta;
                }
                else if (b == v)
                {
                    h = 4 + (r - g) / delta;
                }

                h *= 60;
                if (h < 0.0)
                {
                    h = h + 360;
                }
            }

            var hsv = new double[3];
            hsv[0] = h / 360.0;
            hsv[1] = s;
            hsv[2] = v / 255.0;
            return hsv;
        }

        /// <summary>
        /// Converts from a <see cref="Color"/> to HSV values (byte)
        /// </summary>
        /// <param name="color">
        /// </param>
        /// <returns>
        /// Array of [Hue,Saturation,Value] in the range [0,255]
        /// </returns>
        public static byte[] ColorToHsvBytes(Color color)
        {
            double[] hsv1 = ColorToHsv(color);
            var hsv2 = new byte[3];
            hsv2[0] = (byte)(hsv1[0] * 255);
            hsv2[1] = (byte)(hsv1[1] * 255);
            hsv2[2] = (byte)(hsv1[2] * 255);
            return hsv2;
        }

        /// <summary>
        /// Convert a <see cref="Color"/> to unsigned int
        /// </summary>
        /// <param name="c">
        /// </param>
        /// <returns>
        /// The color to uint.
        /// </returns>
        public static uint ColorToUint(Color c)
        {
            uint u = (UInt32)c.A << 24;
            u += (UInt32)c.R << 16;
            u += (UInt32)c.G << 8;
            u += c.B;
            return u;

            // (UInt32)((UInt32)c.A << 24 + (UInt32)c.R << 16 + (UInt32)c.G << 8 + (UInt32)c.B);
        }

        /// <summary>
        /// Calculates the complementary color
        /// </summary>
        /// <param name="c">
        /// </param>
        /// <returns>
        /// </returns>
        public static Color Complementary(Color c)
        {
            // http://en.wikipedia.org/wiki/Complementary_color
            // todo...
            double[] hsv = ColorToHsv(c);
            double newHue = hsv[0] - 0.5;

            // clamp to [0,1]
            if (newHue < 0)
            {
                newHue += 1.0;
            }

            return HsvToColor(newHue, hsv[1], hsv[2]);
        }

        /// <summary>
        /// Convert a hexadecimal string to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">
        /// </param>
        /// <returns>
        /// </returns>
        public static Color HexToColor(string value)
        {
            value = value.Trim('#');
            if (value.Length == 0)
            {
                return UndefinedColor;
            }

            if (value.Length <= 6)
            {
                value = "FF" + value.PadLeft(6, '0');
            }

            uint u;
            if (uint.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out u))
            {
                return UIntToColor(u);
            }

            return UndefinedColor;
        }

        /// <summary>
        /// Converts from HSV to a RGB <see cref="Color"/>
        /// </summary>
        /// <param name="hue">
        /// The hue.
        /// </param>
        /// <param name="saturation">
        /// The saturation.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// </returns>
        public static Color HsvToColor(byte hue, byte saturation, byte value)
        {
            double r, g, b;
            double h = hue * 360.0 / 255;
            double s = saturation / 255.0;
            double v = value / 255.0;

            if (s == 0)
            {
                r = v;
                g = v;
                b = v;
            }
            else
            {
                int i;
                double f, p, q, t;

                if (h == 360)
                {
                    h = 0;
                }
                else
                {
                    h = h / 60;
                }

                i = (int)Math.Truncate(h);
                f = h - i;

                p = v * (1.0 - s);
                q = v * (1.0 - (s * f));
                t = v * (1.0 - (s * (1.0 - f)));

                switch (i)
                {
                    case 0:
                        r = v;
                        g = t;
                        b = p;
                        break;

                    case 1:
                        r = q;
                        g = v;
                        b = p;
                        break;

                    case 2:
                        r = p;
                        g = v;
                        b = t;
                        break;

                    case 3:
                        r = p;
                        g = q;
                        b = v;
                        break;

                    case 4:
                        r = t;
                        g = p;
                        b = v;
                        break;

                    default:
                        r = v;
                        g = p;
                        b = q;
                        break;
                }
            }

            return Color.FromArgb(255, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        /// <summary>
        /// Create a color from the specified HSV.
        /// </summary>
        /// <param name="hsv">
        /// The HSV.
        /// </param>
        /// <returns>
        /// A color.
        /// </returns>
        public static Color HsvToColor(double[] hsv)
        {
            if (hsv.Length != 3)
            {
                throw new InvalidOperationException("Wrong length of hsv array.");
            }

            return HsvToColor(hsv[0], hsv[1], hsv[2]);
        }

        /// <summary>
        /// Convert from HSV to <see cref="Color"/>
        /// http://en.wikipedia.org/wiki/HSL_color_space
        /// </summary>
        /// <param name="hue">
        /// Hue [0,1]
        /// </param>
        /// <param name="sat">
        /// Saturation [0,1]
        /// </param>
        /// <param name="val">
        /// Value [0,1]
        /// </param>
        /// <returns>
        /// </returns>
        public static Color HsvToColor(double hue, double sat, double val)
        {
            int i;
            double aa, bb, cc, f;
            double r, g, b;
            r = g = b = 0;

            if (sat == 0)
            {
                // Gray scale
                r = g = b = val;
            }
            else
            {
                if (hue == 1.0)
                {
                    hue = 0;
                }

                hue *= 6.0;
                i = (int)Math.Floor(hue);
                f = hue - i;
                aa = val * (1 - sat);
                bb = val * (1 - (sat * f));
                cc = val * (1 - (sat * (1 - f)));
                switch (i)
                {
                    case 0:
                        r = val;
                        g = cc;
                        b = aa;
                        break;
                    case 1:
                        r = bb;
                        g = val;
                        b = aa;
                        break;
                    case 2:
                        r = aa;
                        g = val;
                        b = cc;
                        break;
                    case 3:
                        r = aa;
                        g = bb;
                        b = val;
                        break;
                    case 4:
                        r = cc;
                        g = aa;
                        b = val;
                        break;
                    case 5:
                        r = val;
                        g = aa;
                        b = bb;
                        break;
                }
            }

            return Color.FromRgb((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }

        /// <summary>
        /// Calculate the difference in hue between two <see cref="Color"/>s.
        /// </summary>
        /// <param name="c1">
        /// </param>
        /// <param name="c2">
        /// </param>
        /// <returns>
        /// The hue difference.
        /// </returns>
        public static double HueDifference(Color c1, Color c2)
        {
            double[] hsv1 = ColorToHsv(c1);
            double[] hsv2 = ColorToHsv(c2);
            double dh = hsv1[0] - hsv2[0];

            // clamp to [-0.5,0.5]
            if (dh > 0.5)
            {
                dh -= 1.0;
            }

            if (dh < -0.5)
            {
                dh += 1.0;
            }

            double e = dh * dh;
            return Math.Sqrt(e);
        }

        /// <summary>
        /// Linear interpolation between two <see cref="Color"/>s.
        /// </summary>
        /// <param name="c0">
        /// </param>
        /// <param name="c1">
        /// </param>
        /// <param name="x">
        /// </param>
        /// <returns>
        /// </returns>
        public static Color Interpolate(Color c0, Color c1, double x)
        {
            double r = c0.R * (1 - x) + c1.R * x;
            double g = c0.G * (1 - x) + c1.G * x;
            double b = c0.B * (1 - x) + c1.B * x;
            double a = c0.A * (1 - x) + c1.A * x;
            return Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }

        /// <summary>
        /// Convert an unsigned int (32bit) to <see cref="Color"/>
        /// </summary>
        /// <param name="color">
        /// </param>
        /// <returns>
        /// </returns>
        public static Color UIntToColor(uint color)
        {
            var a = (byte)(color >> 24);
            var r = (byte)(color >> 16);
            var g = (byte)(color >> 8);
            var b = (byte)(color >> 0);
            return Color.FromArgb(a, r, g, b);
        }

    }
}