/*
The MIT License (MIT)
Copyright (c) 2022 Helix Toolkit contributors

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
 
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.

Original code from:
SharpDX project. https://github.com/sharpdx/SharpDX
SlimMath project. http://code.google.com/p/slimmath/

Copyright (c) 2010-2014 SharpDX - Alexandre Mutel
The MIT License (MIT)
Copyright (c) 2007-2011 SlimDX Group
The MIT License (MIT)
*/
using System.Runtime.InteropServices;

namespace HelixToolkit.Maths
{
    /// <summary>
    /// Helper class to perform Half/Float conversion.
    /// Code extract from paper : www.fox-toolkit.org/ftp/fasthalffloatconversion.pdf by Jeroen van der Zijp
    /// </summary>
    internal class HalfUtils
    {

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatToUint
        {
            [FieldOffset(0)]
            public uint uintValue;
            [FieldOffset(0)]
            public float floatValue;
        }

        /// <summary>
        /// Unpacks the specified h.
        /// </summary>
        /// <param name="h">The h.</param>
        /// <returns></returns>
        public static float Unpack(ushort h)
        {
            var conv = new FloatToUint();
            conv.uintValue = halfToFloatMantissaTable_[halfToFloatOffsetTable_[h >> 10] + (((uint)h) & 0x3ff)] + halfToFloatExponentTable_[h >> 10];
            return conv.floatValue;
        }

        /// <summary>
        /// Packs the specified f.
        /// </summary>
        /// <param name="f">The f.</param>
        /// <returns></returns>
        public static ushort Pack(float f)
        {
            var conv = new FloatToUint();
            conv.floatValue = f;
            return (ushort)(floatToHalfBaseTable_[(conv.uintValue >> 23) & 0x1ff] + ((conv.uintValue & 0x007fffff) >> floatToHalfShiftTable_[(conv.uintValue >> 23) & 0x1ff]));
        }

        static readonly uint[] halfToFloatMantissaTable_ = new uint[2048];
        static readonly uint[] halfToFloatExponentTable_ = new uint[64];
        static readonly uint[] halfToFloatOffsetTable_ = new uint[64];
        static readonly ushort[] floatToHalfBaseTable_ = new ushort[512];
        static readonly byte[] floatToHalfShiftTable_ = new byte[512];

        static HalfUtils()
        {
            int i;

            // -------------------------------------------------------------------
            // Half to Float tables
            // -------------------------------------------------------------------

            // Mantissa table

            // 0 => 0
            halfToFloatMantissaTable_[0] = 0;

            // Transform subnormal to normalized
            for (i = 1; i < 1024; i++)
            {
                var m = ((uint)i) << 13;
                uint e = 0;

                while ((m & 0x00800000) == 0)
                {
                    e -= 0x00800000;
                    m <<= 1;
                }
                m &= ~ 0x00800000U;
                e += 0x38800000;
                halfToFloatMantissaTable_[i] = m | e;
            }

            // Normal case
            for (i = 1024; i < 2048; i++)
            {
                halfToFloatMantissaTable_[i] = 0x38000000 + (((uint)(i - 1024)) << 13);
            }

            // Exponent table

            // 0 => 0
            halfToFloatExponentTable_[0] = 0;

            for (i = 1; i < 63; i++)
            {
                if (i < 31) // Positive Numbers
                {
                    halfToFloatExponentTable_[i] = ((uint)i) << 23;
                }
                else // Negative Numbers
                {
                    halfToFloatExponentTable_[i] = 0x80000000 + (((uint)(i - 32)) << 23);
                }
            }
            halfToFloatExponentTable_[31] = 0x47800000;
            halfToFloatExponentTable_[32] = 0x80000000;
            halfToFloatExponentTable_[63] = 0xC7800000;

            // Offset table
            halfToFloatOffsetTable_[0] = 0;
            for (i = 1; i < 64; i++)
            {
                halfToFloatOffsetTable_[i] = 1024;
            }

            halfToFloatOffsetTable_[32] = 0;

            // -------------------------------------------------------------------
            // Float to Half tables
            // -------------------------------------------------------------------
#pragma warning disable S2437 // Silly bit operations should not be performed       
            for (i = 0; i < 256; i++)
            {
                var e = i - 127;
                if (e < -24)
                { // Very small numbers map to zero

                    floatToHalfBaseTable_[i | 0x000] = 0x0000;
                    floatToHalfBaseTable_[i | 0x100] = 0x8000;
                    floatToHalfShiftTable_[i | 0x000] = 24;
                    floatToHalfShiftTable_[i | 0x100] = 24;
                }
                else if (e < -14)
                { // Small numbers map to denorms
                    floatToHalfBaseTable_[i | 0x000] = (ushort)(0x0400 >> (-e - 14));
                    floatToHalfBaseTable_[i | 0x100] = (ushort)((0x0400 >> (-e - 14)) | 0x8000);
                    floatToHalfShiftTable_[i | 0x000] = (byte)(-e - 1);
                    floatToHalfShiftTable_[i | 0x100] = (byte)(-e - 1);
                }
                else if (e <= 15)
                { // Normal numbers just lose precision
                    floatToHalfBaseTable_[i | 0x000] = (ushort)((e + 15) << 10);
                    floatToHalfBaseTable_[i | 0x100] = (ushort)(((e + 15) << 10) | 0x8000);
                    floatToHalfShiftTable_[i | 0x000] = 13;
                    floatToHalfShiftTable_[i | 0x100] = 13;
                }
                else if (e < 128)
                { // Large numbers map to Infinity
                    floatToHalfBaseTable_[i | 0x000] = 0x7C00;
                    floatToHalfBaseTable_[i | 0x100] = 0xFC00;
                    floatToHalfShiftTable_[i | 0x000] = 24;
                    floatToHalfShiftTable_[i | 0x100] = 24;
                }
                else
                { // Infinity and NaN's stay Infinity and NaN's
                    floatToHalfBaseTable_[i | 0x000] = 0x7C00;
                    floatToHalfBaseTable_[i | 0x100] = 0xFC00;
                    floatToHalfShiftTable_[i | 0x000] = 13;
                    floatToHalfShiftTable_[i | 0x100] = 13;
                }
            }
#pragma warning restore S2437 // Silly bit operations should not be performed
        }
    }
}
