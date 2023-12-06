using HelixToolkit.SharpDX.Utilities;

namespace HelixToolkit.SharpDX.Model;

/// <summary>
/// 
/// </summary>
public sealed class VolumeTextureDiffuseMaterialCore : VolumeTextureMaterialCoreBase<VolumeTextureGradientParams>
{
    protected override string DefaultPassName => DefaultPassNames.Diffuse;

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
}
