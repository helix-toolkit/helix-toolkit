﻿using Microsoft.Extensions.Logging;
using SharpDX.Direct3D11;
using SharpDX.WIC;
using System.Diagnostics.CodeAnalysis;

namespace HelixToolkit.SharpDX.Utilities;

public static class ScreenCapture
{
    private static readonly ILogger logger = Logger.LogManager.Create(nameof(ScreenCapture));
    /// <summary>
    /// Captures the texture.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="source">The source.</param>
    /// <param name="stagingTexture">The staging texture.</param>
    /// <returns></returns>
    public static bool CaptureTexture(DeviceContext context, Texture2D? source, [NotNullWhen(true)] out Texture2D? stagingTexture)
    {
        stagingTexture = null;
        if (source == null)
        {
            return false;
        }
        var desc = source.Description;
        if (source.Description.SampleDescription.Count > 1)
        {

            desc.SampleDescription.Count = 1;
            desc.SampleDescription.Quality = 0;
            using var texture = new Texture2D(context.Device, desc);
            for (var i = 0; i < desc.ArraySize; ++i)
            {
                for (var level = 0; level < desc.MipLevels; ++level)
                {
                    var index = texture.CalculateSubResourceIndex(level, i, out int mipSize);
                    context.ResolveSubresource(source, index, texture, index, desc.Format);
                }
            }
            desc.BindFlags = BindFlags.None;
            desc.Usage = ResourceUsage.Staging;
            desc.CpuAccessFlags = CpuAccessFlags.Read;
            desc.OptionFlags &= ResourceOptionFlags.TextureCube;
            stagingTexture = new Texture2D(context.Device, desc);
            context.CopyResource(texture, stagingTexture);
        }
        else if (desc.Usage == ResourceUsage.Staging && desc.CpuAccessFlags == CpuAccessFlags.Read)
        {
            stagingTexture = source;
        }
        else
        {
            desc.BindFlags = BindFlags.None;
            desc.OptionFlags &= ResourceOptionFlags.TextureCube;
            desc.CpuAccessFlags = CpuAccessFlags.Read;
            desc.Usage = ResourceUsage.Staging;
            stagingTexture = new Texture2D(context.Device, desc);
            context.CopyResource(source, stagingTexture);
        }
        return true;
    }
    /// <summary>
    /// Saves the wic texture to file.
    /// </summary>
    /// <param name="deviceResource">The device resource.</param>
    /// <param name="source">The source.</param>
    /// <param name="file">The file.</param>
    /// <param name="format">The format.</param>
    /// <returns></returns>
    public static bool SaveWICTextureToFile(IDeviceResources deviceResource, Texture2D? source, string file, Direct2DImageFormat format)
    {
        return SaveWICTextureToFile(deviceResource, source, file, BitmapExtensions.ToWICImageFormat(format));
    }


    /// <summary>
    /// Saves the wic texture to file.
    /// </summary>
    /// <param name="deviceResource">The device resource.</param>
    /// <param name="source">The source.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="containerFormat">The container format.</param>
    /// <returns></returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public static bool SaveWICTextureToFile(IDeviceResources deviceResource, Texture2D? source, string fileName, Guid containerFormat)
    {
        if (deviceResource.Device is null || !CaptureTexture(deviceResource.Device.ImmediateContext, source, out Texture2D? staging))
        {
            return false;
        }
        var desc = staging.Description;
        var sRGB = false;
        var pfGuid = GetPfGuid(desc.Format, ref sRGB);

        if (pfGuid == Guid.Empty)
        {
            staging.Dispose();
            throw new NotSupportedException($"Format: {desc.Format} does not support yet.");
        }

        using var stream = new WICStream(deviceResource.WICImgFactory, fileName, global::SharpDX.IO.NativeFileAccess.Write);
        return CopyTextureToWICStream(deviceResource, staging, stream, pfGuid, containerFormat);
    }

    /// <summary>
    /// Saves the wic texture to bitmap stream.
    /// </summary>
    /// <param name="deviceResource">The device resource.</param>
    /// <param name="source">The source.</param>
    /// <param name="bitmapStream">The bitmap stream.</param>
    /// <returns></returns>
    /// <exception cref="System.NotSupportedException"></exception>
    public static bool SaveWICTextureToBitmapStream(IDeviceResources deviceResource, Texture2D? source, System.IO.MemoryStream bitmapStream)
    {
        if (deviceResource.Device is null)
        {
            return false;
        }

        if (!CaptureTexture(deviceResource.Device.ImmediateContext, source, out Texture2D? staging))
        {
            Disposer.RemoveAndDispose(ref staging);
            return false;
        }
        var desc = staging.Description;
        var sRGB = false;
        var pfGuid = GetPfGuid(desc.Format, ref sRGB);

        if (pfGuid == Guid.Empty)
        {
            Disposer.RemoveAndDispose(ref staging);
            throw new NotSupportedException($"Format: {desc.Format} does not support yet.");
        }
        var succ = false;
        try
        {
            using var stream = new WICStream(deviceResource.WICImgFactory, bitmapStream);
            succ = CopyTextureToWICStream(deviceResource, staging, stream, pfGuid, BitmapExtensions.ToWICImageFormat(Direct2DImageFormat.Bmp));
        }
        finally
        {
            Disposer.RemoveAndDispose(ref staging);
        }
        return succ;
    }

    private static Guid GetPfGuid(global::SharpDX.DXGI.Format format, ref bool sRGB)
    {
        var pfGuid = Guid.Empty;
        sRGB = false;
        switch (format)
        {
            case global::SharpDX.DXGI.Format.R32G32B32A32_Float:
                pfGuid = PixelFormat.Format128bppRGBAFloat;
                break;
            case global::SharpDX.DXGI.Format.R16G16B16A16_Float:
                pfGuid = PixelFormat.Format64bppRGBAHalf;
                break;
            case global::SharpDX.DXGI.Format.R16G16B16A16_UNorm:
                pfGuid = PixelFormat.Format64bppRGBA;
                break;
            case global::SharpDX.DXGI.Format.R32_Float:
                pfGuid = PixelFormat.Format32bppGrayFloat;
                break;
            case global::SharpDX.DXGI.Format.R16_Float:
                pfGuid = PixelFormat.Format16bppGrayHalf;
                break;
            case global::SharpDX.DXGI.Format.R16_UNorm:
                pfGuid = PixelFormat.Format16bppGray;
                break;
            case global::SharpDX.DXGI.Format.R8G8B8A8_UNorm:
                pfGuid = PixelFormat.Format32bppRGBA;
                break;
            case global::SharpDX.DXGI.Format.R8G8B8A8_UNorm_SRgb:
                pfGuid = PixelFormat.Format32bppRGBA;
                sRGB = true;
                break;
            case global::SharpDX.DXGI.Format.B8G8R8A8_UNorm:
                pfGuid = PixelFormat.Format32bppBGR;
                break;
            case global::SharpDX.DXGI.Format.B8G8R8A8_UNorm_SRgb:
                pfGuid = PixelFormat.Format32bppBGR;
                sRGB = true;
                break;
            default:
                break;
        }
        return pfGuid;
    }

    private static bool CopyTextureToWICStream(IDeviceResources deviceResource, Texture2D staging, WICStream stream, Guid pfGuid, Guid containerFormat)
    {
        using var encoder = new BitmapEncoder(deviceResource.WICImgFactory, containerFormat);
        var desc = staging.Description;
        encoder.Initialize(stream);
        var targetGuid = Guid.Empty;
        using var frame = new BitmapFrameEncode(encoder);
        frame.Initialize();
        frame.SetSize(desc.Width, desc.Height);
        frame.SetResolution(72, 72);
        targetGuid = desc.Format switch
        {
            global::SharpDX.DXGI.Format.R32G32B32A32_Float or global::SharpDX.DXGI.Format.R16G16B16A16_Float => PixelFormat.Format96bppRGBFloat,
            global::SharpDX.DXGI.Format.R16G16B16A16_UNorm => PixelFormat.Format48bppBGR,
            global::SharpDX.DXGI.Format.R32_Float or global::SharpDX.DXGI.Format.R16_Float or global::SharpDX.DXGI.Format.R16_UNorm or global::SharpDX.DXGI.Format.R8_UNorm or global::SharpDX.DXGI.Format.A8_UNorm => PixelFormat.Format48bppBGR,
            _ => PixelFormat.Format24bppBGR,
        };
        frame.SetPixelFormat(ref targetGuid);
        var databox = deviceResource.Device?.ImmediateContext.MapSubresource(staging, 0, MapMode.Read, MapFlags.None);

        if (databox is not null)
        {
            try
            {
                if (targetGuid != pfGuid)
                {
                    using var bitmap = new Bitmap(deviceResource.WICImgFactory, desc.Width, desc.Height, pfGuid,
                        new global::SharpDX.DataRectangle(databox.Value.DataPointer, databox.Value.RowPitch));
                    using var converter = new FormatConverter(deviceResource.WICImgFactory);
                    if (converter.CanConvert(pfGuid, targetGuid))
                    {
                        converter.Initialize(bitmap, targetGuid, BitmapDitherType.None, null, 0, BitmapPaletteType.MedianCut);
                        frame.WriteSource(converter);
                    }
                    else
                    {
                        logger.LogDebug("Cannot convert");
                    }
                }
                else
                {
                    frame.WritePixels(desc.Height, new global::SharpDX.DataRectangle(databox.Value.DataPointer, databox.Value.RowPitch), databox.Value.RowPitch * desc.Height);
                }
            }
            finally
            {
                deviceResource.Device?.ImmediateContext.UnmapSubresource(staging, 0);
            }
        }
        frame.Commit();
        encoder.Commit();
        return true;
    }
}
