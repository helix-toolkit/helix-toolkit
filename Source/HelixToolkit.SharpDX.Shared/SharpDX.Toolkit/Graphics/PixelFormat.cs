/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Runtime.InteropServices;

using SharpDX.DXGI;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// PixelFormat is equivalent to <see cref="SharpDX.DXGI.Format"/>.
    /// </summary>
    /// <remarks>
    /// This structure is implicitly castable to and from <see cref="SharpDX.DXGI.Format"/>, you can use it inplace where <see cref="SharpDX.DXGI.Format"/> is required
    /// and vice-versa.
    /// Usage is slightly different from <see cref="SharpDX.DXGI.Format"/>, as you have to select the type of the pixel format first
    /// and then access the available pixel formats for this type. Example: PixelFormat.UNorm.R8.
    /// </remarks>
    /// <msdn-id>bb173059</msdn-id>	
    /// <unmanaged>DXGI_FORMAT</unmanaged>	
    /// <unmanaged-short>DXGI_FORMAT</unmanaged-short>	
    [StructLayout(LayoutKind.Sequential, Size = 4)]
    public struct PixelFormat : IEquatable<PixelFormat>
    {
        /// <summary>
        /// Gets the value as a <see cref="SharpDX.DXGI.Format"/> enum.
        /// </summary>
        public readonly Format Value;

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <param name="format"></param>
        private PixelFormat(Format format)
        {
            this.Value = format;
        }

        /// <summary>
        /// 
        /// </summary>
        public int SizeInBytes { get { return (int)FormatHelper.SizeOfInBytes(this); } }

        /// <summary>
        /// 
        /// </summary>
        public static readonly PixelFormat Unknown = new PixelFormat(Format.Unknown);

        /// <summary>
        /// 
        /// </summary>
        public static class A8
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.A8_UNorm);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class B5G5R5A1
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.B5G5R5A1_UNorm);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class B5G6R5
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.B5G6R5_UNorm);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class B8G8R8A8
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.B8G8R8A8_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.B8G8R8A8_UNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNormSRgb = new PixelFormat(Format.B8G8R8A8_UNorm_SRgb);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class B8G8R8X8
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.B8G8R8X8_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.B8G8R8X8_UNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNormSRgb = new PixelFormat(Format.B8G8R8X8_UNorm_SRgb);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class BC1
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.BC1_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.BC1_UNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNormSRgb = new PixelFormat(Format.BC1_UNorm_SRgb);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class BC2
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.BC2_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.BC2_UNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNormSRgb = new PixelFormat(Format.BC2_UNorm_SRgb);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class BC3
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.BC3_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.BC3_UNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNormSRgb = new PixelFormat(Format.BC3_UNorm_SRgb);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class BC4
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SNorm = new PixelFormat(Format.BC4_SNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.BC4_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.BC4_UNorm);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class BC5
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SNorm = new PixelFormat(Format.BC5_SNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.BC5_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.BC5_UNorm);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class BC6H
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.BC6H_Typeless);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class BC7
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.BC7_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.BC7_UNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNormSRgb = new PixelFormat(Format.BC7_UNorm_SRgb);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R10G10B10A2
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.R10G10B10A2_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UInt = new PixelFormat(Format.R10G10B10A2_UInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.R10G10B10A2_UNorm);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R11G11B10
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Float = new PixelFormat(Format.R11G11B10_Float);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R16
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Float = new PixelFormat(Format.R16_Float);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SInt = new PixelFormat(Format.R16_SInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SNorm = new PixelFormat(Format.R16_SNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.R16_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UInt = new PixelFormat(Format.R16_UInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.R16_UNorm);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R16G16
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Float = new PixelFormat(Format.R16G16_Float);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SInt = new PixelFormat(Format.R16G16_SInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SNorm = new PixelFormat(Format.R16G16_SNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.R16G16_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UInt = new PixelFormat(Format.R16G16_UInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.R16G16_UNorm);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R16G16B16A16
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Float = new PixelFormat(Format.R16G16B16A16_Float);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SInt = new PixelFormat(Format.R16G16B16A16_SInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SNorm = new PixelFormat(Format.R16G16B16A16_SNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.R16G16B16A16_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UInt = new PixelFormat(Format.R16G16B16A16_UInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.R16G16B16A16_UNorm);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R32
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Float = new PixelFormat(Format.R32_Float);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SInt = new PixelFormat(Format.R32_SInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.R32_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UInt = new PixelFormat(Format.R32_UInt);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R32G32
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Float = new PixelFormat(Format.R32G32_Float);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SInt = new PixelFormat(Format.R32G32_SInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.R32G32_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UInt = new PixelFormat(Format.R32G32_UInt);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R32G32B32
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Float = new PixelFormat(Format.R32G32B32_Float);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SInt = new PixelFormat(Format.R32G32B32_SInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.R32G32B32_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UInt = new PixelFormat(Format.R32G32B32_UInt);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R32G32B32A32
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Float = new PixelFormat(Format.R32G32B32A32_Float);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SInt = new PixelFormat(Format.R32G32B32A32_SInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.R32G32B32A32_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UInt = new PixelFormat(Format.R32G32B32A32_UInt);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R8
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SInt = new PixelFormat(Format.R8_SInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SNorm = new PixelFormat(Format.R8_SNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.R8_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UInt = new PixelFormat(Format.R8_UInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.R8_UNorm);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R8G8
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SInt = new PixelFormat(Format.R8G8_SInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SNorm = new PixelFormat(Format.R8G8_SNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.R8G8_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UInt = new PixelFormat(Format.R8G8_UInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.R8G8_UNorm);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        public static class R8G8B8A8
        {
            #region Constants and Fields

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SInt = new PixelFormat(Format.R8G8B8A8_SInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat SNorm = new PixelFormat(Format.R8G8B8A8_SNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat Typeless = new PixelFormat(Format.R8G8B8A8_Typeless);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UInt = new PixelFormat(Format.R8G8B8A8_UInt);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNorm = new PixelFormat(Format.R8G8B8A8_UNorm);

            /// <summary>
            /// 
            /// </summary>
            public static readonly PixelFormat UNormSRgb = new PixelFormat(Format.R8G8B8A8_UNorm_SRgb);

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        public static implicit operator Format(PixelFormat from)
        {
            return from.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        public static implicit operator PixelFormat(Format from)
        {
            return new PixelFormat(from);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(PixelFormat other)
        {
            return Value == other.Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is PixelFormat && Equals((PixelFormat) obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(PixelFormat left, PixelFormat right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(PixelFormat left, PixelFormat right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}", Value);
        }
    }
}