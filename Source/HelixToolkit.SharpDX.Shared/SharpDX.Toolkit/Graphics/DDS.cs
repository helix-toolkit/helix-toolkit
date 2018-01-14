/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Runtime.InteropServices;
using SharpDX.Direct3D11;
using SharpDX.Multimedia;

namespace SharpDX.Toolkit.Graphics
{
    internal class DDS
    {
        /// <summary>
        /// Magic code to identify DDS header
        /// </summary>
        public const uint MagicHeader = 0x20534444; // "DDS "

        /// <summary>
        /// Internal structure used to describe a DDS pixel format.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct PixelFormat
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="PixelFormat" /> struct.
            /// </summary>
            /// <param name="flags">The flags.</param>
            /// <param name="fourCC">The four CC.</param>
            /// <param name="rgbBitCount">The RGB bit count.</param>
            /// <param name="rBitMask">The r bit mask.</param>
            /// <param name="gBitMask">The g bit mask.</param>
            /// <param name="bBitMask">The b bit mask.</param>
            /// <param name="aBitMask">A bit mask.</param>
            public PixelFormat(PixelFormatFlags flags, int fourCC, int rgbBitCount, uint rBitMask, uint gBitMask, uint bBitMask, uint aBitMask)
            {
                Size = Utilities.SizeOf<PixelFormat>();
                Flags = flags;
                FourCC = fourCC;
                RGBBitCount = rgbBitCount;
                RBitMask = rBitMask;
                GBitMask = gBitMask;
                BBitMask = bBitMask;
                ABitMask = aBitMask;
            }

            public int Size;
            public PixelFormatFlags Flags;
            public int FourCC;
            public int RGBBitCount;
            public uint RBitMask;
            public uint GBitMask;
            public uint BBitMask;
            public uint ABitMask;

            public static readonly PixelFormat DXT1 = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', 'T', '1'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat DXT2 = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', 'T', '2'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat DXT3 = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', 'T', '3'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat DXT4 = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', 'T', '4'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat DXT5 = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', 'T', '5'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat BC4_UNorm = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('B', 'C', '4', 'U'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat BC4_SNorm = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('B', 'C', '4', 'S'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat BC5_UNorm = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('B', 'C', '5', 'U'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat BC5_SNorm = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('B', 'C', '5', 'S'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat R8G8_B8G8 = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('R', 'G', 'B', 'G'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat G8R8_G8B8 = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('G', 'R', 'G', 'B'), 0, 0, 0, 0, 0);

            public static readonly PixelFormat A8R8G8B8 = new PixelFormat(PixelFormatFlags.Rgba, 0, 32, 0x00ff0000, 0x0000ff00, 0x000000ff, 0xff000000);

            public static readonly PixelFormat X8R8G8B8 = new PixelFormat(PixelFormatFlags.Rgb, 0, 32, 0x00ff0000, 0x0000ff00, 0x000000ff, 0x00000000);

            public static readonly PixelFormat A8B8G8R8 = new PixelFormat(PixelFormatFlags.Rgba, 0, 32, 0x000000ff, 0x0000ff00, 0x00ff0000, 0xff000000);

            public static readonly PixelFormat X8B8G8R8 = new PixelFormat(PixelFormatFlags.Rgb, 0, 32, 0x000000ff, 0x0000ff00, 0x00ff0000, 0x00000000);

            public static readonly PixelFormat G16R16 = new PixelFormat(PixelFormatFlags.Rgb, 0, 32, 0x0000ffff, 0xffff0000, 0x00000000, 0x00000000);

            public static readonly PixelFormat R5G6B5 = new PixelFormat(PixelFormatFlags.Rgb, 0, 16, 0x0000f800, 0x000007e0, 0x0000001f, 0x00000000);

            public static readonly PixelFormat A1R5G5B5 = new PixelFormat(PixelFormatFlags.Rgba, 0, 16, 0x00007c00, 0x000003e0, 0x0000001f, 0x00008000);

            public static readonly PixelFormat A4R4G4B4 = new PixelFormat(PixelFormatFlags.Rgba, 0, 16, 0x00000f00, 0x000000f0, 0x0000000f, 0x0000f000);

            public static readonly PixelFormat R8G8B8 = new PixelFormat(PixelFormatFlags.Rgb, 0, 24, 0x00ff0000, 0x0000ff00, 0x000000ff, 0x00000000);

            public static readonly PixelFormat L8 = new PixelFormat(PixelFormatFlags.Luminance, 0, 8, 0xff, 0x00, 0x00, 0x00);

            public static readonly PixelFormat L16 = new PixelFormat(PixelFormatFlags.Luminance, 0, 16, 0xffff, 0x0000, 0x0000, 0x0000);

            public static readonly PixelFormat A8L8 = new PixelFormat(PixelFormatFlags.LuminanceAlpha, 0, 16, 0x00ff, 0x0000, 0x0000, 0xff00);

            public static readonly PixelFormat A8 = new PixelFormat(PixelFormatFlags.Alpha, 0, 8, 0x00, 0x00, 0x00, 0xff);

            public static readonly PixelFormat DX10 = new PixelFormat(PixelFormatFlags.FourCC, new FourCC('D', 'X', '1', '0'), 0, 0, 0, 0, 0);
        }

        /// <summary>
        /// PixelFormat flags.
        /// </summary>
        [Flags]
        public enum PixelFormatFlags
        {
            FourCC = 0x00000004, // DDPF_FOURCC
            Rgb = 0x00000040, // DDPF_RGB
            Rgba = 0x00000041, // DDPF_RGB | DDPF_ALPHAPIXELS
            Luminance = 0x00020000, // DDPF_LUMINANCE
            LuminanceAlpha = 0x00020001, // DDPF_LUMINANCE | DDPF_ALPHAPIXELS
            Alpha = 0x00000002, // DDPF_ALPHA
            Pal8 = 0x00000020, // DDPF_PALETTEINDEXED8            
        }

        /// <summary>
        /// DDS Header flags.
        /// </summary>
        [Flags]
        public enum HeaderFlags
        {
            Texture = 0x00001007, // DDSD_CAPS | DDSD_HEIGHT | DDSD_WIDTH | DDSD_PIXELFORMAT 
            Mipmap = 0x00020000, // DDSD_MIPMAPCOUNT
            Volume = 0x00800000, // DDSD_DEPTH
            Pitch = 0x00000008, // DDSD_PITCH
            LinearSize = 0x00080000, // DDSD_LINEARSIZE
            Height = 0x00000002, // DDSD_HEIGHT
            Width = 0x00000004, // DDSD_WIDTH
        };

        /// <summary>
        /// DDS Surface flags.
        /// </summary>
        [Flags]
        public enum SurfaceFlags
        {
            Texture = 0x00001000, // DDSCAPS_TEXTURE
            Mipmap = 0x00400008,  // DDSCAPS_COMPLEX | DDSCAPS_MIPMAP
            Cubemap = 0x00000008, // DDSCAPS_COMPLEX
        }

        /// <summary>
        /// DDS Cubemap flags.
        /// </summary>
        [Flags]
        public enum CubemapFlags
        {
            CubeMap   = 0x00000200, // DDSCAPS2_CUBEMAP
            Volume    = 0x00200000, // DDSCAPS2_VOLUME
            PositiveX = 0x00000600, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEX
            NegativeX = 0x00000a00, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEX
            PositiveY = 0x00001200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEY
            NegativeY = 0x00002200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEY
            PositiveZ = 0x00004200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_POSITIVEZ
            NegativeZ = 0x00008200, // DDSCAPS2_CUBEMAP | DDSCAPS2_CUBEMAP_NEGATIVEZ

            AllFaces = PositiveX | NegativeX | PositiveY | NegativeY | PositiveZ | NegativeZ,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public int Size;
            public HeaderFlags Flags;
            public int Height;
            public int Width;
            public int PitchOrLinearSize;
            public int Depth; // only if DDS_HEADER_FLAGS_VOLUME is set in dwFlags
            public int MipMapCount;

            private readonly uint unused1;
            private readonly uint unused2;
            private readonly uint unused3;
            private readonly uint unused4;
            private readonly uint unused5;
            private readonly uint unused6;
            private readonly uint unused7;
            private readonly uint unused8;
            private readonly uint unused9;
            private readonly uint unused10;
            private readonly uint unused11;

            public PixelFormat PixelFormat;
            public SurfaceFlags SurfaceFlags;
            public CubemapFlags CubemapFlags;

            private readonly uint Unused12;
            private readonly uint Unused13;

            private readonly uint Unused14;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct HeaderDXT10
        {
            public DXGI.Format DXGIFormat;
            public ResourceDimension ResourceDimension;
            public ResourceOptionFlags MiscFlags; // see DDS_RESOURCE_MISC_FLAG
            public int ArraySize;

            private readonly uint Unused;
        }
    }
}