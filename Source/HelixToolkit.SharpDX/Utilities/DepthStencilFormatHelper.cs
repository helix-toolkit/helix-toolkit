using SharpDX.DXGI;

namespace HelixToolkit.SharpDX.Utilities;

public static class DepthStencilFormatHelper
{
    public static Format ComputeDSVFormat(this Format format)
    {
        switch (format)
        {
            case Format.D32_Float:
            case Format.R32_Typeless:
                return Format.D32_Float;
            case Format.D24_UNorm_S8_UInt:
            case Format.R24G8_Typeless:
                return Format.D24_UNorm_S8_UInt;
            case Format.D32_Float_S8X24_UInt:
            case Format.R32G8X24_Typeless:
                return Format.D32_Float_S8X24_UInt;
        }

        throw new InvalidOperationException(string.Format("Unsupported DXGI.FORMAT [{0}] for depth buffer", format));
    }

    public static Format ComputeTextureFormat(this Format format, out bool canUseAsShaderResource)
    {
        Format viewFormat;
        canUseAsShaderResource = false;

        // Determine TypeLess Format and ShaderResourceView Format
        switch (format)
        {
            case Format.D32_Float:
            case Format.R32_Typeless:
                viewFormat = Format.R32_Typeless;
                canUseAsShaderResource = true;
                break;
            case Format.D24_UNorm_S8_UInt:
            case Format.R24G8_Typeless:
                viewFormat = Format.R24G8_Typeless;
                canUseAsShaderResource = true;
                break;
            case Format.D32_Float_S8X24_UInt:
            case Format.R32G8X24_Typeless:
                viewFormat = Format.R32G8X24_Typeless;
                canUseAsShaderResource = true;
                break;
            default:
                viewFormat = (Format)format;
                break;
        }

        return viewFormat;
    }

    public static Format ComputeSRVFormat(this Format format)
    {
        switch (format)
        {
            case Format.D32_Float:
            case Format.R32_Typeless:
                return Format.R32_Float;
            case Format.D24_UNorm_S8_UInt:
            case Format.R24G8_Typeless:
                return Format.R24_UNorm_X8_Typeless;
            case Format.D32_Float_S8X24_UInt:
            case Format.R32G8X24_Typeless:
                return Format.R32_Float_X8X24_Typeless;
        }

        throw new InvalidOperationException(string.Format("Unsupported DXGI.FORMAT [{0}] for creating shaderResourceView", format));
    }

    public static bool CanUseAsShaderResource(this Format format)
    {
        ComputeTextureFormat(format, out var canUse);
        return canUse;
    }
}
