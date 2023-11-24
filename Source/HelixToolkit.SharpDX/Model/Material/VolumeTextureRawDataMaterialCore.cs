using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// Used to use raw data as Volume 3D texture. 
/// User must create their own data reader to read texture files as pixel byte[] and pass the necessary information as <see cref="VolumeTextureParams"/>
/// <para>
/// Pixel Byte[] is equal to Width * Height * Depth * BytesPerPixel.
/// </para>
/// </summary>
public sealed class VolumeTextureRawDataMaterialCore : VolumeTextureMaterialCoreBase<VolumeTextureParams>
{
    protected override ShaderResourceViewProxy? OnCreateTexture(IEffectsManager? manager)
    {
        if (VolumeTexture.VolumeTextures != null && manager?.Device is not null)
        {
            return ShaderResourceViewProxy.CreateViewFromPixelData(manager.Device, VolumeTexture.VolumeTextures,
            VolumeTexture.Width, VolumeTexture.Height, VolumeTexture.Depth, VolumeTexture.Format, true, false);
        }
        else
        {
            return null;
        }
    }

    public static VolumeTextureParams LoadRAWFile(string filename, int width, int height, int depth)
    {
        using var file = new FileStream(filename, FileMode.Open);
        var length = file.Length;
        var bytePerPixel = length / (width * height * depth);
        var buffer = new byte[width * height * depth * bytePerPixel];
        using (var reader = new BinaryReader(file))
        {
            reader.Read(buffer, 0, buffer.Length);
        }
        var format = global::SharpDX.DXGI.Format.Unknown;
        switch (bytePerPixel)
        {
            case 1:
                format = global::SharpDX.DXGI.Format.R8_UNorm;
                break;
            case 2:
                format = global::SharpDX.DXGI.Format.R16_UNorm;
                break;
            case 4:
                format = global::SharpDX.DXGI.Format.R32_Float;
                break;
        }
        return new VolumeTextureParams(buffer, width, height, depth, format);
    }
}
