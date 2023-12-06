using SharpDX;

namespace HelixToolkit.SharpDX.Model;

public struct VolumeTextureGradientParams
{
    public Half4[] VolumeTextures
    {
        get;
    }
    public int Width
    {
        get;
    }
    public int Height
    {
        get;
    }
    public int Depth
    {
        get;
    }
    public global::SharpDX.DXGI.Format Format
    {
        get;
    }
    public VolumeTextureGradientParams(Half4[] data, int width, int height, int depth)
    {
        VolumeTextures = data;
        Width = width;
        Height = height;
        Depth = depth;
        Format = global::SharpDX.DXGI.Format.R16G16B16A16_Float;
    }
}
