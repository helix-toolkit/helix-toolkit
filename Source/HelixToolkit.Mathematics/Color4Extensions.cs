/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HelixToolkit.Mathematics
{
    /// <summary>
    /// Ref https://referencesource.microsoft.com/#System.Drawing/commonui/System/Drawing/ColorConverter.cs
    /// </summary>
    public static class Color4Extensions
    {
        private static readonly Dictionary<string, object> Colors = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Initializes the <see cref="Color4Extensions"/> class.
        /// </summary>
        static Color4Extensions()
        {
            FillConstants(Colors, typeof(Color));
        }


        private static void FillConstants(Dictionary<string, object> hash, Type enumType)
        {
            //MethodAttributes attrs = MethodAttributes.Public | MethodAttributes.Static;
            var fields = enumType.GetFields();

            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (field.FieldType == typeof(Color))
                {
                    hash.Add(field.Name, field.GetValue(field));
                }
            }
        }

        /// <summary>
        /// Tries the prase.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <param name="culture">The culture.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Color4 ToColor4(this string color, CultureInfo culture = null)
        {
            string text = color.Trim();
            if(text.Length == 0)
            {
                return Color.Transparent;
            }
            else
            {
                object obj = GetNamedColor(text);
                if(obj == null)
                {
                    if(culture == null)
                    {
                        culture = CultureInfo.CurrentCulture;
                    }
                    char sep = culture.TextInfo.ListSeparator[0];
                    bool tryMappingToKnownColor = true;

                    TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));

                    // If the value is a 6 digit hex number only, then
                    // we want to treat the Alpha as 255, not 0
                    //
                    if (text.IndexOf(sep) == -1)
                    {

                        // text can be '' (empty quoted string)
                        if (text.Length >= 2 && (text[0] == '\'' || text[0] == '"') && text[0] == text[text.Length - 1])
                        {
                            // In quotes means a named value
                            string colorName = text.Substring(1, text.Length - 2);
                            obj = GetNamedColor(colorName);
                            tryMappingToKnownColor = false;
                        }
                        else if ((text.Length == 7 && text[0] == '#') ||
                                 (text.Length == 8 && (text.StartsWith("0x") || text.StartsWith("0X"))) ||
                                 (text.Length == 8 && (text.StartsWith("&h") || text.StartsWith("&H"))))
                        {
                            // Note: ConvertFromString will raise exception if value cannot be converted.
                            obj = FromArgb(unchecked((int)(0xFF000000 | (uint)(int)intConverter.ConvertFromString(text))));
                        }
                    }

                    // Nope.  Parse the RGBA from the text.
                    //
                    if (obj == null)
                    {
                        string[] tokens = text.Split(new char[] { sep });
                        int[] values = new int[tokens.Length];
                        for (int i = 0; i < values.Length; i++)
                        {
                            values[i] = unchecked((int)intConverter.ConvertFromString(tokens[i]));
                        }

                        // We should now have a number of parsed integer values.
                        // We support 1, 3, or 4 arguments:
                        //
                        // 1 -- full ARGB encoded
                        // 3 -- RGB
                        // 4 -- ARGB
                        //
                        switch (values.Length)
                        {
                            case 1:
                                obj = FromArgb(values[0]);
                                break;

                            case 3:
                                obj = FromArgb(values[0], values[1], values[2]);
                                break;

                            case 4:
                                obj = FromArgb(values[0], values[1], values[2], values[3]);
                                break;
                        }
                        tryMappingToKnownColor = true;
                    }

                    if ((obj != null) && tryMappingToKnownColor)
                    {

                        // Now check to see if this color matches one of our known colors.
                        // If it does, then substitute it.  We can only do this for "Colors"
                        // because system colors morph with user settings.
                        //
                        int targetARGB = ((Color)obj).ToArgb();

                        foreach (Color c in Colors.Values)
                        {
                            if (c.ToArgb() == targetARGB)
                            {
                                obj = c;
                                break;
                            }
                        }
                    }
                }

                if (obj == null)
                {
                    throw new ArgumentException($"Invalid Color string {text}");
                }
                return (Color)obj;
            }          
        }


        /// <summary>
        /// Froms the ARGB int.
        /// </summary>
        /// <param name="argb">The ARGB.</param>
        /// <returns></returns>
        public static Color FromArgb(this int argb)
        {
            return new Color((byte)(argb >> 16), (byte)(argb >> 8), (byte)argb, (byte)(argb >> 24));
        }


        /// <summary>
        /// Froms the RGB.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static Color FromArgb(int r, int g, int b)
        {
            return new Color((byte)r, (byte)g, (byte)b, (byte)(255));
        }

        /// <summary>
        /// Froms the ARGB.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="r">The r.</param>
        /// <param name="g">The g.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static Color FromArgb(int a, int r, int g, int b)
        {
            return new Color((byte)r, (byte)g, (byte)b, (byte)a);
        }


        public static int ToArgb(this Color color)
        {
            int value = color.B;
            value |= color.G << 8;
            value |= color.R << 16;
            value |= color.A << 24;

            return value;
        }

        //private const float encodeDiv = 1f / 16777216;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float EncodeToFloat(this Color4 color)
        {
            var ex = (uint)(color.Red * 255);
            var ey = (uint)(color.Green * 255);
            var ez = (uint)(color.Blue * 255);
            var v = (ex << 16) | (ey << 8) | ez;
            return v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Encode2FloatToFloat(float a, float b)
        {
            uint aScaled = (uint)a * 0xFFFF;
            uint bScaled = (uint)b * 0xFFFF;
            uint abPacked = (aScaled << 16) | (bScaled & 0xFFFF);
            return abPacked;
        }

        internal static object GetNamedColor(string name)
        {
            Colors.TryGetValue(name, out object color);
            return color;
        }
    }
}
