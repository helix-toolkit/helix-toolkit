/*
The MIT License (MIT)
Copyright (c) 2018 Helix Toolkit contributors
*/
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using SharpDX.DXGI;
using SharpDX.WIC;

namespace SharpDX.Toolkit.Graphics
{
    /// <summary>
    /// 
    /// </summary>
    public class WICHelper
    {
        private static WIC.ImagingFactory _factory = new ImagingFactory();

        private static ImagingFactory Factory { get { return _factory ?? (_factory = new ImagingFactory()); } }

        //-------------------------------------------------------------------------------------
        // WIC Pixel Format Translation Data
        //-------------------------------------------------------------------------------------
        private struct WICTranslate
        {
            public WICTranslate(Guid wic, Format format)
            {
                this.WIC = wic;
                this.Format = format;
            }

            public readonly Guid WIC;
            public readonly DXGI.Format Format;
        };

        private static readonly WICTranslate[] WICToDXGIFormats =
            {
                new WICTranslate(SharpDX.WIC.PixelFormat.Format128bppRGBAFloat, SharpDX.DXGI.Format.R32G32B32A32_Float),

                new WICTranslate(SharpDX.WIC.PixelFormat.Format64bppRGBAHalf, SharpDX.DXGI.Format.R16G16B16A16_Float),
                new WICTranslate(SharpDX.WIC.PixelFormat.Format64bppRGBA, SharpDX.DXGI.Format.R16G16B16A16_UNorm),

                new WICTranslate(SharpDX.WIC.PixelFormat.Format32bppRGBA, SharpDX.DXGI.Format.R8G8B8A8_UNorm),
                new WICTranslate(SharpDX.WIC.PixelFormat.Format32bppBGRA, SharpDX.DXGI.Format.B8G8R8A8_UNorm), // DXGI 1.1
                new WICTranslate(SharpDX.WIC.PixelFormat.Format32bppBGR, SharpDX.DXGI.Format.B8G8R8X8_UNorm), // DXGI 1.1

                new WICTranslate(SharpDX.WIC.PixelFormat.Format32bppRGBA1010102XR, SharpDX.DXGI.Format.R10G10B10_Xr_Bias_A2_UNorm), // DXGI 1.1
                new WICTranslate(SharpDX.WIC.PixelFormat.Format32bppRGBA1010102, SharpDX.DXGI.Format.R10G10B10A2_UNorm),
                new WICTranslate(SharpDX.WIC.PixelFormat.Format32bppRGBE, SharpDX.DXGI.Format.R9G9B9E5_Sharedexp),

                new WICTranslate(SharpDX.WIC.PixelFormat.Format16bppBGRA5551, SharpDX.DXGI.Format.B5G5R5A1_UNorm),
                new WICTranslate(SharpDX.WIC.PixelFormat.Format16bppBGR565, SharpDX.DXGI.Format.B5G6R5_UNorm),

                new WICTranslate(SharpDX.WIC.PixelFormat.Format32bppGrayFloat, SharpDX.DXGI.Format.R32_Float),
                new WICTranslate(SharpDX.WIC.PixelFormat.Format16bppGrayHalf, SharpDX.DXGI.Format.R16_Float),
                new WICTranslate(SharpDX.WIC.PixelFormat.Format16bppGray, SharpDX.DXGI.Format.R16_UNorm),
                new WICTranslate(SharpDX.WIC.PixelFormat.Format8bppGray, SharpDX.DXGI.Format.R8_UNorm),

                new WICTranslate(SharpDX.WIC.PixelFormat.Format8bppAlpha, SharpDX.DXGI.Format.A8_UNorm),

                new WICTranslate(SharpDX.WIC.PixelFormat.FormatBlackWhite, SharpDX.DXGI.Format.R1_UNorm),

#if DIRECTX11_1
                new WICTranslate(SharpDX.WIC.PixelFormat.Format96bppRGBFloat,         SharpDX.DXGI.Format.R32G32B32_Float ),
#endif
            };

        //-------------------------------------------------------------------------------------
        // WIC Pixel Format nearest conversion table
        //-------------------------------------------------------------------------------------

        private struct WICConvert
        {
            public WICConvert(Guid source, Guid target)
            {
                this.source = source;
                this.target = target;
            }

            public readonly Guid source;
            public readonly Guid target;
        };

        private static readonly WICConvert[] WICConvertTable =
            {
                // Directly support the formats listed in XnaTexUtil::g_WICFormats, so no conversion required
                // Note target Guid in this conversion table must be one of those directly supported formats.

                new WICConvert(WIC.PixelFormat.Format1bppIndexed, WIC.PixelFormat.Format32bppRGBA), // DXGI.Format.R8G8B8A8_UNorm 
                new WICConvert(WIC.PixelFormat.Format2bppIndexed, WIC.PixelFormat.Format32bppRGBA), // DXGI.Format.R8G8B8A8_UNorm 
                new WICConvert(WIC.PixelFormat.Format4bppIndexed, WIC.PixelFormat.Format32bppRGBA), // DXGI.Format.R8G8B8A8_UNorm 
                new WICConvert(WIC.PixelFormat.Format8bppIndexed, WIC.PixelFormat.Format32bppRGBA), // DXGI.Format.R8G8B8A8_UNorm 

                new WICConvert(WIC.PixelFormat.Format2bppGray, WIC.PixelFormat.Format8bppGray), // DXGI.Format.R8_UNorm 
                new WICConvert(WIC.PixelFormat.Format4bppGray, WIC.PixelFormat.Format8bppGray), // DXGI.Format.R8_UNorm 

                new WICConvert(WIC.PixelFormat.Format16bppGrayFixedPoint, WIC.PixelFormat.Format16bppGrayHalf), // DXGI.Format.R16_FLOAT 
                new WICConvert(WIC.PixelFormat.Format32bppGrayFixedPoint, WIC.PixelFormat.Format32bppGrayFloat), // DXGI.Format.R32_FLOAT 

                new WICConvert(WIC.PixelFormat.Format16bppBGR555, WIC.PixelFormat.Format16bppBGRA5551), // DXGI.Format.B5G5R5A1_UNorm 
                new WICConvert(WIC.PixelFormat.Format32bppBGR101010, WIC.PixelFormat.Format32bppRGBA1010102), // DXGI.Format.R10G10B10A2_UNorm

                new WICConvert(WIC.PixelFormat.Format24bppBGR, WIC.PixelFormat.Format32bppRGBA), // DXGI.Format.R8G8B8A8_UNorm 
                new WICConvert(WIC.PixelFormat.Format24bppRGB, WIC.PixelFormat.Format32bppRGBA), // DXGI.Format.R8G8B8A8_UNorm 
                new WICConvert(WIC.PixelFormat.Format32bppPBGRA, WIC.PixelFormat.Format32bppRGBA), // DXGI.Format.R8G8B8A8_UNorm 
                new WICConvert(WIC.PixelFormat.Format32bppPRGBA, WIC.PixelFormat.Format32bppRGBA), // DXGI.Format.R8G8B8A8_UNorm 

                new WICConvert(WIC.PixelFormat.Format48bppRGB, WIC.PixelFormat.Format64bppRGBA), // DXGI.Format.R16G16B16A16_UNorm
                new WICConvert(WIC.PixelFormat.Format48bppBGR, WIC.PixelFormat.Format64bppRGBA), // DXGI.Format.R16G16B16A16_UNorm
                new WICConvert(WIC.PixelFormat.Format64bppBGRA, WIC.PixelFormat.Format64bppRGBA), // DXGI.Format.R16G16B16A16_UNorm
                new WICConvert(WIC.PixelFormat.Format64bppPRGBA, WIC.PixelFormat.Format64bppRGBA), // DXGI.Format.R16G16B16A16_UNorm
                new WICConvert(WIC.PixelFormat.Format64bppPBGRA, WIC.PixelFormat.Format64bppRGBA), // DXGI.Format.R16G16B16A16_UNorm

                new WICConvert(WIC.PixelFormat.Format48bppRGBFixedPoint, WIC.PixelFormat.Format64bppRGBAHalf), // DXGI.Format.R16G16B16A16_FLOAT 
                new WICConvert(WIC.PixelFormat.Format48bppBGRFixedPoint, WIC.PixelFormat.Format64bppRGBAHalf), // DXGI.Format.R16G16B16A16_FLOAT 
                new WICConvert(WIC.PixelFormat.Format64bppRGBAFixedPoint, WIC.PixelFormat.Format64bppRGBAHalf), // DXGI.Format.R16G16B16A16_FLOAT 
                new WICConvert(WIC.PixelFormat.Format64bppBGRAFixedPoint, WIC.PixelFormat.Format64bppRGBAHalf), // DXGI.Format.R16G16B16A16_FLOAT 
                new WICConvert(WIC.PixelFormat.Format64bppRGBFixedPoint, WIC.PixelFormat.Format64bppRGBAHalf), // DXGI.Format.R16G16B16A16_FLOAT 
                new WICConvert(WIC.PixelFormat.Format64bppRGBHalf, WIC.PixelFormat.Format64bppRGBAHalf), // DXGI.Format.R16G16B16A16_FLOAT 
                new WICConvert(WIC.PixelFormat.Format48bppRGBHalf, WIC.PixelFormat.Format64bppRGBAHalf), // DXGI.Format.R16G16B16A16_FLOAT 

                new WICConvert(WIC.PixelFormat.Format128bppPRGBAFloat, WIC.PixelFormat.Format128bppRGBAFloat), // DXGI.Format.R32G32B32A32_FLOAT 
                new WICConvert(WIC.PixelFormat.Format128bppRGBFloat, WIC.PixelFormat.Format128bppRGBAFloat), // DXGI.Format.R32G32B32A32_FLOAT 
                new WICConvert(WIC.PixelFormat.Format128bppRGBAFixedPoint, WIC.PixelFormat.Format128bppRGBAFloat), // DXGI.Format.R32G32B32A32_FLOAT 
                new WICConvert(WIC.PixelFormat.Format128bppRGBFixedPoint, WIC.PixelFormat.Format128bppRGBAFloat), // DXGI.Format.R32G32B32A32_FLOAT 

                new WICConvert(WIC.PixelFormat.Format32bppCMYK, WIC.PixelFormat.Format32bppRGBA), // DXGI.Format.R8G8B8A8_UNorm 
                new WICConvert(WIC.PixelFormat.Format64bppCMYK, WIC.PixelFormat.Format64bppRGBA), // DXGI.Format.R16G16B16A16_UNorm
                new WICConvert(WIC.PixelFormat.Format40bppCMYKAlpha, WIC.PixelFormat.Format64bppRGBA), // DXGI.Format.R16G16B16A16_UNorm
                new WICConvert(WIC.PixelFormat.Format80bppCMYKAlpha, WIC.PixelFormat.Format64bppRGBA), // DXGI.Format.R16G16B16A16_UNorm

#if DIRECTX11_1
                new WICConvert( WIC.PixelFormat.Format32bppRGB,              WIC.PixelFormat.Format32bppRGBA ), // DXGI.Format.R8G8B8A8_UNorm
                new WICConvert( WIC.PixelFormat.Format64bppRGB,              WIC.PixelFormat.Format64bppRGBA ), // DXGI.Format.R16G16B16A16_UNorm
                new WICConvert( WIC.PixelFormat.Format64bppPRGBAHalf,        WIC.PixelFormat.Format64bppRGBAHalf ), // DXGI.Format.R16G16B16A16_FLOAT 
                new WICConvert( WIC.PixelFormat.Format96bppRGBFixedPoint,    WIC.PixelFormat.Format96bppRGBFloat ), // DXGI.Format.R32G32B32_FLOAT 
#else
                new WICConvert(WIC.PixelFormat.Format96bppRGBFixedPoint, WIC.PixelFormat.Format128bppRGBAFloat), // DXGI.Format.R32G32B32A32_FLOAT 
#endif

                // We don't support n-channel formats
            };

        /// <summary>
        /// Converts a WIC <see cref="WIC.PixelFormat"/> to a <see cref="SharpDX.DXGI.Format"/>.
        /// </summary>
        /// <param name="guid">A WIC <see cref="WIC.PixelFormat"/> </param>
        /// <returns>A <see cref="SharpDX.DXGI.Format"/></returns>
        private static DXGI.Format ToDXGI(Guid guid)
        {
            for (int i = 0; i < WICToDXGIFormats.Length; ++i)
            {
                if (WICToDXGIFormats[i].WIC == guid)
                    return WICToDXGIFormats[i].Format;
            }

            return SharpDX.DXGI.Format.Unknown;
        }

        /// <summary>
        /// Converts a <see cref="SharpDX.DXGI.Format"/> to a a WIC <see cref="WIC.PixelFormat"/>.
        /// </summary>
        /// <param name="format">A <see cref="SharpDX.DXGI.Format"/></param>
        /// <param name="guid">A WIC <see cref="WIC.PixelFormat"/> Guid.</param>
        /// <returns>True if conversion succeed, false otherwise.</returns>
        private static bool ToWIC(DXGI.Format format, out Guid guid)
        {
            for (int i = 0; i < WICToDXGIFormats.Length; ++i)
            {
                if (WICToDXGIFormats[i].Format == format)
                {
                    guid = WICToDXGIFormats[i].WIC;
                    return true;
                }
            }

            // Special cases
            switch (format)
            {
                case SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb:
                    guid = SharpDX.WIC.PixelFormat.Format32bppRGBA;
                    return true;

                case SharpDX.DXGI.Format.D32_Float:
                    guid = SharpDX.WIC.PixelFormat.Format32bppGrayFloat;
                    return true;

                case SharpDX.DXGI.Format.D16_UNorm:
                    guid = SharpDX.WIC.PixelFormat.Format16bppGray;
                    return true;

                case SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb:
                    guid = SharpDX.WIC.PixelFormat.Format32bppBGRA;
                    return true;

                case SharpDX.DXGI.Format.B8G8R8X8_UNorm_SRgb:
                    guid = SharpDX.WIC.PixelFormat.Format32bppBGR;
                    return true;
            }

            guid = Guid.Empty;
            return false;
        }

        /// <summary>
        /// Gets the number of bits per pixels for a WIC <see cref="WIC.PixelFormat"/> Guid.
        /// </summary>
        /// <param name="targetGuid">A WIC <see cref="WIC.PixelFormat"/> Guid.</param>
        /// <returns>The number of bits per pixels for a WIC. If this method is failing to calculate the number of pixels, return 0.</returns>
        private static int GetBitsPerPixel(Guid targetGuid)
        {
            using (var info = new ComponentInfo(Factory, targetGuid))
            {
                if (info.ComponentType != ComponentType.PixelFormat)
                    return 0;

                var pixelFormatInfo = info.QueryInterfaceOrNull<PixelFormatInfo>();
                if (pixelFormatInfo == null)
                    return 0;

                int bpp = pixelFormatInfo.BitsPerPixel;
                pixelFormatInfo.Dispose();
                return bpp;
            }
        }


        //-------------------------------------------------------------------------------------
        // Returns the DXGI format and optionally the WIC pixel Guid to convert to
        //-------------------------------------------------------------------------------------
        private static DXGI.Format DetermineFormat(Guid pixelFormat, WICFlags flags, out Guid pixelFormatOut)
        {
            DXGI.Format format = ToDXGI(pixelFormat);
            pixelFormatOut = Guid.Empty;

            if (format == DXGI.Format.Unknown)
            {
                for (int i = 0; i < WICConvertTable.Length; ++i)
                {
                    if (WICConvertTable[i].source == pixelFormat)
                    {
                        pixelFormatOut = WICConvertTable[i].target;

                        format = ToDXGI(WICConvertTable[i].target);
                        Debug.Assert(format != DXGI.Format.Unknown);
                        break;
                    }
                }
            }

            // Handle special cases based on flags
            switch (format)
            {
                case DXGI.Format.B8G8R8A8_UNorm: // BGRA
                case DXGI.Format.B8G8R8X8_UNorm: // BGRX
                    if ((flags & WICFlags.ForceRgb) != 0)
                    {
                        format = DXGI.Format.R8G8B8A8_UNorm;
                        pixelFormatOut = WIC.PixelFormat.Format32bppRGBA;
                    }
                    break;

                case DXGI.Format.R10G10B10_Xr_Bias_A2_UNorm:
                    if ((flags & WICFlags.NoX2Bias) != 0)
                    {
                        format = DXGI.Format.R10G10B10A2_UNorm;
                        pixelFormatOut = WIC.PixelFormat.Format32bppRGBA1010102;
                    }
                    break;

                case DXGI.Format.B5G5R5A1_UNorm:
                case DXGI.Format.B5G6R5_UNorm:
                    if ((flags & WICFlags.No16Bpp) != 0)
                    {
                        format = DXGI.Format.R8G8B8A8_UNorm;
                        pixelFormatOut = WIC.PixelFormat.Format32bppRGBA;
                    }
                    break;

                case DXGI.Format.R1_UNorm:
                    if ((flags & WICFlags.FlagsAllowMono) == 0)
                    {
                        // By default we want to promote a black & white to greyscale since R1 is not a generally supported D3D format
                        format = DXGI.Format.R8_UNorm;
                        pixelFormatOut = WIC.PixelFormat.Format8bppGray;
                    }
                    break;
            }

            return format;
        }

        /// <summary>
        /// Determines metadata for image
        /// </summary>
        /// <param name="flags">The flags.</param>
        /// <param name="decoder">The decoder.</param>
        /// <param name="frame">The frame.</param>
        /// <param name="pixelFormat">The pixel format.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">If pixel format is not supported.</exception>
        private static ImageDescription? DecodeMetadata(WICFlags flags, BitmapDecoder decoder, BitmapFrameDecode frame, out Guid pixelFormat)
        {
            var size = frame.Size;

            var metadata = new ImageDescription
                               {
                                   Dimension = TextureDimension.Texture2D,
                                   Width = size.Width,
                                   Height = size.Height,
                                   Depth = 1,
                                   MipLevels = 1,
                                   ArraySize = (flags & WICFlags.AllFrames) != 0 ? decoder.FrameCount : 1,
                                   Format = DetermineFormat(frame.PixelFormat, flags, out pixelFormat)
                               };

            if (metadata.Format == DXGI.Format.Unknown)
                return null;

            return metadata;
        }

        private static BitmapDitherType GetWICDither(WICFlags flags)
        {
            if ((flags & WICFlags.Dither) != 0)
                return BitmapDitherType.Ordered4x4;

            if ((flags & WICFlags.DitherDiffusion) != 0)
                return BitmapDitherType.ErrorDiffusion;

            return BitmapDitherType.None;
        }


        private static BitmapInterpolationMode GetWICInterp(WICFlags flags)
        {
            if ((flags & WICFlags.FilterPoint) != 0)
                return BitmapInterpolationMode.NearestNeighbor;

            if ((flags & WICFlags.FilterLinear) != 0)
                return BitmapInterpolationMode.Linear;

            if ((flags & WICFlags.FilterCubic) != 0)
                return BitmapInterpolationMode.Cubic;

            return BitmapInterpolationMode.Fant;
        }

        //-------------------------------------------------------------------------------------
        // Decodes a single frame
        //-------------------------------------------------------------------------------------
        private static Image DecodeSingleFrame(WICFlags flags, ImageDescription metadata, Guid convertGUID, BitmapFrameDecode frame)
        {
            var image = Image.New(metadata);

            var pixelBuffer = image.PixelBuffer[0];

            if (convertGUID == Guid.Empty)
            {
                frame.CopyPixels(pixelBuffer.RowStride, pixelBuffer.DataPointer, pixelBuffer.BufferStride);
            }
            else
            {
                using (var converter = new FormatConverter(Factory))
                {
                    converter.Initialize(frame, convertGUID, GetWICDither(flags), null, 0, BitmapPaletteType.Custom);
                    converter.CopyPixels(pixelBuffer.RowStride, pixelBuffer.DataPointer, pixelBuffer.BufferStride);
                }
            }

            return image;
        }

        //-------------------------------------------------------------------------------------
        // Decodes an image array, resizing/format converting as needed
        //-------------------------------------------------------------------------------------
        private static Image DecodeMultiframe(WICFlags flags, ImageDescription metadata, BitmapDecoder decoder)
        {
            var image = Image.New(metadata);

            Guid sourceGuid;
            if (!ToWIC(metadata.Format, out sourceGuid))
                return null;

            for (int index = 0; index < metadata.ArraySize; ++index)
            {
                var pixelBuffer = image.PixelBuffer[index, 0];

                using (var frame = decoder.GetFrame(index))
                {
                    var pfGuid = frame.PixelFormat;
                    var size = frame.Size;

                    if (pfGuid == sourceGuid)
                    {
                        if (size.Width == metadata.Width && size.Height == metadata.Height)
                        {
                            // This frame does not need resized or format converted, just copy...
                            frame.CopyPixels(pixelBuffer.RowStride, pixelBuffer.DataPointer, pixelBuffer.BufferStride);
                        }
                        else
                        {
                            // This frame needs resizing, but not format converted
                            using (var scaler = new BitmapScaler(Factory))
                            {
                                scaler.Initialize(frame, metadata.Width, metadata.Height, GetWICInterp(flags));
                                scaler.CopyPixels(pixelBuffer.RowStride, pixelBuffer.DataPointer, pixelBuffer.BufferStride);
                            }
                        }
                    }
                    else
                    {
                        // This frame required format conversion
                        using (var converter = new FormatConverter(Factory))
                        {
                            converter.Initialize(frame, pfGuid, GetWICDither(flags), null, 0, BitmapPaletteType.Custom);

                            if (size.Width == metadata.Width && size.Height == metadata.Height)
                            {
                                converter.CopyPixels(pixelBuffer.RowStride, pixelBuffer.DataPointer, pixelBuffer.BufferStride);
                            }
                            else
                            {
                                // This frame needs resizing, but not format converted
                                using (var scaler = new BitmapScaler(Factory))
                                {
                                    scaler.Initialize(frame, metadata.Width, metadata.Height, GetWICInterp(flags));
                                    scaler.CopyPixels(pixelBuffer.RowStride, pixelBuffer.DataPointer, pixelBuffer.BufferStride);
                                }
                            }
                        }
                    }
                }
            }
            return image;
        }

        //-------------------------------------------------------------------------------------
        // Load a WIC-supported file in memory
        //-------------------------------------------------------------------------------------
        internal static Image LoadFromWICMemory(IntPtr pSource, int size, bool makeACopy, GCHandle? handle)
        {
            var flags = WICFlags.AllFrames;

            Image image = null;
            // Create input stream for memory
            using (var stream = new WICStream(Factory, new DataPointer(pSource, size)))
            {
                // If the decoder is unable to decode the image, than return null
                BitmapDecoder decoder = null;
                try
                {
                    decoder = new BitmapDecoder(Factory, stream, DecodeOptions.CacheOnDemand);
                    using (var frame = decoder.GetFrame(0))
                    {
                        // Get metadata
                        Guid convertGuid;
                        var tempDesc = DecodeMetadata(flags, decoder, frame, out convertGuid);

                        // If not supported.
                        if (!tempDesc.HasValue)
                            return null;

                        var mdata = tempDesc.Value;

                        if ((mdata.ArraySize > 1) && (flags & WICFlags.AllFrames) != 0)
                        {
                            return DecodeMultiframe(flags, mdata, decoder);
                        }

                        image = DecodeSingleFrame(flags, mdata, convertGuid, frame);
                    }
                }
                catch
                {
                    image = null;
                }
                finally
                {
                    if (decoder != null)
                        decoder.Dispose();
                }
            }

            // For WIC, we are not keeping the original buffer.
            if (image != null && !makeACopy)
            {
                if (handle.HasValue)
                {
                    handle.Value.Free();
                }
                else
                {
                    Utilities.FreeMemory(pSource);
                }
            }
            return image;
        }

        //-------------------------------------------------------------------------------------
        // Encodes a single frame
        //-------------------------------------------------------------------------------------
        private static void EncodeImage(PixelBuffer image, WICFlags flags, BitmapFrameEncode frame)
        {
            Guid pfGuid;
            if (!ToWIC(image.Format, out pfGuid))
                throw new NotSupportedException("Format not supported");

            frame.Initialize();
            frame.SetSize(image.Width, image.Height);
            frame.SetResolution(72, 72);
            Guid targetGuid = pfGuid;
            frame.SetPixelFormat(ref targetGuid);

            if (targetGuid != pfGuid)
            {
                using (var source = new Bitmap(Factory, image.Width, image.Height, pfGuid, new DataRectangle(image.DataPointer, image.RowStride), image.BufferStride))
                {
                    using (var converter = new FormatConverter(Factory))
                    {
                        using (var palette = new Palette(Factory))
                        {
                            palette.Initialize(source, 256, true);
                            converter.Initialize(source, targetGuid, GetWICDither(flags), palette, 0, BitmapPaletteType.Custom);

                            int bpp = GetBitsPerPixel(targetGuid);
                            if (bpp == 0) throw new NotSupportedException("Unable to determine the Bpp for the target format");

                            int rowPitch = (image.Width * bpp + 7) / 8;
                            int slicePitch = rowPitch * image.Height;

                            var temp = Utilities.AllocateMemory(slicePitch);
                            try
                            {
                                converter.CopyPixels(rowPitch, temp, slicePitch);
                                frame.Palette = palette;
                                frame.WritePixels(image.Height, temp, rowPitch, slicePitch);
                            }
                            finally
                            {
                                Utilities.FreeMemory(temp);
                            }
                        }
                    }
                }
            }
            else
            {
                // No conversion required
                frame.WritePixels(image.Height, image.DataPointer, image.RowStride, image.BufferStride);
            }

            frame.Commit();
        }

        private static void EncodeSingleFrame(PixelBuffer pixelBuffer, WICFlags flags, Guid guidContainerFormat, Stream stream)
        {
            using (var encoder = new BitmapEncoder(Factory, guidContainerFormat, stream))
            {
                using (var frame = new BitmapFrameEncode(encoder))
                {
                    if (guidContainerFormat == ContainerFormatGuids.Bmp)
                    {
                        try
                        {
                            frame.Options.Set("EnableV5Header32bppBGRA", true);
                        }
                        catch
                        {
                        }
                    }
                    EncodeImage(pixelBuffer, flags, frame);
                    encoder.Commit();
                }
            }
        }

        //-------------------------------------------------------------------------------------
        // Encodes an image array
        //-------------------------------------------------------------------------------------
        private static void EncodeMultiframe(PixelBuffer[] images, int count, WICFlags flags, Guid guidContainerFormat, Stream stream)
        {
            if (images.Length < 2)
                throw new ArgumentException("Cannot encode to multiple frame. Image doesn't have multiple frame");

            using (var encoder = new BitmapEncoder(Factory, guidContainerFormat))
            {
                using (var eInfo = encoder.EncoderInfo)
                {
                    if (!eInfo.IsMultiframeSupported)
                        throw new NotSupportedException("Cannot encode to multiple frame. Format is not supporting multiple frame");
                }

                encoder.Initialize(stream);

                for (int i = 0; i < Math.Min(images.Length, count); i++)
                {
                    var pixelBuffer = images[i];
                    using (var frame = new BitmapFrameEncode(encoder))
                        EncodeImage(pixelBuffer, flags, frame);
                }

                encoder.Commit();
            }
        }

        private static Guid GetContainerFormatFromFileType(ImageFileType fileType)
        {
            switch (fileType)
            {
                case ImageFileType.Bmp:
                    return ContainerFormatGuids.Bmp;
                case ImageFileType.Jpg:
                    return ContainerFormatGuids.Jpeg;
                case ImageFileType.Gif:
                    return ContainerFormatGuids.Gif;
                case ImageFileType.Png:
                    return ContainerFormatGuids.Png;
                case ImageFileType.Tiff:
                    return ContainerFormatGuids.Tiff;
                case ImageFileType.Wmp:
                    return ContainerFormatGuids.Wmp;
                default:
                    throw new NotSupportedException("Format not supported");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Dispose()
        {
            Utilities.Dispose(ref _factory);
        }

        public static void SaveGifToWICMemory(PixelBuffer[] pixelBuffers, int count, ImageDescription description, Stream imageStream)
        {
            SaveToWICMemory(pixelBuffers, count, WICFlags.AllFrames, ImageFileType.Gif, imageStream);
        }

        public static void SaveTiffToWICMemory(PixelBuffer[] pixelBuffers, int count, ImageDescription description, Stream imageStream)
        {
            SaveToWICMemory(pixelBuffers, count, WICFlags.AllFrames, ImageFileType.Tiff, imageStream);
        }

        public static void SaveBmpToWICMemory(PixelBuffer[] pixelBuffers, int count, ImageDescription description, Stream imageStream)
        {
            SaveToWICMemory(pixelBuffers, 1, WICFlags.None, ImageFileType.Bmp, imageStream);
        }

        public static void SaveJpgToWICMemory(PixelBuffer[] pixelBuffers, int count, ImageDescription description, Stream imageStream)
        {
            SaveToWICMemory(pixelBuffers, 1, WICFlags.None, ImageFileType.Jpg, imageStream);
        }

        public static void SavePngToWICMemory(PixelBuffer[] pixelBuffers, int count, ImageDescription description, Stream imageStream)
        {
            SaveToWICMemory(pixelBuffers, 1, WICFlags.None, ImageFileType.Png, imageStream);
        }

        public static void SaveWmpToWICMemory(PixelBuffer[] pixelBuffers, int count, ImageDescription description, Stream imageStream)
        {
            SaveToWICMemory(pixelBuffers, 1, WICFlags.None, ImageFileType.Wmp, imageStream);
        }

        private static void SaveToWICMemory(PixelBuffer[] pixelBuffer, int count, WICFlags flags, ImageFileType fileType, Stream stream)
        {
            if (count > 1)
                EncodeMultiframe(pixelBuffer, count, flags, GetContainerFormatFromFileType(fileType), stream);
            else
                EncodeSingleFrame(pixelBuffer[0], flags, GetContainerFormatFromFileType(fileType), stream);
        }
    }
}
