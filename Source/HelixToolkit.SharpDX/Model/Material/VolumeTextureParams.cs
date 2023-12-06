namespace HelixToolkit.SharpDX.Model;

public struct VolumeTextureParams
{
    public byte[] VolumeTextures
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
    public VolumeTextureParams(byte[] data, int width, int height, int depth, global::SharpDX.DXGI.Format format)
    {
        VolumeTextures = data;
        Width = width;
        Height = height;
        Depth = depth;
        Format = format;
    }
}
